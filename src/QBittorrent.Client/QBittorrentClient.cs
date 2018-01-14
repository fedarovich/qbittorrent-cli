using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QBittorrent.Client
{
    public class QBittorrentClient : IDisposable
    {
        private readonly Uri _uri;
        private readonly HttpClient _client;

        public QBittorrentClient(Uri uri) 
            : this(uri, new HttpClient())
        {
        }

        public QBittorrentClient(Uri uri, HttpMessageHandler handler, bool disposeHandler)
            : this(uri, new HttpClient(handler, disposeHandler))
        {
        }

        private QBittorrentClient(Uri uri, HttpClient client)
        {
            _uri = uri ?? throw new ArgumentNullException(nameof(uri));
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        #region Authentication

        public async Task LoginAsync(string username, string password)
        {
            await _client.PostAsync(new Uri(_uri, "/login"),
                BuildForm(
                    ("username", username),
                    ("password", password)
                ))
                .ConfigureAwait(false);
        }

        public async Task LogoutAsync()
        {
            await _client.PostAsync(new Uri(_uri, "/logout"), BuildForm()).ConfigureAwait(false);
        }

        #endregion

        public async Task<IReadOnlyList<TorrentInfo>> GetTorrenListAsync(TorrentListQuery query = null)
        {
            query = query ?? new TorrentListQuery();
            var uri = BuildUri("/query/torrents",
                ("filter", query.Filter.ToString().ToLowerInvariant()),
                ("category", query.Category),
                ("sort", query.SortBy),
                ("reverse", query.ReverseSort.ToString().ToLowerInvariant()),
                ("limit", query.Limit?.ToString()),
                ("offset", query.Offset?.ToString()));
            var json = await _client.GetStringAsync(uri).ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<TorrentInfo[]>(json);
            return result;
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

        private FormUrlEncodedContent BuildForm(params (string key, string value)[] fields)
        {
            return new FormUrlEncodedContent(fields.Select(f => new KeyValuePair<string, string>(f.key, f.value)));
        }

        private Uri BuildUri(string path, params (string key, string value)[] parameters)
        {
            var builder = new UriBuilder(_uri)
            {
                Path = path,
                Query = string.Join("&", parameters
                    .Where(t => t.value != null)
                    .Select(t => $"{Uri.EscapeDataString(t.key)}={Uri.EscapeDataString(t.value)}"))
            };
            return builder.Uri;
        }
    }
}
