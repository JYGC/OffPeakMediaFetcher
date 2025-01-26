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

    public class OPMFLog
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public OPMFLogType Type { get; set; }
        public Dictionary<string, object> Variables { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public string ExceptionObject { get; set; }

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
    }
}
