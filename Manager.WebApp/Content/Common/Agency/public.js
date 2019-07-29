var postData = { licenseKey: MyBKConfigs.licenseKey };
jQuery(document).ready(function () {
    $.post(MyBKBasePath + "/demo", postData, function (result) {
        if (result) {
            InitForm(result.Success);
            if (!result.Success) {
                alert(result.Message);
            }
        }
    });
});

function InitForm(isValid) {
    var loaderImg = MyBKBasePath + '/Content/images/loading.gif';
    var topElem = '<div class="row" id="booking-header" style=" cursor: pointer; background-color: #a2ff67; margin-right: 0px; margin-left: 0; padding: 5px 0px 5px 5px; color: #212121; font-weight: bold;"><div class="col-md-6">Goingo Website</div><div style="text-align:right" class="col-md-6"><a id="booking-close-btn" href="#" style=" color: #fff; font-weight: bold; padding-left: 10px;text-decoration: none;">-</a></div> </div>';
    var mainElem = '<div id="booking-container" style="position: fixed;bottom: -1px;right: 0;min-width:500px; height: auto" class="container"><div class="row" style="background:url(' + loaderImg + ') center center no-repeat;"> <div class="col-md-12"> <iframe id="booking-site" src="" draggable="true" style="width:100%;min-height:450px;" scrolling="yes" marginwidth="0" marginheight="0" frameborder="1" vspace="0" hspace="0">Manage portal</iframe> </div> </div></div>';
    jQuery("body").append(mainElem);
    var bookingContainer = jQuery("#booking-container");
    var myFrm = jQuery("#booking-site");

    if (isValid) {
        myFrm.attr("src", MyBKBasePath);
        bookingContainer.prepend(topElem);
        bookingContainer.show();
    }

    jQuery("#booking-header").click(function () {
        if (myFrm.hasClass("openned")) {
            jQuery("#booking-close-btn").html("-");
            myFrm.removeClass("openned");
            myFrm.show();
        } else {
            jQuery("#booking-close-btn").html("+");
            myFrm.addClass("openned");
            myFrm.hide();
        }
    });
}
