using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Manager.SharedLib
{
    public static class ExceptionExtensions
    {
        public static string GetInnerMessage(this Exception ex)
        {
            string strMessage = "";

            if (ex != null)
            {
                Exception innerEx = ex.InnerException;
                while (innerEx != null && innerEx.InnerException != null)
                {
                    innerEx = innerEx.InnerException;
                }

                strMessage = innerEx != null ? innerEx.Message : ex.Message;
            }

            return strMessage;
        }
    }
}