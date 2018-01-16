using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using QBittorrent.Client.Extensions;

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

        #region Get

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

        public async Task<TorrentProperties> GetTorrentPropertiesAsync(string hash)
        {
            var uri = BuildUri($"/query/propertiesGeneral/{hash}");
            var json = await _client.GetStringAsync(uri).ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<TorrentProperties>(json);
            return result;
        }

        #endregion

        #region Download

        public async Task DownloadAsync(DownloadWithTorrentFilesRequest request)
        {
            var uri = BuildUri("/command/upload");
            var data = new MultipartFormDataContent();
            foreach (var file in request.TorrentFiles)
            {
                data.AddFile("torrents", file, "application/x-bittorrent");
            }           
            await DownloadCoreAsync(uri, data, request).ConfigureAwait(false);
        }

        public async Task DownloadAsync(DownloadWithTorrentUrlsRequest request)
        {
            var uri = BuildUri("/command/download");
            var urls = string.Join("\n", request.TorrentUrls.Select(url => url.AbsoluteUri));
            var data = new MultipartFormDataContent().AddValue("urls", urls);
            await DownloadCoreAsync(uri, data, request).ConfigureAwait(false);
        }

        protected async Task DownloadCoreAsync(Uri uri, MultipartFormDataContent data, DownloadRequest request)
        {
            data
                .AddNonEmptyString("savepath", request.DownloadFolder)
                .AddNonEmptyString("cookie", request.Cookie)
                .AddNonEmptyString("category", request.Category)
                .AddValue("skip_checking", request.SkipHashChecking)
                .AddValue("paused", request.Paused)
                .AddNotNullValue("root_folder", request.CreateRootFolder)
                .AddNonEmptyString("rename", request.Rename)
                .AddNotNullValue("upLimit", request.UploadLimit)
                .AddNotNullValue("dlLimit", request.DownloadLimit)
                .AddValue("sequentialDownload", request.SequentialDownload)
                .AddValue("firstLastPiecePrio", request.FirstLastPiecePrioritized);

            var response = await _client.PostAsync(uri, data).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
        }

        #endregion

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
