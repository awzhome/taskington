using System;
using Taskington.Base.Service;

[assembly: TaskingtonExtension(typeof(Taskington.Update.Windows.UpdateServices))]

namespace Taskington.Update.Windows
{
    public static class UpdateServices
    {
        public static void Bind(IAppServiceBinder binder)
        {
            //binder
            //    .Bind<?>();
        }
    }
}
