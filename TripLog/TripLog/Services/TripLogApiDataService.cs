using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TripLog.Models;

namespace TripLog.Services
{
    // todo 2. implement an api data service using BaseHttpService

    public class TripLogApiDataService : BaseHttpService, ITripLogDataService
    {
        readonly Uri _baseUri;
        readonly IDictionary<string, string> _headers;

        public TripLogApiDataService(Uri baseUri)
        {
            _baseUri = baseUri;
            _headers = new Dictionary<string, string>();

            // TODO: Add header with auth-based token in chapter 7
        }

        public async Task<IList<TripLogEntry>> GetEntriesAsync()
        {
            // todo 2a. use Azure function to get all TripLog entries
            // here 'triplogentry' is the name of the called Azure function
            var url = new Uri(_baseUri, "/api/triplogentry");
            var response = await SendRequestAsync<TripLogEntry[]>(url, HttpMethod.Get, _headers);

            return response;
        }

        public async Task<TripLogEntry> AddEntryAsync(TripLogEntry entry)
        {
            // todo 2b. use Azure function to add new TripLog entry
            var url = new Uri(_baseUri, "/api/triplogentry");
            var response = await SendRequestAsync<TripLogEntry>(url, HttpMethod.Post, _headers, entry);

            return response;
        }
    }
}
