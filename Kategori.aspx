<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Kategori.aspx.vb" Inherits="Sistem_Akreditasi.Kategori" MasterPageFile="~/Site.Master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script src="https://cdn.tailwindcss.com"></script>
    <link rel="preconnect" href="https://fonts.googleapis.com" />
    <link rel="preconnect" href="https://fonts.gstatic.com" crossorigin="anonymous" />
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;600;700&display=swap" rel="stylesheet" />

    <style>
        body {
            font-family: 'Inter', sans-serif;
        }

        .progress-bar-fill {
            transition: width 0.5s ease-in-out;
        }

        /* ---------- ACCORDION (replace previous accordion CSS) ---------- */
.accordion-content {
  max-height: 0;
  overflow: hidden;                /* clip everything when closed */
  transition: max-height 280ms ease, padding 280ms ease;
  box-sizing: border-box;
  padding: 0 1rem;                 /* horizontal padding kept closed */
  display: block;
  will-change: max-height;
}

/* inner wrapper holds the padding and content so outer can clip reliably */
.accordion-content .accordion-inner {
  padding: 0;
  box-sizing: border-box;
  display: block;
}

/* when expanded give the inner wrapper padding */
.accordion-content.open .accordion-inner {
  padding: 1rem;
  padding-bottom: 1.25rem; /* extra breathing room at bottom */
}

/* small spacing between list items (overrides space-y if needed) */
.accordion-content .accordion-inner > ul {
  margin: 0;
  padding: 0;
  list-style: none;
}
.accordion-content .accordion-inner > ul > li {
  margin: 0 0 0.5rem 0;
}

/* make sure the accordion button stays above content while animating */
.accordion-button {
  position: relative;
  z-index: 2;
}



        /* modal styles */
        .modal-backdrop {
            position: fixed;
            inset: 0;
            background: rgba(0,0,0,0.45);
            display: none;
            align-items: center;
            justify-content: center;
            z-index: 50;
        }

        .modal-card {
            width: 100%;
            max-width: 720px;
            background: white;
            border-radius: 12px;
            padding: 18px;
            box-shadow: 0 10px 30px rgba(0,0,0,0.2);
        }

        .drop-area {
            border: 2px dashed #e5e7eb;
            border-radius: 8px;
            padding: 20px;
            text-align: center;
            cursor: pointer;
        }

            .drop-area.dragover {
                background: #f8fafc;
                border-color: #60a5fa;
            }

        .small-btn {
            padding: .35rem .6rem;
            border-radius: .375rem;
            font-size: .875rem;
        }
        /* hide the inline file inputs if any */
        .file-input-wrapper input[type=file] {
            display: none;
        }

        /* Style for the sub-menu UL */
        .submenu {
            display: none;
            padding-left: 20px;
            margin: 0;
            padding-top: 5px;
            padding-bottom: 5px; /* makes sure there's always some space at the bottom */
            list-style: none;
        }

            /* Style for sub-menu list items */
            .submenu li {
                margin: 3px 0; /* spacing between each sub item */
            }

            /* Extra bottom gap when submenu is visible */
            .submenu.show {
                display: block;
                margin-bottom: 10px; /* ensures gap below the whole submenu */
            }
    </style>

    <div class="container mx-auto p-4 md:p-8 max-w-4xl">
        <header class="bg-white p-6 rounded-lg shadow-md mb-8">
            <h1 class="text-2xl md:text-3xl font-bold text-gray-800">Portal Muat Naik Dokumen Akreditasi</h1>
            <div class="mt-2 text-gray-600">
                <p>
                    <span class="font-semibold">Kursus:</span>
                    <asp:Literal ID="litCourseTitle" runat="server" />
                </p>
                <p>
                    <span class="font-semibold">Breadcrumbs:</span>
                    <asp:Literal ID="litBreadcrumbs" runat="server" />
                </p>
            </div>
        </header>

        <main class="bg-white p-6 rounded-lg shadow-md">
            <section id="progress-section" class="mb-8">
                <h2 class="text-xl font-semibold mb-3 text-gray-700">Kemajuan Keseluruhan</h2>
                <div class="w-full bg-gray-200 rounded-full h-6">
                    <div id="progressBarFill" class="progress-bar-fill bg-blue-600 h-6 text-xs font-medium text-blue-100 text-center p-1 leading-none rounded-full" style="width: 0%">
                        <span id="progressText">0%</span>
                    </div>
                </div>
                <p id="progressSummary" class="text-sm text-gray-500 mt-2">—</p>
            </section>

            <section id="document-list">
                <h2 class="text-xl font-semibold mb-4 text-gray-700">Senarai Semak Dokumen</h2>
                <asp:Literal ID="litAccordion" runat="server" />
            </section>
        </main>

        <footer class="text-center mt-8">
            <p class="text-gray-500 text-sm">&copy; 2025 Sistem e-Akreditasi Universiti</p>
        </footer>
    </div>

    <!-- UPLOAD MODAL -->
    <div id="uploadModal" class="modal-backdrop" role="dialog" aria-hidden="true">
        <div class="modal-card">
            <div class="flex justify-between items-center mb-4">
                <h3 id="modalTitle" class="text-lg font-semibold">Muat naik dokumen</h3>
                <button type="button" onclick="closeUploadModal()" class="text-gray-600 hover:text-gray-800">✕</button>
            </div>

            <input type="hidden" id="modal_kursusId" />
            <input type="hidden" id="modal_id_kat" />
            <input type="hidden" id="modal_id_sub" />

            <div id="dropArea" class="drop-area" tabindex="0">
                <p id="dropText">Seret & lepas fail di sini, atau klik untuk pilih fail.</p>
                <p class="text-xs text-gray-500 mt-2">Sokongan: .pdf, .doc, .docx, .zip, .jpg, .jpeg, .png — Max 50MB</p>
            </div>

            <input type="file" id="modalFile" accept=".pdf,.doc,.docx,.zip,.jpg,.jpeg,.png" style="display: none" />

            <div class="flex items-center gap-3 mt-3">
                <button id="modalUploadBtn" type="button" onclick="doUpload()" class="bg-green-600 text-white small-btn">Muat Naik</button>
                <button type="button" onclick="closeUploadModal()" class="bg-gray-200 text-gray-800 small-btn">Batal</button>
                <div id="selectedFileInfo" class="text-sm text-gray-700 ml-3"></div>
            </div>
        </div>
    </div>

    <script>
        // ACCORDION: allow only one open at a time; set maxHeight accurately and toggle padding via .open class
        document.addEventListener('click', function (e) {
            var btn = e.target.closest('.accordion-button');
            if (!btn) return;

            // close others
            document.querySelectorAll('.accordion-button').forEach(function (b) {
                if (b === btn) return;
                b.classList.remove('open');
                b.setAttribute('aria-expanded', 'false');
                var t = b.getAttribute('data-target');
                if (t) {
                    var el = document.querySelector(t);
                    if (el) {
                        el.style.maxHeight = '0px';
                        // ensure padding removed after transition
                        setTimeout(function () { el.classList.remove('open'); }, 300);
                    }
                }
                var svg = b.querySelector('svg'); if (svg) svg.classList.remove('rotate-180');
            });

            // toggle this one
            var expanded = btn.classList.toggle('open');
            btn.setAttribute('aria-expanded', expanded ? 'true' : 'false');
            var target = btn.getAttribute('data-target');
            var el = document.querySelector(target);
            if (!el) return;

            if (expanded) {
                // add padding class then set maxHeight to scrollHeight so transition works
                el.classList.add('open');
                // Need to wait a tick so the browser recognizes the changed padding before measuring
                requestAnimationFrame(function () {
                    el.style.maxHeight = el.scrollHeight + 'px';
                });
            } else {
                el.style.maxHeight = '0px';
                setTimeout(function () { el.classList.remove('open'); }, 280);
            }

            var svg = btn.querySelector('svg');
            if (svg) svg.classList.toggle('rotate-180');
        });

        // modal helpers
        function openUploadModal(kursusId, id_kat, id_sub, subName) {
            document.getElementById('modal_kursusId').value = kursusId;
            document.getElementById('modal_id_kat').value = id_kat;
            document.getElementById('modal_id_sub').value = id_sub;
            document.getElementById('modalTitle').innerText = 'Muat naik: ' + (subName || id_sub);
            document.getElementById('selectedFileInfo').innerText = '';
            document.getElementById('modalFile').value = '';
            showModal(true);
        }
        function showModal(show) {
            var m = document.getElementById('uploadModal');
            m.style.display = show ? 'flex' : 'none';
            m.setAttribute('aria-hidden', show ? 'false' : 'true');
            if (show) { document.body.style.overflow = 'hidden'; document.getElementById('dropArea').focus(); }
            else document.body.style.overflow = '';
        }
        function closeUploadModal() { showModal(false); }

        // drag & drop and file validation (single file)
        (function () {
            var drop = document.getElementById('dropArea');
            var fileInput = document.getElementById('modalFile');
            var info = document.getElementById('selectedFileInfo');
            if (!drop || !fileInput) return;

            drop.addEventListener('click', function () { fileInput.click(); });

            drop.addEventListener('dragover', function (e) { e.preventDefault(); drop.classList.add('dragover'); });
            drop.addEventListener('dragleave', function (e) { drop.classList.remove('dragover'); });
            drop.addEventListener('drop', function (e) {
                e.preventDefault(); drop.classList.remove('dragover');
                var f = e.dataTransfer.files && e.dataTransfer.files[0];
                if (f) setFile(f);
            });

            fileInput.addEventListener('change', function () {
                var f = fileInput.files && fileInput.files[0];
                if (f) setFile(f);
            });


            function setFile(f) {
                var allowed = ['.pdf', '.doc', '.docx', '.zip', '.jpg', '.jpeg', '.png'];
                var name = f.name || '';
                var ext = name.substring(name.lastIndexOf('.')).toLowerCase();
                var maxBytes = 50 * 1024 * 1024;
                if (allowed.indexOf(ext) === -1) { alert('Jenis fail tidak dibenarkan.'); fileInput.value = ''; return; }
                if (f.size > maxBytes) { alert('Fail terlalu besar (max 50MB).'); fileInput.value = ''; return; }
                info.innerText = 'Dipilih: ' + name + ' (' + Math.round(f.size / 1024) + ' KB)';
            }
        })();

        // AJAX upload
        async function doUpload() {
            var fileInput = document.getElementById('modalFile');
            var f = fileInput.files[0];
            if (!f) { alert('Sila pilih fail dahulu.'); return; }

            var kursusId = document.getElementById('modal_kursusId').value;
            var id_kat = document.getElementById('modal_id_kat').value;
            var id_sub = document.getElementById('modal_id_sub').value;

            var fd = new FormData();
            fd.append('action', 'upload');
            fd.append('kursusId', kursusId);
            fd.append('id_kat', id_kat);
            fd.append('id_sub', id_sub);
            fd.append('file', f);

            var btn = document.getElementById('modalUploadBtn');
            btn.disabled = true; btn.innerText = 'Menghantar...';

            try {
                var resp = await fetch(window.location.href, {
                    method: 'POST',
                    body: fd,
                    headers: { 'X-Requested-With': 'XMLHttpRequest' }
                });
                var data = await resp.json();
                if (data && data.success) {
                    closeUploadModal();
                    location.reload();
                } else {
                    alert('Upload failed: ' + (data && data.error ? data.error : 'Unknown error'));
                }
            } catch (ex) {
                alert('Upload failed: ' + ex.toString());
            } finally {
                btn.disabled = false; btn.innerText = 'Muat Naik';
            }
        }

        // AJAX delete (server will set IsActive=0)
        async function deleteDokumen(dokId) {
            if (!confirm('Padam dokumen ini?')) return;
            try {
                var fd = new FormData();
                fd.append('action', 'delete');
                fd.append('dokId', dokId);
                var resp = await fetch(window.location.href, {
                    method: 'POST',
                    body: fd,
                    headers: { 'X-Requested-With': 'XMLHttpRequest' }
                });
                var data = await resp.json();
                if (data && data.success) location.reload(); else alert('Padam failed: ' + (data && data.error ? data.error : 'Unknown'));
            } catch (ex) {
                alert('Padam failed: ' + ex.toString());
            }
        }
    </script>
</asp:Content>
