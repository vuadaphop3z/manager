using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Data;
using System.Text;

using Excel;
using CsvHelper;

using Manager.SharedLib;

namespace Manager.WebApp.Helpers
{
    public static class FileHelpers
    {
        /// <summary>
        /// Upload a file to a specific folder
        /// </summary>        
        public static void UploadFile(HttpPostedFileBase file, string strDestFolder)
        {
            if (file == null || file.ContentLength == 0) return;
            string fileName = Path.GetFileName(file.FileName);
            string folderPath = EnsureFolder(strDestFolder);

            string filePath = folderPath + fileName;
            file.SaveAs(filePath);
        }


        /// <summary>
        /// Ensure a folder is correct and existed
        /// </summary>        
        public static string EnsureFolder(string strFolderPath)
        {
            if (string.IsNullOrEmpty(strFolderPath))
            {
                throw new ArgumentNullException("strFolderPath");
            }

            if (!strFolderPath.EndsWith("//"))
            {
                strFolderPath = strFolderPath + "//";
            }

            if (!Directory.Exists(strFolderPath))
            {
                Directory.CreateDirectory(strFolderPath);
            }

            return strFolderPath;
        }

        public static void MoveFileToFolder(string strFilePath, string strFolderPath)
        {
            try
            {
                string strFileName = Path.GetFileName(strFilePath);

                string strDestFolder = EnsureFolder(strFolderPath);
                string strDestFilePath = strDestFolder + strFileName;

                File.Move(strFilePath, strDestFilePath);
            }
            catch (Exception ex)
            {
                string strErrorMessage = ex.Message;
            }
        }

        public static void ClearFolder(string strFolderPath)
        {
            if (Directory.Exists(strFolderPath))
            {
                var dir = new DirectoryInfo(strFolderPath);

                foreach (FileInfo fi in dir.GetFiles())
                {
                    fi.IsReadOnly = false;
                    fi.Delete();
                }

                foreach (DirectoryInfo di in dir.GetDirectories())
                {
                    ClearFolder(di.FullName);
                    di.Delete();
                }
            }
        }
    }


    public class Period
    {
        public DateTime StartDateTime { get; private set; }
        public DateTime EndDateTime { get; private set; }

        public Period(DateTime StartDateTime, DateTime EndDateTime)
        {
            /*
            if (StartDateTime > EndDateTime)
                throw new InvalidPeriodException("End DateTime Must Be Greater Than Start DateTime!");
            */
            this.StartDateTime = StartDateTime;
            this.EndDateTime = EndDateTime;
        }

        public bool Overlaps(Period other)
        {
            /*
            
            Simple check to see if two time periods overlap:

            bool overlap = a.start < b.end && b.start < a.end;

            or in your code:

            bool overlap = tStartA < tEndB && tStartB < tEndA;

            (Use <= instead of < if you change your mind about wanting to say that two periods that just touch each other overlap.)

             */

            return (this.StartDateTime < other.EndDateTime && other.StartDateTime < this.EndDateTime);

            //return DoDateRangesOverlap(this.StartDateTime, this.EndDateTime, other.StartDateTime, other.EndDateTime);
        }

        public bool DoDateRangesOverlap(DateTime startDateRange1, DateTime endDateRange1, DateTime startDateRange2, DateTime endDateRange2)
        {
            if (((startDateRange1 < endDateRange2) && (startDateRange1 > startDateRange2)) ||
                    ((endDateRange1 < endDateRange2) && (endDateRange1 > startDateRange2)))
                return true;
            else
                return false;
        }

        /*
        public bool DoDateRangesOverlap(DateTime startDateRange1, DateTime endDateRange1, DateTime startDateRange2, DateTime endDateRange2)
        {
            if (((startDateRange1 < endDateRange2) && (startDateRange1 >= startDateRange2)) ||
                    ((endDateRange1 <= endDateRange2) && (endDateRange1 > startDateRange2)))
                return true;
            else
                return false;
        }
        */

        /*
        public bool Overlaps(Period other)
        {
            if (this.StartDateTime >= other.StartDateTime && this.StartDateTime <= other.EndDateTime)
            {
                return true;
            }

            if (other.StartDateTime >= this.StartDateTime && other.StartDateTime <= this.EndDateTime)
            {
                return true;
            }

            return false;
        }
        */

        public TimeSpan GetDuration()
        {
            return EndDateTime - StartDateTime;
        }
    }


    public class Range<T> where T : IComparable
    {
        readonly T min;
        readonly T max;

        public Range(T min, T max)
        {
            this.min = min;
            this.max = max;
        }

        public bool IsOverlapped(Range<T> other)
        {
            return Min.CompareTo(other.Max) < 0 && other.Min.CompareTo(Max) < 0;
        }

        public T Min { get { return min; } }
        public T Max { get { return max; } }
    }

    public class EpgFileUploadViewModel
    {
        public HttpPostedFileBase ExcelFile { get; set; }
    }

    public class ExcelFileInfo
    {
        public string FileName { get; set; }
        public int RecordCount { get; set; }
    }

}