function uploadImage(image) {
    var data = new FormData();
    data.append("image", image);   

    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Post/UploadPostImage';
    params['requestType'] = 'POST';
    params['data'] = data;
    params['cache'] = false;
    params['contentType'] = false;
    params['processData'] = false;

    params['successCallbackFunction'] = function (result) {
        if (result) {
            if (result.data) {
                var image = $('<img>').attr('src', result.data[0]);
                $('.BodyContent').summernote("insertNode", image[0]);
            }
        }
    };
    doAjax(params);
}

var SummernoteDemo = {
    init: function () {
        $(".BodyContent").summernote({
            height: 500,
            lang: 'vi-VN',
            toolbar: [
                // [groupName, [list of button]]
                ['style', ['bold', 'italic', 'underline', 'clear']],
                ['font', ['strikethrough', 'superscript', 'subscript']],
                ['fontsize', ['fontsize']],
                ['fontstyle', ['clear']],
                ['color', ['color']],
                ['para', ['ul', 'ol', 'paragraph', 'style','height']],
                ['Insert', ['picture', 'link','video','table','hr']],
                ['misc', ['fullscreen', 'codeview', 'undo', 'redo','help']]
            ],
            callbacks: {
                onImageUpload: function (image) {
                    uploadImage(image[0]);
                }
            }
        })
    },
    showCode: function () {
        return $(".BodyContent").summernote('code');
    }
};

jQuery(document).ready(function () {
    SummernoteDemo.init();
    $("#frmCreate").on("submit", function () {
        $('#BodyContent').html(SummernoteDemo.showCode());
        $("#frmCreate").submit();      
    });   
});