using System;
using Ninject.Modules;
using TripLog.Services;
using TripLog.ViewModels;

namespace TripLog.Modules
{
    public class TripLogCoreModule : NinjectModule
    {
        public override void Load()
        {
            // ViewModels
            Bind<MainViewModel>().ToSelf();
            Bind<DetailViewModel>().ToSelf();
            Bind<NewEntryViewModel>().ToSelf();

            // Core Services aded for Azure api
            var tripLogService = new TripLogApiDataService(new Uri("https://triplogxamarin.azurewebsites.net/api/TripLogEntry"));
            Bind<ITripLogDataService>()
                .ToMethod(x => tripLogService)
                .InSingletonScope();
        }
    }
}
