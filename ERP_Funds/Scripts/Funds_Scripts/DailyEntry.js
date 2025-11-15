$(document).ready(function () {

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
                    $("#paidtoday").val(loan.DailyReturn);
                }
            }
        });

    });

});
