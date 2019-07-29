using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Manager.WebApp.Models
{
    public class CommonPagingModel
    {
        public string SearchExec { get; set; }

        //For paging
        public int TotalCount { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int StartCount()
        {
            var index = this.CurrentPage * PageSize - PageSize + 1;
            return index;
        }

        public int EndCount(int searchResultsCount)
        {
            var index = StartCount() + searchResultsCount - 1;
            return index;
        }

        public bool WasOverRecordsInPage()
        {
            return (this.TotalCount > PageSize);
        }

        public string CurrentUser { get; set; }
        public string CurrentUserName { get; set; }
        public int Id { get; set; }
        public string Keyword { get; set; }
    }

    public class CommonAjaxPagingModel
    {
        public CommonMetaPagingModel meta { get; set; }
    }

    public class CommonMetaPagingModel
    {
        public string field { get; set; }
        public int page { get; set; }
        public int pages { get; set; }
        public int perpage { get; set; }
        public string desc { get; set; }
        public int total { get; set; }
        public string sort { get; set; }
    }

    public class AjaxResponseModel
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public object DataSearch { get; set; }
    }

    public class ResponseApiModel
    {
        public int Code { get; set; }
        public string Msg { get; set; }
        public string TokenKey { get; set; }
        public object Data { get; set; }
    }

    public class Connector : Member
    {
        //Purposes: SendMessage, SendRequest,...
        public string Purpose { get; set; }

        //TargetUserId may be UserId (Chat)
        public string TargetUserId { get; set; }

        //TargetId maybe: TicketId, ExportId
        public string TargetId { get; set; }

        public List<ConnectionInfo> Connections { get; set; }

        public bool HasLogout { get { return false; } set { } }

        public Connector()
        {
            Connections = new List<ConnectionInfo>();
        }
    }

    public class Member
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string DisplayName { get; set; }
        public string Avatar { get; set; }

        public string ConnectionId { get; set; }
        public DateTime LastConnected { get; set; }
        public string LastIPConnected { get; set; }
    }

    public class ConnectionInfo
    {
        public string ConnectionId { get; set; }
        public DateTime ConnectedTime { get; set; }
        public string Ip { get; set; }
        public bool IsMobile { get; set; }
        public string DeviceName { get; set; }
    }

    public class MediaFileViewModel
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }

        public string StorageElementName { get; set; }
    }
}
