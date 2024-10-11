let toggleButton = window.localStorage.getItem("toggleButton");
let toggle = document.querySelector("#toggleButton");


document.getElementById('toggle-sidebar').addEventListener('click', () => {
    let isActive = localStorage.getItem('sidebar-active') === 'true';
    localStorage.setItem('sidebar-active', !isActive);
    updateSidebar();
});
function updateSidebar() {
    const isActive = localStorage.getItem('sidebar-active') === 'true';
    const sidebar = document.getElementById('mysidebar');
    const dashboaradsidebar = document.getElementById('dashboard-main-container') == null ? document.getElementById('feespayment-main-container') : document.getElementById('dashboard-main-container');
 

    if (isActive) {
        sidebar.classList.add('active');
        dashboaradsidebar.classList.add('active');
    } else {
        sidebar.classList.remove('active');
        dashboaradsidebar.classList.remove('active');
    }
} function showloadingoverlay() {
    const loadingOverlay = document.getElementById('loadingOverlay');
    loadingOverlay.style.display = 'flex'; // Display the loading overlay
}
function hideloadingoverlay() {
    loadingOverlay.style.display = 'none';
}
document.addEventListener('DOMContentLoaded', updateSidebar);

function printPage() {
    var printWindow = window.open('/Home/OR_Print', '_blank');
    // Wait for the new window to load and then trigger the print dialog
    $('#modal-success').modal('show');
    $('#defaultmodal').modal('hide');
    printWindow.onload = function () {
        printWindow.print();
    
    };
 
}

function print_or()
{
    fetch(`/Home/OR_Print`) // Adjust URL to your endpoint
        .then(response => response.text())
        .then(html => {
            document.querySelector('#modal-xl .modal-body-2').innerHTML = html;
            $('#modal-xl').modal('show'); // Show the modal using Bootstrap
        })
        .catch(error => console.error('Error loading content:', error));
  
    //$('#modal-success').modal('show');
}
function printDiv(divId) {

    $('#defaultmodal').modal('hide');
    var iframe = document.createElement('iframe');
    iframe.style.position = 'absolute';
    iframe.style.width = '0';
    iframe.style.height = '0';
    iframe.style.border = 'none';

    document.body.appendChild(iframe);

    var doc = iframe.contentWindow.document;
    doc.open();
    doc.write('<html><head><title>Print</title>');
    doc.write('<style>body{font-family: Arial, sans-serif;}</style>');
    doc.write('</head><body>');
    doc.write(document.getElementById(divId).innerHTML);
    doc.write('</body></html>');
    doc.close();

    iframe.contentWindow.focus();
    iframe.contentWindow.print();

    document.body.removeChild(iframe);
    $('#modal-success').modal('show');c 
}
function loadModal(url, modal, title, size, isToggled) {

    console.log(modal);
    $(modal + ' .overlay').removeClass('d-none');
    $(modal + ' .modal-dialog').removeClass('modal-sm');
    $(modal + ' .modal-dialog').removeClass('modal-md');
    $(modal + ' .modal-dialog').removeClass('modal-lg');
    $(modal + ' .modal-dialog').removeClass('modal-xl');
    $(modal + ' .modal-dialog').removeClass('modal-fullscreen');
    $(modal + ' .modal-dialog').addClass('modal-' + size);
    $(modal + ' .modal-body').html('<div class="mt-5 mb-5"></div>');

    //if not toggle in button
    if (!isToggled)
        $(modal).modal('show');

    $.ajax({
        type: 'GET',
        url: url,
        success: function (res) {
       
            $(modal + ' .modal-body').html(res);
            $(modal + ' .modal-title').html(title);
            $(modal + ' .overlay').addClass('d-none');

            //load tooltip
            $('.modal [data-toggle="tooltip"]')
                .tooltip({ trigger: 'hover' });
        },
        error: function (result) {
            $(modal + ' .modal-body').html(`<p>${result.responseText}</p>`);
            if (result.status == 403 || result.status == 401)
                $(modal + ' .modal-body').html(`<p><b>Unauthorized!</b> Access to the requested resource is forbidden.</p>`);

            $(modal + ' .modal-title').html('Error Message');
            $(modal + ' .overlay').addClass('d-none');

        }
    });
};


document.addEventListener('DOMContentLoaded', function () {
});
function checklist() {
    $.ajax({
        url: "/Home/checquelist",
        data: {},
        type: "POST",
        datatype: "json"
    })
        .done(function (data) {
            //console.log(data);
            $(".checque-list").empty();
            $(".checque-list").append('<option value="" disabled selected>-Select Cheque Type-</option>');
            for (var i = 0; i < data.length; i++) {
                $(".checque-list").append('<option value="' + data[i].chequE_TYPE_CODE + '">' + data[i].chequE_TYPE + "</option>");
            }
        });

}
function checklistcount(count) {
    $.ajax({
        url: "/Home/checquelist",
        data: {},
        type: "POST",
        datatype: "json"
    })
        .done(function (data) {
            //console.log(data);
            $("#check" +count).empty();
            $("#check" + count).append('<option value="" disabled selected>-Select Cheque Type-</option>');
            for (var i = 0; i < data.length; i++) {
                $("#check" + count).append('<option value="' + data[i].chequE_TYPE_CODE + '">' + data[i].chequE_TYPE + "</option>");
            }
        });

}
