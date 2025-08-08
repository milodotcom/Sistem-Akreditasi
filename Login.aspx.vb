Imports System.Data.SqlClient
Imports System.Configuration

Partial Class Login
    Inherits System.Web.UI.Page

    Protected Sub btnLogin_Click(sender As Object, e As EventArgs) Handles btnLogin.Click
        Dim connStr As String = ConfigurationManager.ConnectionStrings("AkreditasiDB").ConnectionString

        Using conn As New SqlConnection(connStr)
            Dim sql As String = "SELECT ac01_idStaf 
                                 FROM ac01_Pengguna 
                                 WHERE ac01_idStaf = @u AND ac01_password = @p"

            Using cmd As New SqlCommand(sql, conn)
                cmd.Parameters.Add("@u", SqlDbType.NVarChar, 50).Value = txtUsername.Text.Trim()

                cmd.Parameters.Add("@p", SqlDbType.NVarChar, 50).Value = txtPassword.Text.Trim()

                conn.Open()
                Dim result As Object = cmd.ExecuteScalar()

                If result IsNot Nothing Then
                    Session("idStaf") = result.ToString()

                    Session("username") = txtUsername.Text.Trim()
                    Response.Redirect("DaftarR.aspx", False) ' False avoids thread abort
                Else
                    lblMessage.Text = "Invalid username or password."
                End If
            End Using
        End Using
    End Sub
End Class
