namespace Taskington.Base.Log
{
    enum LogLevel
    {
        Verbose,
        Info,
        Warning,
        Error
    }

    interface IReconfigurableLog
    {
        void SetMiminumLevel(LogLevel minimumLevel);
    }
}
