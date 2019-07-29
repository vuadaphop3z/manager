var DatatableRemoteAjaxDemo = {
    init: function () {
        var t;
        t = $(".m_datatable").mDatatable({
            data: {
                type: "remote", source: {
                    read: {
                        url: "/Post/GetListPost", map: function (t) {
                            var e = t;
                            return void 0 !== t.data && (e = t.data), e
                        }
                    }
                }
                , pageSize: 10, serverPaging: !0, serverFiltering: !0, serverSorting: !0
            }
            , layout: {
                scroll: !1, footer: !1
            }
            , sortable: !0, pagination: !0, toolbar: {
                items: {
                    pagination: {
                        pageSizeSelect: [10, 20, 30, 50, 100]
                    }
                }
            },
            translate: {
                records: {
                    processing: LanguageDic["LB_LOADING"] + "...",
                    noRecords: LanguageDic["LB_NO_RECORD"]
                },
                toolbar: {
                    pagination: {
                        items: {
                            default: {
                                first: "First",
                                prev: "Previous",
                                next: "Next",
                                last: "Last",
                                more: "More pages",
                                input: "Page number",
                                select: "Select page size"
                            },
                            info: LanguageDic["LB_DISPLAY"] + " {{start}} - {{end}} của {{total}} " + LanguageDic["LB_RECORD"]
                        }
                    }
                }
            }
            , search: {
                input: $("#generalSearch")
            }
            , columns: [
                {
                    field: "Id", title: "#", width: 100, selector: !1, textAlign: "center"
                }
                ,
                {
                    //field: "Title", title: "Title", filterable: !1, width: 150, template: "{{Title}} - {{CategoryName}}"
                    field: "Title", title: LanguageDic["LB_TITLE"], filterable: !1, width: 150, template: function (t) {
                        //return '<a target="_blank" title="' + LanguageDic["LB_CLICK_VIEW_DETAIL"] + '" href="' + _haloSocialUrl + '/post/getinfo/' + t.Id + '">' + t.Title + '</a>';
                        return t.Title;
                    }
                },              
                {
                    field: "Cover", title: LanguageDic["LB_COVER_IMAGE"], sortable: !1, width: 100, template: function (t) {
                        return '<img class="cover-img" width="80" height="50" src= "' + t.Cover + '" title="' + t.Title + '" />'
                    }
                }
                ,
                {
                    field: "CreatedDate", title: LanguageDic["LB_CREATED_DATE"], textAlign: "left", template: function (t) {
                        return t.CreatedDateLabel
                    }
                },
                {
                    field: "PostTypeLabel", title: LanguageDic["LB_POST_TYPE"], textAlign: "left", template: function (t) {
                        //return '<span class="m-badge m-badge--focus m-badge--wide">' + t.PostTypeLabel + '</span >';
                        return '<b>' + t.PostTypeLabel + '</b >';
                    }
                } 
                //_cdnURL
                //, {
                //    field: "ShipCountry", title: "Ship Country", width: 150, template: function (t) {
                //        return t.ShipCountry + " - " + t.ShipCity
                //    }
                //}                
                //, {
                //    field: "Currency", title: "Currency", width: 100
                //}
                //, {
                //    field: "ShipDate", title: "Ship Date", type: "date", format: "MM/DD/YYYY"
                //}
                //, {
                //    field: "Latitude", title: "Latitude", type: "number"
                //}
                ,
                {
                    field: "IsHighlights", title: LanguageDic["LB_POST_HIGHLIGHTS"], template: function (t) {
                        if (t.IsHighlights) {
                            return '<i class="fa fa-check text-danger"></i>';
                        }

                        return "";
                    },
                    textAlign: "center"
                },
                {
                    field: "Status", title: LanguageDic["LB_STATUS"], template: function (t) {
                        var e = {
                            1: {
                                title: LanguageDic["LB_ACTIVATED"], class: "m-badge--success"
                            }                           
                            , 0: {
                                title: LanguageDic["LB_LOCKED"], class: " m-badge--danger"
                            }                           
                        }
                            ;
                        return '<span class="m-badge ' + e[t.Status].class + ' m-badge--wide">' + e[t.Status].title + "</span>"
                    }
                }
                //, {
                //    field: "Type", title: "Type", template: function (t) {
                //        var e = {
                //            1: {
                //                title: "Online", state: "danger"
                //            }
                //            , 2: {
                //                title: "Retail", state: "primary"
                //            }
                //            , 3: {
                //                title: "Direct", state: "accent"
                //            }
                //        }
                //            ;
                //        return '<span class="m-badge m-badge--' + e[t.Type].state + ' m-badge--dot"></span>&nbsp;<span class="m--font-bold m--font-' + e[t.Type].state + '">' + e[t.Type].title + "</span>"
                //    }
                //}
                , {
                    field: "Actions", width: 110, title: LanguageDic["LB_ACTION"], sortable: !1, overflow: "visible", template: function (t, e, a) {
                        var btnDelete = '<a href="/Post/Delete/' + t.Id + '" onclick="return ShowModal(this);" class="m-portlet__nav-link btn m-btn m-btn--hover-danger m-btn--icon m-btn--icon-only m-btn--pill" data-modal="" title="' + LanguageDic["LB_DELETE"] + '">\t\t\t\t\t\t\t<i class="la la-trash"></i>\t\t\t\t\t\t</a>';
                        var btnEdit = '<a href="/Post/Edit/' + t.Id + '" class="m-portlet__nav-link btn m-btn m-btn--hover-danger m-btn--icon m-btn--icon-only m-btn--pill" title="' + LanguageDic["LB_EDIT"] + '">\t\t\t\t\t\t\t<i class="la la-pencil"></i>\t\t\t\t\t\t</a>';
                        var btnAssignCat = '<a class="dropdown-item" href="/Post/AssignCategory/' + t.Id + '" onclick="return ShowModal(this,\'modal-lg\');"><i class="la la-edit"></i> ' + LanguageDic['LB_ASSIGN_CATEGORY'] + '</a>';
                        return btnEdit + btnDelete;
                    }
                },              
            ]
        }
        ),
            $("#m_form_status").on("change", function () {
                t.search($(this).val(), "Status")
            }
            ),
            $("#m_form_type").on("change", function () {
                t.search($(this).val(), "Type")
            }
            ),
            $("#m_form_status, #m_form_type").selectpicker()
    }
}

    ;
jQuery(document).ready(function () {  
    DatatableRemoteAjaxDemo.init();
}

);