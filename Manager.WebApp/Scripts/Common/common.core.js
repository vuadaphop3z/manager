$.fn.exists = function () {
    return this.length !== 0;
}

String.prototype.endsWith = function (suffix) {
    return this.indexOf(suffix, this.length - suffix.length) !== -1;
};

var doAjax_params_default = {
    'showloading': true,
    'url': null,
    'requestType': "GET",
    'contentType': 'application/x-www-form-urlencoded; charset=UTF-8',
    'dataType': 'json',
    'processData': true,
    'async': true,
    'data': {},
    'beforeSendCallbackFunction': null,
    'successCallbackFunction': null,
    'completeCallbackFunction': null,
    'errorCallBackFunction': null,
    'context': null
};


function doAjax(doAjax_params) {

    var url = doAjax_params['url'];
    var showloading = doAjax_params['showloading'];
    var requestType = doAjax_params['requestType'];
    var contentType = doAjax_params['contentType'];
    var dataType = doAjax_params['dataType'];
    var data = doAjax_params['data'];
    var context = doAjax_params['context'];
    var processData = doAjax_params['processData'];
    var async = doAjax_params['async'];
    var beforeSendCallbackFunction = doAjax_params['beforeSendCallbackFunction'];
    var successCallbackFunction = doAjax_params['successCallbackFunction'];
    var completeCallbackFunction = doAjax_params['completeCallbackFunction'];
    var errorCallBackFunction = doAjax_params['errorCallBackFunction'];

    //make sure that url ends with '/'
    if (!url.endsWith("/")) {
        url = url + "/";
    }

    if (context) {
        var me = $(context);

        if (me.data('requestRunning')) {
            return false;
        }

        me.data('requestRunning', true);
    }

    //data.push({ __RequestVerificationToken: $('input[name = "__RequestVerificationToken"]').val() });
    data.__RequestVerificationToken = $('input[name = "__RequestVerificationToken"]').val();

    $.ajax({
        url: url,
        crossDomain: true,
        async: async,
        type: requestType,
        contentType: contentType,
        dataType: dataType,
        processData: processData,
        data: data,
        context: context,
        headers: { 'X-Requested-With': 'XMLHttpRequest' },
        beforeSend: function (jqXHR, settings) {
            if (showloading) {
                showLoading();
            }

            if (typeof beforeSendCallbackFunction === "function") {
                if (context)
                    beforeSendCallbackFunction(context);
                else
                    beforeSendCallbackFunction();
            }
        },
        success: function (data, textStatus, jqXHR) {
            if (data.Error) {
                if (data.clientcallback)
                    eval(data.clientcallback);

                return false;
            }

            if (typeof successCallbackFunction === "function") {
                successCallbackFunction(data);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            if (typeof errorCallBackFunction === "function") {
                errorCallBackFunction(errorThrown);
            }

        },
        complete: function (jqXHR, textStatus) {
            hideLoading();
            if (typeof completeCallbackFunction === "function") {
                completeCallbackFunction();
            }

            if (context)
                me.data('requestRunning', false);
        }
    });
}

const numberWithCommas = (x, seperateChar = ".") => {
    return x.toString().replace(/\B(?=(\d{3})+(?!\d))/g, seperateChar);
}

function FeatureCommingSoon() {
    bootbox.alert(LanguageDic["LB_FEATURE_COMMING"]);
}


function AutoSetTime() {
    $(".livetimestamp").each(function () {
        SettingElement(this);
    });
    $(".livetimestamp-title").each(function () {
        SettingElementTitle(this);
    });
}

function NeedToLogin() {
    window.location.href = "/webauth/login";
}

function NeedToLogout() {
    window.location.href = "/webauth/logout";
}

(function ($) {
    $.fn.changeElementType = function (newType) {
        var attrs = {};

        $.each(this[0].attributes, function (idx, attr) {
            attrs[attr.nodeName] = attr.nodeValue;
        });

        this.replaceWith(function () {
            return $("<" + newType + "/>", attrs).append($(this).contents());
        });
    }
})(jQuery);

function InitNumberFormat(seperateChar = ".") {
    $(".number-format").bind("input", function () {
        // format number
        $(this).val(function (index, value) {
            return value
                .replace(/\D/g, "")
                .replace(/\B(?=(\d{3})+(?!\d))/g, ",")
                ;
        });

        var val = $(this).val().toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');

        var currentVal = $(this).val().replace(",", "");
        //currentVal = $(this).val().replace(".", "");
        var newValue = $(this).val().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,');
        $(this).val(newValue);

        //return $(this).val().toString().replace(/(\d)(?=(\d{3})+(?!\d))/g, '$1,')
    });

}

function searching(keyword) {
    var url = window.location.href;

    if (url.indexOf("/search/") > -1) {
        var index = url.indexOf("=");
        history.pushState({}, document.title, url.substring(0, index + 1) + keyword);
        reSearch();
    }
    else {
        window.location.href = "/search/top/q=" + keyword;
    }
}

function GetDateTimeNowByFormat() {
    var mydate = new Date();
    year = mydate.getFullYear();
    month = mydate.getMonth();

    months = new Array('01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12');
    d = mydate.getDate();
    day = mydate.getDay();
    days = new Array('Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday');
    h = mydate.getHours();

    var strMonth = "";
    var strDay = "";
    if (h < 10) {
        h = "0" + h;
    }
    m = mydate.getMinutes();
    if (m < 10) {
        m = "0" + m;
    }

    if (month < 10) {
        strMonth = "0" + month;
    }

    if (day < 10) {
        strDay = "0" + day;
    }

    s = mydate.getSeconds();
    if (s < 10) {
        s = "0" + s;
    }

    result = '' + year + months[month] + d + h + m + s;
    
    return result;
}

function DynamicRealTime() {
    var mydate = new Date();
    year = mydate.getFullYear();
    month = mydate.getMonth();

    months = new Array('01', '02', '03', '04', '05', '06', '07', '08', '09', '10', '11', '12');
    d = mydate.getDate();
    day = mydate.getDay();
    days = new Array('Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday');
    h = mydate.getHours();

    var strMonth = "";
    var strDay = "";
    if (h < 10) {
        h = "0" + h;
    }
    m = mydate.getMinutes();
    if (m < 10) {
        m = "0" + m;
    }

    if (month < 10) {
        strMonth = "0" + month;
    }

    if (day < 10) {
        strDay = "0" + day;
    }

    s = mydate.getSeconds();
    if (s < 10) {
        s = "0" + s;
    }

    result = '' + year + '/' + months[month] + '/' + d + ' ' + h + ':' + m + ':' + s;
    $('.dynamic-timer').html(result);
    setTimeout('DynamicRealTime();', '1000');
    return true;
}

$("body").on("click", ".reset-btn", function () {
    var thisForm = $(this).closest("form");
    thisForm.find(".m-input").val("");
    thisForm.find(".selectpicker").val(0);
    thisForm.find(".selectpicker").each(function () {
        $(this).val($(this).find("option:first").val());
        if ($(this).hasClass("select-trigger")) {
            $(this).find('option').not(':first').remove();
        }

        $(this).selectpicker("refresh");
    });

    thisForm.find(".m-select2").each(function () {
        $(this).val('').trigger('change');
    });

    thisForm.find('input:checkbox').each(function () {
        $(this).removeAttr('checked');
    });
});

$(document).ready(function () {  
    DynamicRealTime();
});

function initContenteditable() {
    var editors = document.querySelectorAll("div[contenteditable]");
    if (editors.length > 0) {
        for (var i = 0; i < editors.length; i++) {
            editors[i].addEventListener("paste", function (e) {
                e.preventDefault();
                var text = e.clipboardData.getData("text/plain");
                document.execCommand("insertText", true, text);
                if ($(this).find("div").length > 0) {
                    $(this).find("div").changeElementType("span");
                }
                $(this).html($(this).html().replace(/<span><\/span>/g, "<br>"));
                $(this).find("span").each(function () {
                    if ($(this).text().length == 1) {
                        $(this).replaceWith("<br>");
                    }
                    else {
                        $(this).after("<br>");
                    }
                });
            });
        }
    }
}

function initGetTextInContentEditale() {

    var editors = document.querySelectorAll("div[contenteditable].msg2send");
    if (editors.length > 0) {
        for (var i = 0; i < editors.length; i++) {
            editors[i].addEventListener("paste", function (e) {
                e.preventDefault();
                var target = this;
                var text = e.clipboardData.getData("text/plain");
                document.execCommand("insertHTML", false, text);
                if ($(this).find("div").length > 0) {
                    $(this).find("div").changeElementType("span");
                }
                $(this).html($(this).html().replace(/<span><\/span>/g, "<br>"));
                $(this).find("span").each(function () {
                    if ($(this).text().length == 1) {
                        $(this).replaceWith("<br>");
                    }
                    else {
                        $(this).after("<br>");
                    }
                });

                placeCaretAtEnd(target);
            });
        }
    }
}

function placeCaretAtEnd(el) {
    el.focus();
    if (typeof window.getSelection != "undefined"
        && typeof document.createRange != "undefined") {
        var range = document.createRange();
        range.selectNodeContents(el);
        range.collapse(false);
        var sel = window.getSelection();
        sel.removeAllRanges();
        sel.addRange(range);
    } else if (typeof document.body.createTextRange != "undefined") {
        var textRange = document.body.createTextRange();
        textRange.moveToElementText(el);
        textRange.collapse(false);
        textRange.select();
    }
    $(el).parent().scrollTop($(el).parent()[0].scrollHeight);
}


var confirmButtons = {
    confirm: {
        label: LanguageDic["BT_ALLOW"],
        className: 'btn-danger'
    },
    cancel: {
        label: LanguageDic["BT_CANCEL"],
        className: 'btn-info'
    }
};

function ConfirmFirst(callback, message, data) {
    if (!message)
        message = LanguageDic["LB_CONFIRM"];
    bootbox.confirm({
        message: message,
        buttons: confirmButtons,
        callback: function (confirmed) {
            if (confirmed)
                callback(data);
        }
    });
    return false;
}

function ConfirmDynamic(title, message) {
    if (!message)
        message = LanguageDic["LB_CONFIRM"];
    bootbox.confirm({
        title: title,
        message: message,
        buttons: confirmButtons,
        callback: function (confirmed) {
            if (confirmed) {
                return true;
            }
            else {
                return false;
            }
        }
    });
}

bootbox.showmessage = function (options, isSuccess, clientcallback) {
    // Set the defaults
    var className = (!isSuccess) ? 'text-danger' : 'text-info';
    var title = (!isSuccess) ? LanguageDic['LB_ERROR'] : LanguageDic['LB_NOTIFICATION'];
    var defaults = {
        className: className,
        title: title,
        message: 'An Error has occurred. Please contact your system administrator.',
        closeButton: false,
        buttons: {
            "OK": function () {
                if (clientcallback) {
                    clientcallback();
                }
            }
        }
    };

    // Extend the defaults with any passed in options
    var settings = $.extend(defaults, options);
    var iconHtml = (!isSuccess) ? '<i class="fa fa-warning"></i>' : '<i class="fa fa-check"></i>';
    // Build and show the dialog
    bootbox.dialog({
        className: settings.className,
        title: settings.title,
        message: iconHtml + ' ' + settings.message,
        closeButton: settings.closeButton,
        buttons: settings.buttons
    });
};

function ScrollToElement(el) {
    el.addClass("focuss");
    $('html, body').animate({
        scrollTop: (el.offset().top)
    }, 500);

    setTimeout(function () {
        el.removeClass("focuss");
    }, 3000);
}

function DoNoThing() {

}

function ShowMsg(msg, isSuccess) {
    if (isSuccess)
        toastr["success"](msg, 'Thông báo');
    else
        toastr["error"](msg, 'Thông báo');
}

function formatErrorMessage(jqXHR, exception) {

    if (jqXHR.status === 0) {
        return ('Not connected.\nPlease verify your network connection.');
    } else if (jqXHR.status == 404) {
        return ('The requested page not found. [404].\n' + jqXHR.responseText);
    } else if (jqXHR.status == 500) {
        var resultObj = $.parseJSON(jqXHR.responseText);
        return ('' + resultObj.message);
    } else if (exception === 'parsererror') {
        return ('Requested JSON parse failed.');
    } else if (exception === 'timeout') {
        return ('Time out error.');
    } else if (exception === 'abort') {
        return ('Ajax request aborted.');
    } else {
        return ('Uncaught Error.\n' + jqXHR.responseText);
    }
}

function ShowMyModalAgain(modalId = "myModal") {   
    var myModal = $("#" + modalId);
    myModal.modal("toggle");   
}

function showLoading() {
    $('.common-loading').show();
}
function hideLoading() {
    $('.common-loading').fadeOut(1000);
}

function urlfriendly(str) {
    //Tiến hành thay thế , lọc bỏ dấu cho chuỗi
    for (var i = 1; i < VietnameseSigns.Length; i++) {
        for (var j = 0; j < VietnameseSigns[i].Length; j++)
            str = str.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
    }

    //str = str.Replace(" ", "-").Replace(",", "-").Replace(".", "-").Replace("–","-").Replace("&","-").ToLower();
    //while (str.contains("--"))
    //{
    //    str = str.Replace("--", "-");
    //}


    str = URLFriendly(str);
    //console.log(str);
    if (str == "") {
        str = "invalid";
    }

    return str;
}

var VietnameseSigns = [

    "aAeEoOuUiIdDyY",

    "áàạảãâấầậẩẫăắằặẳẵ",

    "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",

    "éèẹẻẽêếềệểễ",

    "ÉÈẸẺẼÊẾỀỆỂỄ",

    "óòọỏõôốồộổỗơớờợởỡ",

    "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",

    "úùụủũưứừựửữ",

    "ÚÙỤỦŨƯỨỪỰỬỮ",

    "íìịỉĩ",

    "ÍÌỊỈĨ",

    "đ",

    "Đ",

    "ýỳỵỷỹ",

    "ÝỲỴỶỸ"

];

function URLFriendly(title) {
    if (title == null) return "";

    var maxlen = 80;
    var len = title.length;
    var prevdash = false;
    var sb = "";
    var c;

    for (var i = 0; i < len; i++) {
        if (i <= maxlen) {
            c = title[i];
            if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9')) {
                sb += c;
                prevdash = false;
            }
            else if (c >= 'A' && c <= 'Z') {
                // tricky way to convert to lowercase
                sb += c.toLowerCase();
                prevdash = false;
            }
            else if (c == ' ' || c == ',' || c == '.' || c == '/' ||
                c == '\\' || c == '-' || c == '_' || c == '=') {
                if (!prevdash && sb.length > 0) {
                    sb += '-';
                    prevdash = true;
                }
            }
            else {
                var prevlen = sb.length;
                sb += RemapInternationalCharToAscii(c);
                if (prevlen != sb.length) prevdash = false;
            }
        }
    }

    //console.log(sb);
    if (prevdash)
        return sb.Substring(0, sb.length - 1);
    else
        return sb;
}

function RemapInternationalCharToAscii(c) {
    var s = c.toLowerCase();
    if ("àáảạãâẩầấậẫăẳắằặẵäåą".indexOf(s) != -1) {
        return "a";
    }
    else if ("èéẹẻẽêểếễệềëę".indexOf(s) != -1) {
        return "e";
    }
    else if ("ìíĩỉịîïı".indexOf(s) != -1) {
        return "i";
    }
    else if ("òóỏọõôổộốồỗõöøőð".indexOf(s) != -1) {
        return "o";
    }
    else if ("ùúũủụûüŭů".indexOf(s) != -1) {
        return "u";
    }
    else if ("çćčĉ".indexOf(s) != -1) {
        return "c";
    }
    else if ("żźž".indexOf(s) != -1) {
        return "z";
    }
    else if ("śşšŝ".indexOf(s) != -1) {
        return "s";
    }
    else if ("ñń".indexOf(s) != -1) {
        return "n";
    }
    else if ("ýỳỵỷỹÿ".indexOf(s) != -1) {
        return "y";
    }
    else if ("ğĝ".indexOf(s) != -1) {
        return "g";
    }
    else if (s == 'ř') {
        return "r";
    }
    else if (s == 'ł') {
        return "l";
    }
    else if (s == 'đ') {
        return "d";
    }
    else if (s == 'ß') {
        return "ss";
    }
    else if (s == 'Þ') {
        return "th";
    }
    else if (s == 'ĥ') {
        return "h";
    }
    else if (s == 'ĵ') {
        return "j";
    }
    else {
        return "";
    }
}
function forceNumber(n, t) {
    var r = window.Event && t.which ? t.which : t.keyCode,
        i;
    return r == 9 ? !0 : r >= 48 && r <= 57 || r == 8 ? (i = n.value.replace(/,/g, ""), r == 8 && i.length != 0 && (i = i.substr(0, i.length - 1)), parseFloat(i) == 0 && (i = ""), !0) : (t.returnValue = !1, !1)
}

function exportTableToCSV($table, filename) {
    var universalBOM = "\uFEFF";
    var $rows = $table.find('tr:has(td),tr:has(th)'),

        // Temporary delimiter characters unlikely to be typed by keyboard
        // This is to avoid accidentally splitting the actual contents
        tmpColDelim = String.fromCharCode(11), // vertical tab character
        tmpRowDelim = String.fromCharCode(0), // null character

        // actual delimiter characters for CSV format
        colDelim = '","',
        rowDelim = '"\r\n"',

        // Grab text from table into CSV formatted string
        csv = '"' + $rows.map(function (i, row) {
            var $row = jQuery(row), $cols = $row.find('td,th');

            return $cols.map(function (j, col) {

                var $col = jQuery(col), text = $col.text().trim();
                return text; // escape double quotes

            }).get().join(tmpColDelim);

        }).get().join(tmpRowDelim)
            .split(tmpRowDelim).join(rowDelim)
            .split(tmpColDelim).join(colDelim) + '"',



        // Data URI        
        csvData = 'data:application/csv;charset=utf-8,' + encodeURIComponent(universalBOM + csv);

    if (window.navigator.msSaveBlob) { // IE 10+
        //alert('IE' + csv);
        window.navigator.msSaveOrOpenBlob(new Blob([csv], { type: "text/plain;charset=utf-8;" }), filename);
    }
    else {
        jQuery(this).attr({ 'download': filename, 'href': csvData, 'target': '_blank' });
    }
}

// This must be a hyperlink
$("body").on("click", ".exportcsv", function (event) {
    var fileName = "";
    fileName = $(this).data("filename");

    if (fileName.length === 0) {
        fileName = "Export";
    }

    fileName = fileName + "_" + GetDateTimeNowByFormat();

    var tableId = "";
    tableId = $(this).data("tableid");

    if (tableId.length === 0) {
        tableId = "dataTable";
    }

    exportTableToCSV.apply(this, [$('#' + tableId), fileName + '.csv']);
});