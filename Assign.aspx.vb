Imports System.Data.SqlClient
Imports System.Configuration

Partial Class Assign
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadKodMQA()
            LoadPenyelaras()
            LoadAssignments()
        End If
        LoadAssignments()
    End Sub

    Private Sub LoadKodMQA()
        Dim connStr As String = ConfigurationManager.ConnectionStrings("AkreditasiDB").ConnectionString
        Using conn As New SqlConnection(connStr)
            Dim sql As String = "
    SELECT DISTINCT dr.ac04_KodMQA
    FROM ac04_DaftarRepositori dr
    INNER JOIN acKursus k ON dr.KursusID = k.KursusID
"


            Using cmd As New SqlCommand(sql, conn)
                conn.Open()
                Using rdr As SqlDataReader = cmd.ExecuteReader()
                    ddlRole1.DataSource = rdr
                    ddlRole1.DataTextField = "ac04_KodMQA"   ' what the user will see
                    ddlRole1.DataValueField = "ac04_KodMQA"  ' what will be saved
                    ddlRole1.DataBind()
                End Using
            End Using
        End Using

        ddlRole1.Items.Insert(0, New ListItem("-- Pilih Kod MQA --", ""))
    End Sub

    Private Sub LoadPenyelaras()
        Dim connStr As String = ConfigurationManager.ConnectionStrings("AkreditasiDB").ConnectionString
        Using conn As New SqlConnection(connStr)
            Dim sql As String = "SELECT ac01_idStaf, ac01_nama, 
                             ac01_idStaf + ' - ' + ac01_nama AS DisplayText
                     FROM ac01_Pengguna
                     WHERE ac01_Tahap = 3"
            Using cmd As New SqlCommand(sql, conn)
                conn.Open()
                Using rdr As SqlDataReader = cmd.ExecuteReader()
                    ddlRole2.DataSource = rdr
                    ddlRole2.DataTextField = "DisplayText"   ' shows "idStaf - nama"
                    ddlRole2.DataValueField = "ac01_idStaf"  ' value still idStaf
                    ddlRole2.DataBind()
                End Using
            End Using
        End Using
        ddlRole2.Items.Insert(0, New ListItem("-- Pilih Penyelaras --", ""))
    End Sub

    Protected Sub save_Click(sender As Object, e As EventArgs)
        ' Validation to ensure both are selected
        If String.IsNullOrEmpty(ddlRole1.SelectedValue) OrElse String.IsNullOrEmpty(ddlRole2.SelectedValue) Then
            Response.Write("<script>alert('Please select both Kod Kursus and Penyelaras.');</script>")
            Exit Sub
        End If

        Dim connStr As String = ConfigurationManager.ConnectionStrings("AkreditasiDB").ConnectionString
        Dim sql As String = "
        INSERT INTO ac02_Assign (ac01_idStaf, ac04_KodMQA)
        VALUES (@IdStaf, @KodMQA)
    "

        Using conn As New SqlConnection(connStr)
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@IdStaf", ddlRole2.SelectedValue)
                cmd.Parameters.AddWithValue("@KodMQA", ddlRole1.SelectedValue)

                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
        LoadAssignments()
        Response.Write("<script>alert('Assignment saved successfully!');</script>")
    End Sub

    Private Sub LoadAssignments()
        Dim connStr As String = ConfigurationManager.ConnectionStrings("AkreditasiDB").ConnectionString
        Using conn As New SqlConnection(connStr)
            Dim sql As String = "
            SELECT a.ac01_idStaf, p.ac01_nama, a.ac02_KodKursus, a.ac04_KodMQA
            FROM ac02_Assign a
            INNER JOIN ac01_Pengguna p ON a.ac01_idStaf = p.ac01_idStaf
        "
            Using cmd As New SqlCommand(sql, conn)
                conn.Open()
                Using rdr As SqlDataReader = cmd.ExecuteReader()
                    gvAssignments.DataSource = rdr
                    gvAssignments.DataBind()
                End Using
            End Using
        End Using
    End Sub


End Class
