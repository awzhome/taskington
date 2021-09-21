using Serilog;
using Serilog.Core;
using System;
using System.IO;

namespace Taskington.Base.Log
{
    class FileLog : ILog, IReconfigurableLog, IDisposable
    {
        private static string LogPath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "taskington");

        private Logger? logger;
        private bool disposed;

        public FileLog()
        {
            if (!Directory.Exists(LogPath))
            {
                Directory.CreateDirectory(LogPath);
            }

            InitializeLogger();
        }

        public void Dispose()
        {
            if (!disposed)
            {
                DisposeLogger();
                disposed = true;
            }
        }

        private void InitializeLogger(LogLevel minimumLevel = LogLevel.Info)
        {
            if (logger != null)
            {
                DisposeLogger();
            }

            var logFile = Path.Combine(LogPath, "taskington.log");
            var loggerConfig = new LoggerConfiguration()
                .WriteTo.File(logFile);
            loggerConfig = WithMinimumLevel(loggerConfig, minimumLevel);

            logger = loggerConfig.CreateLogger();
        }

        private LoggerConfiguration WithMinimumLevel(LoggerConfiguration prevConfig, LogLevel minimumLevel) => minimumLevel switch
        {
            LogLevel.Verbose => prevConfig.MinimumLevel.Debug(),
            LogLevel.Warning => prevConfig.MinimumLevel.Warning(),
            LogLevel.Error => prevConfig.MinimumLevel.Error(),
            _ => prevConfig.MinimumLevel.Information(),
        };

        private void DisposeLogger()
        {
            logger?.Dispose();
            logger = null;
        }

        public void Info(object sender, string messageTemplate, params object[] parameters)
        {
            logger?.ForContext(sender.GetType()).Information(messageTemplate, parameters);
        }

        public void Debug(object sender, string messageTemplate, params object[] parameters)
        {
            logger?.ForContext(sender.GetType()).Debug(messageTemplate, parameters);
        }

        public void Warning(object sender, string messageTemplate, params object[] parameters)
        {
            logger?.ForContext(sender.GetType()).Warning(messageTemplate, parameters);
        }

        public void Warning(Exception exception)
        {
            logger?.Warning(exception.ToString());
        }

        public void Warning(string message, Exception exception)
        {
            logger?.Warning(exception.ToString());
        }

        public void Error(object sender, string messageTemplate, params object[] parameters)
        {
            logger?.ForContext(sender.GetType()).Error(messageTemplate, parameters);
        }

        public void Error(Exception exception)
        {
            logger?.Error(exception.ToString());
        }

        public void Error(string message, Exception exception)
        {
            logger?.Error(exception.ToString());
        }

        public void SetMiminumLevel(LogLevel minimumLevel)
        {
            InitializeLogger(minimumLevel);
        }
    }
}
