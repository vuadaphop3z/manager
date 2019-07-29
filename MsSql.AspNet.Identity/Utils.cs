using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

using System.Configuration;
using System.Web;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Globalization;
using System.ComponentModel;

namespace MsSql.AspNet.Identity
{   
    public class Format
    {
        public static String FormatDate(DateTime dt)
        {
            if (dt == DateTime.MinValue) {
                return String.Empty;
            }
            return dt.ToString("dd/MM/yyyy");
        }

        public static String FormatDate(string dt)
        {
            return Convert.ToDateTime(dt).ToString("dd/MM/yyyy");
        }

        public static String FormatNumber(string value)
        {
            System.Globalization.NumberFormatInfo numFormat = new System.Globalization.NumberFormatInfo();
            numFormat.NumberGroupSeparator = ",";
            numFormat.NumberDecimalDigits = 0;
            numFormat.NumberNegativePattern = 0;

            return Convert.ToInt64(value).ToString("N", numFormat);
        }
        
        public static String FormatNumber(Int32 value)
        {
            System.Globalization.NumberFormatInfo numFormat = new System.Globalization.NumberFormatInfo();
            numFormat.NumberGroupSeparator = ",";
            numFormat.NumberDecimalDigits = 0;
            numFormat.NumberNegativePattern = 0;

            return value.ToString("N", numFormat);
        }

        public static String FormatNumber(Int64 value)
        {
            System.Globalization.NumberFormatInfo numFormat = new System.Globalization.NumberFormatInfo();
            numFormat.NumberGroupSeparator = ",";
            numFormat.NumberDecimalDigits = 0;
            numFormat.NumberNegativePattern = 0;

            return value.ToString("N", numFormat);
        }

        public static String FormatNumber(double value)
        {
            System.Globalization.NumberFormatInfo numFormat = new System.Globalization.NumberFormatInfo();
            numFormat.NumberGroupSeparator = ",";
            numFormat.NumberDecimalDigits = 2;
            numFormat.NumberNegativePattern = 0;

            return value.ToString("N", numFormat);
        }

        public static String FormatNumber(decimal value)
        {
            System.Globalization.NumberFormatInfo numFormat = new System.Globalization.NumberFormatInfo();
            numFormat.NumberGroupSeparator = ",";
            numFormat.NumberDecimalDigits = 2;
            numFormat.NumberNegativePattern = 0;

            return value.ToString("N", numFormat);
        }

        

    }

    public class Season
    {
        public static int GetSeasonByDateTime(DateTime dt)
        {
            if (dt.Month >= 1 && dt.Month <= 3)
            {
                return 1;
            }
            else if (dt.Month >= 4 && dt.Month <= 6)
            {
                return 2;
            }
            else if (dt.Month >= 7 && dt.Month <= 9)
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }

        public static DateTime GetFirstDayOfSeason(int year, int season)
        {
            if (season == 1)
            {
                return new DateTime(year, 1, 1);
            }
            else if (season == 2)
            {
                return new DateTime(year, 4, 1);
            }
            else if (season == 3)
            {
                return new DateTime(year, 7, 1);
            }
            else
            {
                return new DateTime(year, 10, 1);
            }
        }

        public static DateTime GetLastDayOfSeason(int year, int season)
        {
            if (season == 1)
            {
                return new DateTime(year, 3, 31);
            }
            else if (season == 2)
            {
                return new DateTime(year, 6, 30);
            }
            else if (season == 3)
            {
                return new DateTime(year, 9, 30);
            }
            else
            {
                return new DateTime(year, 12, 31);
            }
        }

        public static DateTime GetFirstDayOfYear(int year)
        {
            return new DateTime(year, 1, 1);
        }

        public static DateTime GetFirstDayOfSeason(DateTime dt)
        {
            if (dt.Month >= 1 && dt.Month <= 3)
            {
                return new DateTime(dt.Year, 1, 1);
            }
            else if (dt.Month >= 4 && dt.Month <= 6)
            {
                return new DateTime(dt.Year, 4, 1);
            }
            else if (dt.Month >= 7 && dt.Month <= 9)
            {
                return new DateTime(dt.Year, 7, 1);
            }
            else
            {
                return new DateTime(dt.Year, 10, 1);
            }
        }

        public static DateTime GetLastDayOfSeason(DateTime dt)
        {
            if (dt.Month >= 1 && dt.Month <= 3)
            {
                return new DateTime(dt.Year, 3, 31);
            }
            else if (dt.Month >= 4 && dt.Month <= 6)
            {
                return new DateTime(dt.Year, 6, 30);
            }
            else if (dt.Month >= 7 && dt.Month <= 9)
            {
                return new DateTime(dt.Year, 9, 30);
            }
            else
            {
                return new DateTime(dt.Year, 12, 31);
            }
        }

        public static DateTime GetFirstDayOfYear(DateTime dt)
        {
            return new DateTime(dt.Year, 1, 1);
        }

        public static DateTime GetFirstDayOfMonth(int month, int year)
        {
            return new DateTime(year, month, 1);
        }

        //public static DateTime GetLastDayOfMonth(int month, int year)
        //{
        //    if (month == 1 || month == 3 || month == 5 || month == 7 || month == 8 || month == 10 || month == 12)
        //    {
        //        return new DateTime(year, month, 31);
        //    }
        //    else if (month == 4 || month == 6 || month == 9 || month == 10)
        //    {
        //        return new DateTime(year, month, 30);
        //    }
        //    else if (month == 2)
        //    {
        //        if (CheckLeapYear(year))
        //        {
        //            return new DateTime(year, month, 29);
        //        }
        //        else
        //        {
        //            return new DateTime(year, month, 28);
        //        }
        //    }
        //    return Constant.DEFAULT_DATE;
        //}

        //public static DateTime GetFirstDayOfWeek(DateTime dt)
        //{
        //    if (dt.DayOfWeek == DayOfWeek.Monday)
        //    {
        //        return dt;
        //    }
        //    else if(dt.DayOfWeek == DayOfWeek.Tuesday) 
        //    {
        //        return dt.AddDays(-1);
        //    }
        //    else if (dt.DayOfWeek == DayOfWeek.Wednesday)
        //    {
        //        return dt.AddDays(-2);
        //    }
        //    else if (dt.DayOfWeek == DayOfWeek.Thursday)
        //    {
        //        return dt.AddDays(-3);
        //    }
        //    else if (dt.DayOfWeek == DayOfWeek.Friday)
        //    {
        //        return dt.AddDays(-4);
        //    }
        //    else if (dt.DayOfWeek == DayOfWeek.Saturday)
        //    {
        //        return dt.AddDays(-5);
        //    }
        //    else if (dt.DayOfWeek == DayOfWeek.Sunday)
        //    {
        //        return dt.AddDays(-6);
        //    }
        //    else
        //    {
        //        return Constant.DEFAULT_DATE;
        //    }
        //}

        //public static DateTime GetLastDayOfWeek(DateTime dt)
        //{
        //    if (dt.DayOfWeek == DayOfWeek.Monday)
        //    {
        //        return dt.AddDays(6);
        //    }
        //    else if (dt.DayOfWeek == DayOfWeek.Tuesday)
        //    {
        //        return dt.AddDays(5);
        //    }
        //    else if (dt.DayOfWeek == DayOfWeek.Wednesday)
        //    {
        //        return dt.AddDays(4);
        //    }
        //    else if (dt.DayOfWeek == DayOfWeek.Thursday)
        //    {
        //        return dt.AddDays(3);
        //    }
        //    else if (dt.DayOfWeek == DayOfWeek.Friday)
        //    {
        //        return dt.AddDays(2);
        //    }
        //    else if (dt.DayOfWeek == DayOfWeek.Saturday)
        //    {
        //        return dt.AddDays(1);
        //    }
        //    else if (dt.DayOfWeek == DayOfWeek.Sunday)
        //    {
        //        return dt;
        //    }
        //    else
        //    {
        //        return Constant.DEFAULT_DATE;
        //    }
        //}

        private static bool CheckLeapYear(int year)
        {
            if (year % 4 != 0)
            {
                return false;
            }
            else
            {
                if (year % 100 != 0)
                {
                    return true;
                }
                else
                {
                    if (year % 400 == 0)
                    {
                        return true;
                    }
                    else return false;
                }
            }
        }
    };

    public class ConvertFilePattern
    {
        private string _path;
        private int _oldID;
        private int _newID;

        public ConvertFilePattern()
        {
            _path = "";
            _oldID = -1;
            _newID = -1;
        }

        public ConvertFilePattern(string path, int oldID, int newID)
        {
            _path = path;
            _oldID = oldID;
            _newID = newID;
        }
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }
        public int OldID
        {
            get { return _oldID; }
            set { _oldID = value; }
        }
        public int NewID
        {
            get { return _newID; }
            set { _newID = value; }
        }
    }
        

    public static class Utils
    {
        public static string GetFormatStringDateTime(DateTime datetime)
        {
            string String_DateTime = "";
            TimeSpan time = DateTime.Now - datetime;

            double se = time.TotalSeconds;
            int tg = (int)se;
            String_DateTime = tg.ToString() + " giây";
            if (se > 60)
            {
                se = time.TotalMinutes;
                tg = (int)se;
                String_DateTime = tg.ToString() + " phút";
                if (se > 60)
                {
                    se = time.TotalHours;
                    tg = (int)se;
                    String_DateTime = tg.ToString() + " giờ";
                    if (se > 24)
                    {
                        int Thu = (int)datetime.DayOfWeek;
                        string Thus = "";
                        switch (Thu)
                        {
                            case 1: Thus = "Thứ Hai"; break;
                            case 2: Thus = "Thứ Ba"; break;
                            case 3: Thus = "Thứ Tư"; break;
                            case 4: Thus = "Thứ Năm"; break;
                            case 5: Thus = "Thứ Sáu"; break;
                            case 6: Thus = "Thứ Bảy"; break;
                            default: Thus = "Chủ nhật"; break;
                        };

                        String_DateTime = Thus + ",Ngày " + datetime.ToString("dd/MM/yyyy");
                        //se = time.TotalDays;
                        //tg = (int)se;
                        //String_DateTime = tg.ToString() + " ngày";
                        //if (se > 30)
                        //{
                        //    //se = time.TotalDays;
                        //    tg = (int)se / 30;
                        //    String_DateTime = tg.ToString() + " tháng";
                        //    if (se > 365)
                        //    {
                        //        //se = time.TotalDays;
                        //        tg = (int)se / 365;
                        //        String_DateTime = tg.ToString() + " năm";

                        //    }
                        //}
                    }
                }
            }
            return String_DateTime;
        }
        public static string GetDescription(object enumValue, string defDesc)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

            if (null != fi)
            {
                object[] attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return defDesc;
        }
        public static bool IsValidEmail(string input)
        {
            Regex regex = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            return regex.IsMatch(input);
        }

        public static bool IsValidDouble(string input)
        {
            Double temp;
            return Double.TryParse(input, out temp);
        }

        public static bool IsValidInt64(string input)
        {
            Int64 temp;
            return Int64.TryParse(input, out temp);
        }

        public static bool IsValidInt32(string input)
        {
            Int32 temp;
            return Int32.TryParse(input, out temp);
        }

        public static string ConvertToString(object value, string defaultValue)
        {
            try
            {
                return Convert.ToString(value.ToString().Trim());
            }
            catch
            {
                return defaultValue;
            }
        }

        public static int ConvertToInt32(object value, int defaultValue = 0)
        {
            try
            {
                return Convert.ToInt32(value.ToString().Trim());
            }
            catch
            {
                return defaultValue;
            }
        }

        public static decimal ConvertToDecimal(object value, int defaultValue = 0)
        {
            try
            {
                return Convert.ToDecimal(value.ToString().Trim());
            }
            catch
            {
                return defaultValue;
            }
        }

        public static double ConvertToDouble(object value, double defaultValue = 0)
        {
            try
            {
                return Convert.ToDouble(value.ToString().Trim());
            }
            catch
            {
                return defaultValue;
            }
        }

        public static int? ConvertToNullableInt32(object value, int? defaultValue)
        {
            try
            {
                return Convert.ToInt32(value.ToString().Trim());
            }
            catch
            {
                return defaultValue;
            }
        }

        public static long ConvertToInt64(object value, long defaultValue)
        {
            try
            {
                return Convert.ToInt64(value.ToString().Trim());
            }
            catch
            {
                return defaultValue;
            }
        }

        public static long? ConvertToNullableInt64(object value, long? defaultValue)
        {
            try
            {
                return Convert.ToInt64(value.ToString().Trim());
            }
            catch
            {
                return defaultValue;
            }
        }

        public static bool ConvertToBoolean(object value, bool defaultValue = false)
        {
            try
            {
                return Convert.ToBoolean(value.ToString().Trim());
            }
            catch
            {
                return defaultValue;
            }
        }

        public static DateTime ConvertToDateTime(object value, DateTime defaultValue)
        {
            try
            {
                DateTime datetime = new DateTime();
                bool result = DateTime.TryParseExact(value.ToString(), "dd/MM/yyyy", null, DateTimeStyles.None, out datetime);
                if (!result)
                {
                    datetime = DateTime.Parse(value.ToString().Trim());
                }
                return datetime;
            }
            catch
            {               
                return defaultValue;                
            }
        }

        public static DateTime ConvertToVietNameDateTime(object value, DateTime defaultValue)
        {
            try
            {
                DateTime datetime = new DateTime();
                bool result = DateTime.TryParseExact(value.ToString(), "dd/MM/yyyy HH:mm:ss tt", null, DateTimeStyles.None, out datetime);
                if (!result)
                {
                    datetime = DateTime.Parse(value.ToString().Trim());
                }
                return datetime;
            }
            catch
            {
                return defaultValue;
            }
        }

        public static DateTime? ConvertToNullableDateTime(object value, DateTime? defaultValue)
        {
            try
            {
                return DateTime.ParseExact(value.ToString(), "d/m/yyyy", null);
            }
            catch
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// Bacth: Hash file content into string
        /// for example: 1e5e4212f86d8ecbe5acc956c97fa373
        /// </summary>
        /// <param name="file">file content - array of bytes</param>
        /// <returns>string with 32 characters of length</returns>
        public static string HashFile(byte[] file)
        {
            MD5 md5 = MD5.Create();
            StringBuilder sb = new StringBuilder();

            byte[] hashed = md5.ComputeHash(file);
            foreach (byte b in hashed)
                // convert to hexa
                sb.Append(b.ToString("x2").ToLower());

            // sb = set of hexa characters
            return sb.ToString();
        }

        /// <summary>
        /// Bacth: detemine path to store file
        /// for example: [1e]-[5e]-[42]-[1e5e4212f86d8ecbe5acc956c97fa373]
        /// </summary>
        /// <param name="file">file content - array of bytes</param>
        /// <returns>hashed path</returns>
        public static List<string> GetPath(byte[] file)
        {
            string hashed = HashFile(file);
            List<string> toReturn = new List<string>(3);
            toReturn.Add(hashed.Substring(0, 2));
            toReturn.Add(hashed.Substring(2, 2));
            toReturn.Add(hashed.Substring(4, 2));
            toReturn.Add(hashed);
            return toReturn; // for example: [1e]-[5e]-[42]-[1e5e4212f86d8ecbe5acc956c97fa373]
        }

        /// <summary>
        /// Gets the object., 
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="valueIfNull">The value if null.</param>
        /// <returns></returns>
        public static object GetObject(object value, object valueIfNull)
        {
            if ((value != null) && (value != DBNull.Value))
            {
                return value;
            }
            return valueIfNull;
        }

        /// <summary>
        /// Gets the date time.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="valueIfNull">The value if null.</param>
        /// <returns></returns>
        public static DateTime GetDateTime(object value, DateTime valueIfNull)
        {
            value = GetObject(value, null);
            if (value == null)
            {
                return valueIfNull;
            }
            if (value is DateTime)
            {
                return (DateTime)value;
            }
            return DateTime.Parse(value.ToString());
        }

        public static Decimal GetDecimal(object value, Decimal valueIfNull)
        {
            value = GetObject(value, null);
            if (value == null)
            {
                return valueIfNull;
            }
            if (value is Decimal)
            {
                return (Decimal)value;
            }
            return Decimal.Parse(value.ToString());
        }
        /// <summary>
        /// Gets the byte.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="valueIfNull">The value if null.</param>
        /// <returns></returns>
        public static byte GetByte(object value, byte valueIfNull)
        {
            value = GetObject(value, null);
            if (value == null)
            {
                return valueIfNull;
            }
            if (value is byte)
            {
                return (byte)value;
            }
            return byte.Parse(value.ToString());
        }
        /// <summary>
        /// Gets the boolean.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="valueIfNull">if set to <c>true</c> [value if null].</param>
        /// <returns></returns>
        public static bool GetBoolean(object value, bool valueIfNull)
        {
            value = GetObject(value, valueIfNull);
            if (value == null)
            {
                return valueIfNull;
            }
            if (value is bool)
            {
                return (bool)value;
            }
            if (!(value is byte))
            {
                return bool.Parse(value.ToString());
            }
            if (((byte)value) == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Gets the string. 
        /// </summary>
        /// <param name="dataReader">The data reader.</param>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="valueIfNull">The value if null.</param>
        /// <returns></returns>
        public static string GetString(object value, string valueIfNull)
        {
            value = GetObject(value, null);
            if (value == null)
            {
                return valueIfNull;
            }
            if (value is string)
            {
                return (string)value;
            }
            return value.ToString();
        }

        /// <summary>
        /// Gets the single.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="valueIfNull">The value if null.</param>
        /// <returns></returns>
        public static float GetSingle(object value, float valueIfNull)
        {
            value = GetObject(value, null);
            if (value == null)
            {
                return valueIfNull;
            }
            if (value is float)
            {
                return (float)value;
            }
            return float.Parse(value.ToString());
        }

        /// <summary>
        /// Gets the int64.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="valueIfNull">The value if null.</param>
        /// <returns></returns>
        public static long GetInt64(object value, long valueIfNull)
        {
            value = GetObject(value, null);
            if (value == null)
            {
                return valueIfNull;
            }
            if (value is long)
            {
                return (long)value;
            }
            return long.Parse(value.ToString());
        }

        /// <summary>
        /// Gets the int32.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="valueIfNull">The value if null.</param>
        /// <returns></returns>
        public static int GetInt32(object value, int valueIfNull)
        {
            value = GetObject(value, null);
            if (value == null)
            {
                return valueIfNull;
            }
            if (value is int)
            {
                return (int)value;
            }
            return int.Parse(value.ToString());
        }

        public static string GetExtension(string fileName)
        {
            int dotIndex = fileName.LastIndexOf(".");
            return dotIndex == -1 ? string.Empty : fileName.Substring(dotIndex + 1);
        }
        public static string GetFileNameWithoutExtention(string fileName)
        {
            int dotIndex = fileName.LastIndexOf(".");
            return dotIndex == -1 ? fileName : fileName.Substring(0, dotIndex);
        }


        //This code EnCode Base64
        private static string[] d2c = new string[] { "V", "_", "C", "M", "S" };

        private static int c2d(string c)
        {
            int d = 0;
            for (int i = 0, n = d2c.Length; i < n; ++i)
            {
                if (c == d2c[i])
                {
                    d = i;
                    break;
                }
            }
            return d;
        }

        public static int IDFromString(string base64)
        {
            int pos = base64.Length / 2;
            int step = c2d(base64.Substring(pos, 1)) + d2c.Length;
            string orginal = String.Format("{0}{1}", base64.Substring(0, pos), base64.Substring(pos + 1));
            for (int i = 0; i < step; ++i)
            {
                orginal = DecodeFrom64(orginal);
            }
            return Convert.ToInt32(orginal);
        }

        public static string IDToString(int id)
        {
            Random random = new Random();
            int step = random.Next(5, 10);
            string base64 = id.ToString();
            for (int i = 0; i < step; ++i)
            {
                base64 = EncodeTo64(base64);
            }
            int pos = base64.Length / 2;
            return String.Format("{0}{1}{2}", base64.Substring(0, pos), d2c[step - d2c.Length], base64.Substring(pos));
        }

        private static string DecodeFrom64(string encodedData)
        {

            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);

            string returnValue = ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);

            return returnValue;

        }

        private static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = ASCIIEncoding.ASCII.GetBytes(toEncode);

            string returnValue = Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;

        }                
    }
}
