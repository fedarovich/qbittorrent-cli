using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
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

        public async Task<IReadOnlyList<TorrentInfo>> GetTorrentListAsync(TorrentListQuery query = null)
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

        public Task<TorrentProperties> GetTorrentPropertiesAsync([NotNull] string hash)
        {
            ValidateHash(hash);
            return ExecuteAsync();

            async Task<TorrentProperties> ExecuteAsync()
            {
                var uri = BuildUri($"/query/propertiesGeneral/{hash}");
                var json = await _client.GetStringAsync(uri).ConfigureAwait(false);
                var result = JsonConvert.DeserializeObject<TorrentProperties>(json);
                return result;
            }
        }

        public Task<IReadOnlyList<TorrentContent>> GetTorrentContentsAsync([NotNull] string hash)
        {
            ValidateHash(hash);
            return ExecuteAsync();

            async Task<IReadOnlyList<TorrentContent>> ExecuteAsync()
            {
                var uri = BuildUri($"/query/propertiesFiles/{hash}");
                var json = await _client.GetStringAsync(uri).ConfigureAwait(false);
                var result = JsonConvert.DeserializeObject<TorrentContent[]>(json);
                return result;
            }
        }

        public Task<IReadOnlyList<TorrentTracker>> GetTorrentTrackersAsync([NotNull] string hash)
        {
            ValidateHash(hash);
            return ExecuteAsync();

            async Task<IReadOnlyList<TorrentTracker>> ExecuteAsync()
            {
                var uri = BuildUri($"/query/propertiesTrackers/{hash}");
                var json = await _client.GetStringAsync(uri).ConfigureAwait(false);
                var result = JsonConvert.DeserializeObject<TorrentTracker[]>(json);
                return result;
            }
        }

        public Task<IReadOnlyList<Uri>> GetTorrentWebSeedsAsync([NotNull] string hash)
        {
            ValidateHash(hash);
            return ExecuteAsync();

            async Task<IReadOnlyList<Uri>> ExecuteAsync()
            {
                var uri = BuildUri($"/query/propertiesWebSeeds/{hash}");
                var json = await _client.GetStringAsync(uri).ConfigureAwait(false);
                var result = JsonConvert.DeserializeObject<UrlItem[]>(json);
                return result.Select(x => x.Url).ToArray();
            }
        }

        public Task<IReadOnlyList<TorrentPieceState>> GetTorrentPiecesStatesAsync([NotNull] string hash)
        {
            ValidateHash(hash);
            return ExecuteAsync();

            async Task<IReadOnlyList<TorrentPieceState>> ExecuteAsync()
            {
                var uri = BuildUri($"/query/getPieceStates/{hash}");
                var json = await _client.GetStringAsync(uri).ConfigureAwait(false);
                var result = JsonConvert.DeserializeObject<TorrentPieceState[]>(json);
                return result;
            }
        }

        public Task<IReadOnlyList<string>> GetTorrentPiecesHashesAsync([NotNull] string hash)
        {
            ValidateHash(hash);
            return ExecuteAsync();

            async Task<IReadOnlyList<string>> ExecuteAsync()
            {
                var uri = BuildUri($"/query/getPieceHashes/{hash}");
                var json = await _client.GetStringAsync(uri).ConfigureAwait(false);
                var result = JsonConvert.DeserializeObject<string[]>(json);
                return result;
            }
        }

        public async Task<GlobalTransferInfo> GetGlobalTransferInfoAsync()
        {
            var uri = BuildUri("/query/transferInfo");
            var json = await _client.GetStringAsync(uri).ConfigureAwait(false);
            var result = JsonConvert.DeserializeObject<GlobalTransferInfo>(json);
            return result;
        }

        #endregion

        #region Add

        public Task AddTorrentsAsync([NotNull] AddTorrentFilesRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/upload");
                var data = new MultipartFormDataContent();
                foreach (var file in request.TorrentFiles)
                {
                    data.AddFile("torrents", file, "application/x-bittorrent");
                }

                await DownloadCoreAsync(uri, data, request).ConfigureAwait(false);
            }
        }

        public Task AddTorrentsAsync([NotNull] AddTorrentUrlsRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/download");
                var urls = string.Join("\n", request.TorrentUrls.Select(url => url.AbsoluteUri));
                var data = new MultipartFormDataContent().AddValue("urls", urls);
                await DownloadCoreAsync(uri, data, request).ConfigureAwait(false);
            }
        }

        protected async Task DownloadCoreAsync(Uri uri, MultipartFormDataContent data, AddTorrentRequest request)
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

        #region Pause/Resume

        public Task PauseAsync([NotNull] string hash)
        {
            ValidateHash(hash);
            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/pause");
                await _client.PostAsync(uri, BuildForm(("hash", hash))).ConfigureAwait(false);
            }
        }

        public async Task PauseAllAsync()
        {
            var uri = BuildUri("/command/pauseAll");
            await _client.PostAsync(uri, BuildForm()).ConfigureAwait(false);
        }

        public Task ResumeAsync([NotNull] string hash)
        {
            ValidateHash(hash);
            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/resume");
                await _client.PostAsync(uri, BuildForm(("hash", hash))).ConfigureAwait(false);
            }
        }

        public async Task ResumeAllAsync()
        {
            var uri = BuildUri("/command/resumeAll");
            await _client.PostAsync(uri, BuildForm()).ConfigureAwait(false);
        }

        #endregion

        #region Categories

        public Task AddCategoryAsync([NotNull] string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("The name cannot be empty.", nameof(name));

            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/addCategory");
                await _client.PostAsync(uri, BuildForm(("category", name))).ConfigureAwait(false);
            }
        }

        public Task DeleteCategoriesAsync([NotNull, ItemNotNull] params string[] names)
        {
            if (names == null)
                throw new ArgumentNullException(nameof(names));
            if (names.Length == 0)
                throw new ArgumentException("The name list cannot be empty.", nameof(names));
            if (names.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("A category name cannot be null or empty string.", nameof(names));

            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/removeCategories");
                await _client.PostAsync(
                        uri,
                        BuildForm(("categories", string.Join("\n", names))))
                    .ConfigureAwait(false);
            }
        }

        public Task DeleteCategoriesAsync([NotNull, ItemNotNull] IEnumerable<string> names)
        {
            return DeleteCategoriesAsync(names?.ToArray());
        }

        public Task SetTorrentCategoryAsync(
            [NotNull] string hash, 
            [NotNull] string category)
        {
            ValidateHash(hash);
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/setCategory");
                await _client.PostAsync(uri,
                    BuildForm(
                        ("hashes", hash),
                        ("category", category)
                    )).ConfigureAwait(false);
            }
        }

        public Task SetTorrentCategoryAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes,
            [NotNull] string category)
        {
            if (hashes == null)
                throw new ArgumentNullException(nameof(hashes));
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/setCategory");
                await _client.PostAsync(uri,
                    BuildForm(
                        ("hashes", string.Join("|", hashes)),
                        ("category", category)
                    )).ConfigureAwait(false);
            }
        }

        #endregion

        #region Limits

        public Task<long?> GetTorrentDownloadLimitAsync([NotNull] string hash)
        {
            ValidateHash(hash);
            return ExecuteAsync();

            async Task<long?> ExecuteAsync()
            {
                var dict = await GetTorrentDownloadLimitAsync(new[] {hash}).ConfigureAwait(false);
                return dict?.Values?.SingleOrDefault();
            }
        }

        public Task<IReadOnlyDictionary<string, long>> GetTorrentDownloadLimitAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes)
        {
            var hashesString = JoinHashes(hashes);
            return ExecuteAsync();

            async Task<IReadOnlyDictionary<string, long>> ExecuteAsync()
            {
                var uri = BuildUri("/command/getTorrentsDlLimit");
                var response = await _client.PostAsync(
                        uri,
                        BuildForm(("hashes", hashesString)))
                    .ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, long>>(json);
                return dict;
            }
        }

        public Task SetTorrentDownloadLimitAsync(
            [NotNull] string hash, 
            long limit)
        {
            ValidateHash(hash);
            return SetTorrentDownloadLimitAsync(new[] {hash}, limit);
        }

        public Task SetTorrentDownloadLimitAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes, 
            long limit)
        {
            var hashesString = JoinHashes(hashes);
            if (limit < 0)
                throw new ArgumentOutOfRangeException(nameof(limit));

            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/setTorrentsDlLimit");
                await _client.PostAsync(
                        uri,
                        BuildForm(
                            ("hashes", hashesString),
                            ("limit", limit.ToString())))
                    .ConfigureAwait(false);
            }
        }

        public Task<long?> GetTorrentUploadLimitAsync([NotNull] string hash)
        {
            ValidateHash(hash);
            return ExecuteAsync();

            async Task<long?> ExecuteAsync()
            {
                var dict = await GetTorrentUploadLimitAsync(new[] {hash}).ConfigureAwait(false);
                return dict?.Values?.SingleOrDefault();
            }
        }

        public Task<IReadOnlyDictionary<string, long>> GetTorrentUploadLimitAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes)
        {
            var hashesString = JoinHashes(hashes);
            return ExecuteAsync();

            async Task<IReadOnlyDictionary<string, long>> ExecuteAsync()
            {
                var uri = BuildUri("/command/getTorrentsUpLimit");
                var response = await _client.PostAsync(
                        uri,
                        BuildForm(("hashes", hashesString)))
                    .ConfigureAwait(false);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var dict = JsonConvert.DeserializeObject<Dictionary<string, long>>(json);
                return dict;
            }
        }

        public Task SetTorrentUploadLimitAsync(
            [NotNull] string hash, 
            long limit)
        {
            ValidateHash(hash);
            return SetTorrentUploadLimitAsync(new[] { hash }, limit);
        }

        public Task SetTorrentUploadLimitAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes, 
            long limit)
        {
            var hashesString = JoinHashes(hashes);
            if (limit < 0)
                throw new ArgumentOutOfRangeException(nameof(limit));

            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/setTorrentsUpLimit");
                await _client.PostAsync(
                        uri,
                        BuildForm(
                            ("hashes", hashesString),
                            ("limit", limit.ToString())))
                    .ConfigureAwait(false);
            }
        }

        public async Task<long?> GetGlobalDownloadLimitAsync()
        {
            var uri = BuildUri("/command/getGlobalDlLimit");
            var response = await _client.PostAsync(uri, null).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var strValue = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return long.TryParse(strValue, out long value) ? value : 0;
        }

        public Task SetGlobalDownloadLimitAsync(long limit)
        {
            if (limit < 0)
                throw new ArgumentOutOfRangeException(nameof(limit));

            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/setGlobalDlLimit");
                await _client.PostAsync(uri, BuildForm(("limit", limit.ToString()))).ConfigureAwait(false);
            }
        }

        public async Task<long?> GetGlobalUploadLimitAsync()
        {
            var uri = BuildUri("/command/getGlobalUpLimit");
            var response = await _client.PostAsync(uri, null).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            var strValue = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return long.TryParse(strValue, out long value) ? value : 0;
        }

        public Task SetGlobalUploadLimitAsync(long limit)
        {
            if (limit < 0)
                throw new ArgumentOutOfRangeException(nameof(limit));

            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/setGlobalUpLimit");
                await _client.PostAsync(uri, BuildForm(("limit", limit.ToString()))).ConfigureAwait(false);
            }
        }

        #endregion

        #region Priority

        public Task ChangeTorrentPriorityAsync(
            [NotNull] string hash, 
            TorrentPriorityChange change)
        {
            ValidateHash(hash);
            return ChangeTorrentPriorityAsync(new[] {hash}, change);
        }

        public Task ChangeTorrentPriorityAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes, 
            TorrentPriorityChange change)
        {
            var hashesString = JoinHashes(hashes);
            var path = GetPath();
            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri(path);
                await _client.PostAsync(
                        uri,
                        BuildForm(("hashes", hashesString)))
                    .ConfigureAwait(false);
            }

            string GetPath()
            {
                switch (change)
                {
                    case TorrentPriorityChange.Minimal:
                        return "/command/bottomPrio";
                    case TorrentPriorityChange.Increase:
                        return "/command/decreasePrio";
                    case TorrentPriorityChange.Decrease:
                        return "/command/increasePrio";
                    case TorrentPriorityChange.Maximal:
                        return "/command/topPrio";
                    default:
                        throw new ArgumentOutOfRangeException(nameof(change), change, null);
                }
            }
        }

        public Task SetFilePriorityAsync(
            [NotNull] string hash, 
            int fileId, 
            TorrentContentPriority priority)
        {
            ValidateHash(hash);
            if (fileId < 0)
                throw new ArgumentOutOfRangeException(nameof(fileId));
            if (!Enum.GetValues(typeof(TorrentContentPriority)).Cast<TorrentContentPriority>().Contains(priority))
                throw new ArgumentOutOfRangeException(nameof(priority));

            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/setFilePrio");
                await _client.PostAsync(
                        uri,
                        BuildForm(
                            ("hash", hash),
                            ("id", fileId.ToString()),
                            ("priority", priority.ToString("D"))))
                    .ConfigureAwait(false);
            }
        }

        #endregion

        #region Other

        public Task DeleteAsync(
            [NotNull] string hash, 
            bool deleteDownloadedData = false)
        {
            ValidateHash(hash);
            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = deleteDownloadedData
                    ? BuildUri("/command/deletePerm")
                    : BuildUri("/command/delete");
                await _client.PostAsync(uri, BuildForm(("hashes", hash))).ConfigureAwait(false);
            }
        }

        public Task DeleteAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes, 
            bool deleteDownloadedData = false)
        {
            var hashesString = JoinHashes(hashes);
            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = deleteDownloadedData
                    ? BuildUri("/command/deletePerm")
                    : BuildUri("/command/delete");
                await _client.PostAsync(
                        uri,
                        BuildForm(("hashes", hashesString)))
                    .ConfigureAwait(false);
            }
        }

        public Task SetLocationAsync(
            [NotNull] string hash, 
            [NotNull] string newLocation)
        {
            ValidateHash(hash);
            if (newLocation == null)
                throw new ArgumentNullException(nameof(newLocation));
            if (string.IsNullOrEmpty(newLocation))
                throw new ArgumentException("The location cannot be an empty string.", nameof(newLocation));

            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/setLocation");
                await _client.PostAsync(uri,
                    BuildForm(
                        ("hashes", hash),
                        ("location", newLocation)
                    )).ConfigureAwait(false);
            }
        }

        public Task SetLocationAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes, 
            [NotNull] string newLocation)
        {
            var hashesString = JoinHashes(hashes);
            if (newLocation == null)
                throw new ArgumentNullException(nameof(newLocation));
            if (string.IsNullOrEmpty(newLocation))
                throw new ArgumentException("The location cannot be an empty string.", nameof(newLocation));

            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/setLocation");
                await _client.PostAsync(uri,
                    BuildForm(
                        ("hashes", hashesString),
                        ("location", newLocation)
                    )).ConfigureAwait(false);
            }
        }

        public Task RenameAsync(
            [NotNull] string hash, 
            [NotNull] string newName)
        {
            ValidateHash(hash);
            if (newName == null)
                throw new ArgumentNullException(nameof(newName));
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("The name cannot be an empty string.", nameof(newName));

            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/rename");
                var response = await _client.PostAsync(uri,
                    BuildForm(
                        ("hash", hash),
                        ("name", newName)
                    )).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
            }
        }

        public Task AddTrackersAsync(
            [NotNull] string hash, 
            [NotNull, ItemNotNull] params Uri[] trackers)
        {
            return AddTrackersAsync(hash, trackers?.AsEnumerable());
        }

        public Task AddTrackersAsync(
            [NotNull] string hash,
            [NotNull, ItemNotNull] IEnumerable<Uri> trackers)
        {
            ValidateHash(hash);
            var urls = GetUrls();

            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/addTrackers");
                await _client.PostAsync(uri,
                    BuildForm(
                        ("hash", hash),
                        ("urls", urls)
                    )).ConfigureAwait(false);
            }

            string GetUrls()
            {
                if (trackers == null)
                    throw new ArgumentNullException(nameof(trackers));

                var builder = new StringBuilder(4096);
                foreach (var tracker in trackers)
                {
                    if (tracker == null)
                        throw new ArgumentException("The collection must not contain nulls.", nameof(trackers));
                    if (!tracker.IsAbsoluteUri)
                        throw new ArgumentException("The collection must contain absolute URIs.", nameof(trackers));

                    if (builder.Length > 0)
                    {
                        builder.Append('\n');
                    }

                    builder.Append(tracker.AbsoluteUri);
                }

                if (builder.Length == 0)
                    throw new ArgumentException("The collection must contain at least one URI.", nameof(trackers));

                return builder.ToString();
            }
        }

        public Task RecheckAsync([NotNull] string hash)
        {
            ValidateHash(hash);
            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/recheck");
                await _client.PostAsync(uri, BuildForm(("hash", hash))).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<TorrentLogEntry>> GetLogAsync(TorrentLogSeverity severity = TorrentLogSeverity.All, int afterId = -1)
        {
            var uri = BuildUri("/query/getLog",
                ("normal", severity.HasFlag(TorrentLogSeverity.Normal).ToString().ToLowerInvariant()),
                ("info", severity.HasFlag(TorrentLogSeverity.Info).ToString().ToLowerInvariant()),
                ("warning", severity.HasFlag(TorrentLogSeverity.Warning).ToString().ToLowerInvariant()),
                ("critical", severity.HasFlag(TorrentLogSeverity.Critical).ToString().ToLowerInvariant()),
                ("last_known_id", afterId.ToString())
            );

            var json = await _client.GetStringAsync(uri).ConfigureAwait(false);
            return JsonConvert.DeserializeObject<IEnumerable<TorrentLogEntry>>(json);
        }

        public async Task<bool> GetAlternativeSpeedLimitsEnabledAsync()
        {
            var uri = BuildUri("/command/alternativeSpeedLimitsEnabled");
            var result = await _client.GetStringAsync(uri).ConfigureAwait(false);
            return result == "1";
        }

        public async Task ToggleAlternativeSpeedLimitsAsync()
        {
            var uri = BuildUri("/command/toggleAlternativeSpeedLimits");
            var result = await _client.PostAsync(uri, BuildForm()).ConfigureAwait(false);
            result.EnsureSuccessStatusCode();
        }

        public Task SetAutomaticTorrentManagementAsync(
            [NotNull] string hash, 
            bool enabled)
        {
            ValidateHash(hash);
            return SetAutomaticTorrentManagementAsync(new[] {hash}, enabled);
        }

        public Task SetAutomaticTorrentManagementAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes, 
            bool enabled)
        {
            var hashesString = JoinHashes(hashes);
            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/setAutoTMM");
                await _client.PostAsync(uri,
                    BuildForm(
                        ("hashes", hashesString),
                        ("enable", enabled.ToString().ToLowerInvariant())
                    )).ConfigureAwait(false);
            }
        }

        public Task SetForceStartAsync(
            [NotNull] string hash, 
            bool enabled)
        {
            ValidateHash(hash);
            return SetForceStartAsync(new[] { hash }, enabled);
        }

        public Task SetForceStartAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes, 
            bool enabled)
        {
            var hashesString = JoinHashes(hashes);
            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/setForceStart");
                await _client.PostAsync(uri,
                    BuildForm(
                        ("hashes", hashesString),
                        ("value", enabled.ToString().ToLowerInvariant())
                    )).ConfigureAwait(false);
            }
        }

        public Task SetSuperSeedingAsync(
            [NotNull] string hash, 
            bool enabled)
        {
            ValidateHash(hash);
            return SetSuperSeedingAsync(new[] { hash }, enabled);
        }

        public Task SetSuperSeedingAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes, 
            bool enabled)
        {
            var hashesString = JoinHashes(hashes);
            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/setSuperSeeding");
                await _client.PostAsync(uri,
                    BuildForm(
                        ("hashes", hashesString),
                        ("value", enabled.ToString().ToLowerInvariant())
                    )).ConfigureAwait(false);
            }
        }

        public Task ToggleFirstLastPiecePrioritizedAsync([NotNull] string hash)
        {
            ValidateHash(hash);
            return ToggleFirstLastPiecePrioritizedAsync(new[] { hash });
        }

        public Task ToggleFirstLastPiecePrioritizedAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes)
        {
            var hashesString = JoinHashes(hashes);
            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/toggleFirstLastPiecePrio");
                await _client.PostAsync(uri,
                    BuildForm(("hashes", hashesString))).ConfigureAwait(false);
            }
        }

        public Task ToggleSequentialDownloadAsync([NotNull] string hash)
        {
            ValidateHash(hash);
            return ToggleSequentialDownloadAsync(new[] { hash });
        }

        public Task ToggleSequentialDownloadAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes)
        {
            var hashesString = JoinHashes(hashes);
            return ExecuteAsync();

            async Task ExecuteAsync()
            {
                var uri = BuildUri("/command/toggleSequentialDownload");
                await _client.PostAsync(uri,
                    BuildForm(("hashes", hashesString))).ConfigureAwait(false);
            }
        }

        #endregion

        public void Dispose()
        {
            _client?.Dispose();
        }

        private HttpContent BuildForm(params (string key, string value)[] fields)
        {
            return new CompatibleFormUrlEncodedContent(fields);
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

        private struct UrlItem
        {
            [JsonProperty("url")]
            public Uri Url { get; set; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool HashIsValid(string hash)
        {
            return hash.Length == 40 && hash.All(c => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [ContractAnnotation("null => halt")]
        private static void ValidateHash(string hash)
        {
            if (hash == null)
                throw new ArgumentNullException(nameof(hash));

            if (!HashIsValid(hash))
                throw new ArgumentException("The parameter must be a hexadecimal representation of SHA-1 hash.", nameof(hash));
        }

        [ContractAnnotation("null => halt")]
        private static string JoinHashes(IEnumerable<string> hashes)
        {
            if (hashes == null)
                throw new ArgumentNullException(nameof(hashes));

            var builder = new StringBuilder(4096);
            foreach (var hash in hashes)
            {
                if (hash == null || !HashIsValid(hash))
                    throw new ArgumentException("The values must be hexadecimal representations of SHA-1 hashes.", nameof(hashes));
                
                if (builder.Length > 0)
                {
                    builder.Append('|');
                }

                builder.Append(hash);
            }

            if (builder.Length == 0)
                throw new ArgumentException("The list of hashes cannot be empty.", nameof(hashes));

            return builder.ToString();
        }
    }
}
