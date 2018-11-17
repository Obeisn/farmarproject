function PopupForm(url) {
    var formDiv = $('<div/>');
    $.get(url)
        .done(function (response) {
            formDiv.html(response);

            Popup = formDiv.dialog({
                autoOpen: true,
                resizable: false,
                title: '',
                height: 500,
                width: 700,
                close: function () {
                    Popup.dialog('destroy').remove();
                }

            });
        });
}

    function readURL(input) {
        if (input.files && input.files[0]) {
            var reader = new FileReader();

            reader.onload = function (e) {
        $('#imgp').attr('src', e.target.result);
    }
    reader.readAsDataURL(input.files[0]);
}

}
    $("#File1").change(function () {
        readURL(this);
    });



