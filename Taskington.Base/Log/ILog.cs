using System;

namespace Taskington.Base.Log
{
    public interface ILog
    {
        public void Info(object sender, string messageTemplate, params object[] parameters);
        public void Debug(object sender, string messageTemplate, params object[] parameters);
        public void Warning(object sender, string messageTemplate, params object[] parameters);
        public void Warning(Exception exception);
        public void Warning(string message, Exception exception);
        public void Error(object sender, string messageTemplate, params object[] parameters);
        public void Error(Exception exception);
        public void Error(string message, Exception exception);
    }
}
