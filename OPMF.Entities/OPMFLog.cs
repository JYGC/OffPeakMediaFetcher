using System;
using System.Collections.Generic;

namespace OPMF.Entities
{
    public enum OPMFLogType
    {
        Info,
        Warning,
        Error
    }

    public interface IOPMFLog : IStringId
    {
        string Message { get; set; }
        OPMFLogType Type { get; set; }
        Dictionary<string, object> Variables { get; set; }
        DateTime DateCreated { get; set; }
    }

    public class OPMFLog : IOPMFLog
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public OPMFLogType Type { get; set; }
        public Dictionary<string, object> Variables { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;

        public OPMFLog()
        {
            Id = Guid.NewGuid().ToString();
        }
    }

    public class OPMFError : OPMFLog
    {
        public OPMFError() : base() { }
        public OPMFError(Exception exception) : base()
        {
            Message = exception.Message;
            Type = OPMFLogType.Error;
            ExceptionObject = exception.ToString();
        }
        public string ExceptionObject { get; set; }
    }
}
