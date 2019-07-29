$(document).ready(function () {
    $("#AreaId").change(function () {
        AreaChangeEvent();
    });

    $("#CountryId").change(function () {
        CountryChange();
    });

    $("#ProvinceId").change(function () {
        ProvinceChange();
    });

    BootstrapTimepicker.init();
});

function AreaChangeEvent() {
    var changeElement = $("#CountryId");
    changeElement.selectpicker('refresh');

    $("#ProvinceId").find("option").not(':first').remove();
    $("#ProvinceId").selectpicker('refresh');

    $("#DistrictId").find("option").not(':first').remove();
    $("#DistrictId").selectpicker('refresh');

    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Area/GetCountryByArea';
    params['requestType'] = 'GET';
    params['data'] = { areaId: $("#AreaId").val() };
    params['dataType'] = "json";

    params['successCallbackFunction'] = function (result) {
        if (result) {
            if (result.data) {
                changeElement.html(result.data).selectpicker('refresh');
            }
        }
    };
    doAjax(params);
}

function CountryChange() {
    var changeElement = $("#ProvinceId");
    changeElement.selectpicker('refresh');

    $("#DistrictId").find("option").not(':first').remove();
    $("#DistrictId").selectpicker('refresh');

    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Area/GetProvinceByCountry';
    params['requestType'] = 'GET';
    params['data'] = { countryId: $("#CountryId").val() };
    params['dataType'] = "json";

    params['successCallbackFunction'] = function (result) {
        if (result) {
            if (result.data) {
                changeElement.html(result.data).selectpicker('refresh');
            }
        }
    };
    doAjax(params);
}

function ProvinceChange() {
    var changeElement = $("#DistrictId");

    var params = $.extend({}, doAjax_params_default);
    params['url'] = '/Area/GetDistrictByProvince';
    params['requestType'] = 'GET';
    params['data'] = { provinceId: $("#ProvinceId").val() };
    params['dataType'] = "json";

    params['successCallbackFunction'] = function (result) {
        if (result) {
            if (result.data) {
                changeElement.html(result.data).selectpicker('refresh');
            }
        }
    };
    doAjax(params);
}

var BootstrapTimepicker = {
    init: function () {
        //$(".m_timepicker, .m_timepicker_modal").timepicker(),
        //    $("#m_timepicker_2, #m_timepicker_2_modal").timepicker({
        //        minuteStep: 1, defaultTime: "", showSeconds: !0, showMeridian: !1, snapToStep: !0
        //    }
        //    ),
        //    $("#m_timepicker_3, #m_timepicker_3_modal").timepicker({
        //        defaultTime: "11:45:20 AM", minuteStep: 1, showSeconds: !0, showMeridian: !0
        //    }
        //    ),
        //    $("#m_timepicker_4, #m_timepicker_4_modal").timepicker({
        //        defaultTime: "10:30:20 AM", minuteStep: 1, showSeconds: !0, showMeridian: !0
        //    }
        //    ),
        //    $("#m_timepicker_1_validate, #m_timepicker_2_validate, #m_timepicker_3_validate").timepicker({
        //        minuteStep: 1, showSeconds: !0, showMeridian: !1, snapToStep: !0
        //    })
        $(".m_timepicker, .m_timepicker_modal").timepicker();
    }
};