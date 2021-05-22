﻿using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using TripLog.Models;
using TripLog.Services;

namespace TripLog.ViewModels
{
    public class NewEntryViewModel : BaseValidationViewModel
    {
        readonly ILocationService _locService;
        readonly ITripLogDataService _tripLogDataService;

     
        string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                Validate(() => !string.IsNullOrWhiteSpace(_title), "Title must be provided.");
                OnPropertyChanged();
                SaveCommand.ChangeCanExecute();
            }
        }

        double _latitude;
        public double Latitude
        {
            get => _latitude;
            set
            {
                _latitude = value;
                OnPropertyChanged();
            }
        }

        double _longitude;
        public double Longitude
        {
            get => _longitude;
            set
            {
                _longitude = value;
                OnPropertyChanged();
            }
        }

        DateTime _date;
        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        int _rating;
        public int Rating
        {
            get => _rating;
            set
            {
                _rating = value;
                Validate(() => _rating >= 1 && _rating <= 5, "Rating must be between 1 and 5.");
                OnPropertyChanged();
                SaveCommand.ChangeCanExecute();
            }
        }

        string _notes;
        public string Notes
        {
            get => _notes;
            set
            {
                _notes = value;
                OnPropertyChanged();
            }
        }

        Command _saveCommand;
        public Command SaveCommand =>
            _saveCommand ?? (_saveCommand = new Command(async () => await Save(), CanSave));

        public NewEntryViewModel(
            INavService navService, 
            ILocationService locService,
            ITripLogDataService tripLogDataService)
           : base(navService)
        {
            _locService = locService;
            _tripLogDataService = tripLogDataService;

            Date = DateTime.Today;
            Rating = 1;
        }

        public override async void Init()
        {
            try
            {
                GeoCoords coords = await _locService.GetGeoCoordinatesAsync();

                Latitude = coords.Latitude;
                Longitude = coords.Longitude;
            }
            catch (Exception)
            {
                // TODO: handle exceptions from location service
            }
        }

        async Task Save()
        {
            if (IsBusy) return;

            IsBusy = true;

            try
            {
                var newItem = new TripLogEntry
                {
                    Title = Title,
                    Latitude = Latitude,
                    Longitude = Longitude,
                    Date = Date,
                    Rating = Rating,
                    Notes = Notes
                };

                // TODO: Persist entry in a later chapter

                // TODO: Remove this in Chapter 6
                //await Task.Delay(1000);
                await _tripLogDataService.AddEntryAsync(newItem);
                await NavService.GoBack();
            }
            finally
            {
                IsBusy = false;
            }
        }

        bool CanSave() => !string.IsNullOrWhiteSpace(Title) && !HasErrors;
    }
}
