// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

window.companyTools = {
    printPage: function () {
        window.print();
    },
    saveAsPdf: async function () {
        const title = document.querySelector('h1')?.innerText ?? 'Internship Companies Database';
        const source = document.querySelector('main');
        if (!source || !window.jspdf) return;

        const { jsPDF } = window.jspdf;
        const pdf = new jsPDF({
            orientation: 'landscape',
            unit: 'pt',
            format: 'a4'
        });

        await pdf.html(source, {
            margin: [20, 20, 20, 20],
            autoPaging: 'text',
            html2canvas: {
                scale: 0.55,
                useCORS: true
            },
            callback: function (doc) {
                doc.save('internship-companies.pdf');
            }
        });
    },
    exportWord: function () {
        const title = document.querySelector('h1')?.innerText ?? 'Internship Companies Database';
        const table = document.querySelector('.company-table');
        if (!table) return;

        const html = `
            <html xmlns:o='urn:schemas-microsoft-com:office:office'
                  xmlns:w='urn:schemas-microsoft-com:office:word'
                  xmlns='http://www.w3.org/TR/REC-html40'>
            <head><meta charset='utf-8'><title>${title}</title></head>
            <body>
                <h1>${title}</h1>
                ${table.outerHTML}
            </body>
            </html>`;

        const blob = new Blob(['\ufeff', html], { type: 'application/msword' });
        const url = URL.createObjectURL(blob);
        const link = document.createElement('a');
        link.href = url;
        link.download = 'internship-companies.doc';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
        URL.revokeObjectURL(url);
    }
};
