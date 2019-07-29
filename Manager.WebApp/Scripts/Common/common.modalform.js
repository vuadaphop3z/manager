//$(function () {

//    $.ajaxSetup({ cache: false });    

//    $("body").on("click", "a[data-modal]", function (e) {
//        // hide dropdown if any
//        //$(e.target).closest('.btn-group').children('.dropdown-toggle').dropdown('toggle');
//        showLoading();
//        $('#myModalContent').load(this.href, function (response, status, xhr) {
//            if (status == "error") {
//                var msg = "Sorry but there was an error: ";
//                alert(msg + xhr.status + " " + xhr.statusText);
//            }
//            else {
//                $('#myModal').modal({
//                    /*backdrop: 'static',*/
//                    keyboard: true
//                }, 'show');

//                bindForm(this);
//            }

//            hideLoading();
//        });

//        return false;
//    });
//});

LoadModalFormEvents();

function LoadModalFormEvents() {
    $.ajaxSetup({ cache: false });

    $("body").on("click", "a[data-modal]", function (e) {
        // hide dropdown if any
        //$(e.target).closest('.btn-group').children('.dropdown-toggle').dropdown('toggle');
        showLoading();
        $('#myModalContent').load(this.href, function (response, status, xhr) {
            if (status == "error") {
                var msg = "Sorry but there was an error: ";
                alert(msg + xhr.status + " " + xhr.statusText);
            }
            else {
                $('#myModal').modal({
                    /*backdrop: 'static',*/
                    keyboard: true
                }, 'show');

                bindForm(this);
            }

            hideLoading();
        });

        return false;
    });
}

function ShowModal(element, classSize) {
    $('#myModalContent').load(element.href, function (response, status, xhr) {
        if (status == "error") {
            var msg = "Sorry but there was an error: ";
            alert(msg + xhr.status + " " + xhr.statusText);
        }
        else {
            $('#myModal').modal({
                /*backdrop: 'static',*/
                keyboard: true
            }, 'show');
            bindForm(this);
        }

        if (classSize)
            $(".modal-dialog").addClass(classSize);
    });

    return false;
}

function bindForm(dialog) {
    
        $('form', dialog).submit(function () {           
            
            var dataPosted = $(this).serialize();
            var $form = $(this);
            if ($form.valid()) {
                showLoading();
                $.ajax({
                    url: this.action,
                    type: this.method,
                    data: dataPosted,
                    success: function (result) {
                        hideLoading();
                        $('#myModal').modal('hide');
                        //Refresh
                        //alert('The chosen operation is executed successfully');
                        var msg = 'The chosen operation is executed successfully.';
                        var title = LanguageDic["LB_NOTIFICATION"];
                        var callback = null;
                        if (result.hasOwnProperty('message'))
                            msg = result.message;

                        if (result.hasOwnProperty('title'))
                            title = result.title;

                        if (result.hasOwnProperty('clientcallback'))
                            callback = result.clientcallback;

                        if (result.success) {                          
                            if (!callback) {
                                $.showSuccessMessage(title, msg, function () { location.reload(); });
                            } else {
                                $.showSuccessMessage(title, msg, function () { eval(callback); });
                            }
                            //location.reload();
                        } else {
                            if (!callback) {
                                $.showErrorMessage(title, msg, null);
                            } else {
                                $.showErrorMessage(title, msg, function () { eval(callback); });
                            }

                           // bindForm();
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        //var responseTitle = $(jqXHR.responseText).filter('title').get(0);                    
                        hideLoading();
                        var responseTitle = "Error encountered"
                        $.showErrorMessage('Error message', $(responseTitle).text() + "\n" + formatErrorMessage(jqXHR, errorThrown), null);
                    },
                });
            }
            
            return false;
        });           
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