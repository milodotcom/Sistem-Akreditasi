Imports System.IO
Imports System.Text
Imports System.Web

Partial Class RepositoryHandle
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim root = Server.MapPath("~/App_Data/UserRepositories/")
        Dim virtualPath = Request.QueryString("path")
        Dim viewMode = Request.QueryString("view")
        Dim physPath = If(String.IsNullOrEmpty(virtualPath), root, Path.Combine(root, virtualPath))
        If Not Directory.Exists(physPath) Then physPath = root

        ' Handle delete/rename
        Dim action = Request.QueryString("action")
        Dim itemType = Request.QueryString("type")   ' "folder" or "file"
        Dim itemName = Request.QueryString("item")
        Dim newName = Request.QueryString("newName")

        If Not String.IsNullOrEmpty(action) AndAlso Not String.IsNullOrEmpty(itemName) Then
            If action = "delete" Then
                If itemType = "folder" Then
                    Dim dp = Path.Combine(physPath, itemName)
                    If Directory.Exists(dp) Then Directory.Delete(dp, True)
                Else
                    Dim fp = Path.Combine(physPath, itemName)
                    If File.Exists(fp) Then File.Delete(fp)
                End If
            ElseIf action = "rename" AndAlso Not String.IsNullOrEmpty(newName) Then
                newName = Path.GetFileName(newName)
                If itemType = "folder" Then
                    Dim oldP = Path.Combine(physPath, itemName)
                    Dim newP = Path.Combine(physPath, newName)
                    If Directory.Exists(oldP) AndAlso Not Directory.Exists(newP) Then Directory.Move(oldP, newP)
                Else
                    Dim oldF = Path.Combine(physPath, itemName)
                    Dim newF = Path.Combine(physPath, newName)
                    If File.Exists(oldF) AndAlso Not File.Exists(newF) Then File.Move(oldF, newF)
                End If
            End If

            ' Redirect back (preserve view and path)
            Dim qs = "?path=" & HttpUtility.UrlEncode(virtualPath) & "&view=" & viewMode
            Response.Redirect("RepositoryHandle.aspx" & qs)
            Return
        End If

        If Not IsPostBack Then
            LoadFoldersAndFiles(physPath, virtualPath, viewMode)
        End If
    End Sub

    Private Sub LoadFoldersAndFiles(folderPhysicalPath As String, virtualPath As String, viewMode As String)
        Dim sb As New StringBuilder()
        Dim isList = (viewMode = "list")

        If isList Then
            ' --- LIST VIEW: Bootstrap table ---
            sb.AppendLine("<table class='table table-striped'>")
            sb.AppendLine("  <thead><tr>")
            sb.AppendLine("    <th>Name</th><th>Type</th><th>Size</th><th>Date Modified</th><th>Actions</th>")
            sb.AppendLine("  </tr></thead>")
            sb.AppendLine("  <tbody>")

            ' Folders
            For Each folderPath As String In Directory.GetDirectories(folderPhysicalPath)
                Dim nm = Path.GetFileName(folderPath)
                Dim newV = If(String.IsNullOrEmpty(virtualPath), nm, virtualPath & "/" & nm)
                Dim modTime = Directory.GetLastWriteTime(folderPath).ToString("dd MMM yyyy HH:mm")

                sb.AppendLine("    <tr>")
                sb.AppendLine("      <td><a href='?path=" & HttpUtility.UrlEncode(newV) & "&view=list'>" _
                              & HttpUtility.HtmlEncode(nm) & "</a></td>")
                sb.AppendLine("      <td>Folder</td>")
                sb.AppendLine("      <td>—</td>")
                sb.AppendLine("      <td>" & modTime & "</td>")
                sb.AppendLine("      <td>")
                sb.AppendLine($"        <button class='btn btn-sm btn-outline-primary me-1' onclick=""doRename('folder','{HttpUtility.UrlEncode(virtualPath)}','{HttpUtility.HtmlEncode(nm)}')"">Rename</button>")
                sb.AppendLine($"        <button class='btn btn-sm btn-outline-danger' onclick=""doDelete('folder','{HttpUtility.UrlEncode(virtualPath)}','{HttpUtility.HtmlEncode(nm)}')"">Delete</button>")
                sb.AppendLine("      </td>")

                sb.AppendLine("    </tr>")
            Next

            ' Files
            For Each filePath As String In Directory.GetFiles(folderPhysicalPath)
                Dim fn = Path.GetFileName(filePath)
                Dim fi = New FileInfo(filePath)
                Dim sizeKb = (fi.Length / 1024).ToString("N1") & " KB"
                Dim modTime = fi.LastWriteTime.ToString("dd MMM yyyy HH:mm")

                sb.AppendLine("    <tr>")
                sb.AppendLine("      <td>" & HttpUtility.HtmlEncode(fn) & "</td>")
                sb.AppendLine("      <td>" & HttpUtility.HtmlEncode(fi.Extension.TrimStart(".")) & "</td>")
                sb.AppendLine("      <td>" & sizeKb & "</td>")
                sb.AppendLine("      <td>" & modTime & "</td>")
                sb.AppendLine("      <td>")
                sb.AppendLine($"        <button class='btn btn-sm btn-outline-primary me-1' onclick=""doRename('folder','{HttpUtility.UrlEncode(virtualPath)}','{HttpUtility.HtmlEncode(fn)}')"">Rename</button>")
                sb.AppendLine($"        <button class='btn btn-sm btn-outline-danger' onclick=""doDelete('folder','{HttpUtility.UrlEncode(virtualPath)}','{HttpUtility.HtmlEncode(fn)}')"">Delete</button>")
                sb.AppendLine("      </td>")

                sb.AppendLine("    </tr>")
            Next

            sb.AppendLine("  </tbody>")
            sb.AppendLine("</table>")

        Else
            ' --- TILE VIEW: big icons + name only ---
            sb.AppendLine("<div class='view-tiles'>")
            For Each folderPath As String In Directory.GetDirectories(folderPhysicalPath)
                Dim nm = Path.GetFileName(folderPath)
                Dim newV = If(String.IsNullOrEmpty(virtualPath), nm, virtualPath & "/" & nm)
                sb.AppendLine("  <div class='item-card'>")
                sb.AppendLine("    <a href='?path=" & HttpUtility.UrlEncode(newV) & "&view=tiles'>")
                sb.AppendLine("      <i class='fas fa-folder fa-3x text-warning'></i>")
                sb.AppendLine("    </a>")
                sb.AppendLine("    <span>" & HttpUtility.HtmlEncode(nm) & "</span>")
                sb.AppendLine("  </div>")
            Next
            For Each filePath As String In Directory.GetFiles(folderPhysicalPath)
                Dim fn = Path.GetFileName(filePath)
                sb.AppendLine("  <div class='item-card'>")
                sb.AppendLine("    <i class='fas fa-file fa-3x text-secondary'></i>")
                sb.AppendLine("    <span>" & HttpUtility.HtmlEncode(fn) & "</span>")
                sb.AppendLine("  </div>")
            Next
            sb.AppendLine("</div>")
        End If

        litFolders.Text = sb.ToString()
    End Sub

    Protected Sub btnCreateFolder_Click(sender As Object, e As EventArgs) Handles btnCreateFolder.Click
        ' (unchanged)
    End Sub

    Protected Sub btnUpload_Click(sender As Object, e As EventArgs) Handles btnUpload.Click
        ' (unchanged)
    End Sub
End Class