(function ($) {

    $.showErrorMessage = function (title, text, afterCloseEvent) {
        swal({
            title: "<div class='text-danger'>" + title + "</div>",
            //text: text,
            html: "<div class='text-danger'>" + text + "</div>",
            type: "error",
            confirmButtonText: "OK",
            reverseButtons: !0
        }).then(function (e) {
            if (e.value) {
                if (afterCloseEvent && (typeof afterCloseEvent == "function")) {
                    afterCloseEvent();
                }
            }

        });

        return false;
    };


    $.showSuccessMessage = function (title, text, afterCloseEvent) {

        //toastr.options = {
        //    "closeButton": true,
        //    "debug": false,
        //    "newestOnTop": false,
        //    "progressBar": true,
        //    "positionClass": "toast-top-right",
        //    "preventDuplicates": true,
        //    "onclick": afterCloseEvent,
        //    "showDuration": "300",
        //    "hideDuration": "1000",
        //    "timeOut": "2000",
        //    "extendedTimeOut": "1000",
        //    "showEasing": "swing",
        //    "hideEasing": "linear",
        //    "showMethod": "fadeIn",
        //    "hideMethod": "fadeOut"
        //};

        //toastr.info(title, text);

        swal({
            title: title,
            //text: text,
            html : text,
            type: "success",            
            confirmButtonText: "OK",
            reverseButtons: !0
        }).then(function (e) {
            if (e.value) {
                if (afterCloseEvent && (typeof afterCloseEvent == "function")) {
                    afterCloseEvent();
                } 
            }
             
            });

        return false;
    };
})(jQuery);