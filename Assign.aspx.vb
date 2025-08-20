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
            Dim sql As String = " SELECT DISTINCT dr.ac04_KodMQA 
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
        ' Validation
        If String.IsNullOrEmpty(ddlRole1.SelectedValue) OrElse String.IsNullOrEmpty(ddlRole2.SelectedValue) Then
            Response.Write("<script>alert('Please select both Kod MQA and Penyelaras.');</script>")
            Exit Sub
        End If

        Dim connStr As String = ConfigurationManager.ConnectionStrings("AkreditasiDB").ConnectionString
        Dim kursusID As Integer

        Using conn As New SqlConnection(connStr)
            ' 1. Get KursusID from ac04_DaftarRepositori based on selected MQA
            Dim lookupSql As String = "SELECT TOP 1 KursusID FROM ac04_DaftarRepositori WHERE ac04_KodMQA = @KodMQA"
            Using cmd As New SqlCommand(lookupSql, conn)
                cmd.Parameters.AddWithValue("@KodMQA", ddlRole1.SelectedValue)
                conn.Open()
                kursusID = Convert.ToInt32(cmd.ExecuteScalar())
                conn.Close()
            End Using
        End Using

        ' 2. Now insert with KursusID
        Dim insertSql As String = "
        INSERT INTO ac02_Assign (ac01_idStaf, ac04_KodMQA, KursusID)
        VALUES (@IdStaf, @KodMQA, @KursusID)
    "

        Using conn As New SqlConnection(connStr)
            Using cmd As New SqlCommand(insertSql, conn)
                cmd.Parameters.AddWithValue("@IdStaf", ddlRole2.SelectedValue)
                cmd.Parameters.AddWithValue("@KodMQA", ddlRole1.SelectedValue)
                cmd.Parameters.AddWithValue("@KursusID", kursusID)

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
            SELECT a.KursusID, a.ac04_KodMQA, a.ac01_idStaf, 
                   p.ac01_nama, k.Kod_Kursus
            FROM ac02_Assign a
            INNER JOIN ac01_Pengguna p ON a.ac01_idStaf = p.ac01_idStaf
            INNER JOIN ac04_DaftarRepositori dr ON a.ac04_KodMQA = dr.ac04_KodMQA
            INNER JOIN acKursus k ON dr.KursusID = k.KursusID
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


    Protected Sub gvAssign_RowDeleting(sender As Object, e As GridViewDeleteEventArgs) Handles gvAssignments.RowDeleting
        Dim kursusID As String = gvAssignments.DataKeys(e.RowIndex).Values("KursusID").ToString()
        Dim kodMQA As String = gvAssignments.DataKeys(e.RowIndex).Values("ac04_KodMQA").ToString()

        Dim query As String = "DELETE FROM ac02_Assign WHERE KursusID=@KursusID AND ac04_KodMQA=@KodMQA"
        Dim connStr As String = ConfigurationManager.ConnectionStrings("AkreditasiDB").ConnectionString
        Using con As New SqlConnection(connStr)
            Using cmd As New SqlCommand(query, con)
                cmd.Parameters.AddWithValue("@KursusID", kursusID)
                cmd.Parameters.AddWithValue("@KodMQA", kodMQA)
                con.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using

        ' Refresh with same query that set up keys
        LoadAssignments()
    End Sub

End Class
