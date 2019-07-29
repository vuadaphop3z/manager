using Manager.WebApp.Resources;
using System;
using System.Configuration;

namespace Manager.WebApp.Settings
{
    public class NotifSettings
    {
        #region Success notifications
        public static string Success_Created
        {
            get
            {
                return ManagerResource.LB_INSERT_SUCCESS;
            }
        }

        public static string Success_Updated
        {
            get
            {
                return ManagerResource.LB_UPDATE_SUCCESS;
            }
        }

        public static string Success_Deleted
        {
            get
            {
                return ManagerResource.LB_DELETE_SUCCESS;
            }
        }

        #endregion

        #region Error notifications

        public static string Error_SystemBusy
        {
            get
            {
                return ManagerResource.LB_SYSTEM_BUSY;
            }
        }

        #endregion
    }
}