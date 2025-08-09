// JavaScript hỗ trợ in hóa đơn với các template khác nhau

class InvoicePrinter {
    constructor() {
        this.baseUrl = '/HoaDon';
    }

    // In hóa đơn tự động chọn template dựa trên loại hóa đơn
    async printInvoice(hoaDonId) {
        try {
            const response = await fetch(`${this.baseUrl}/GetHoaDonForPrint/${hoaDonId}`);
            if (!response.ok) {
                throw new Error('Không thể tải hóa đơn');
            }
            
            const html = await response.text();
            this.openPrintWindow(html, 'Hóa đơn');
        } catch (error) {
            console.error('Lỗi khi in hóa đơn:', error);
            alert('Có lỗi xảy ra khi in hóa đơn: ' + error.message);
        }
    }

    // In hóa đơn cho bán hàng tại quầy
    async printTaiQuayInvoice(hoaDonId) {
        try {
            const response = await fetch(`${this.baseUrl}/GetHoaDonForPrint/${hoaDonId}`);
            if (!response.ok) {
                throw new Error('Không thể tải hóa đơn');
            }
            
            const html = await response.text();
            this.openPrintWindow(html, 'Hóa đơn bán tại quầy');
        } catch (error) {
            console.error('Lỗi khi in hóa đơn tại quầy:', error);
            alert('Có lỗi xảy ra khi in hóa đơn: ' + error.message);
        }
    }

    // In hóa đơn cho bán hàng ship
    async printShipInvoice(hoaDonId) {
        try {
            const response = await fetch(`${this.baseUrl}/GetHoaDonForPrintShip/${hoaDonId}`);
            if (!response.ok) {
                throw new Error('Không thể tải hóa đơn ship');
            }
            
            const html = await response.text();
            this.openPrintWindow(html, 'Hóa đơn bán hàng ship');
        } catch (error) {
            console.error('Lỗi khi in hóa đơn ship:', error);
            alert('Có lỗi xảy ra khi in hóa đơn ship: ' + error.message);
        }
    }

    // In hóa đơn cho bán hàng online
    async printOnlineInvoice(hoaDonId) {
        try {
            const response = await fetch(`${this.baseUrl}/GetHoaDonForPrintOnline/${hoaDonId}`);
            if (!response.ok) {
                throw new Error('Không thể tải hóa đơn online');
            }
            
            const html = await response.text();
            this.openPrintWindow(html, 'Hóa đơn bán hàng online');
        } catch (error) {
            console.error('Lỗi khi in hóa đơn online:', error);
            alert('Có lỗi xảy ra khi in hóa đơn online: ' + error.message);
        }
    }

    // Mở cửa sổ in
    openPrintWindow(htmlContent, title) {
        const printWindow = window.open('', '_blank', 'width=900,height=700,scrollbars=yes');
        
        if (!printWindow) {
            alert('Vui lòng cho phép popup để in hóa đơn');
            return;
        }

        const printContent = `
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="utf-8">
                <title>${title}</title>
                <style>
                    body { margin: 0; padding: 20px; font-family: Arial, sans-serif; }
                    @media print {
                        body { margin: 0; padding: 0; }
                        .no-print { display: none !important; }
                    }
                    .print-controls {
                        text-align: center;
                        margin-bottom: 20px;
                        padding: 10px;
                        background: #f8f9fa;
                        border-radius: 5px;
                    }
                    .print-btn {
                        background: #007bff;
                        color: white;
                        border: none;
                        padding: 10px 20px;
                        border-radius: 5px;
                        cursor: pointer;
                        margin: 0 5px;
                        font-size: 14px;
                    }
                    .print-btn:hover {
                        background: #0056b3;
                    }
                    .close-btn {
                        background: #6c757d;
                    }
                    .close-btn:hover {
                        background: #5a6268;
                    }
                </style>
            </head>
            <body>
                <div class="print-controls no-print">
                    <button class="print-btn" onclick="window.print()">🖨️ In hóa đơn</button>
                    <button class="print-btn close-btn" onclick="window.close()">❌ Đóng</button>
                </div>
                ${htmlContent}
                <script>
                    // Tự động focus vào cửa sổ in
                    window.focus();
                    
                    // Xử lý phím tắt
                    document.addEventListener('keydown', function(e) {
                        if (e.ctrlKey && e.key === 'p') {
                            e.preventDefault();
                            window.print();
                        }
                        if (e.key === 'Escape') {
                            window.close();
                        }
                    });
                </script>
            </body>
            </html>
        `;

        printWindow.document.write(printContent);
        printWindow.document.close();
    }

    // Xem trước hóa đơn trong modal
    async previewInvoice(hoaDonId, modalId = '#invoicePreviewModal') {
        try {
            const response = await fetch(`${this.baseUrl}/GetHoaDonForPrint/${hoaDonId}`);
            if (!response.ok) {
                throw new Error('Không thể tải hóa đơn');
            }
            
            const html = await response.text();
            const modalBody = document.querySelector(`${modalId} .modal-body`);
            
            if (modalBody) {
                modalBody.innerHTML = html;
                // Hiển thị modal nếu đang sử dụng Bootstrap
                if (typeof bootstrap !== 'undefined') {
                    const modal = new bootstrap.Modal(document.querySelector(modalId));
                    modal.show();
                } else if (typeof $ !== 'undefined') {
                    $(modalId).modal('show');
                }
            }
        } catch (error) {
            console.error('Lỗi khi xem trước hóa đơn:', error);
            alert('Có lỗi xảy ra khi xem trước hóa đơn: ' + error.message);
        }
    }

    // Tạo nút in cho từng loại hóa đơn
    createPrintButtons(hoaDonId, loaiHoaDon, container) {
        const buttonContainer = document.createElement('div');
        buttonContainer.className = 'print-buttons-container';
        buttonContainer.style.cssText = 'display: flex; gap: 10px; flex-wrap: wrap;';

        // Nút in tự động (chọn template phù hợp)
        const autoPrintBtn = this.createButton('🖨️ In hóa đơn', 'btn btn-primary', () => {
            this.printInvoice(hoaDonId);
        });

        // Nút xem trước
        const previewBtn = this.createButton('👁️ Xem trước', 'btn btn-info', () => {
            this.previewInvoice(hoaDonId);
        });

        buttonContainer.appendChild(autoPrintBtn);
        buttonContainer.appendChild(previewBtn);

        // Thêm nút in theo loại cụ thể
        if (loaiHoaDon !== 'TaiQuay') {
            const specificPrintBtn = this.createButton(
                loaiHoaDon === 'Online' ? '🌐 In hóa đơn Online' : '🚚 In hóa đơn Ship',
                'btn btn-secondary',
                () => {
                    if (loaiHoaDon === 'Online') {
                        this.printOnlineInvoice(hoaDonId);
                    } else {
                        this.printShipInvoice(hoaDonId);
                    }
                }
            );
            buttonContainer.appendChild(specificPrintBtn);
        }

        if (container) {
            container.appendChild(buttonContainer);
        }

        return buttonContainer;
    }

    createButton(text, className, onClick) {
        const button = document.createElement('button');
        button.textContent = text;
        button.className = className;
        button.style.cssText = 'margin: 2px; font-size: 13px;';
        button.addEventListener('click', onClick);
        return button;
    }
}

// Tạo instance global
window.invoicePrinter = new InvoicePrinter();

// Utility functions cho việc sử dụng dễ dàng
window.printInvoice = (hoaDonId) => window.invoicePrinter.printInvoice(hoaDonId);
window.printShipInvoice = (hoaDonId) => window.invoicePrinter.printShipInvoice(hoaDonId);
window.printOnlineInvoice = (hoaDonId) => window.invoicePrinter.printOnlineInvoice(hoaDonId);
window.previewInvoice = (hoaDonId, modalId) => window.invoicePrinter.previewInvoice(hoaDonId, modalId);

// Auto-setup khi document ready
document.addEventListener('DOMContentLoaded', function() {
    // Tự động thêm nút in cho các hóa đơn có data attribute
    const invoiceElements = document.querySelectorAll('[data-invoice-id]');
    invoiceElements.forEach(element => {
        const hoaDonId = element.getAttribute('data-invoice-id');
        const loaiHoaDon = element.getAttribute('data-loai-hoa-don') || 'TaiQuay';
        
        // Tìm container để thêm nút in
        let container = element.querySelector('.print-actions');
        if (!container) {
            container = document.createElement('div');
            container.className = 'print-actions';
            element.appendChild(container);
        }
        
        window.invoicePrinter.createPrintButtons(hoaDonId, loaiHoaDon, container);
    });
});