$(document).ready(function () {

    // Load customer list on page load
    getList();

    // Handle Add / Update button
    $("#btnSubmit").on('click', function () {
        if (!validateCustomerForm()) return;

        const isEdit = $("#hdncustomerid").val() !== "";
        $("#btnSubmit").prop("disabled", true)
            .html('<i class="fas fa-spinner fa-spin me-2"></i>' + (isEdit ? "Updating..." : "Adding..."));

        AddOrUpdateCustomer();
    });
});

// ================= Validation =================
function validateCustomerForm() {
    const name = $("#Name").val().trim();
    if (!name) {
        toastr.warning("Customer Name is required.");
        $("#Name").focus();
        return false;
    }
    return true;
}

// ================= Get Customer List =================
function getList() {
    $.ajax({
        url: "/Customer/CustomerList",
        type: "GET",
        cache: false,
        success: function (data) {
            renderGrid(data);
        },
        error: function () {
            toastr.error("Failed to load customers.");
        }
    });
}

function renderGrid(data) {
    let html = '';
    $.each(data, function (index, row) {
        html += `
            <tr>
                <td data-label="Sr No">${index + 1}</td>
                <td data-label="Name">${row.CustomerName || ''}</td>
                <td data-label="Phone">${row.MobileNo || ''}</td>
                <td data-label="Address">${row.Address || ''}</td>
                <td data-label="AdhaarNo">${row.AdhaarNo || ''}</td>
                <td data-label="PanNo">${row.PanNo || ''}</td>
                <td class="text-center" data-label="Actions">
                    <button class="btn btn-sm btn-primary edit-icon" data-id="${row.C_Id}">
                        <i class="fas fa-edit"></i> Edit
                    </button>
                    <button class="btn btn-sm btn-danger delete-icon" data-id="${row.C_Id}">
                        <i class="fas fa-trash-alt"></i> Delete
                    </button>
                </td>
            </tr>`;
    });

    const $table = $('#tblcustomer');
    const $tbody = $('#tblcustomertbody');

    // Destroy old DataTable safely
    if ($.fn.DataTable.isDataTable($table)) {
        $table.DataTable().clear().destroy();
    }

    // Replace tbody HTML
    $tbody.html(html);

    // Reinitialize DataTable AFTER replacing tbody
    $table.DataTable({
        pageLength: 5,
        lengthMenu: [5, 10, 25, 50],
        language: {
            search: "_INPUT_",
            searchPlaceholder: "Search customers..."
        },
        columnDefs: [
            { targets: [4], orderable: false }
        ]
    });
}

// ================= Edit Customer =================
$(document).on('click', '.edit-icon', function () {
    const C_Id = $(this).data('id');
    EditCustomer(C_Id);
    $('#Name').focus();
});

function EditCustomer(C_Id) {
    $.ajax({
        url: '/Customer/CustomerById',
        type: 'GET',
        data: { C_Id: C_Id },
        success: function (response) {
            if (response) {
                $('#hdncustomerid').val(response.C_Id);
                $('#Name').val(response.CustomerName);
                $('#phone').val(response.MobileNo);
                $('#address').val(response.Address);
                $('#aadharcard').val(response.AdhaarNo);
                $('#pancard').val(response.PanNo);
                $('#btnSubmit').html('<i class="fas fa-save me-2"></i>Update Customer');
            } else {
                toastr.error("Customer not found.");
            }
        },
        error: function () {
            toastr.error("Failed to load customer details.");
        }
    });
}

// ================= Add / Update Customer =================
function AddOrUpdateCustomer() {
    const model = getCustomerModel();
    const isEdit = $("#hdncustomerid").val() !== "";
    const url = "/Customer/AddCustomer";

    $.ajax({
        url: url,
        type: "POST",
        data: JSON.stringify(model),
        contentType: "application/json; charset=utf-8",
        cache: false,
        success: function (response) {
            const msg = response.includes("Updated") ? "updated" : "added";
            toastr.success(`Customer ${msg} successfully!`);
            clearForm();

            // Reload list after successful add/update
            setTimeout(() => getList(), 300);
        },
        error: function (xhr) {
            const err = xhr.responseText || "Operation failed.";
            toastr.error("Error: " + err);
        },
        complete: function () {
            resetSubmitButton(isEdit);
        }
    });
}

function getCustomerModel() {
    return {
        C_Id: $("#hdncustomerid").val() || 0,
        CustomerName: $("#Name").val().trim(),
        MobileNo: $("#phone").val().trim(),
        Address: $("#address").val().trim() ,
        AdhaarNo: $("#aadharcard").val().trim() ,
        PanNo: $("#pancard").val().trim()
    };
}

function resetSubmitButton(isEdit) {
    $("#btnSubmit").prop("disabled", false)
        .html(isEdit
            ? '<i class="fas fa-save me-2"></i>Update Customer'
            : '<i class="fas fa-user-plus me-2"></i>Register Customer'
        );
}

// ================= Clear Form =================
function clearForm() {
    $('#hdncustomerid').val('');
    $('#Name').val('');
    $('#phone').val('');
    $('#address').val('');
    $('#aadharcard').val('');
    $('#pancard').val('');
    $("#btnSubmit").html('<i class="fas fa-user-plus me-2"></i>Register Customer')
        .prop("disabled", false);
}

// ================= Delete Customer =================
$(document).on('click', '.delete-icon', function () {
    const C_Id = $(this).data('id');
    toastr.warning(
        "<br><button type='button' class='btn btn-danger btn-sm confirm-delete'>Yes, Delete</button>" +
        "<button type='button' class='btn btn-secondary btn-sm ms-2 cancel-delete'>Cancel</button>",
        "Confirm Delete",
        {
            allowHtml: true,
            closeButton: false,
            timeOut: 0,
            extendedTimeOut: 0,
            onclick: null
        }
    );

    // Confirm Delete
    $(document).off('click', '.confirm-delete').on('click', '.confirm-delete', function () {
        toastr.remove();
        performDelete(C_Id);
    });

    // Cancel Delete
    $(document).off('click', '.cancel-delete').on('click', '.cancel-delete', function () {
        toastr.remove();
        toastr.info("Delete canceled.");
    });
});

function performDelete(C_Id) {
    $.ajax({
        url: '/Customer/DeleteById',
        type: 'POST',
        data: { C_Id: C_Id },
        success: function (response) {
            if (response && response.includes("Successfully")) {
                toastr.success("Customer deleted successfully!");
            } else {
                toastr.warning(response || "Could not delete customer.");
            }

            // Refresh list after delete
            setTimeout(() => getList(), 300);
        },
        error: function () {
            toastr.error("Failed to delete customer.");
        }
    });
}

// ================= Toastr Config =================
toastr.options = {
    closeButton: true,
    progressBar: true,
    positionClass: "toast-top-right",
    timeOut: 3000,
    extendedTimeOut: 1000,
    showEasing: "swing",
    hideEasing: "linear",
    showMethod: "fadeIn",
    hideMethod: "fadeOut"
};
