$(document).ready(function () {

    // ========================= HIDE TABLE INITIALLY =========================
    $("#tblEntries").hide();

    // ========================= ON DROPDOWN CHANGE =========================
    $("#customerName, #loanno").change(function () {

        let customerId = $("#customerName").val();
        let loanNoId = $("#loanno").val();

        if (customerId !== "" && loanNoId !== "") {
            loadFilteredList(customerId, loanNoId);
        } else {
            // Hide table if any dropdown is unselected
            $("#tblEntries").hide();
            $('#tblEntriesBody').html('');
        }
    });

    // When CUSTOMER is selected → Load Loan Numbers
    $("#customerName").on("change", function () {

        let customerId = $(this).val();
        $("#loanno").html('<option value="">-- Select Loan No --</option>');

        if (customerId === "") return;

        $.ajax({
            url: "/DailyEntry/getLoanById",
            type: "GET",
            data: { CustomerId: customerId },
            success: function (response) {

                let html = '<option value="">-- Select Loan No --</option>';

                response.forEach(function (loan) {
                    // Show = LoanNo | Amount | Duration
                    html += `<option value="${loan.LoanId}">
                                ${loan.LoanNo} | ${loan.LoanAmount} | ${loan.LoanDurationDays} Days
                             </option>`;
                });

                $("#loanno").html(html);
            }
        });
    });

    // When LOAN NO is selected → Load summary values
    $("#loanno").on("change", function () {

        let customerId = $("#customerName").val();
        let loanNoId = $(this).val();

        if (customerId === "" || loanNoId === "") return;

        $.ajax({
            url: "/DailyEntry/getLoanSummaryById",
            type: "GET",
            data: { CustomerId: customerId, LoanNoId: loanNoId },
            success: function (response) {

                if (response.length > 0) {

                    let loan = response[0];

                    // Fill textboxes
                    $("#loanamount").val(loan.LoanAmount);
                    $("#loanduration").val(loan.LoanDurationDays);
                    $("#perdayinstallment").val(loan.DailyReturn);
                    //$("#paidtoday").val(loan.DailyReturn);
                }
            }
        });

    });

    // Handle Add / Update button
    $("#btnSubmit").on('click', function () {

        if (!validateDailyEntryForm()) return;

        const isEdit = $("#hdndailycollectionsid").val() !== "";

        $("#btnSubmit")
            .prop("disabled", true)
            .html('<i class="fas fa-spinner fa-spin me-2"></i>' + (isEdit ? "Updating..." : "Saving..."));

        AddOrUpdateDailyCollections();
    });

});

// ========================= LOAD FILTERED DAILY ENTRY LIST =========================
function loadFilteredList(customerId, loanNoId) {

    $.ajax({
        url: "/DailyEntry/GetDailyCollectionsList",
        type: "GET",
        data: { customerId: customerId, loanNoId: loanNoId },
        success: function (data) {
            renderGrid(data);
            $("#tblEntries").show();
        },
        error: function () {
            toastr.error("Failed to load filtered list.");
        }
    });
}



// ========================= RENDER TABLE =========================
function renderGrid(data) {
    let html = '';
    let totalAmountPaidToday = 0;

    // --------------------------------------------------------------
    // 1. Collect distinct dates (multiple entries on same day = 1 day)
    // --------------------------------------------------------------
    const dateSet = new Set();
    data.forEach(row => {
        if (row.TodaysDate) {
            // row.TodaysDate comes from JSON as "/Date(1731791400000)/"
            const jsDate = new Date(parseInt(row.TodaysDate.replace(/\/Date\((\d+)\)\//, '$1')));
            const ymd = jsDate.toISOString().split('T')[0]; // "2025-11-16"
            dateSet.add(ymd);
        }
    });
    const distinctDaysPaid = dateSet.size;               // <-- real "Days Paid"

    // --------------------------------------------------------------
    // 2. Original loan duration (same for every row)
    // --------------------------------------------------------------
    const originalLoanDuration = data.length > 0 ? (data[0].LoanDuration || 0) : 0;
    const remainingDays = Math.max(originalLoanDuration - distinctDaysPaid, 0);

    // --------------------------------------------------------------
    // 3. Update the three text-boxes (outside the table)
    // --------------------------------------------------------------
    $('#loanduration').val(originalLoanDuration);          // total days of the loan
    $('#dayspaid').val(distinctDaysPaid);                  // distinct dates paid
    $('#remainingdays').val(remainingDays);                // what is left

    // --------------------------------------------------------------
    // 4. Build rows (override DaysPaid / RemainingDays)
    // --------------------------------------------------------------
    $.each(data, function (i, row) {
        let amountToday = parseFloat(row.AmountPaidToday) || 0;
        totalAmountPaidToday += amountToday;

        html += `
            <tr>
                <td>${i + 1}</td>
                <td>${row.CustomerName || ''}</td>
                <td>${row.LoanNo || ''}</td>
                <td>${originalLoanDuration}</td>               <!-- total loan days -->
                <td>${row.PerDayInstallment || ''}</td>
                <td>${row.TotalPaid || ''}</td>
                <td>${row.PendingAmount || ''}</td>
                <td>${distinctDaysPaid}</td>                  <!-- days paid (distinct) -->
                <td>${remainingDays}</td>                     <!-- remaining -->
                <td>${row.TodaysDate ? formatJsonDate(row.TodaysDate) : ''}</td>
                <td>${amountToday}</td>
                <td class="text-center">
                    <button class="btn btn-sm btn-danger delete-icon" data-id="${row.DailyCollectionId}">
                        <i class="fas fa-trash-alt"></i>
                    </button>
                </td>
            </tr>`;
    });

    // --------------------------------------------------------------
    // 5. Re-initialise DataTable
    // --------------------------------------------------------------
    const table = $('#tblEntries');
    if ($.fn.DataTable.isDataTable(table)) {
        table.DataTable().destroy();
    }
    $('#tblEntriesBody').html(html);

    // totals
    $('#amountPaidTodayTotal').text(totalAmountPaidToday.toFixed(2));
    $('#totalpaid').val(totalAmountPaidToday.toFixed(2));

    table.DataTable({
        pageLength: 5,
        lengthMenu: [5, 10, 25],
        language: {
            search: "_INPUT_",
            searchPlaceholder: "Search Loan..."
        }
    });
}

// ============================= ADD / UPDATE LOAN =============================
function AddOrUpdateDailyCollections() {

    const model = getDailyCollections();

    $.ajax({
        url: "/DailyEntry/AddDailyEntry",
        type: "POST",
        data: JSON.stringify(model),
        contentType: "application/json; charset=utf-8",
        success: function (response) {
            toastr.success(response);

            clearForm();

            // ========================= FIX START =========================
            let customerId = $("#customerName").val();
            let loanNoId = $("#loanno").val();

            if (customerId && loanNoId) {
                loadFilteredList(customerId, loanNoId);   // ✔ Reload correct list
            }
            // ========================= FIX END =========================
        },
        error: function (xhr) {
            toastr.error(xhr.responseText || "Operation failed.");
        },
        complete: function () {
            $("#btnSubmit")
                .prop("disabled", false)
                .html('<i class="fas fa-save me-2"></i> Save Entry');
        }
    });
}

function getDailyCollections() {
    return {
        DailyCollectionId: $("#hdndailycollectionsid").val() || 0,

        CustomerId: $("#customerName").val(),
        LoanNoId: $("#loanno").val(),

        LoanAmount: $("#loanamount").val(),
        LoanDuration: $("#loanduration").val(),
        PerDayInstallment: $("#perdayinstallment").val(),

        TotalPaid: $("#totalpaid").val(),
        PendingAmount: $("#pendingamount").val(),
        DaysPaid: $("#dayspaid").val(),
        RemainingDays: $("#remainingdays").val(),

        TodaysDate: $("#todaydate").val(),
        AmountPaidToday: $("#paidtoday").val()
    };
}
function clearForm() {

    $("#hdndailycollectionsid").val("");
    $("#customerName").val("");
    $("#loanno").val("");
    $("#loanamount").val("");
    $("#loanduration").val("");
    $("#perdayinstallment").val("");
    $("#totalpaid").val("");
    $("#pendingamount").val("");
    $("#dayspaid").val("");
    $("#remainingdays").val("");
   // $("#todaydate").val("");
    $("#paidtoday").val("");
    $("#btnSubmit").html('<i class="fas fa-save me-2"></i> Save Entry');
}

// ============================= VALIDATION ============================= //
function validateDailyEntryForm() {

    const customer = $("#customerName").val();
    const loanno = $("#loanno").val();
    const todayamount = $("#paidtoday").val().trim();
    const todaydate = $("#todaydate").val();

    if (!customer) {
        toastr.warning("Please select a customer.");
        $("#customerName").focus();
        return false;
    }
    if (!loanno) {
        toastr.warning("Please select a Loan No.");
        $("#loanno").focus();
        return false;
    }
    if (!todaydate) {
        toastr.warning("Please select today's date.");
        $("#todaydate").focus();
        return false;
    }
    if (!todayamount || isNaN(todayamount)) {
        toastr.warning("Enter valid amount.");
        $("#paidtoday").focus();
        return false;
    }

    return true;
}

function formatJsonDate(jsonDate) {
    // "/Date(1731791400000)/"  →  "16/11/2025"
    const millis = parseInt(jsonDate.replace(/\/Date\((\d+)\)\//, '$1'));
    const d = new Date(millis);
    const day = String(d.getDate()).padStart(2, '0');
    const month = String(d.getMonth() + 1).padStart(2, '0');
    const year = d.getFullYear();
    return `${day}/${month}/${year}`;
}


// ============================= DELETE LOAN =============================
$(document).on('click', '.delete-icon', function () {
    const DailyCollectionId = $(this).data("id");

    toastr.warning(
        "<br><button type='button' class='btn btn-danger btn-sm confirm-delete'>Yes, Delete</button>" +
        "<button type='button' class='btn btn-secondary btn-sm ms-2 cancel-delete'>Cancel</button>",
        "Confirm Delete",
        {
            allowHtml: true,
            closeButton: false,
            timeOut: 0,
            extendedTimeOut: 0,
        }
    );

    $(document).off("click", ".confirm-delete").on("click", ".confirm-delete", function () {
        deleteLoan(DailyCollectionId);
        toastr.clear();
    });

    $(document).off("click", ".cancel-delete").on("click", ".cancel-delete", function () {
        toastr.clear();
    });
});

function deleteLoan(DailyCollectionId) {
    $.ajax({
        url: "/DailyEntry/DeleteDailyCollection",
        type: "POST",
        data: { DailyCollectionId: DailyCollectionId },
        success: function () {
            toastr.success("Daily Collections deleted successfully.");
            clearForm();
            // ========================= FIX START =========================
            let customerId = $("#customerName").val();
            let loanNoId = $("#loanno").val();
            if (customerId && loanNoId) {
                loadFilteredList(customerId, loanNoId);   // ✔ Reload correct list
            }
        },
        error: function () {
            toastr.error("Failed to delete loan.");
        }
    });
}


$("#paidtoday").on("input", function () {

    let pending = parseFloat($("#pendingamount").val()) || 0;
    let paid = parseFloat($(this).val()) || 0;

    // If pending <= 0 → disable textbox + button
    if (pending <= 0) {
        $(this).val(0);
        $("#paidtoday").prop("disabled", true);
        $("#btnSubmit").prop("disabled", true);
        return;
    } else {
        $("#paidtoday").prop("disabled", false);
        $("#btnSubmit").prop("disabled", false);
    }

    // If paidToday > pendingAmount → set paidToday = pendingAmount
    if (paid > pending) {
        $(this).val(pending);
    }
});
