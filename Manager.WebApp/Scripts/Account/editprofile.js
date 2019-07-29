
$(function () {
    $(".btn-control").click(function () {
        var myControl = $(this).data("control");
        var myLabel = $(this).data("label");
        var currentText = $(this).parent().parent().find(".item-text");
        var currentVal = currentText.find("span").text();

        FillForm(myControl, myLabel, currentVal);
    });

    $("#btnUploadFile").click(function () {
        $("#AvatarFileUpload").click();
    });
});

function FillForm(control, label, value) {
    $(".item .edit-form").remove();
    $(".item .plain").removeClass("hidden");
    $(".item").each(function () {
        if ($(this).attr("id") == control) {
            $(this).append($(".edit-template").html());
            var myForm = $(this).find(".edit-form");
            myForm.attr("data-form", control);
            myForm.addClass("active-form");

            $("#" + control).find(".plain").addClass("hidden");

            if ($(this).hasClass("date")) {
                $(this).find("input").mask("99/99/9999", { placeholder: "dd/mm/yyyy" });               
            }

            $(this).find("input").val(value);
           
            $(this).find(".edit-label").html(label);            
        }
    });
}

function CancelModify() {
    $(".item").find(".plain").removeClass("hidden");
    $(".item").find(".edit-form").remove();
}

function SaveInfo() {
    var currentForm = $(".active-form");
    var currentText = currentForm.find("input");
    var errorInput = currentForm.find(".error-input");
    if (!currentText.val()) {
        errorInput.html(LanguageDic["VAL_ERROR_NULL"]);
        currentText.focus();
        return false;
    }

    var currentAction = currentForm.data("form");
    SavingExecution(currentAction, currentText.val());

    return false;
}

function ApplyChanged() {
    var currentForm = $(".active-form");
    var currentVal = currentForm.find("input").val();
    var currentText = currentForm.parent().find(".item-text");
    currentText.find("span").text(currentVal);
    CancelModify();
}

function afterSavingExecution(data) {
    if (data) {
        if (data.success) {
            bootbox.showmessage({
                message: LanguageDic["MS_UPDATE_PROFILE_SUCCESS"]
            }, true); 

            ApplyChanged();
        } else {
            if (data.message) {
                bootbox.showmessage({
                    message: data.message
                }, false); 
            }
        }
    }
}

function SavingExecution(field, value) {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Account/Update';
    params['requestType'] = 'POST';
    params['data'] = { field: field, value: value };
    params['dataType'] = "json";
    params['context'] = $(".btn-save");
    params['successCallbackFunction'] = afterSavingExecution;
    doAjax(params);
}

$("#AvatarFileUpload").on("change", function (e) {
    var files = e.target.files;
    if (files[0]['size'] / 1024 / 1024 >= _MaxFileSizeUpload) { // anh qua 2 MB
        bootbox.showmessage({
            message: LanguageDic["COMMON_ERROR_MAXFILEUPLOAD"].replace("{0}", _MaxFileSizeUpload)
        }, false); 
        return false;
    }

    UploadAvatar();
});

function UploadAvatar() {
    var params = $.extend({}, doAjax_params_default);

    var data = new FormData();
    $.each($("#AvatarFileUpload")[0].files, function (i, file) {
        data.append('file', file);
    });

    // Add the uploaded image content to the form data collection
    data.append("UserId", $("#CurrentEditorId").val());
    data.append("__RequestVerificationToken", $('input[name = "__RequestVerificationToken"]').val());

    params['url'] = "/Account/UploadAvatar";
    params['requestType'] = 'POST';
    params['data'] = data;
    params['contentType'] = false;
    params['processData'] = false;


    params['successCallbackFunction'] = afterUploadAvatarSuccess;
    doAjax(params);
}

function afterUploadAvatarSuccess(data) {
    if (data) {
        if ((data.Code != 1)) {
            if (data.message)
                bootbox.showmessage({
                    message: data.message
                }, false); 
            else {
                if (data.html)
                    bootbox.showmessage({
                        message: data.html
                    }, false); 
            }
        }

        $('#preview-pane .preview-container img').attr('src', data.data);
        var img = $('#crop-avatar-target');
        img.attr('src', data.data);

        if (!keepUploadBox) {
            $('#avatar-upload-box').addClass('hidden');
        }
        $('#avatar-crop-box').removeClass('hidden');
        initAvatarCrop(img);
        $("#ChangeAvatar").addClass("hidden");
        $('#AvatarFileUpload').val('');
    }
}

function SaveAvatar() {
    var params = $.extend({}, doAjax_params_default);
    var img = $('#avatar-crop-box').find(".jcrop-preview");
    var post = $('.jcrop-holder div').first();

    var data = new FormData();
    data.append("w", post.css('width'));
    data.append("h", post.css('height'));
    data.append("l", post.css('left'));
    data.append("t", post.css('top'));
    data.append("fileName", img.attr('src'));
    data.append("__RequestVerificationToken", $('input[name = "__RequestVerificationToken"]').val());

    params['url'] = "/Account/CropAvatar";
    params['requestType'] = 'POST';
    params['data'] = data;
    params['contentType'] = false;
    params['processData'] = false;


    params['successCallbackFunction'] = afterSaveAvatarSuccess;

    doAjax(params);
}

function UpdateAvatarCache(value) {
    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Account/UpdateAvatar';
    params['requestType'] = 'POST';
    params['data'] = { field: "avatar", value: value };
    params['dataType'] = "json";
    doAjax(params);
}

function afterSaveAvatarSuccess(data) {
    if (data) {
        if (!data.success) {
            if (data.message) {
                bootbox.showmessage({
                    message: data.message
                }, false);
            }
        } else {
            $('#avatar-result img').attr('src', data.data);
            $("#Avatar").val(data.data);

            //$('#avatar-result').removeClass('hidden');
            $('#avatar-upload-box').removeClass('hidden');
            $('#avatar-upload-form').removeClass('hidden');
            $('.upload-progress').addClass('hidden');
            if (!keepCropBox) {
                $('#avatar-crop-box').addClass('hidden');
            }
            clearCropBox();

            $("#ChangeAvatar").removeClass("hidden");
            $(".profile-avatar").attr("src", data.data);
            UpdateAvatarCache(data.data);
        }
    }
}

function CancelCropAvatar() {
    $('#avatar-upload-box').removeClass('hidden');
    $('#avatar-upload-form').removeClass('hidden');
    $('.upload-progress').addClass('hidden');
    $('#avatar-crop-box').addClass('hidden');
    clearCropBox();

    $("#ChangeAvatar").removeClass("hidden");
    $("#AvatarFileUpload").val("");
}