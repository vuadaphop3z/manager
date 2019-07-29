function loadJs(url, callback) {
    var head = document.getElementsByTagName('head')[0];
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.src = url;
    script.onload = script.onreadystatechange = function () {
        callback();
    };
    head.appendChild(script);
}

function doEmbed() {
    loadJs(MyBKBasePath + '/Content/Common/Agency/public.js?v=' + new Date().getTime(), function () {
        //loadJs(MyBKBasePath + '/Content/Agency/embed_detail.js?v=' + new Date().getTime(), function () {
        //    console.log('lib init completed');
        //});
    });
}

$jq = null;
if (typeof jQuery === 'undefined') {
    loadJs(IBEBasePath + '/assets/js/jquery-2.1.4.min.js', function () {
        var isEqual = false;
        isEqual = $ == jQuery;
        $jq = jQuery = jQuery.noConflict();
        if (isEqual) {
            $ = jQuery;
        }
        doEmbed();
    });
} else {
    var isEqual = false;
    isEqual = $ == jQuery;
    $jq = jQuery = jQuery.noConflict();
    if (isEqual) {
        $ = jQuery;
    }
    $jq(function () {
        doEmbed();
    });
}




console.log(MyBKBasePath);
console.log(MyBKConfigs);