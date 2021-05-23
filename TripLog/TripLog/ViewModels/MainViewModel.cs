using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using TripLog.Models;
using TripLog.Services;
using System.Threading.Tasks;
using Akavache;
using Xamarin.Essentials;

namespace TripLog.ViewModels {
    public class MainViewModel : BaseViewModel {

        readonly ITripLogDataService _tripLogService;

        //     IBlobCache is the core interface on which Akavache is built, it is an interface
        //     describing an asynchronous persistent key-value store.
        readonly IBlobCache _cache;

        ObservableCollection<TripLogEntry> _logEntries;
        public ObservableCollection<TripLogEntry> LogEntries {
            get => _logEntries;
            set {
                _logEntries = value;
                OnPropertyChanged();
            }
        }

        public Command<TripLogEntry> ViewCommand => new Command<TripLogEntry>(async entry => await NavService.NavigateTo<DetailViewModel, TripLogEntry>(entry));

        public Command NewCommand => new Command(async () => await NavService.NavigateTo<NewEntryViewModel>());

        Command _refreshCommand;
        public Command RefreshCommand => _refreshCommand ?? (_refreshCommand = new Command(() => LoadEntries(true)));

        // todo for Azure api, add an ITripLogDataService as parameter to the MainViewModel
        public MainViewModel(INavService navService,
            ITripLogDataService tripLogService,
            IBlobCache cache
            )
            : base(navService) {
            _tripLogService = tripLogService;
            _cache = cache;

            LogEntries = new ObservableCollection<TripLogEntry>();
        }

        public override void Init() {
            LoadEntries();
        }

        void LoadEntries(bool force=false) {
            if (IsBusy) {
                return;
            }

            IsBusy = true;

            try {
                // Load from local cache and then immediately load from API
                _cache.GetAndFetchLatest(
                    "entries",
                    async () => await _tripLogService.GetEntriesAsync(),
                     offset => {
                         // When no network is available return false to just retrieve data from the cache
                         if (Connectivity.NetworkAccess == NetworkAccess.None) {
                             return false;
                         }

                         TimeSpan elapsed = DateTimeOffset.Now - offset;
                         bool invalidateCache = (force || elapsed > new TimeSpan(24, 0, 0));
                         return invalidateCache;
                     }
                    )
                    .Subscribe(entries => {
                        // possible Exception (If service does not exist then entries = null):
                        //   T:System.ArgumentNullException: The collection parameter cannot be null.
                        LogEntries = new ObservableCollection<TripLogEntry>(entries);
                        IsBusy = false;
                    });
            } finally {
                IsBusy = false;
            }
        }

        // testdata version without Azure data access
        //void LoadEntriesOld() {
        //    if (IsBusy) {
        //        return;
        //    }

        //    IsBusy = true;

        //    LogEntries.Clear();

        //    // TODO: Remove this in chapter 6 and make persistent
        //    Task.Delay(1000).ContinueWith(_ => Device.BeginInvokeOnMainThread(() => {
        //        LogEntries = new ObservableCollection<TripLogEntry> {
               
        //        new TripLogEntry {
        //            Title = "Grote Kerk Breda",
        //            Notes = "gebouwd 1410 - 1547, 97 meter hoog",
        //            Rating = 5,
        //            Date = new DateTime(2018, 5, 21),
        //            Latitude = 51.588889,
        //            Longitude = 4.775278
        //        },
        //        new TripLogEntry {
        //            Title = "Begijnhof Breda",
        //            Notes = "13e eeuws, naast stadpark valkenberg",
        //            Rating = 3,
        //            Date = new DateTime(2019, 2, 28),
        //            Latitude = 51.588889,
        //            Longitude = 4.775278
        //        },
        //        new TripLogEntry {
        //            Title = "Mezz popppodium",
        //            Rating = 4,
        //            Notes = "Het poppodium bestaat uit een verbouwde officiersmess uit 1899 en een koperen schelpvormige uitbreiding. De Mezz is de opvolger van poppodium Para.",
        //            Date = new DateTime(2020, 3, 6),
        //            Latitude = 51.583824,
        //            Longitude = 4.778921
        //        },
        //        new TripLogEntry {
        //            Title = "Chasse theater en cinema",
        //            Rating = 4,
        //            Notes = "Modern gebouw met een opvallend golvend dak naar een ontwerp van Herman Hertzberger",
        //            Date = new DateTime(2019, 12, 6),
        //            Latitude = 51.5875,
        //            Longitude = 4.7822
        //        },
        //        new TripLogEntry {
        //            Title = "Avans Hogeschoollaan",
        //            Rating = 4,
        //            Notes = "Helaas nog even gesloten",
        //            Date = new DateTime(2020, 3, 20),
        //            Latitude = 51.5835,
        //            Longitude = 4.7964
        //        },
        //        new TripLogEntry {
        //            Title = "Pathe Cinema",
        //            Rating = 3,
        //            Notes = "Wachten op de nieuwe James Bond...",
        //            Date = new DateTime(2020, 4, 20),
        //            Latitude = 51.5897,
        //            Longitude = 4.7850
        //        }
        //    };

        //        IsBusy = false;
        //    }));
        //}
    }
}
