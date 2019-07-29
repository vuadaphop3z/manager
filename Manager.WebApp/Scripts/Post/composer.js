tinymce.init({
    selector: "#BodyContent", theme: "modern", height: 350,
    plugins: [
        "advlist autolink link image lists charmap print preview hr anchor pagebreak",
        "searchreplace wordcount visualblocks visualchars insertdatetime media nonbreaking",
        "table contextmenu directionality emoticons paste textcolor responsivefilemanager"
    ],
    toolbar1: "undo redo | bold italic underline | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | styleselect",
    toolbar2: "| responsivefilemanager | link unlink anchor | image media | forecolor backcolor  | print preview code ",
    image_advtab: true,
    filemanager_crossdomain: true,
    external_filemanager_path: _fileManagerUrl + "/",
    external_plugins: { "filemanager": _fileManagerUrl + "/plugin.min.js" },
    filemanager_title: "File Manager"
});

