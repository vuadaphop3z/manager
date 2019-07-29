(function ($) {
    $.fn.serializeJSON = function () {
        var json = {};
        jQuery.map($(this).serializeArray(), function (n, i) {
            json[n['name']] = n['value'];
        });
        return json;
    };

    $.fn.formToObj = function () {

        var fields = {};

        $(this).find(":input").each(function () {
            // The selector will match buttons; if you want to filter
            // them out, check `this.tagName` and `this.type`; see
            // below

            /*
            var inputType = this.tagName.toUpperCase() === "INPUT" && this.type.toUpperCase();
            if (inputType !== "BUTTON" && inputType !== "SUBMIT") {
                // ...include it, either it's an `input` with a different `type`
                // or it's a `textarea` or a `select`...
            }
            */
            fields[this.name] = $(this).val();
        });
    };


    $.fn.objectifyForm = function () {
        returnArray = {};
        formArray = $(this).serializeArray();
        for (var i = 0; i < formArray.length; i++) {
            returnArray[formArray[i]['name']] = formArray[i]['value'];
        }
        return returnArray;
    };


})(jQuery);