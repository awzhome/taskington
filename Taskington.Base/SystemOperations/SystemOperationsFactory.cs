namespace Taskington.Base.SystemOperations
{
    public static class SystemOperationsFactory
    {
        public static ISystemOperations CreateSystemOperations()
        {
#if SYS_OPS_DRYRUN
            return new DryRunSystemOperations();
#else
            return new WindowsSystemOperations();
#endif
        }
    }
}
