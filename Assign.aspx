<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Assign.aspx.vb" Inherits="Sistem_Akreditasi.Assign" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <!DOCTYPE html>

    <html xmlns="http://www.w3.org/1999/xhtml">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <title>Repository Manager - Akreditasi</title>
        <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
        <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.4.0/css/all.min.css" rel="stylesheet" />
        <link href="Content/repository-manager.css" rel="stylesheet" type="text/css" />
    </head>
    <body>
        <div class="container py-5">
            <!-- Outer container -->
            <div class="bg-light p-4 rounded shadow-sm">
                <!-- Inner styled container -->
                <h2 class="mb-4">Assign Penyelaras</h2>

                <div class="row mb-3">
                    <div class="col-md-6">
                        <label for="ddlRole1" class="form-label">Kod MQA </label>
                        <asp:DropDownList ID="ddlRole1" runat="server" CssClass="form-select select1">
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="row mb-3">
                    <div class="col-md-6">
                        <label for="ddlRole2" class="form-label">Nama Penyelaras </label>
                        <asp:DropDownList ID="ddlRole2" runat="server" CssClass="form-select select2">
                        </asp:DropDownList>
                    </div>
                </div>

                <asp:Button ID="save" runat="server" Text="Save" OnClick="save_Click" />

                <hr class="my-4" />

                <!-- List view / table -->
                <h4>Senarai Assignments</h4>
                <asp:GridView ID="gvAssignments" runat="server" AutoGenerateColumns="False"
                    DataKeyNames="KursusID,ac04_KodMQA"
                    OnRowDeleting="gvAssign_RowDeleting" CssClass="table table-bordered">

                    <Columns>
                        <asp:BoundField DataField="ac01_idStaf" HeaderText="ID Staf" />
                        <asp:BoundField DataField="ac01_nama" HeaderText="Nama" />
                        <asp:BoundField DataField="Kod_Kursus" HeaderText="Kod Kursus" />
                        <asp:BoundField DataField="ac04_KodMQA" HeaderText="Kod MQA" />

                        <asp:CommandField HeaderText="Action" ShowDeleteButton="True" />
                    </Columns>
                </asp:GridView>

            </div>
        </div>


        <!-- Bootstrap JS -->
        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
        <script>
            $(document).ready(function () {
                $('#<%= ddlRole1.ClientID %>').select2({
                    placeholder: "-- Pilih Kod MQA --",
                    allowClear: true,
                    width: '100%'
                });
                $('#<%= ddlRole2.ClientID %>').select2({
                    placeholder: "-- Pilih Penyelaras --",
                    allowClear: true,
                    width: '100%'
                });
            });
        </script>

    </body>
    </html>
</asp:Content>
