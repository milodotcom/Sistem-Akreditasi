    Imports System.Data
    Imports System.Data.SqlClient
    Imports System.Configuration
    Imports System.IO
    Imports System.Text
    Imports System.Web
    Imports System.Web.Script.Serialization

    Partial Class Kategori
        Inherits System.Web.UI.Page

        Private ReadOnly Property ConnStr As String
            Get
                Return ConfigurationManager.ConnectionStrings("AkreditasiDB").ConnectionString
            End Get
        End Property

        Private ReadOnly AllowedExts As String() = {".pdf", ".doc", ".docx", ".zip", ".jpg", ".jpeg", ".png"}
        Private Const MaxFileSizeBytes As Integer = 50 * 1024 * 1024

        Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
            Dim isAjax = String.Equals(Request.Headers("X-Requested-With"), "XMLHttpRequest", StringComparison.OrdinalIgnoreCase)

            If String.Equals(Request.HttpMethod, "POST", StringComparison.OrdinalIgnoreCase) Then
                Dim action = Request.Form("action")
                If Not String.IsNullOrEmpty(action) Then
                    Select Case action.ToLower()
                        Case "upload"
                            If isAjax Then
                                HandleUploadAjax()
                            Else
                                HandleUploadNormal()
                            End If
                            Return
                        Case "delete"
                            If isAjax Then
                                HandleDeleteAjax()
                                Return
                            End If
                    End Select
                End If
            End If

            If Not IsPostBack Then
                Dim kursusId As Integer = ResolveKursusIdFromQuery()
                If kursusId <= 0 Then
                    litCourseTitle.Text = "Tiada kursus dipilih."
                    litAccordion.Text = "<div class='text-red-600'>No course selected. Click a course first.</div>"
                    Return
                End If

                ViewState("KursusID") = kursusId
                litCourseTitle.Text = GetCourseTitle(kursusId)
                litBreadcrumbs.Text = BuildBreadcrumbs(kursusId)
                BuildAccordion(kursusId)
            End If
        End Sub

        Private Function ResolveKursusIdFromQuery() As Integer
            Dim kursusId As Integer = 0
            Dim kursusIdQs = Request.QueryString("kursusId")
            Dim kodQs = Request.QueryString("kod")
            Dim courseIdQs = Request.QueryString("courseId")

            If Not String.IsNullOrEmpty(kursusIdQs) AndAlso Integer.TryParse(kursusIdQs, kursusId) Then Return kursusId
            If Not String.IsNullOrEmpty(courseIdQs) Then
                If Integer.TryParse(courseIdQs, kursusId) Then Return kursusId
                Return ResolveKursusIdByKod(courseIdQs)
            End If
            If Not String.IsNullOrEmpty(kodQs) Then Return ResolveKursusIdByKod(kodQs)
            Return 0
        End Function

        Private Function ResolveKursusIdByKod(kod As String) As Integer
            Dim result As Integer = 0
            Using conn As New SqlConnection(ConnStr)
                Using cmd As New SqlCommand("SELECT TOP 1 KursusID FROM acKursus WHERE Kod_Kursus = @kod", conn)
                    cmd.Parameters.AddWithValue("@kod", kod)
                    conn.Open()
                    Dim obj = cmd.ExecuteScalar()
                    If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then Integer.TryParse(obj.ToString(), result)
                End Using
            End Using
            Return result
        End Function

        Private Function GetCourseTitle(kursusId As Integer) As String
            Using conn As New SqlConnection(ConnStr)
                Using cmd As New SqlCommand("SELECT TOP 1 Kod_Kursus + ' - ' + Nama_Kursus FROM acKursus WHERE KursusID = @id", conn)
                    cmd.Parameters.AddWithValue("@id", kursusId)
                    conn.Open()
                    Dim obj = cmd.ExecuteScalar()
                    If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then Return obj.ToString()
                End Using
            End Using
            Return $"Kursus {kursusId}"
        End Function

        Private Function BuildBreadcrumbs(kursusId As Integer) As String
            Dim facultyCode As String = ""
            Using conn As New SqlConnection(ConnStr)
                Using cmd As New SqlCommand("SELECT Kod_Fakulti FROM acKursus WHERE KursusID=@id", conn)
                    cmd.Parameters.AddWithValue("@id", kursusId)
                    conn.Open()
                    Dim obj = cmd.ExecuteScalar()
                    If obj IsNot Nothing Then facultyCode = obj.ToString()
                End Using
            End Using

            Return $"<a class='text-blue-600 underline' href='Faculty.aspx'>Fakulti</a> &gt; " &
                   $"<a class='text-blue-600 underline' href='CourseList.aspx?facultyId={HttpUtility.UrlEncode(facultyCode)}'>Kursus</a> &gt; Senarai Dokumen"
        End Function

        Private Sub BuildAccordion(kursusId As Integer)
            Dim sb As New StringBuilder()
            Dim sql As String = "
    SELECT k.id_kat, k.NamaKategori,
           s.id_sub, s.NamaSub,
           r.DokumenID, r.FileName, r.FilePath, r.UploadedAt, r.FileSize, r.UploadedBy
    FROM tbl_kategori k
    LEFT JOIN tbl_subkategori s ON s.id_kat = k.id_kat
    LEFT JOIN (
        SELECT d.*
        FROM (
            SELECT *, ROW_NUMBER() OVER (PARTITION BY id_sub ORDER BY UploadedAt DESC, DokumenID DESC) rn
            FROM Dokumen
            WHERE KursusID = @KursusID AND IsActive = 1
        ) d
        WHERE d.rn = 1
    ) r ON r.id_sub = s.id_sub
    ORDER BY k.id_kat, s.id_sub;"
            Dim dt As New DataTable()
            Using conn As New SqlConnection(ConnStr)
                Using cmd As New SqlCommand(sql, conn)
                    cmd.Parameters.AddWithValue("@KursusID", kursusId)
                    conn.Open()
                    dt.Load(cmd.ExecuteReader())
                End Using
            End Using

            Dim grouped = dt.AsEnumerable().GroupBy(Function(r) New With {
                Key .id_kat = If(IsDBNull(r("id_kat")), 0, Convert.ToInt32(r("id_kat"))),
                Key .NamaKategori = If(IsDBNull(r("NamaKategori")), "", Convert.ToString(r("NamaKategori")))
            })

            Dim totalDocs As Integer = 0
            Dim uploaded As Integer = 0

            For Each grp In grouped
                Dim catId = grp.Key.id_kat
                Dim catName = HttpUtility.HtmlEncode(grp.Key.NamaKategori)
                Dim catPanelId = "cat-" & catId.ToString()

                sb.AppendLine("<div class='border border-gray-200 rounded-lg mb-3'>")
                sb.AppendLine($"  <button type='button' class='accordion-button w-full flex justify-between items-center p-4 text-left bg-gray-50 hover:bg-gray-100' aria-expanded='false' data-target='#{catPanelId}'>")
                sb.AppendLine($"    <span class='font-semibold text-gray-800'>{catName}</span>")
                sb.AppendLine("    <svg class='w-5 h-5 transform transition-transform' fill='none' stroke='currentColor' viewBox='0 0 24 24'><path stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M19 9l-7 7-7-7'></path></svg>")
                sb.AppendLine("  </button>")

                sb.AppendLine($"  <div id='{catPanelId}' class='accordion-content'>")
                sb.AppendLine("    <ul class='space-y-3'>")

                For Each row In grp.OrderBy(Function(r) If(IsDBNull(r("id_sub")), 0, Convert.ToInt32(r("id_sub"))))
                    Dim id_sub As Integer = If(IsDBNull(row("id_sub")), -1, Convert.ToInt32(row("id_sub")))

                    Dim namaSub = HttpUtility.HtmlEncode(If(IsDBNull(row("NamaSub")), "", row("NamaSub").ToString()))
                    Dim dokId As Integer = If(IsDBNull(row("DokumenID")), 0, Convert.ToInt32(row("DokumenID")))
                    Dim hasDoc As Boolean = dokId > 0

                    totalDocs += 1
                    If hasDoc Then uploaded += 1

                    sb.AppendLine("      <li class='flex items-center justify-between p-3 rounded-md border'>")
                    sb.AppendLine($"        <div class='flex-1 mr-4'><p class='text-sm text-gray-800'>{namaSub}</p></div>")
                    sb.AppendLine("        <div class='flex items-center space-x-3'>")

                    If hasDoc Then
                        Dim fileName = If(dt.Columns.Contains("FileName") AndAlso Not IsDBNull(row("FileName")), Convert.ToString(row("FileName")), "Fail")
                        Dim filePath = If(dt.Columns.Contains("FilePath") AndAlso Not IsDBNull(row("FilePath")), Convert.ToString(row("FilePath")), "")
                        sb.AppendLine($"          <a class='text-blue-600 underline text-sm' target='_blank' href='{HttpUtility.HtmlAttributeEncode(filePath)}'>Lihat: {HttpUtility.HtmlEncode(fileName)}</a>")
                        sb.AppendLine("          <span class='text-xs px-2 py-1 rounded-full text-green-700 bg-green-100'>Telah Dimuat Naik</span>")
                        sb.AppendLine($"          <button type='button' class='ml-2 bg-red-600 text-white small-btn' onclick='deleteDokumen({dokId})'>Padam</button>")
                    Else
                        sb.AppendLine("          <span class='text-xs px-2 py-1 rounded-full text-yellow-700 bg-yellow-100'>Belum Dimuat Naik</span>")
                        ' Show only the "Pilih Fail" that opens modal (no inline form)
                        Dim safeSubName = HttpUtility.JavaScriptStringEncode(If(IsDBNull(row("NamaSub")), "", row("NamaSub").ToString()))
                        sb.AppendLine($"          <button type='button' class='ml-2 bg-blue-500 text-white small-btn' onclick=""openUploadModal({kursusId},{catId},{id_sub},'{safeSubName}')"" >Pilih Fail</button>")
                    End If

                    sb.AppendLine("        </div>")
                    sb.AppendLine("      </li>")
                Next

                sb.AppendLine("    </ul>")
                sb.AppendLine("  </div>")
                sb.AppendLine("</div>")
            Next

            litAccordion.Text = sb.ToString()

            Dim pct = If(totalDocs > 0, CInt(Math.Round((uploaded / totalDocs) * 100.0)), 0)
            Dim script = $"<script>document.getElementById('progressBarFill').style.width='{pct}%'; document.getElementById('progressText').innerText='{pct}%'; document.getElementById('progressSummary').innerText='{uploaded} daripada {totalDocs} dokumen telah dimuat naik.';</script>"
            Page.ClientScript.RegisterStartupScript(Me.GetType(), "progressUpdate", script)
        End Sub

        ' AJAX upload: but block if there is existing active doc for the same (course,cat,sub)
        Private Sub HandleUploadAjax()
            Dim js As New JavaScriptSerializer()
            Dim result As New Dictionary(Of String, Object) From {{"success", False}}

            Try
                Dim kursusId As Integer = 0
                Integer.TryParse(Request.Form("kursusId"), kursusId)
                Dim id_kat As Integer = 0
                Integer.TryParse(Request.Form("id_kat"), id_kat)
                Dim id_sub As Integer = 0
                Integer.TryParse(Request.Form("id_sub"), id_sub)

                If kursusId <= 0 OrElse id_kat <= 0 Then
                    Throw New Exception("Parameter tidak sah.")
                End If

                ' Check if active document exists — block upload unless deleted first
                Using connCheck As New SqlConnection(ConnStr)
                    Using cmdC As New SqlCommand("SELECT TOP 1 DokumenID FROM Dokumen WHERE KursusID=@k AND id_kat=@cat AND id_sub=@sub AND IsActive=1", connCheck)
                        cmdC.Parameters.AddWithValue("@k", kursusId)
                        cmdC.Parameters.AddWithValue("@cat", id_kat)
                        cmdC.Parameters.AddWithValue("@sub", id_sub)
                        connCheck.Open()
                        Dim existing = cmdC.ExecuteScalar()
                        If existing IsNot Nothing AndAlso Not IsDBNull(existing) Then
                            result("error") = "Sila padam fail sedia ada dahulu sebelum memuat naik yang baru."
                            Response.ContentType = "application/json"
                            Response.Write(js.Serialize(result))
                            Response.[End]()
                            Return
                        End If
                    End Using
                End Using

                If Request.Files.Count = 0 Then Throw New Exception("Tiada fail dihantar.")
                Dim uploadedFile = Request.Files(0)
                If uploadedFile Is Nothing OrElse uploadedFile.ContentLength = 0 Then Throw New Exception("Tiada fail dihantar.")
                If uploadedFile.ContentLength > MaxFileSizeBytes Then Throw New Exception("Fail terlalu besar.")

                Dim origName = Path.GetFileName(uploadedFile.FileName)
                Dim ext = Path.GetExtension(origName).ToLowerInvariant()
                If Array.IndexOf(AllowedExts, ext) = -1 Then Throw New Exception("Jenis fail tidak dibenarkan.")

                Dim timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss")
                Dim safeBase = Path.GetFileNameWithoutExtension(origName).Replace(" ", "_")
                Dim finalFileName = $"{safeBase}_{timeStamp}{ext}"
                Dim relativeFolder = $"/Uploads/{kursusId}/{id_kat}/{id_sub}/"
                Dim physicalFolder = Server.MapPath("~" & relativeFolder)
                If Not Directory.Exists(physicalFolder) Then Directory.CreateDirectory(physicalFolder)
                Dim physicalPath = Path.Combine(physicalFolder, finalFileName)
                Dim virtualPath = VirtualPathUtility.ToAbsolute(relativeFolder & finalFileName)

                uploadedFile.SaveAs(physicalPath)

                Using conn As New SqlConnection(ConnStr)
                    conn.Open()
                    Using tran = conn.BeginTransaction()
                        Using cmdIns As New SqlCommand("
    INSERT INTO Dokumen (KursusID, id_kat, id_sub, FileName, FilePath, ContentType, FileSize, UploadedBy, UploadedAt, IsActive)
    VALUES (@KursusID, @id_kat, @id_sub, @FileName, @FilePath, @ContentType, @FileSize, @UploadedBy, SYSUTCDATETIME(), 1);
    SELECT CAST(SCOPE_IDENTITY() AS INT);", conn, tran)
                            cmdIns.Parameters.AddWithValue("@KursusID", kursusId)
                            cmdIns.Parameters.AddWithValue("@id_kat", id_kat)
                            cmdIns.Parameters.AddWithValue("@id_sub", id_sub)
                            cmdIns.Parameters.AddWithValue("@FileName", origName)
                            cmdIns.Parameters.AddWithValue("@FilePath", virtualPath)
                            cmdIns.Parameters.AddWithValue("@ContentType", uploadedFile.ContentType)
                            cmdIns.Parameters.AddWithValue("@FileSize", CLng(uploadedFile.ContentLength))
                            cmdIns.Parameters.AddWithValue("@UploadedBy", If(Context.User.Identity.IsAuthenticated, Context.User.Identity.Name, "anonymous"))
                            Dim newIdObj = cmdIns.ExecuteScalar()
                            Dim newId As Integer = 0
                            If newIdObj IsNot Nothing Then Integer.TryParse(newIdObj.ToString(), newId)
                            result("DokumenID") = newId
                        End Using
                        tran.Commit()
                    End Using
                End Using

                result("success") = True
                result("fileName") = origName
                result("filePath") = virtualPath
            Catch ex As Exception
                result("error") = ex.Message
            End Try

            Response.ContentType = "application/json"
            Response.Write(New JavaScriptSerializer().Serialize(result))
            Response.[End]()
        End Sub

        ' AJAX delete handler: set IsActive = 0 and attempt physical file delete
        Private Sub HandleDeleteAjax()
            Dim js As New JavaScriptSerializer()
            Dim result As New Dictionary(Of String, Object) From {{"success", False}}

            Try
                Dim dokId As Integer = 0
                Integer.TryParse(Request.Form("dokId"), dokId)
                If dokId <= 0 Then Throw New Exception("Invalid DokumenID.")

                Dim filePath As String = Nothing
                Using conn As New SqlConnection(ConnStr)
                    conn.Open()

                    ' get file path
                    Using cmd As New SqlCommand("SELECT FilePath FROM Dokumen WHERE DokumenID=@id AND IsActive=1", conn)
                        cmd.Parameters.AddWithValue("@id", dokId)
                        Dim obj = cmd.ExecuteScalar()
                        If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then filePath = Convert.ToString(obj)
                    End Using

                    ' mark inactive
                    Using cmdU As New SqlCommand("UPDATE Dokumen SET IsActive=0 WHERE DokumenID=@id", conn)
                        cmdU.Parameters.AddWithValue("@id", dokId)
                        cmdU.ExecuteNonQuery()
                    End Using
                End Using

                ' attempt physical delete (best-effort)
                If Not String.IsNullOrEmpty(filePath) Then
                    Try
                        Dim phys = Server.MapPath(filePath)
                        If System.IO.File.Exists(phys) Then
                            System.IO.File.Delete(phys)
                        End If
                    Catch exDel As Exception
                        ' ignore physical delete errors (DB already marked inactive)
                    End Try
                End If

                result("success") = True
            Catch ex As Exception
                result("error") = ex.Message
            End Try

            Response.ContentType = "application/json"
            Response.Write(js.Serialize(result))
            Response.[End]()
        End Sub

        ' Legacy non-AJAX (kept to be safe)
        Private Sub HandleUploadNormal()
            Try
                Dim kursusId As Integer = 0
                Integer.TryParse(Request.Form("kursusId"), kursusId)
                Dim id_kat As Integer = 0
                Integer.TryParse(Request.Form("id_kat"), id_kat)
                Dim id_sub As Integer = 0
                Integer.TryParse(Request.Form("id_sub"), id_sub)

                If kursusId <= 0 OrElse id_kat <= 0 Then
                    Throw New Exception("Parameter tidak sah.")
                End If
                If Request.Files.Count = 0 Then Throw New Exception("Tiada fail dipilih.")
                Dim uploadedFile = Request.Files(0)
                If uploadedFile.ContentLength = 0 Then Throw New Exception("Tiada fail dipilih.")
                If uploadedFile.ContentLength > MaxFileSizeBytes Then Throw New Exception("Fail terlalu besar.")

                ' block if active exists
                Using connCheck As New SqlConnection(ConnStr)
                    Using cmdC As New SqlCommand("SELECT TOP 1 DokumenID FROM Dokumen WHERE KursusID=@k AND id_kat=@cat AND id_sub=@sub AND IsActive=1", connCheck)
                        cmdC.Parameters.AddWithValue("@k", kursusId)
                        cmdC.Parameters.AddWithValue("@cat", id_kat)
                        cmdC.Parameters.AddWithValue("@sub", id_sub)
                        connCheck.Open()
                        Dim existing = cmdC.ExecuteScalar()
                        If existing IsNot Nothing AndAlso Not IsDBNull(existing) Then Throw New Exception("Sila padam fail sedia ada dahulu sebelum memuat naik yang baru.")
                    End Using
                End Using

                Dim origName = Path.GetFileName(uploadedFile.FileName)
                Dim ext = Path.GetExtension(origName).ToLowerInvariant()
                If Array.IndexOf(AllowedExts, ext) = -1 Then Throw New Exception("Jenis fail tidak dibenarkan.")

                Dim timeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss")
                Dim safeBase = Path.GetFileNameWithoutExtension(origName).Replace(" ", "_")
                Dim finalFileName = $"{safeBase}_{timeStamp}{ext}"
                Dim relativeFolder = $"/Uploads/{kursusId}/{id_kat}/{id_sub}/"
                Dim physicalFolder = Server.MapPath("~" & relativeFolder)
                If Not Directory.Exists(physicalFolder) Then Directory.CreateDirectory(physicalFolder)
                Dim physicalPath = Path.Combine(physicalFolder, finalFileName)
                Dim virtualPath = VirtualPathUtility.ToAbsolute(relativeFolder & finalFileName)

                uploadedFile.SaveAs(physicalPath)

                Using conn As New SqlConnection(ConnStr)
                    conn.Open()
                    Using tran = conn.BeginTransaction()
                        Using cmdIns As New SqlCommand("
    INSERT INTO Dokumen (KursusID, id_kat, id_sub, FileName, FilePath, ContentType, FileSize, UploadedBy, UploadedAt, IsActive)
    VALUES (@KursusID, @id_kat, @id_sub, @FileName, @FilePath, @ContentType, @FileSize, @UploadedBy, SYSUTCDATETIME(), 1)", conn, tran)
                            cmdIns.Parameters.AddWithValue("@KursusID", kursusId)
                            cmdIns.Parameters.AddWithValue("@id_kat", id_kat)
                            cmdIns.Parameters.AddWithValue("@id_sub", id_sub)
                            cmdIns.Parameters.AddWithValue("@FileName", origName)
                            cmdIns.Parameters.AddWithValue("@FilePath", virtualPath)
                            cmdIns.Parameters.AddWithValue("@ContentType", uploadedFile.ContentType)
                            cmdIns.Parameters.AddWithValue("@FileSize", CLng(uploadedFile.ContentLength))
                            cmdIns.Parameters.AddWithValue("@UploadedBy", If(Context.User.Identity.IsAuthenticated, Context.User.Identity.Name, "anonymous"))
                            cmdIns.ExecuteNonQuery()
                        End Using
                        tran.Commit()
                    End Using
                End Using

                Response.Redirect("Kategori.aspx?kursusId=" & HttpUtility.UrlEncode(kursusId.ToString()), True)
            Catch ex As Exception
                Page.ClientScript.RegisterStartupScript(Me.GetType(), "uploadError", $"alert('Upload failed: {HttpUtility.HtmlEncode(ex.Message)}');")
            End Try
        End Sub
    End Class
