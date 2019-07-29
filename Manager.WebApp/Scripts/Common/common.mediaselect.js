// Handles message from ResponsiveFilemanager
//
function OnMessage(e) {
    var event = e.originalEvent;
    // Make sure the sender of the event is trusted
    if (event.data.sender === 'responsivefilemanager') {
        if (event.data.field_id) {
            var fieldID = event.data.field_id;
            var url = event.data.url;
            $('#' + fieldID).val(url).trigger('change');
            $("#fileModal").modal("hide");
            var previewContainer = $('#' + fieldID).data("preview");
            if (previewContainer) {
                var currentPreviewContainer = $("#" + previewContainer);
                var previewImg = currentPreviewContainer.children(".thumbImg").first();
                if (previewImg.length <= 0) {
                    currentPreviewContainer.html('<img src="' + url + '" class="img-res thumbImg img-thum-preview" width="100" height="50" style="float:left;margin-right:10px;" />');
                } else {
                    previewImg.attr('src', url);
                }
            }

            // Delete handler of the message from ResponsiveFilemanager
            $(window).off('message', OnMessage);
        }
    }
}

// Handler for a message from ResponsiveFilemanager
$('.myBrowserBtn').on('click', function () {
    $(window).on('message', OnMessage);
});

function PreviewImageFromBrowseDialog(input) {
    var previewContainer = $(input).data("preview");
    var currentPreviewContainer = $("#" + previewContainer);
    var previewImg = currentPreviewContainer.children(".thumbImg").first();
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            if (previewImg.length <= 0) {
                currentPreviewContainer.html('<img src="' + e.target.result + '" class="img-res thumbImg img-thum-preview" width="100" height="50" style="float:left;margin-right:10px;" />');
            } else {
                previewImg.attr('src', e.target.result);
            }
        };

        reader.readAsDataURL(input.files[0]);
    } else {
        if (previewImg)
            previewImg.remove();
    }
}

$(".FilePreviewChange").change(function () {
    PreviewImageFromBrowseDialog(this);
});

$('.myBrowserBtn').click(function () {
    var affetedItem = $(this).data("control");
    var iframeParms = '/dialog.php?crossdomain=1&type=1&fldr=&field_id=' + affetedItem;
    var iframeFullUrl = _fileManagerUrl + iframeParms;
    $('#fileModalBody').html('<iframe id="fileManagerPopupFrame" src="' + iframeFullUrl + '" frameborder="0" style="overflow:hidden;min-height:600px; height:100%;width:100%"></iframe>');
});