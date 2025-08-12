Imports System.Data.SqlClient
Imports System.Configuration

Public Class DaftarR
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load

        If Session("idStaf") Is Nothing Then
            Response.Redirect("Login.aspx") ' kick back to login if not logged in
            Return
        End If

        If Not IsPostBack Then
            LoadFakulti()
            ' Start with an empty Kursus dropdown
            ddlRole3.Items.Clear()
            ddlRole3.Items.Insert(0, New ListItem("-- Pilih Kursus --", ""))
            LoadRepositori()
        End If
    End Sub

    ' Try to load Fakulti from a Fakulti table if it exists,
    ' otherwise populate distinct Kod_Fakulti from acKursus (fallback).
    Private Sub LoadFakulti()
        Dim connStr As String = ConfigurationManager.ConnectionStrings("AkreditasiDB").ConnectionString

        Using conn As New SqlConnection(connStr)
            Dim query As String = "SELECT Kod_Fakulti, Nama_Fakulti FROM Fakulti ORDER BY Nama_Fakulti"
            Dim usedFallback As Boolean = False

            Try
                Using cmd As New SqlCommand(query, conn)
                    conn.Open()
                    Dim reader As SqlDataReader = cmd.ExecuteReader()
                    ddlRole2.DataSource = reader
                    ddlRole2.DataTextField = "Nama_Fakulti"
                    ddlRole2.DataValueField = "Kod_Fakulti"
                    ddlRole2.DataBind()
                    reader.Close()
                End Using
            Catch ex As SqlException
                ' Fakulti table probably doesn't exist — fallback to distinct Kod_Fakulti from acKursus
                usedFallback = True
            Finally
                If conn.State <> ConnectionState.Closed Then conn.Close()
            End Try

            If usedFallback Then
                Dim q2 As String = "SELECT DISTINCT Kod_Fakulti FROM acKursus ORDER BY Kod_Fakulti"
                Using cmd2 As New SqlCommand(q2, conn)
                    conn.Open()
                    Dim reader2 As SqlDataReader = cmd2.ExecuteReader()
                    ddlRole2.DataSource = reader2
                    ddlRole2.DataTextField = "Kod_Fakulti"   ' no separate name available, show the code
                    ddlRole2.DataValueField = "Kod_Fakulti"
                    ddlRole2.DataBind()
                    reader2.Close()
                End Using
                If conn.State <> ConnectionState.Closed Then conn.Close()
            End If
        End Using

        ddlRole2.Items.Insert(0, New ListItem("-- Pilih Fakulti --", ""))
    End Sub

    ' Fired when the user picks a fakulti 
    Protected Sub ddlRole2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ddlRole2.SelectedIndexChanged
        If String.IsNullOrEmpty(ddlRole2.SelectedValue) Then
            ddlRole3.Items.Clear()
            ddlRole3.Items.Insert(0, New ListItem("-- Pilih Kursus --", ""))
        Else
            LoadKursus(ddlRole2.SelectedValue)
        End If
    End Sub

    ' Load courses from acKursus using Kod_Fakulti and bind to ddlRole3
    Private Sub LoadKursus(kodFakulti As String)
        Dim connStr As String = ConfigurationManager.ConnectionStrings("AkreditasiDB").ConnectionString
        Using conn As New SqlConnection(connStr)
            Dim query As String = "SELECT KursusID, Nama_Kursus FROM acKursus WHERE Kod_Fakulti = @KodFakulti ORDER BY Nama_Kursus"
            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@KodFakulti", kodFakulti)
                conn.Open()
                Dim reader As SqlDataReader = cmd.ExecuteReader()
                ddlRole3.DataSource = reader
                ddlRole3.DataTextField = "Nama_Kursus"   ' display name
                ddlRole3.DataValueField = "KursusID"     ' store the PK for insertion
                ddlRole3.DataBind()
                reader.Close()
            End Using
        End Using
        ddlRole3.Items.Insert(0, New ListItem("-- Pilih Kursus --", ""))
    End Sub


    ' Save - inserts Kod_Kursus (foreign key) into ac04_DaftarRepositori
    Protected Sub save_Click(sender As Object, e As EventArgs) Handles save.Click
        Dim connStr As String = ConfigurationManager.ConnectionStrings("AkreditasiDB").ConnectionString

        If Session("idStaf") Is Nothing Then
            Response.Redirect("Login.aspx?sessionExpired=1")
            Exit Sub
        End If

        Dim staffId As String = Convert.ToString(Session("idStaf"))



        ' Parse dates safely
        Dim dtStart As Object = DBNull.Value
        Dim dtEnd As Object = DBNull.Value
        Dim tmpDate As DateTime

        If DateTime.TryParse(txtStartDate.Text, tmpDate) Then
            dtStart = tmpDate
        End If
        If DateTime.TryParse(txtEndDate.Text, tmpDate) Then
            dtEnd = tmpDate
        End If

        Using conn As New SqlConnection(connStr)
            Dim query As String =
                "INSERT INTO ac04_DaftarRepositori
                 (ac04_KodMQA, KursusID, ac04_TarikhMula, ac04_TarikhTamat, ac04_Keterangan, ac01_idStaf)
                 VALUES (@KodMQA, @KursusID, @TarikhMula, @TarikhTamat, @Keterangan, @idStaf)"

            Using cmd As New SqlCommand(query, conn)
                cmd.Parameters.AddWithValue("@KodMQA", txtKodMQA.Text)
                cmd.Parameters.AddWithValue("@KursusID", If(String.IsNullOrEmpty(ddlRole3.SelectedValue), DBNull.Value, ddlRole3.SelectedValue))
                cmd.Parameters.AddWithValue("@TarikhMula", If(dtStart Is DBNull.Value, DBNull.Value, dtStart))
                cmd.Parameters.AddWithValue("@TarikhTamat", If(dtEnd Is DBNull.Value, DBNull.Value, dtEnd))
                ' Keterangan from your dropdown control (you used ddlRoles6 in earlier markup)
                cmd.Parameters.AddWithValue("@Keterangan", If(String.IsNullOrEmpty(ddlRoles6.Text), DBNull.Value, ddlRoles6.Text))
                cmd.Parameters.AddWithValue("@idStaf", staffId)


                If String.IsNullOrEmpty(ddlRole3.SelectedValue) Then
                    Response.Write("<script>alert('Sila pilih kursus sebelum menyimpan.');</script>")
                    Exit Sub
                End If



                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using

        LoadRepositori()
        Response.Write("<script>alert('Data berjaya disimpan!');</script>")
    End Sub

    Private Sub LoadRepositori()
        Dim connStr As String = ConfigurationManager.ConnectionStrings("AkreditasiDB").ConnectionString
        Using conn As New SqlConnection(connStr)
            Dim sql As String = "
            SELECT dr.ac04_id, dr.ac04_KodMQA, k.Nama_Kursus, dr.ac04_TarikhMula, dr.ac04_TarikhTamat, dr.ac04_Keterangan
            FROM ac04_DaftarRepositori dr
            INNER JOIN acKursus k ON dr.KursusID = k.KursusID
        "
            Using cmd As New SqlCommand(sql, conn)
                conn.Open()
                Dim rdr As SqlDataReader = cmd.ExecuteReader()
                gvRepositori.DataSource = rdr
                gvRepositori.DataBind()
            End Using
        End Using
    End Sub

    Protected Sub gvRepositori_RowDeleting(sender As Object, e As GridViewDeleteEventArgs) Handles gvRepositori.RowDeleting
        Dim id As Integer = Convert.ToInt32(gvRepositori.DataKeys(e.RowIndex).Value)
        Dim connStr As String = ConfigurationManager.ConnectionStrings("AkreditasiDB").ConnectionString
        Using conn As New SqlConnection(connStr)
            Dim sql As String = "DELETE FROM ac04_DaftarRepositori WHERE ac04_id = @id"
            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.AddWithValue("@id", id)
                conn.Open()
                cmd.ExecuteNonQuery()
            End Using
        End Using
        LoadRepositori()
    End Sub


End Class
