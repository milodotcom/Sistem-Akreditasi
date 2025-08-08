<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DaftarR.aspx.vb" Inherits="Sistem_Akreditasi.DaftarR" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!DOCTYPE html>

    <html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <title></title>
    </head>
    <body>
        <div class="container py-5">
            <!-- Outer container -->
            <div class="bg-light p-4 rounded shadow-sm">
                <!-- Inner styled container -->
                <h2 class="mb-4">Daftar Repositori</h2>

                <div class="row mb-3">
                    <div class="col-md-6">
                        <label for="ddlRole1" class="form-label">Kod MQA </label>
                        <asp:TextBox ID="txtKodMQA" runat="server" CssClass="form-control" placeholder="Masukkan Kod MQA" />
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-md-6">
                        <label for="ddlRole2" class="form-label">Fakulti </label>
                        <asp:DropDownList
                            ID="ddlRole2"
                            runat="server"
                            AutoPostBack="True"
                            OnSelectedIndexChanged="ddlRole2_SelectedIndexChanged">
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-md-6">
                        <label for="ddlRole2" class="form-label">Kursus </label>
                        <asp:DropDownList
                            ID="ddlRole3"
                            runat="server">
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-md-6">
                        <label for="txtStartDate" class="form-label">Tarikh Mula</label>
                        <asp:TextBox ID="txtStartDate" runat="server" CssClass="form-control" TextMode="Date" />
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-md-6">
                        <label for="txtEndDate" class="form-label">Tarikh Tamat</label>
                        <asp:TextBox ID="txtEndDate" runat="server" CssClass="form-control" TextMode="Date" />
                    </div>
                </div>


                <div class="row mb-3">
                    <div class="col-md-6">
                        <label for="ddlRole2" class="form-label">Keterangan </label>
                        <asp:TextBox ID="ddlRoles6" runat="server" CssClass="form-control auto-resize"
                            TextMode="MultiLine"
                            Rows="1"></asp:TextBox>
                    </div>
                </div>

                <asp:Button ID="save" runat="server" Text="Save" OnClick="save_Click" />


            </div>
        </div>
    </body>
    </html>
</asp:Content>
