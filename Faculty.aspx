<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Faculty.aspx.vb" Inherits="Sistem_Akreditasi.Faculty" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h1>Faculty List</h1>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />

    <style>
        .Repo-bg { padding:20px; border-radius:12px; background: linear-gradient(90deg,#E6F7FF,#F3E8FF); }
        .square-card { width:150px; height:150px; border-radius:12px; margin:auto; }
        .square-card .card-body { display:flex; align-items:center; justify-content:center; }
        .faculty-link { text-decoration:none; color:inherit; }
    </style>

    <div class="Repo-bg">
        <div class="row row-cols-2 row-cols-md-4 g-4">
            <asp:Repeater ID="FacultyRepeater" runat="server">
                <ItemTemplate>
                    <div class="col">
                        <a class="faculty-link" href='CourseList.aspx?facultyId=<%# Eval("Kod_Fakulti") %>'>
                            <div class="card shadow-sm text-center square-card">
                                <div class="card-body d-flex flex-column justify-content-center">
                                    <h5 class="card-title mb-0"><%# Eval("Kod_Fakulti") %></h5>
                                </div>
                            </div>
                        </a>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>
