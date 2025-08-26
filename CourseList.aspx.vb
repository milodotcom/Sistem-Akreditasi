Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration

Partial Class CourseList
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            Dim facultyId As String = Request.QueryString("facultyId")
            If String.IsNullOrEmpty(facultyId) Then
                litMessage.Text = "<div class='alert alert-warning'>No faculty selected. Click a faculty first.</div>"
                Return
            End If

            LoadCourses(facultyId)
            LoadBreadcrumbs(facultyId)
        End If
    End Sub

    Private Sub LoadCourses(facultyId As String)
        Dim connStr As String = ConfigurationManager.ConnectionStrings("AkreditasiDB").ConnectionString
        Dim dt As New DataTable()

        Using conn As New SqlConnection(connStr)
            ' ✅ Only courses registered in MQA (ac04_DaftarRepositori)
            Dim query As String = "
                SELECT k.Kod_Kursus, k.KursusID, r.ac04_KodMQA
                FROM acKursus k
                INNER JOIN ac04_DaftarRepositori r ON k.KursusID = r.KursusID
                WHERE k.Kod_Fakulti = @facultyId
                ORDER BY k.Kod_Kursus"
            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@facultyId", facultyId)
                conn.Open()
                dt.Load(cmd.ExecuteReader())
            End Using
        End Using

        If dt.Rows.Count = 0 Then
            litMessage.Text = $"<div class='alert alert-info'>Tiada kursus MQA dijumpai untuk faculty <strong>{HttpUtility.HtmlEncode(facultyId)}</strong>.</div>"
        Else
            litMessage.Text = $"<div class='alert alert-success'>{dt.Rows.Count} kursus MQA dijumpai untuk faculty <strong>{HttpUtility.HtmlEncode(facultyId)}</strong>.</div>"
        End If

        courseList.DataSource = dt
        courseList.DataBind()
    End Sub

    Private Sub LoadBreadcrumbs(facultyId As String)
        litBreadcrumbs.Text = $"<a href='Faculty.aspx'>Faculty</a> &gt; <strong>{HttpUtility.HtmlEncode(facultyId)}</strong> &gt; Courses"
    End Sub
End Class
