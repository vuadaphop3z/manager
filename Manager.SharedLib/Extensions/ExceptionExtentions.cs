using System;


namespace Manager.SharedLib.Extensions
{
    public static class ExceptionExtentions
    {
        public static Exception ToOriginalExeception(this Exception ex)
        {
            var tempEx = ex;
            while (tempEx.InnerException != null) tempEx = tempEx.InnerException;
            return tempEx;
        }
    }

    public class CustomSQLException : Exception
    {
        public CustomSQLException()
            : base()
        {
        }

        public CustomSQLException(string key, string value)
            : base()
        {
            base.Data.Add(key, value);
        }

        public CustomSQLException(string message)
            : base(message)
        {
        }

        public CustomSQLException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
