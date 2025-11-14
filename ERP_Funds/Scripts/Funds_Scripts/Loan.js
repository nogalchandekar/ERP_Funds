$(document).ready(function () {

    // Load loan list
    getList();

    // Handle Add / Update
    $("#btnSubmit").on('click', function (e) {
        e.preventDefault();

        if (!validateLoanForm()) return;

        const isEdit = $("#hdnloanid").val() !== "";

        $("#btnSubmit").prop("disabled", true)
            .html('<i class="fas fa-spinner fa-spin me-2"></i>' + (isEdit ? "Updating..." : "Adding..."));

        AddOrUpdateLoan();
    });
});


// ============================= VALIDATION =============================
function validateLoanForm() {
    const amount = $("#loanAmount").val().trim();
    const customer = $("#customerName").val().trim();
    const isCalculated = $("#isCalculated").val();   // NEW FLAG CHECK

    if (!customer) {
        toastr.warning("Please select a customer.");
        $("#customerName").focus();
        return false;
    }
    if (!amount) {
        toastr.warning("Loan Amount is required.");
        $("#loanAmount").focus();
        return false;
    }

    // 🔥 REQUIRED VALIDATION: USER MUST CALCULATE LOAN BEFORE SUBMITTING
    if (isCalculated == "0") {
        toastr.error("Please calculate the loan before submitting.");
        return false;
    }

    return true;
}


// ============================= EDIT LOAN =============================
$(document).on('click', '.edit-icon', function () {
    const LoanId = $(this).data('id');
    EditLoan(LoanId);
    $("#loanAmount").focus();
});

function EditLoan(LoanId) {
    $.ajax({
        url: '/Loan/GetLoanById',
        type: 'GET',
        data: { LoanId: LoanId },
        success: function (response) {
            if (response) {

                // Fill fields
                $('#hdnloanid').val(response.LoanId);
                $('#customerName').val(response.CustomerId).trigger('change');
                $('#loanAmount').val(response.LoanAmount);
                $('#loanDays').val(response.LoanDurationDays);
                $('#interestRate').val(response.LoanInterest);

                // Display result box
                $('#resultBox').show();
                $('#displayLoanAmount').text(response.LoanAmount);
                $('#deductedInterest').text(response.DeductAmount);
                $('#givenAmount').text(response.AmountGivenToCustomer);
                $('#displayLoanDays').text(response.LoanDurationWithMonths);
                $('#dailyReturn').text(response.DailyReturn);
                $('#totalRepay').text(response.TotalPayable);

                $("#isCalculated").val("1"); // Mark as calculated when editing

                $("#btnSubmit").html('<i class="fas fa-save me-2"></i>Update Loan');
            } else {
                toastr.error("Loan not found.");
            }
        },
        error: function () {
            toastr.error("Failed to load loan details.");
        }
    });
}


// ============================= ADD / UPDATE LOAN =============================
function AddOrUpdateLoan() {

    const model = getLoanModel();

    $.ajax({
        url: "/Loan/AddLoan",
        type: "POST",
        data: JSON.stringify(model),
        contentType: "application/json; charset=utf-8",
        success: function (response) {

            toastr.success(response);

            clearForm();
            setTimeout(() => getList(), 300);
        },
        error: function (xhr) {
            toastr.error(xhr.responseText || "Operation failed.");
        },
        complete: function () {
            $("#btnSubmit").prop("disabled", false)
                .html('<i class="fas fa-paper-plane me-2"></i>Submit Loan Entry');
        }
    });
}

function getLoanModel() {
    return {
        LoanId: $("#hdnloanid").val() || 0,
        CustomerId: $("#customerName").val(),
        LoanAmount: $("#loanAmount").val(),
        LoanDurationDays: $("#loanDays").val(),
        LoanInterest: $("#interestRate").val(),
        DeductAmount: $("#deductedInterest").text(),
        AmountGivenToCustomer: $("#givenAmount").text(),
        LoanDurationWithMonths: $("#displayLoanDays").text(),
        DailyReturn: $("#dailyReturn").text(),
        TotalPayable: $("#totalRepay").text()
    };
}


// ============================= CLEAR FORM =============================
function clearForm() {
    $("#loanForm")[0].reset();
    $('#customerName').val("").trigger('change');
    $('#hdnloanid').val('');

    $("#isCalculated").val("0");   // RESET CALC FLAG

    $('#resultBox').hide();
    $('#deductedInterest').text('');
    $('#givenAmount').text('');
    $('#displayLoanDays').text('');
    $('#dailyReturn').text('');
    $('#totalRepay').text('');

    $("#btnSubmit").html('<i class="fas fa-paper-plane me-2"></i>Submit Loan Entry');
}


// ============================= LOAD LOAN LIST =============================
function getList() {
    $.ajax({
        url: "/Loan/LoanList",
        type: "GET",
        success: function (data) {
            renderGrid(data);
        },
        error: function () {
            toastr.error("Failed to load loan list.");
        }
    });
}

function renderGrid(data) {

    let html = '';

    $.each(data, function (i, row) {
        html += `
            <tr>
                <td>${i + 1}</td>
                <td>${row.CustomerName}</td>
                <td>${row.LoanAmount}</td>
                <td>${row.LoanDurationDays}</td>
                <td>${row.LoanInterest}</td>

                <td class="text-center">
                    <button class="btn btn-sm btn-primary edit-icon" data-id="${row.LoanId}">
                        <i class="fas fa-edit"></i>
                    </button>
                    <button class="btn btn-sm btn-danger delete-icon" data-id="${row.LoanId}">
                        <i class="fas fa-trash-alt"></i>
                    </button>
                </td>
            </tr>`;
    });

    const table = $('#tblLoan');

    if ($.fn.DataTable.isDataTable(table)) {
        table.DataTable().destroy();
    }

    $('#tblLoantbody').html(html);

    table.DataTable({
        pageLength: 5,
        lengthMenu: [5, 10, 25],
        language: {
            search: "_INPUT_",
            searchPlaceholder: "Search Loan..."
        }
    });
}


// ============================= DELETE LOAN =============================
$(document).on('click', '.delete-icon', function () {
    const loanId = $(this).data("id");

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
        deleteLoan(loanId);
        toastr.clear();
    });

    $(document).off("click", ".cancel-delete").on("click", ".cancel-delete", function () {
        toastr.clear();
    });
});

function deleteLoan(loanId) {

    $.ajax({
        url: "/Loan/DeleteLoanById",
        type: "POST",
        data: { LoanId: loanId },
        success: function () {
            toastr.success("Loan deleted successfully.");
            getList();
        },
        error: function () {
            toastr.error("Failed to delete loan.");
        }
    });
}


// ============================= LOAN CALCULATION =============================
function calculateLoan() {
    const loanAmount = parseFloat(document.getElementById('loanAmount').value) || 0;
    const loanDays = parseInt(document.getElementById('loanDays').value) || 0;
    const interestRate = parseFloat(document.getElementById('interestRate').value) || 0;

    if (loanAmount <= 0) {
        toastr.error("Please enter a valid Loan Amount");
        return;
    }
    if (loanDays <= 0) {
        toastr.error("Please enter valid Days");
        return;
    }
    if (interestRate <= 0) {
        toastr.error("Please enter a valid Interest Rate");
        return;
    }

    const totalMonths = Math.ceil(loanDays / 30);
    const totalInterestPercent = interestRate * totalMonths;
    const deductedInterest = (loanAmount * totalInterestPercent) / 100;
    const givenAmount = loanAmount - deductedInterest;

    const dailyReturn = (loanAmount / loanDays).toFixed(2);
    const totalRepay = (dailyReturn * loanDays).toFixed(2);

    document.getElementById('resultBox').style.display = 'block';
    document.getElementById('displayLoanAmount').textContent = loanAmount.toLocaleString();
    document.getElementById('deductedInterest').textContent = Math.round(deductedInterest).toLocaleString();
    document.getElementById('givenAmount').textContent = Math.round(givenAmount).toLocaleString();
    document.getElementById('displayLoanDays').textContent = `${loanDays} days (${totalMonths} month${totalMonths > 1 ? 's' : ''})`;
    document.getElementById('dailyReturn').textContent = dailyReturn.toLocaleString();
    document.getElementById('totalRepay').textContent = totalRepay.toLocaleString();

    $("#isCalculated").val("1");  // MARK AS CALCULATED

    toastr.success("Loan calculation successful!");
}
