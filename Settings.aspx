<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Settings.aspx.vb" Inherits="Sistem_Akreditasi.Settings" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="main-content">
        <!-- Settings Content -->
        <div id="settings-content" >
            <div class="dashboard-header">
                <!---
                <button class="btn btn-outline-secondary mb-3" onclick="goBack(); return false;">
                    <i class="fas fa-arrow-left me-2"></i>
                    Back
                </button>-->
                <h1 class="mb-3">
                    <i class="fas fa-cog me-3 text-primary"></i>
                    Settings
                </h1>
                <p class="text-muted mb-4">Configure your account preferences and application settings.</p>

                <!-- Settings Grid -->
                <div class="row">
                    <div class="col-md-6 mb-4">
                        <div class="card border-0 shadow-sm">
                            <div class="card-body p-4">
                                <h5 class="card-title mb-3">
                                    <i class="fas fa-palette me-2 text-primary"></i>
                                    Theme Settings
                                </h5>
                                <p class="text-muted mb-3">Choose your preferred theme for the application.</p>
                                <div class="d-flex gap-3">
                                    <button class="btn btn-outline-primary active" id="light-mode-btn" onclick="setTheme('light')">
                                        <i class="fas fa-sun me-2"></i>
                                        Light Mode
                                    </button>
                                    <button class="btn btn-outline-primary" id="dark-mode-btn" onclick="setTheme('dark')">
                                        <i class="fas fa-moon me-2"></i>
                                        Dark Mode
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-6 mb-4">
                        <div class="card border-0 shadow-sm">
                            <div class="card-body p-4">
                                <h5 class="card-title mb-3">
                                    <i class="fas fa-font me-2 text-primary"></i>
                                    Font Size
                                </h5>
                                <p class="text-muted mb-3">Adjust the font size for better readability.</p>
                                <div class="d-flex gap-2">
                                    <button class="btn btn-outline-secondary btn-sm" id="small-font-btn" onclick="setFontSize('small')">
                                        <i class="fas fa-font me-1" style="font-size: 0.8rem;"></i>
                                        Small
                                    </button>
                                    <button class="btn btn-outline-secondary btn-sm active" id="medium-font-btn" onclick="setFontSize('medium')">
                                         <i class="fas fa-font me-1"></i>
                                        Medium
                                    </button>
                                    <button class="btn btn-outline-secondary btn-sm" id="large-font-btn" onclick="setFontSize('large')">
                                        <i class="fas fa-font me-1" style="font-size: 1.2rem;"></i>
                                        Large
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>