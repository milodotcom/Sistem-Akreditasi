// Global variables
let currentView = 'dashboard';
let currentPath = '';
let currentRepo = '';
let viewMode = 'list';
let navigationStack = [];

// Sample file data
const sampleFiles = {
    'program-documents': {
        'Faculty of Engineering': {
            'Undergraduate': {
                'Computer Science': {
                    'Curriculum Documents': [
                        { name: 'CS_Curriculum_2024.pdf', type: 'pdf', size: '2.5 MB', modified: '2024-01-15' },
                        { name: 'Course_Descriptions.docx', type: 'doc', size: '1.8 MB', modified: '2024-01-10' }
                    ],
                    'Assessment Reports': [
                        { name: 'Assessment_Report_2023.pdf', type: 'pdf', size: '3.2 MB', modified: '2024-01-12' },
                        { name: 'Student_Feedback.xlsx', type: 'doc', size: '856 KB', modified: '2024-01-08' }
                    ]
                },
                'Mechanical Engineering': {
                    'Curriculum Documents': [
                        { name: 'ME_Curriculum_2024.pdf', type: 'pdf', size: '2.8 MB', modified: '2024-01-14' }
                    ]
                }
            },
            'Graduate': {
                'Master of Engineering': {
                    'Research Guidelines': [
                        { name: 'Research_Guidelines.pdf', type: 'pdf', size: '1.5 MB', modified: '2024-01-11' }
                    ]
                }
            }
        },
        'Faculty of Business': {
            'Undergraduate': {
                'Business Administration': {
                    'Curriculum Documents': [
                        { name: 'BA_Curriculum_2024.pdf', type: 'pdf', size: '2.1 MB', modified: '2024-01-13' }
                    ]
                }
            }
        }
    },
    'quality-assurance': {
        'MQA Reports': [
            { name: 'MQA_Audit_Report_2023.pdf', type: 'pdf', size: '4.2 MB', modified: '2024-01-09' },
            { name: 'Quality_Manual.pdf', type: 'pdf', size: '3.8 MB', modified: '2024-01-07' }
        ],
        'Compliance Documents': [
            { name: 'Compliance_Checklist.xlsx', type: 'doc', size: '1.2 MB', modified: '2024-01-06' },
            { name: 'Standards_Documentation.pdf', type: 'pdf', size: '2.9 MB', modified: '2024-01-05' }
        ]
    }
};

// Navigation functions

function showRepositories() {
    hideAllSections();
    document.getElementById('dashboard-content').style.display = 'block';
    document.getElementById('back-button').style.display = 'block';
    document.getElementById('search-box').style.display = 'block';
    document.getElementById('repository-list').style.display = 'block';
    currentView = 'repositories';
    navigationStack.push('dashboard');
}

function openRepository(repoName) {
    currentRepo = repoName;
    currentPath = '';
    hideAllSections();
    document.getElementById('dashboard-content').style.display = 'block';
    document.getElementById('back-button').style.display = 'block';
    document.getElementById('file-management').style.display = 'block';
    currentView = 'file-management';
    navigationStack.push('repositories');

    // Update breadcrumb and load files
    updateBreadcrumb();
    loadFiles();
}

function goBack() {
    if (navigationStack.length > 0) {
        const previousView = navigationStack.pop();
        switch (previousView) {
            case 'repositories':
                showRepositories();
                break;
        }
    }
}

function hideAllSections() {
    document.getElementById('dashboard-content').style.display = 'none';
    document.getElementById('settings-content').style.display = 'none';
    document.getElementById('back-button').style.display = 'none';
    document.getElementById('search-box').style.display = 'none';
    document.getElementById('repository-list').style.display = 'none';
    document.getElementById('file-management').style.display = 'none';
}

function updateSidebarActive(activeItem) {
    const dashboardLink = document.getElementById('dashboard-link');
    const settingsLink = document.getElementById('settings-link');

    if (!dashboardLink || !settingsLink) return;

    dashboardLink.classList.remove('active');
    settingsLink.classList.remove('active');

    if (activeItem === 'dashboard') {
        dashboardLink.classList.add('active');
    } else if (activeItem === 'settings') {
        settingsLink.classList.add('active');
    }
}

// File management functions
function loadFiles() {
    const container = document.getElementById('fileListContainer');
    const fileCountElement = document.getElementById('fileCount');
    const folderNameElement = document.getElementById('currentFolderName');

    let currentData = sampleFiles[currentRepo];
    let pathParts = currentPath.split('/').filter(part => part !== '');

    // Navigate to current path
    for (let part of pathParts) {
        if (currentData && currentData[part]) {
            currentData = currentData[part];
        }
    }

    if (!currentData) {
        container.innerHTML = '<div class="text-center py-5 text-muted">No files found</div>';
        fileCountElement.textContent = '0 items';
        return;
    }

    let items = [];
    let itemCount = 0;

    // Add folders
    Object.keys(currentData).forEach(key => {
        if (typeof currentData[key] === 'object' && !Array.isArray(currentData[key])) {
            items.push({
                name: key,
                type: 'folder',
                size: '',
                modified: ''
            });
            itemCount++;
        }
    });

    // Add files
    Object.keys(currentData).forEach(key => {
        if (Array.isArray(currentData[key])) {
            currentData[key].forEach(file => {
                items.push(file);
                itemCount++;
            });
        }
    });

    // Update folder name
    if (pathParts.length > 0) {
        folderNameElement.textContent = pathParts[pathParts.length - 1];
    } else {
        folderNameElement.textContent = currentRepo === 'program-documents' ? 'Program Documents' : 'Quality Assurance';
    }

    // Update file count
    fileCountElement.textContent = `${itemCount} items`;

    // Render items based on view mode
    renderFiles(items, container);
}

function renderFiles(items, container) {
    if (viewMode === 'list') {
        renderListView(items, container);
    } else if (viewMode === 'grid') {
        renderGridView(items, container);
    } else if (viewMode === 'tree') {
        renderTreeView(items, container);
    }
}

function renderListView(items, container) {
    container.className = 'list-view';
    let html = '';

    items.forEach(item => {
        const iconClass = getFileIcon(item.type, item.name);
        const iconBg = getFileIconBackground(item.type);

        html += `
            <div class="file-item" ${item.type === 'folder' ? `onclick="navigateToFolder('${item.name}')"` : ''} style="${item.type === 'folder' ? 'cursor: pointer;' : ''}">
                <div class="file-icon ${iconBg}">
                    <i class="${iconClass}"></i>
                </div>
                <div class="file-info">
                    <div class="file-name">${item.name}</div>
                    <div class="file-meta">
                        ${item.size ? `${item.size} • ` : ''}${item.modified ? `Modified ${item.modified}` : ''}
                    </div>
                </div>
                <div class="file-actions">
                    ${item.type !== 'folder' ? `
                        <button class="btn btn-outline-primary btn-sm" onclick="downloadFile('${item.name}')">
                            <i class="fas fa-download"></i>
                        </button>
                        <button class="btn btn-outline-secondary btn-sm" onclick="shareFile('${item.name}')">
                            <i class="fas fa-share"></i>
                        </button>
                    ` : ''}
                </div>
            </div>
        `;
    });

    if (html === '') {
        html = '<div class="text-center py-5 text-muted">This folder is empty</div>';
    }

    container.innerHTML = html;
}

function renderGridView(items, container) {
    container.className = 'grid-view';
    let html = '';

    items.forEach(item => {
        const iconClass = getFileIcon(item.type, item.name);
        const iconBg = getFileIconBackground(item.type);

        html += `
            <div class="file-item" ${item.type === 'folder' ? `onclick="navigateToFolder('${item.name}')"` : ''} style="${item.type === 'folder' ? 'cursor: pointer;' : ''}">
                <div class="file-icon ${iconBg}">
                    <i class="${iconClass}"></i>
                </div>
                <div class="file-info">
                    <div class="file-name">${item.name}</div>
                    <div class="file-meta">
                        ${item.size ? `${item.size}` : ''}${item.modified ? `<br>Modified ${item.modified}` : ''}
                    </div>
                </div>
            </div>
        `;
    });

    if (html === '') {
        html = '<div class="text-center py-5 text-muted">This folder is empty</div>';
    }

    container.innerHTML = html;
}

function renderTreeView(items, container) {
    container.className = 'tree-view';
    let html = '<div class="p-3">';

    items.forEach(item => {
        const iconClass = getFileIcon(item.type, item.name);
        html += `
            <div class="tree-item ${item.type}" ${item.type === 'folder' ? `onclick="navigateToFolder('${item.name}')" style="cursor: pointer;"` : ''}>
                <i class="${iconClass} me-2"></i>${item.name}
                ${item.size ? ` (${item.size})` : ''}
            </div>
        `;
    });

    if (items.length === 0) {
        html += '<div class="text-muted">This folder is empty</div>';
    }

    html += '</div>';
    container.innerHTML = html;
}

function getFileIcon(type, name) {
    if (type === 'folder') return 'fas fa-folder';

    const extension = name.split('.').pop().toLowerCase();
    switch (extension) {
        case 'pdf': return 'fas fa-file-pdf';
        case 'doc':
        case 'docx': return 'fas fa-file-word';
        case 'xls':
        case 'xlsx': return 'fas fa-file-excel';
        case 'ppt':
        case 'pptx': return 'fas fa-file-powerpoint';
        case 'jpg':
        case 'jpeg':
        case 'png':
        case 'gif': return 'fas fa-file-image';
        case 'zip':
        case 'rar': return 'fas fa-file-archive';
        default: return 'fas fa-file';
    }
}

function getFileIconBackground(type) {
    if (type === 'folder') return 'folder-icon';
    return 'file-icon';
}

function navigateToFolder(folderName) {
    if (currentPath === '') {
        currentPath = folderName;
    } else {
        currentPath += '/' + folderName;
    }
    updateBreadcrumb();
    loadFiles();
}

function navigateToPath(path) {
    currentPath = path;
    updateBreadcrumb();
    loadFiles();
}

function updateBreadcrumb() {
    const breadcrumb = document.getElementById('breadcrumb');
    let html = '<li class="breadcrumb-item"><a href="#" onclick="navigateToPath(\'\'); return false;">Home</a></li>';

    if (currentPath) {
        const pathParts = currentPath.split('/');
        let buildPath = '';

        pathParts.forEach((part, index) => {
            buildPath += (buildPath ? '/' : '') + part;
            if (index === pathParts.length - 1) {
                html += `<li class="breadcrumb-item active">${part}</li>`;
            } else {
                html += `<li class="breadcrumb-item"><a href="#" onclick="navigateToPath('${buildPath}'); return false;">${part}</a></li>`;
            }
        });
    }

    breadcrumb.innerHTML = html;
}

function setViewMode(mode) {
    viewMode = mode;

    // Update button states
    document.querySelectorAll('[id$="-view-btn"]').forEach(btn => {
        btn.classList.remove('active');
    });
    document.getElementById(mode + '-view-btn').classList.add('active');

    // Reload files with new view
    loadFiles();
}

// Search and filter functions
function filterRepositories() {
    const searchTerm = document.getElementById('searchInput').value.toLowerCase();
    const repoCards = document.querySelectorAll('.repo-card');

    repoCards.forEach(card => {
        const repoName = card.querySelector('h5').textContent.toLowerCase();
        const repoDesc = card.querySelector('p').textContent.toLowerCase();

        if (repoName.includes(searchTerm) || repoDesc.includes(searchTerm)) {
            card.style.display = 'block';
        } else {
            card.style.display = 'none';
        }
    });
}

function searchFiles() {
    const searchTerm = document.getElementById('fileSearchInput').value.toLowerCase();
    const fileItems = document.querySelectorAll('.file-item');

    fileItems.forEach(item => {
        const fileName = item.querySelector('.file-name').textContent.toLowerCase();

        if (fileName.includes(searchTerm)) {
            item.style.display = 'flex';
        } else {
            item.style.display = 'none';
        }
    });
}

// File operations
function createFolder() {
    const folderName = prompt('Enter folder name:');
    if (folderName) {
        alert(`Folder "${folderName}" created successfully!`);
        // In a real application, this would create the folder and refresh the view
        loadFiles();
    }
}

function uploadFiles() {
    document.getElementById('fileInput').click();
}

function handleFileSelect(event) {
    const files = event.target.files;
    if (files.length > 0) {
        alert(`${files.length} file(s) selected for upload!`);
        // In a real application, this would upload the files
    }
}

function refreshFiles() {
    loadFiles();
    alert('Files refreshed!');
}

function downloadFile(fileName) {
    alert(`Downloading ${fileName}...`);
    // In a real application, this would trigger the file download
}

function shareFile(fileName) {
    alert(`Sharing ${fileName}...`);
    // In a real application, this would open a sharing dialog
}

function createNewRepo() {
    const repoName = prompt('Enter repository name:');
    if (repoName) {
        alert(`Repository "${repoName}" created successfully!`);
        // In a real application, this would create the repository
    }
}

// Settings functions
function setTheme(theme) {
    // Update button states
    document.querySelectorAll('[id$="-mode-btn"]').forEach(btn => {
        btn.classList.remove('active');
    });
    document.getElementById(theme + '-mode-btn').classList.add('active');

    if (theme === 'dark') {
        document.body.classList.add('dark-theme');
        alert('Dark theme applied!');
    } else {
        document.body.classList.remove('dark-theme');
        alert('Light theme applied!');
    }
}

function setFontSize(size) {
    // Update button states
    document.querySelectorAll('[id$="-font-btn"]').forEach(btn => {
        btn.classList.remove('active');
    });
    document.getElementById(size + '-font-btn').classList.add('active');

    // Apply font size
    document.body.classList.remove('font-small', 'font-medium', 'font-large');
    document.body.classList.add('font-' + size);

    alert(`Font size changed to ${size}!`);
}

// Drag and drop functionality
function setupDragAndDrop() {
    const dropZone = document.getElementById('dropZone');

    dropZone.addEventListener('dragover', function (e) {
        e.preventDefault();
        dropZone.classList.add('drag-over');
    });

    dropZone.addEventListener('dragleave', function (e) {
        e.preventDefault();
        dropZone.classList.remove('drag-over');
    });

    dropZone.addEventListener('drop', function (e) {
        e.preventDefault();
        dropZone.classList.remove('drag-over');

        const files = e.dataTransfer.files;
        if (files.length > 0) {
            alert(`${files.length} file(s) dropped for upload!`);
            // In a real application, this would handle the file upload
        }
    });
}

function createFolder() {
    const name = document.getElementById('folderNameInput').value.trim();
    if (!name) {
        alert('Please enter a folder name.');
        return;
    }

    // Create the folder “card”
    const container = document.querySelector('.Repo-bg');
    const card = document.createElement('div');
    card.className = 'folder-item text-center me-3 mb-3';
    card.style.cursor = 'pointer';
    card.innerHTML = `
      <i class="fas fa-folder fa-3x text-warning"></i>
      <div>${name}</div>
    `;

    container.appendChild(card);

    // Clear the input and hide the modal
    document.getElementById('folderNameInput').value = '';
    const modalEl = document.getElementById('CreateFolder');
    const bsModal = bootstrap.Modal.getInstance(modalEl);
    bsModal.hide();
}

// Initialize the application5
document.addEventListener('DOMContentLoaded', function () {
    // ONLY run showDashboard() if you’re on a full single-page dashboard view
    const dashboardDiv = document.getElementById('dashboard-content');
    if (dashboardDiv) {
        showDashboard();
    }

    setupDragAndDrop();
});
