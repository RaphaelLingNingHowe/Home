$(document).ready(function () {
    const token = $('input[name="__RequestVerificationToken"]').val();

    $.ajaxSetup({
        headers: {
            'RequestVerificationToken': token
        }
    });
});
