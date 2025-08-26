Imports System.Data
Imports System.Data.SqlClient
Imports System.Text
Imports System.Web
Imports System.Configuration

Partial Class RepositoryHandle
    Inherits System.Web.UI.Page

    Private ReadOnly Property ConnStr As String
        Get
            Return ConfigurationManager.ConnectionStrings("AkreditasiDB").ConnectionString
        End Get
    End Property

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim pathParam = Request.QueryString("path") ' "catId" or "catId/subId"
        Dim viewMode = Request.QueryString("view")

        Dim action = Request.QueryString("action")
        Dim itemType = Request.QueryString("type")   ' "folder" | "sub"
        Dim item = Request.QueryString("item")
        Dim newName = Request.QueryString("newName")

        If Not String.IsNullOrEmpty(action) AndAlso Not String.IsNullOrEmpty(item) Then
            Try
                If action = "delete" Then
                    Select Case itemType
                        Case "folder"
                            DeleteCategoryById(item)
                        Case "sub"
                            DeleteSubcategoryById(item)
                    End Select
                ElseIf action = "rename" AndAlso Not String.IsNullOrEmpty(newName) Then
                    Select Case itemType
                        Case "folder"
                            RenameCategoryById(item, newName)
                        Case "sub"
                            RenameSubcategoryById(item, newName)
                    End Select
                End If
            Catch ex As Exception
                ' Optional: log ex.Message
            End Try

            Dim qs = "?path=" & HttpUtility.UrlEncode(pathParam) & "&view=" & HttpUtility.UrlEncode(viewMode)
            Response.Redirect("RepositoryHandle.aspx" & qs)
            Return
        End If

        If Not IsPostBack Then
            LoadFoldersAndFiles(pathParam, viewMode)
        End If
    End Sub

    ' ------------------------
    ' Delete / Rename helpers (DB-only: tbl_kategori + tbl_subkategori)
    ' ------------------------
    Private Sub DeleteCategoryById(idStr As String)
        Dim id As Integer
        If Integer.TryParse(idStr, id) Then
            Using conn As New SqlConnection(ConnStr)
                conn.Open()
                Using tran = conn.BeginTransaction()
                    ' delete subcategories under category
                    Using cmd2 As New SqlCommand("DELETE FROM tbl_subkategori WHERE id_kat = @id", conn, tran)
                        cmd2.Parameters.AddWithValue("@id", id)
                        cmd2.ExecuteNonQuery()
                    End Using
                    ' delete category
                    Using cmd3 As New SqlCommand("DELETE FROM tbl_kategori WHERE id_kat = @id", conn, tran)
                        cmd3.Parameters.AddWithValue("@id", id)
                        cmd3.ExecuteNonQuery()
                    End Using
                    tran.Commit()
                End Using
            End Using
        End If
    End Sub

    Private Sub DeleteSubcategoryById(idStr As String)
        Dim id As Integer
        If Integer.TryParse(idStr, id) Then
            Using conn As New SqlConnection(ConnStr)
                conn.Open()
                Using cmd As New SqlCommand("DELETE FROM tbl_subkategori WHERE id_sub = @id_sub", conn)
                    cmd.Parameters.AddWithValue("@id_sub", id)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End If
    End Sub

    Private Sub RenameCategoryById(idStr As String, newName As String)
        Dim id As Integer
        newName = newName.Trim()
        If newName = "" Then Return
        If Integer.TryParse(idStr, id) Then
            Using conn As New SqlConnection(ConnStr)
                conn.Open()
                Using cmd As New SqlCommand("UPDATE tbl_kategori SET NamaKategori = @newName WHERE id_kat = @id", conn)
                    cmd.Parameters.AddWithValue("@newName", newName)
                    cmd.Parameters.AddWithValue("@id", id)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End If
    End Sub

    Private Sub RenameSubcategoryById(idStr As String, newName As String)
        Dim id As Integer
        newName = newName.Trim()
        If newName = "" Then Return
        If Integer.TryParse(idStr, id) Then
            Using conn As New SqlConnection(ConnStr)
                conn.Open()
                Using cmd As New SqlCommand("UPDATE tbl_subkategori SET NamaSub = @newName WHERE id_sub = @id_sub", conn)
                    cmd.Parameters.AddWithValue("@newName", newName)
                    cmd.Parameters.AddWithValue("@id_sub", id)
                    cmd.ExecuteNonQuery()
                End Using
            End Using
        End If
    End Sub

    ' ------------------------
    ' Load listing from DB (categories and subcategories only)
    ' ------------------------
    Private Sub LoadFoldersAndFiles(pathParam As String, viewMode As String)
        Dim sb As New StringBuilder()
        Dim isList = (viewMode = "list")

        ' parse pathParam as "catId" or "catId/subId"
        Dim currentCatId As Integer = 0
        Dim currentSubId As Integer = 0
        If Not String.IsNullOrEmpty(pathParam) Then
            Dim parts = pathParam.Split("/"c)
            Integer.TryParse(parts(0), currentCatId)
            If parts.Length > 1 Then Integer.TryParse(parts(1), currentSubId)
        End If

        If isList Then
            sb.AppendLine("<table class='table table-striped'>")
            sb.AppendLine("<thead><tr><th>Name</th><th>Type</th><th>Details</th><th>Actions</th></tr></thead>")
            sb.AppendLine("<tbody>")

            Dim catCount = 0
            Dim subCount = 0

            If currentCatId = 0 Then
                ' root -> list categories (tbl_kategori)
                Using conn As New SqlConnection(ConnStr)
                    conn.Open()
                    Using cmd As New SqlCommand("SELECT id_kat, NamaKategori FROM tbl_kategori ORDER BY NamaKategori", conn)
                        Using rdr = cmd.ExecuteReader()
                            While rdr.Read()
                                catCount += 1
                                Dim id_kat = Convert.ToInt32(rdr("id_kat"))
                                Dim name = Convert.ToString(rdr("NamaKategori"))
                                Dim encPath = HttpUtility.UrlEncode(id_kat.ToString())

                                sb.AppendLine("<tr>")
                                sb.AppendLine($"  <td><a href='?path={encPath}&view=list'>{HttpUtility.HtmlEncode(name)}</a></td>")
                                sb.AppendLine("  <td>Folder</td>")
                                sb.AppendLine("  <td>—</td>")
                                sb.AppendLine("  <td>")
                                sb.AppendLine($"    <button class='btn btn-sm btn-outline-primary me-1' onclick=""doRename('folder','{encPath}','{id_kat}')"">Rename</button>")
                                sb.AppendLine($"    <button class='btn btn-sm btn-outline-danger' onclick=""doDelete('folder','{encPath}','{id_kat}')"">Delete</button>")
                                sb.AppendLine("  </td>")
                                sb.AppendLine("</tr>")
                            End While
                        End Using
                    End Using
                End Using
            Else
                ' inside a category -> show subcategories
                Using conn As New SqlConnection(ConnStr)
                    conn.Open()
                    Using cmd As New SqlCommand("SELECT id_sub, NamaSub FROM tbl_subkategori WHERE id_kat = @id_kat ORDER BY NamaSub", conn)
                        cmd.Parameters.AddWithValue("@id_kat", currentCatId)
                        Using rdr = cmd.ExecuteReader()
                            While rdr.Read()
                                subCount += 1
                                Dim id_sub = Convert.ToInt32(rdr("id_sub"))
                                Dim name = Convert.ToString(rdr("NamaSub"))
                                Dim encPath = HttpUtility.UrlEncode(currentCatId.ToString() & "/" & id_sub.ToString())

                                sb.AppendLine("<tr>")
                                sb.AppendLine($"  <td><a href='?path={encPath}&view=list'>{HttpUtility.HtmlEncode(name)}</a></td>")
                                sb.AppendLine("  <td>Subfolder</td>")
                                sb.AppendLine("  <td>—</td>")
                                sb.AppendLine("  <td>")
                                sb.AppendLine($"    <button class='btn btn-sm btn-outline-primary me-1' onclick=""doRename('sub','{encPath}','{id_sub}')"">Rename</button>")
                                sb.AppendLine($"    <button class='btn btn-sm btn-outline-danger' onclick=""doDelete('sub','{encPath}','{id_sub}')"">Delete</button>")
                                sb.AppendLine("  </td>")
                                sb.AppendLine("</tr>")
                            End While
                        End Using
                    End Using
                End Using

                ' If a subId is present, show a message (no files stored in DB)
                If currentSubId > 0 Then
                    sb.AppendLine("<tr><td colspan='4' class='text-muted'>Subkategori dipilih. Tiada dokumen disimpan dalam sistem ini.</td></tr>")
                End If
            End If

            If catCount = 0 AndAlso subCount = 0 Then
                sb.AppendLine("<tr><td colspan='4' class='text-muted'>Folder ini kosong.</td></tr>")
            End If

            sb.AppendLine("</tbody></table>")

        Else
            ' Tile view (categories or subcategories)
            sb.AppendLine("<div class='view-tiles'>")
            Dim anyItem As Boolean = False

            If currentCatId = 0 Then
                Using conn As New SqlConnection(ConnStr)
                    conn.Open()
                    Using cmd As New SqlCommand("SELECT id_kat, NamaKategori FROM tbl_kategori ORDER BY NamaKategori", conn)
                        Using rdr = cmd.ExecuteReader()
                            While rdr.Read()
                                anyItem = True
                                Dim id_kat = Convert.ToInt32(rdr("id_kat"))
                                Dim name = Convert.ToString(rdr("NamaKategori"))
                                Dim encPath = HttpUtility.UrlEncode(id_kat.ToString())
                                sb.AppendLine("<div class='item-card'>")
                                sb.AppendLine($"  <a href='?path={encPath}&view=tiles'>")
                                sb.AppendLine("    <i class='fas fa-folder fa-3x text-warning'></i>")
                                sb.AppendLine("  </a>")
                                sb.AppendLine($"  <span>{HttpUtility.HtmlEncode(name)}</span>")
                                sb.AppendLine("</div>")
                            End While
                        End Using
                    End Using
                End Using
            Else
                Using conn As New SqlConnection(ConnStr)
                    conn.Open()
                    Using cmd As New SqlCommand("SELECT id_sub, NamaSub FROM tbl_subkategori WHERE id_kat = @id_kat ORDER BY NamaSub", conn)
                        cmd.Parameters.AddWithValue("@id_kat", currentCatId)
                        Using rdr = cmd.ExecuteReader()
                            While rdr.Read()
                                anyItem = True
                                Dim id_sub = Convert.ToInt32(rdr("id_sub"))
                                Dim name = Convert.ToString(rdr("NamaSub"))
                                Dim encPath = HttpUtility.UrlEncode(currentCatId.ToString() & "/" & id_sub.ToString())
                                sb.AppendLine("<div class='item-card'>")
                                sb.AppendLine($"  <a href='?path={encPath}&view=tiles'>")
                                sb.AppendLine("    <i class='fas fa-folder fa-3x text-warning'></i>")
                                sb.AppendLine("  </a>")
                                sb.AppendLine($"  <span>{HttpUtility.HtmlEncode(name)}</span>")
                                sb.AppendLine("</div>")
                            End While
                        End Using
                    End Using
                End Using
            End If

            If Not anyItem Then
                sb.AppendLine("<div class='text-muted'>Folder ini kosong.</div>")
            End If

            sb.AppendLine("</div>")
        End If

        litFolders.Text = sb.ToString()
    End Sub
End Class
