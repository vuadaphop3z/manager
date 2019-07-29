function CancelChangeAble() {
    jQuery(".change-able-ctrl").remove();
    jQuery(".change-able-frm").remove();
}

function ShowChangeAble() {
    var editCtrl = jQuery(".change-able-edit");
    var action = editCtrl.data("action");
    var currentVal = editCtrl.data("val");
    var customClass = editCtrl.data("class");
    if (!action) action = "";
    if (!currentVal) currentVal = "";
    if (!customClass) customClass = "";

    var frmContainer = '<form class="change-able-frm form-inline" style="display: inline-flex;">';
    var editorInput = '<input type="text" class="change-able-val form-control text-right ' + customClass + '" value="' + currentVal + '" />&nbsp;';
    var saveBtn = '<button type="button" class="change-able-save form-control btn btn-info btn-sm" onclick="' + action + '">Save</button>&nbsp;';
    var cancelBtn = '<button type="button" class="change-able-cancel form-control btn btn-warning btn-sm" onclick="CancelChangeAble();">Cancel</button>';

    frmContainer += editorInput + saveBtn + cancelBtn + "</form>";

    if (jQuery(".change-able-frm").is(":visible")) {
        jQuery(".change-able-frm").remove();
    } 
    editCtrl.parent().append(frmContainer);

    $(".ip-number-vn").number(true, 0);
}

jQuery(".change-able").hover(function () {
    jQuery(".change-able-ctrl").remove();
    var action = jQuery(this).data("action");
    var currentVal = jQuery(this).data("val");
    var customClass = jQuery(this).data("class");
    if (!action) action = "";
    if (!currentVal) currentVal = "";
    if (!customClass) customClass = "";

    var editBtn = '<i onclick="ShowChangeAble()" class="fa fa-pencil change-able-ctrl change-able-edit" style="cursor:pointer;color:orange;margin-left:3px" data-action="' + action + '" data-val="' + currentVal + '"data-class="' + customClass + '"></i>';
    if (jQuery(".change-able-frm").is(":visible")) {
        return false;
    } else {
        jQuery(this).append(editBtn);
    }
}, function () {
    jQuery(".change-able-ctrl").remove();
});
