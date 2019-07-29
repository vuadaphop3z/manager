$body = $("body");
$(document).on({
    //ajaxStart: function () {        
    //    $(".div-loading").addClass("hidden");
    //    $(".overlay").fadeIn("fast", function () {
    //        $body.addClass("loading");
    //    });
    //},
    //ajaxStop: function () {      
    //    $(".overlay").fadeOut("fast", function () {
    //        $body.removeClass("loading");
    //    });        
    //}
});

//$('#profile-feed-activity').ace_scroll({
//    height: '250px',
//    mouseWheelLock: true,
//    alwaysVisible: true
//});

function LoadMoreActivity() {
    var pageNum = parseInt($("#hdPageNum").val());
    var loadCount = parseInt($("#hdLoadCount").val());
    var page = $("#hdCurrentPage").val();
    var next_page = parseInt(page) + 1;
   
    if (loadCount < pageNum) {        
        GetActivity(next_page);
        loadCount++;
        $("#hdLoadCount").val(loadCount);
        $("#hdCurrentPage").val(next_page);

        if (loadCount == (pageNum))
            $("#btnLoadMore").hide();
    } else {
        return false;
    }
}

function GetActivity(page) {
    $.ajax({
        type: "GET",
        datatype: "Json",
        url: "/Account/GetActivityLogs",
        data: { page: page },
        success: function (data) {
            var html = '';
            $.each(data, function (index, value) {                
                //$('#ddlDistrict').append('<option value="' + value.DistrictId + '">' + value.DistrictLabel + '</option>');
                html += '<div class="profile-activity clearfix">';
                html += '<div><img class="pull-left" alt="User avatar" src="' + $("#hdCurrentAvatar").attr("src") + '" />';
                html += '<a class="user" href="#">'+ $("#hdCurrentUserName").val() +'</a>';
                html += '<div>'+ value.ActivityText + '.</div>';
                html += '<div class="time"> <i class="ace-icon fa fa-clock-o bigger-110"></i> ';
                html += value.FriendlyRelativeTime;
                html += '</div></div></div>';                
            });

            //If load by btnLoadMore
            if (page > 1)
                $("#profile-feed-activity").append(html);
            else {
                $("#profile-feed-activity").html(html);
                $("#btnLoadMore").show();
                $("#hdLoadCount").val(page);
                $("#hdCurrentPage").val(page);
            }
        }
    });
}

$('.chosen-select').chosen({ allow_single_deselect: true, search_contains: true });
$(window)
        .off('resize.chosen')
        .on('resize.chosen', function () {
            $('.chosen-select').each(function () {
                var $this = $(this);
                $this.next().css({ 'width': $this.parent().width() });
            })
    }).trigger('resize.chosen');

