$(document).ready(function () {

    // 🔹 Load all records on page load
    loadFilteredList(null);

    // 🔹 On dropdown change filter the list
    $(document).on("change", "#customerName", function () {
        const customerId = $(this).val() || null;
        loadFilteredList(customerId);
    });

    // 🔹 GET request method
    function loadFilteredList(customerId) {
        $.ajax({
            url: "/ExcelReport/GetFilteredList",
            type: "GET",
            data: { CustomerId: customerId },
            success: function (data) {
                renderGrid(data);
            },
            error: function () {
                toastr.error("Failed to load data.");
            }
        });
    }

    // 🔹 Render table
    function renderGrid(data) {

        let html = "";

        $.each(data, function (index, row) {
            html += `
                <tr>
                    <td>${index + 1}</td>
                    <td>${row.CustomerName || ''}</td>
                    <td>${row.LoanNo || ''}</td>
                    <td> Rs. ${row.LoanAmount || ''}</td>
                   
                    <td class="text-center">
                        <a href="/ExcelReport/DownloadLoanExcel?CustomerId=${row.CustomerId}" 
                           class="btn btn-success btn-sm">
                            <i class="fas fa-file-excel"></i> Excel Report
                        </a>
                    </td>

                </tr>
            `;
        });

        //<td> Rs. ${row.PendingAmount || 0}</td>
        //          <td>
        //               <span class="badge ${row.PendingAmount <= 0 ? 'bg-success' : 'bg-danger'}">
        //                   ${row.PendingAmount <= 0 ? "Completed" : "Pending"}
        //               </span>
        //           </td>

        const table = $("#tblReports");

        // Destroy existing datatable
        if ($.fn.DataTable.isDataTable(table)) {
            table.DataTable().destroy();
        }

        $("#tblReportBody").html(html);

        // Reinitialize DataTable
        table.DataTable({
            pageLength: 10,
            lengthMenu: [10, 25, 50, 100],
            language: {
                searchPlaceholder: "Search Report...",
                search: ""
            }
        });
    }

});
