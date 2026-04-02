// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

window.companyTools = {
    printPage: function () {
        window.print();
    },
    saveAsPdf: function () {
        const { jsPDF } = window.jspdf;
        const title = document.querySelector('h1')?.innerText || 'Internship Companies Database';

        const table = document.querySelector('.company-table');
        const formGroups = document.querySelectorAll('.form-group');

        if (table) {
            // INDEX PAGE: table with company rows
            var pdf = new jsPDF({ orientation: 'landscape', unit: 'mm', format: 'a4' });
            var pageWidth = 297;
            var pageHeight = 210;
            var margin = 15;
            var usableWidth = pageWidth - margin * 2;
            var y = margin;

            pdf.setFontSize(16);
            pdf.setFont('helvetica', 'bold');
            pdf.text(title, margin, y);
            y += 10;

            // Read headers
            var headers = [];
            table.querySelectorAll('thead th').forEach(function (th) {
                headers.push(th.innerText.trim());
            });

            // Read rows
            var rows = [];
            table.querySelectorAll('tbody tr.company-row').forEach(function (tr) {
                var cells = [];
                tr.querySelectorAll('td').forEach(function (td) {
                    var text = td.innerText.trim();
                    cells.push(text);
                });
                if (cells.length > 0) rows.push(cells);
            });

            if (headers.length > 0 && rows.length > 0) {
                var colCount = headers.length;
                var colWidths = [];
                var totalRatio = 0;
                var ratios = [0.06, 0.20, 0.14, 0.14, 0.14, 0.18, 0.14];
                for (var c = 0; c < colCount; c++) {
                    var r = c < ratios.length ? ratios[c] : (1 / colCount);
                    totalRatio += r;
                    colWidths.push(r);
                }
                for (var c = 0; c < colCount; c++) {
                    colWidths[c] = (colWidths[c] / totalRatio) * usableWidth;
                }

                var rowHeight = 8;
                var headerHeight = 10;
                var fontSize = 8;
                var headerFontSize = 9;

                // Draw header
                function drawHeader() {
                    pdf.setFillColor(52, 58, 64);
                    pdf.rect(margin, y, usableWidth, headerHeight, 'F');
                    pdf.setFontSize(headerFontSize);
                    pdf.setFont('helvetica', 'bold');
                    pdf.setTextColor(255, 255, 255);
                    var hx = margin;
                    for (var c = 0; c < colCount; c++) {
                        pdf.text(headers[c], hx + 2, y + headerHeight - 3, { maxWidth: colWidths[c] - 4 });
                        hx += colWidths[c];
                    }
                    y += headerHeight;
                    pdf.setTextColor(0, 0, 0);
                }

                drawHeader();

                pdf.setFontSize(fontSize);
                pdf.setFont('helvetica', 'normal');

                for (var r = 0; r < rows.length; r++) {
                    // Calculate needed row height based on text wrapping
                    var maxLines = 1;
                    for (var c = 0; c < colCount && c < rows[r].length; c++) {
                        var lines = pdf.splitTextToSize(rows[r][c] || '-', colWidths[c] - 4);
                        if (lines.length > maxLines) maxLines = lines.length;
                    }
                    var currentRowHeight = Math.max(rowHeight, maxLines * 5 + 3);

                    // Check page break
                    if (y + currentRowHeight > pageHeight - margin) {
                        pdf.addPage();
                        y = margin;
                        drawHeader();
                        pdf.setFontSize(fontSize);
                        pdf.setFont('helvetica', 'normal');
                    }

                    // Zebra striping
                    if (r % 2 === 0) {
                        pdf.setFillColor(245, 245, 245);
                        pdf.rect(margin, y, usableWidth, currentRowHeight, 'F');
                    }

                    // Draw cells
                    var cx = margin;
                    for (var c = 0; c < colCount && c < rows[r].length; c++) {
                        var cellText = rows[r][c] || '-';
                        var lines = pdf.splitTextToSize(cellText, colWidths[c] - 4);
                        pdf.text(lines, cx + 2, y + 5);
                        cx += colWidths[c];
                    }

                    // Row border
                    pdf.setDrawColor(220, 220, 220);
                    pdf.line(margin, y + currentRowHeight, margin + usableWidth, y + currentRowHeight);
                    y += currentRowHeight;
                }
            }

            var filename = title.toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/-+$/, '') + '.pdf';
            pdf.save(filename);

        } else if (formGroups.length > 0) {
            // DETAILS PAGE: label/value pairs
            var pdf = new jsPDF({ orientation: 'portrait', unit: 'mm', format: 'a4' });
            var pageWidth = 210;
            var pageHeight = 297;
            var margin = 20;
            var usableWidth = pageWidth - margin * 2;
            var y = margin;

            pdf.setFontSize(18);
            pdf.setFont('helvetica', 'bold');
            pdf.text(title, margin, y);
            y += 12;

            // Subtitle
            var subtitle = document.querySelector('.editor-subtitle');
            if (subtitle) {
                pdf.setFontSize(10);
                pdf.setFont('helvetica', 'normal');
                pdf.setTextColor(120, 120, 120);
                pdf.text(subtitle.innerText.trim(), margin, y);
                pdf.setTextColor(0, 0, 0);
                y += 8;
            }

            // Divider line
            pdf.setDrawColor(200, 200, 200);
            pdf.line(margin, y, margin + usableWidth, y);
            y += 8;

            var labelWidth = 60;
            var valueWidth = usableWidth - labelWidth - 5;

            formGroups.forEach(function (fg) {
                var label = fg.querySelector('.form-label')?.innerText?.trim() || '';
                var input = fg.querySelector('input') || fg.querySelector('textarea');
                var value = '';
                if (input) {
                    value = input.value || input.innerText || input.textContent || '';
                }
                value = value.trim() || '-';

                // Calculate height needed for value text
                pdf.setFontSize(9);
                var valueLines = pdf.splitTextToSize(value, valueWidth);
                var blockHeight = Math.max(8, valueLines.length * 5 + 4);

                // Check page break
                if (y + blockHeight > pageHeight - margin) {
                    pdf.addPage();
                    y = margin;
                }

                // Label
                pdf.setFont('helvetica', 'bold');
                pdf.setFontSize(9);
                pdf.setTextColor(80, 80, 80);
                pdf.text(label, margin, y + 5);

                // Value
                pdf.setFont('helvetica', 'normal');
                pdf.setFontSize(10);
                pdf.setTextColor(0, 0, 0);
                pdf.text(valueLines, margin + labelWidth, y + 5);

                y += blockHeight;

                // Subtle separator
                pdf.setDrawColor(230, 230, 230);
                pdf.line(margin, y, margin + usableWidth, y);
                y += 3;
            });

            // Derive filename from company name
            var companyName = '';
            var firstInput = document.querySelector('.form-group:nth-child(2) input');
            if (firstInput) companyName = firstInput.value?.trim() || '';
            var filename = (companyName || title).toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/-+$/, '') + '.pdf';
            pdf.save(filename);
        }
    },
    exportWord: function () {
        const title = document.querySelector('h1')?.innerText ?? 'Internship Companies Database';

        let contentHtml = '';
        const table = document.querySelector('.company-table');

        if (table) {
            contentHtml = table.outerHTML;
        } else {
            const formGroups = document.querySelectorAll('.form-group');
            if (formGroups.length > 0) {
                contentHtml = '<table border="1" cellpadding="5" cellspacing="0" style="border-collapse: collapse; width: 100%; font-family: Arial, sans-serif;">';
                formGroups.forEach(fg => {
                    const label = fg.querySelector('.form-label')?.innerText || '';
                    const input = fg.querySelector('input') || fg.querySelector('textarea');
                    const value = input ? (input.value || input.innerText || input.textContent) : '';
                    contentHtml += `<tr><td style="font-weight: bold; width: 35%; padding: 8px;">${label}</td><td style="padding: 8px;">${value}</td></tr>`;
                });
                contentHtml += '</table>';
            } else {
                const main = document.querySelector('main');
                if (!main) return;
                contentHtml = main.innerHTML;
            }
        }

        const html = `
            <html xmlns:o='urn:schemas-microsoft-com:office:office'
                  xmlns:w='urn:schemas-microsoft-com:office:word'
                  xmlns='http://www.w3.org/TR/REC-html40'>
            <head><meta charset='utf-8'><title>${title}</title></head>
            <body>
                <h1>${title}</h1>
                ${contentHtml}
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
