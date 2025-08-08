<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Dashboard.aspx.vb"
    Inherits="Sistem_Akreditasi.Dashboard" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="main-content">
        <!-- Dashboard Content -->
        <div id="dashboard-content">
            <div class="dashboard-header">
                <h1 class="mb-3">
                    <i class="fas fa-tachometer-alt me-3 text-primary"></i>
                    Dashboard
                </h1>
                <p class="text-muted mb-4">balsamic vinegar</p>
                <div class="text-center">
                    <button class="btn btn-repo"
                        onclick="window.location.href='RepositoryHandle.aspx'; return false;"
                        style="width: 200px; height: 200px; border-radius: 15px; font-size: 1.1rem; display: flex; flex-direction: column; align-items: center; justify-content: center; gap: 15px;">
                        <i class="fas fa-code-branch" style="font-size: 3rem;"></i>
                        <span>Repositories</span>
                    </button>
                </div>
            </div>

            <!-- Back Button -->
            <!--<div id="back-button" style="display: none;">
                <button class="btn btn-outline-secondary mb-3" onclick="goBack(); return false;">
                    <i class="fas fa-arrow-left me-2"></i>
                    Back5
                </button>
            </div> -->

            <!-- Search and Filter -->
            <div class="search-box" id="search-box" style="display: none;">
                <div class="row align-items-center">
                    <div class="col-md-8">
                        <div class="input-group">
                            <span class="input-group-text bg-light border-0">
                                <i class="fas fa-search text-muted"></i>
                            </span>
                            <input type="text" class="form-control border-0 bg-light" placeholder="Search repositories..." id="searchInput" onkeyup="filterRepositories()" />
                        </div>
                    </div>
                    <div class="col-md-4 text-end">
                        <button class="btn btn-repo" onclick="createNewRepo()">
                            <i class="fas fa-plus me-2"></i>
                            New Repository
                        </button>
                    </div>
                </div>
            </div>

            <!-- Repository List -->
            <div id="repository-list" style="display: none;">
                <div class="repo-card" data-name="program-documents">
                    <div class="d-flex justify-content-between align-items-start">
                        <div class="flex-grow-1">
                            <h5 class="mb-2">
                                <i class="fas fa-folder-open me-2 text-primary"></i>
                                Program Document Repository
                            </h5>
                            <p class="text-muted mb-3">Centralized storage for all documents related to program accreditation. Organized by Faculty > Program Level > Program Name > Document Category.</p>
                            <div class="repo-stats">
                                <div class="stat-item">
                                    <i class="fas fa-file-alt text-info"></i>
                                    <span>156 documents</span>
                                </div>
                                <div class="stat-item">
                                    <i class="fas fa-folder text-warning"></i>
                                    <span>12 programs</span>
                                </div>
                                <div class="stat-item">
                                    <i class="fas fa-shield-alt text-success"></i>
                                    <span>Secure Access</span>
                                </div>
                                <div class="stat-item">
                                    <i class="fas fa-clock text-muted"></i>
                                    <span>Updated today</span>
                                </div>
                            </div>
                        </div>
                        <div class="ms-3">
                            <button class="btn btn-repo btn-sm" onclick="openRepository('program-documents'); return false;">
                                <i class="fas fa-external-link-alt me-1"></i>
                                View
                            </button>
                        </div>
                    </div>
                </div>

                <div class="repo-card" data-name="quality-assurance">
                    <div class="d-flex justify-content-between align-items-start">
                        <div class="flex-grow-1">
                            <h5 class="mb-2">
                                <i class="fas fa-folder-open me-2 text-primary"></i>
                                Quality Assurance Records
                            </h5>
                            <p class="text-muted mb-3">MQA compliance documents, audit reports, and quality assurance materials for institutional accreditation.</p>
                            <div class="repo-stats">
                                <div class="stat-item">
                                    <i class="fas fa-file-alt text-info"></i>
                                    <span>89 documents</span>
                                </div>
                                <div class="stat-item">
                                    <i class="fas fa-certificate text-warning"></i>
                                    <span>MQA Certified</span>
                                </div>
                                <div class="stat-item">
                                    <i class="fas fa-shield-alt text-success"></i>
                                    <span>Audit Ready</span>
                                </div>
                                <div class="stat-item">
                                    <i class="fas fa-clock text-muted"></i>
                                    <span>Updated 3 days ago</span>
                                </div>
                            </div>
                        </div>
                        <div class="ms-3">
                            <button class="btn btn-repo btn-sm" onclick="openRepository('quality-assurance'); return false;">
                                <i class="fas fa-external-link-alt me-1"></i>
                                View
                            </button>
                        </div>
                    </div>
                </div>
            </div>


            <!-- File Management Interface -->
            <div id="file-management" style="display: none;">
                <!-- Breadcrumb Navigation -->
                <div class="card mb-3">
                    <div class="card-body py-2">
                        <nav aria-label="breadcrumb">
                            <ol class="breadcrumb mb-0" id="breadcrumb">
                                <li class="breadcrumb-item"><a href="#" onclick="navigateToPath(''); return false;">Home</a></li>
                            </ol>
                        </nav>
                    </div>
                </div>

                <!-- File Actions Toolbar -->
                <div class="card mb-3">
                    <div class="card-body">
                        <div class="row align-items-center">
                            <div class="col-md-8">
                                <div class="btn-group me-3" role="group">
                                    <button class="btn btn-outline-primary btn-sm" onclick="createFolder()">
                                        <i class="fas fa-folder-plus me-1"></i>
                                        New Folder
                                    </button>
                                    <button class="btn btn-outline-success btn-sm" onclick="uploadFiles()">
                                        <i class="fas fa-upload me-1"></i>
                                        Upload Files
                                    </button>
                                    <button class="btn btn-outline-info btn-sm" onclick="refreshFiles()">
                                        <i class="fas fa-sync me-1"></i>
                                        Refresh
                                    </button>
                                </div>
                                <div class="btn-group" role="group">
                                    <button class="btn btn-outline-secondary btn-sm active" onclick="setViewMode('list')" id="list-view-btn">
                                        <i class="fas fa-list"></i>
                                    </button>
                                    <button class="btn btn-outline-secondary btn-sm" onclick="setViewMode('grid')" id="grid-view-btn">
                                        <i class="fas fa-th"></i>
                                    </button>
                                    <button class="btn btn-outline-secondary btn-sm" onclick="setViewMode('tree')" id="tree-view-btn">
                                        <i class="fas fa-sitemap"></i>
                                    </button>
                                </div>
                            </div>
                            <div class="col-md-4">
                                <div class="input-group">
                                    <span class="input-group-text">
                                        <i class="fas fa-search"></i>
                                    </span>
                                    <input type="text" class="form-control" placeholder="Search files..." id="fileSearchInput" onkeyup="searchFiles()" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <!-- Drag and Drop Zone -->
                <div class="card mb-3" id="dropZone" style="border: 2px dashed #ccc; transition: all 0.3s ease;">
                    <div class="card-body text-center py-5">
                        <i class="fas fa-cloud-upload-alt fa-3x text-muted mb-3"></i>
                        <h5 class="text-muted">Drag and drop files here</h5>
                        <p class="text-muted mb-3">or click to browse files</p>
                        <input type="file" id="fileInput" multiple style="display: none;" onchange="handleFileSelect(event)" />
                        <button class="btn btn-outline-primary" onclick="document.getElementById('fileInput').click()">
                            <i class="fas fa-folder-open me-2"></i>
                            Browse Files
                        </button>
                    </div>
                </div>

                <!-- File List -->
                <div class="card">
                    <div class="card-header d-flex justify-content-between align-items-center">
                        <h6 class="mb-0">
                            <i class="fas fa-folder me-2"></i>
                            <span id="currentFolderName">Documents</span>
                        </h6>
                        <small class="text-muted" id="fileCount">0 items</small>
                    </div>
                    <div class="card-body p-0">
                        <div id="fileListContainer" class="list-view">
                            <!-- Files will be populated here -->
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
