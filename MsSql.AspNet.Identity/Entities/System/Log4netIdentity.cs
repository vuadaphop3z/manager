using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MsSql.AspNet.Identity
{
    public class Log4netRecord
    {
        public string EntryId { get; set; }
        public int Item { get; set; }
        public DateTime? TimeStamp { get; set; }
        public string Level { get; set; }
        public string Thread { get; set; }
        public string Message { get; set; }
        public string MachineName { get; set; }
        public string UserName { get; set; }
        public string HostName { get; set; }
        public string App { get; set; }
        public string Throwable { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
        public string File { get; set; }
        public string Line { get; set; }
        public DateTime? DateCreated { get; set; }
        public string LogPath { get; set; }
        public string FileId { get; set; }

        //Extends
        public string HaveException { get; set; }
        public string AppName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

    }

    public class Log4netFile
    {
        public string FileId { get; set; }
        public string FolderPath { get; set; }
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public DateTime? DateCreated { get; set; }
        public int CurrentItem { get; set; }
        public int FileStatus { get; set; }
        public string MachineName { get; set; }
        public string MachineIP { get; set; }
        public DateTime? LastUpdated { get; set; }
        public string Comments { get; set; }
        public string AppName { get; set; }
    }

    public class Log4netDetails
    {
        public Log4netRecord recordInfo { get; set; }
        public Log4netFile fileInfo { get; set; }
    }
}
