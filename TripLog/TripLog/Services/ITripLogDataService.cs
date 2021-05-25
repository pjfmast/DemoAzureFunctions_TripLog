using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TripLog.Models;

namespace TripLog.Services
{
    // todo 1. create an api data service
    public interface ITripLogDataService
    {
        Task<IList<TripLogEntry>> GetEntriesAsync();
        Task<TripLogEntry> AddEntryAsync(TripLogEntry entry);
    }
}
