Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration

Partial Class Faculty
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            BindFaculties()
        End If
    End Sub

    Private Sub BindFaculties()
        Dim connStr As String = ConfigurationManager.ConnectionStrings("AkreditasiDB").ConnectionString
        Dim dt As New DataTable()

        Using conn As New SqlConnection(connStr)
            ' get distinct faculty codes from acKursus
            Using cmd As New SqlCommand("SELECT DISTINCT Kod_Fakulti FROM acKursus ORDER BY Kod_Fakulti", conn)
                conn.Open()
                dt.Load(cmd.ExecuteReader())
            End Using
        End Using

        FacultyRepeater.DataSource = dt
        FacultyRepeater.DataBind()
    End Sub
End Class
