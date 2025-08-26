<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="RepositoryHandle.aspx.vb" Inherits="Sistem_Akreditasi.RepositoryHandle" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <style>
        .Repo-header {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 1rem;
        }


        .Repo-bg {
            background: linear-gradient(-45deg, #B6E9FA, #B8E2F2, #BEDEEB, #D6F0FF, #DAECED);
            background-size: 400% 400%;
            border-radius: 15px;
            padding: 30px;
            margin-bottom: 30px;
            box-shadow: 0 5px 15px rgba(0,0,0,0.08);
            animation: skyGradient 10s ease infinite;
            position: relative;
            overflow: hidden;
        }

        @keyframes skyGradient {
            0% {
                background-position: 0% 50%;
            }

            50% {
                background-position: 100% 50%;
            }

            100% {
                background-position: 0% 50%;
            }
        }

        .repo-nav {
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-bottom: 1rem;
        }

        .item-card {
            display: flex;
            flex-direction: column; /* ← stack children vertically */
            align-items: center; /* ← center horizontally */
            justify-content: center;
            padding: 1rem;
            border-radius: .5rem;
            text-align: center;
            cursor: pointer;
            transition: background .2s, transform .2s;
            text-decoration: none;
            color: inherit;
        }

            .item-card:hover {
                background: #f8f9fa;
                transform: translateY(-3px);
            }

        .item-label {
            display: block;
            margin-top: .5rem;
            text-align: center;
        }

        .view-tiles {
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(120px, 1fr));
            gap: 1rem;
        }

        .view-list {
            display: flex;
            flex-direction: column;
        }

            .view-list .item-card {
                display: flex;
                align-items: center;
                gap: .75rem;
                justify-content: space-between;
            }

        .details {
            font-size: .8rem;
            color: #777;
            text-align: left;
            margin-top: .5rem;
        }
    </style>

    <div class="Repo-header">
        <div>
            <h1><i class="fas fa-database me-3 text-primary"></i>Repository</h1>
            <p class="text-muted mb-0">Organize and manage your files and folders</p>
        </div>
    </div>

    <div class="Repo-bg">
        <div class="repo-nav">
            <div>
                <% If Not String.IsNullOrEmpty(Request.QueryString("path")) Then %>
                <a href="javascript:history.back()" class="btn btn-link text-secondary">
                    <i class="fas fa-arrow-left"></i>Back
                </a>
                <% End If %>
            </div>
            <div class="btn-group" role="group">
                <button type="button" class="btn btn-outline-secondary btn-sm"
                    onclick="location = '?path=<%= HttpUtility.UrlEncode(Request.QueryString("path")) %>&view=tiles'">
                    <i class="fas fa-th-large"></i>
                </button>
                <button type="button" class="btn btn-outline-secondary btn-sm"
                    onclick="location = '?path=<%= HttpUtility.UrlEncode(Request.QueryString("path")) %>&view=list'">
                    <i class="fas fa-list"></i>
                </button>
            </div>
        </div>

        <div class="input-group mb-3">
            <input type="text" id="searchInput" class="form-control" placeholder="Search folders or files..." />
            <span class="input-group-text"><i class="fas fa-search"></i></span>
        </div>

        <hr />

        <div id="fileContainer">
            <asp:Literal ID="litFolders" runat="server" />
        </div>
    </div>

    <script>
        // Search filter
        document.getElementById('searchInput').addEventListener('input', function () {
            const term = this.value.toLowerCase();
            document.querySelectorAll('.item-card, table tbody tr').forEach(el => {
                const text = el.textContent.toLowerCase();
                el.style.display = text.includes(term) ? '' : 'none';
            });
        });

        // Rename/Delete
        function doDelete(type, path, name) {
            if (!confirm('Delete ' + type + ' "' + name + '"?')) return;
            location.href = '?path=' + encodeURIComponent(path)
                + '&view=list&action=delete&type=' + type
                + '&item=' + encodeURIComponent(name);
        }
        function doRename(type, path, name) {
            const newName = prompt('Rename ' + type + ' "' + name + '" to:', name);
            if (!newName || newName === name) return;
            location.href = '?path=' + encodeURIComponent(path)
                + '&view=list&action=rename&type=' + type
                + '&item=' + encodeURIComponent(name)
                + '&newName=' + encodeURIComponent(newName);
        }
    </script>
</asp:Content>