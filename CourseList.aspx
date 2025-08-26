<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="CourseList.aspx.vb" Inherits="Sistem_Akreditasi.CourseList" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .Repo-bg {
            padding: 20px;
            border-radius: 12px;
            background: linear-gradient(90deg,#E6F7FF,#F3E8FF);
        }

        .square-card {
            width: 150px;
            height: 150px;
            border-radius: 12px;
            margin: auto;
        }

            .square-card .card-body {
                display: flex;
                align-items: center;
                justify-content: center;
            }

        .faculty-link {
            text-decoration: none;
            color: inherit;
        }
    </style>
    <h2>Course List</h2>
    <div class="Repo-bg">
        <asp:Literal ID="litBreadcrumbs" runat="server"></asp:Literal>
        <asp:Literal ID="litMessage" runat="server"></asp:Literal>


        <div class="row row-cols-2 row-cols-md-4 g-4 mt-3">
            <asp:Repeater ID="courseList" runat="server">
                <ItemTemplate>
                    <div class="col">
                        <a class="faculty-link" href='Kategori.aspx?courseId=<%# Eval("KursusID") %>'>
                            <div class="card shadow-sm text-center square-card">
                                <div class="card-body d-flex flex-column justify-content-center">
                                    <h5 class="card-title mb-0"><%# Eval("Kod_Kursus") %></h5>
                                    <small class="text-muted">MQA: <%# Eval("ac04_KodMQA") %></small>
                                </div>
                            </div>
                        </a>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

        </div>
    </div>

</asp:Content>
