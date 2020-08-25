namespace PPBackup.Base.SystemOperations
{
    public static class SystemOperationsFactory
    {
        public static ISystemOperations CreateSystemOperations()
        {
            return new WindowsSystemOperations();
            //return new DryRunSystemOperations();
        }
    }
}
