$('#btnRight').click(function (e) {
    var selectedOpts = $('#selectedRoles option:selected');
    if (selectedOpts.length == 0) {
        alert("Nothing to move.");
        e.preventDefault();
    }

    $('#availRoles').append($(selectedOpts).clone());
    $(selectedOpts).remove();
    e.preventDefault();
});

$('#btnLeft').click(function (e) {
    var selectedOpts = $('#availRoles option:selected');
    if (selectedOpts.length == 0) {
        alert("Nothing to move.");
        e.preventDefault();
    }

    $('#selectedRoles').append($(selectedOpts).clone());
    $(selectedOpts).remove();
    e.preventDefault();
});

$('#btnSubmit').click(function (e) {
    $('#selectedRoles option').prop('selected', true);
});