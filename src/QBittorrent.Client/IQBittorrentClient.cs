using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace QBittorrent.Client
{
    /// <summary>
    /// Provides access to qBittorrent remote API.
    /// </summary>
    public interface IQBittorrentClient
    {
        /// <summary>
        /// Authenticates this client with the remote qBittorrent server.
        /// </summary>
        /// <param name="username">The username.</param>
        /// <param name="password">The password.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task LoginAsync(
            string username,
            string password,
            CancellationToken token = default);

        /// <summary>
        /// Clears authentication on the remote qBittorrent server.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task LogoutAsync(
            CancellationToken token = default);

        /// <summary>
        /// Gets the torrent list.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task<IReadOnlyList<TorrentInfo>> GetTorrentListAsync(
            TorrentListQuery query = null,
            CancellationToken token = default);

        /// <summary>
        /// Gets the torrent generic properties.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task<TorrentProperties> GetTorrentPropertiesAsync(
            [NotNull] string hash,
            CancellationToken token = default);

        /// <summary>
        /// Gets the torrent contents.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task<IReadOnlyList<TorrentContent>> GetTorrentContentsAsync(
            [NotNull] string hash,
            CancellationToken token = default);

        /// <summary>
        /// Gets the torrent trackers.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task<IReadOnlyList<TorrentTracker>> GetTorrentTrackersAsync(
            [NotNull] string hash,
            CancellationToken token = default);

        /// <summary>
        /// Gets the torrent web seeds.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task<IReadOnlyList<Uri>> GetTorrentWebSeedsAsync(
            [NotNull] string hash,
            CancellationToken token = default);

        /// <summary>
        /// Gets the states of the torrent pieces.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task<IReadOnlyList<TorrentPieceState>> GetTorrentPiecesStatesAsync(
            [NotNull] string hash,
            CancellationToken token = default);

        /// <summary>
        /// Gets the hashes of the torrent pieces.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task<IReadOnlyList<string>> GetTorrentPiecesHashesAsync(
            [NotNull] string hash,
            CancellationToken token = default);

        /// <summary>
        /// Gets the global transfer information.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task<GlobalTransferInfo> GetGlobalTransferInfoAsync(
            CancellationToken token = default);

        /// <summary>
        /// Gets the partial data.
        /// </summary>
        /// <param name="responseId">The response identifier.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task<PartialData> GetPartialDataAsync(
            int responseId = 0,
            CancellationToken token = default);

        /// <summary>
        /// Adds the torrent files to download.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task AddTorrentsAsync(
            [NotNull] AddTorrentFilesRequest request,
            CancellationToken token = default);

        /// <summary>
        /// Adds the torrent URLs or magnet-links to download.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task AddTorrentsAsync(
            [NotNull] AddTorrentUrlsRequest request,
            CancellationToken token = default);

        /// <summary>
        /// Pauses the torrent.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task PauseAsync(
            [NotNull] string hash,
            CancellationToken token = default);

        /// <summary>
        /// Pauses all torrents.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task PauseAllAsync(
            CancellationToken token = default);

        /// <summary>
        /// Resumes the torrent.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task ResumeAsync(
            [NotNull] string hash,
            CancellationToken token = default);

        /// <summary>
        /// Resumes all torrents.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task ResumeAllAsync(
            CancellationToken token = default);

        /// <summary>
        /// Adds the category.
        /// </summary>
        /// <param name="category">The category name.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task AddCategoryAsync(
            [NotNull] string category,
            CancellationToken token = default);

        /// <summary>
        /// Deletes the category.
        /// </summary>
        /// <param name="category">The category name.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task DeleteCategoryAsync(
            [NotNull] string category,
            CancellationToken token = default);

        /// <summary>
        /// Deletes the categories.
        /// </summary>
        /// <param name="categories">The list of categories' names.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task DeleteCategoriesAsync(
            [NotNull, ItemNotNull] IEnumerable<string> categories,
            CancellationToken token = default);

        /// <summary>
        /// Sets the torrent category.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="category">The category.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task SetTorrentCategoryAsync(
            [NotNull] string hash,
            [NotNull] string category,
            CancellationToken token = default);

        /// <summary>
        /// Sets the torrent category.
        /// </summary>
        /// <param name="hashes">The torrent hashes.</param>
        /// <param name="category">The category.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task SetTorrentCategoryAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes,
            [NotNull] string category,
            CancellationToken token = default);

        /// <summary>
        /// Gets the torrent download speed limit.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task<long?> GetTorrentDownloadLimitAsync(
            [NotNull] string hash,
            CancellationToken token = default);

        /// <summary>
        /// Gets the torrent download speed limit.
        /// </summary>
        /// <param name="hashes">The torrent hashes.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task<IReadOnlyDictionary<string, long>> GetTorrentDownloadLimitAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes,
            CancellationToken token = default);

        /// <summary>
        /// Sets the torrent download speed limit.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task SetTorrentDownloadLimitAsync(
            [NotNull] string hash,
            long limit,
            CancellationToken token = default);

        /// <summary>
        /// Sets the torrent download speed limit.
        /// </summary>
        /// <param name="hashes">The torrent hashes.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task SetTorrentDownloadLimitAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes,
            long limit,
            CancellationToken token = default);

        /// <summary>
        /// Gets the torrent upload speed limit.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task<long?> GetTorrentUploadLimitAsync(
            [NotNull] string hash,
            CancellationToken token = default);

        /// <summary>
        /// Gets the torrent upload speed limit.
        /// </summary>
        /// <param name="hashes">The torrent hashes.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task<IReadOnlyDictionary<string, long>> GetTorrentUploadLimitAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes,
            CancellationToken token = default);

        /// <summary>
        /// Sets the torrent upload speed limit.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task SetTorrentUploadLimitAsync(
            [NotNull] string hash,
            long limit,
            CancellationToken token = default);

        /// <summary>
        /// Sets the torrent upload speed limit.
        /// </summary>
        /// <param name="hashes">The torrent hashes.</param>
        /// <param name="limit">The limit.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task SetTorrentUploadLimitAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes,
            long limit,
            CancellationToken token = default);

        /// <summary>
        /// Gets the global download speed limit.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task<long?> GetGlobalDownloadLimitAsync(
            CancellationToken token = default);

        /// <summary>
        /// Sets the global download speed limit.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task SetGlobalDownloadLimitAsync(
            long limit,
            CancellationToken token = default);

        /// <summary>
        /// Gets the global upload speed limit.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task<long?> GetGlobalUploadLimitAsync(
            CancellationToken token = default);

        /// <summary>
        /// Sets the global upload speed limit.
        /// </summary>
        /// <param name="limit">The limit.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task SetGlobalUploadLimitAsync(
            long limit,
            CancellationToken token = default);

        /// <summary>
        /// Changes the torrent priority.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="change">The priority change.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task ChangeTorrentPriorityAsync(
            [NotNull] string hash,
            TorrentPriorityChange change,
            CancellationToken token = default);

        /// <summary>
        /// Changes the torrent priority.
        /// </summary>
        /// <param name="hashes">The torrent hashes.</param>
        /// <param name="change">The priority change.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task ChangeTorrentPriorityAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes,
            TorrentPriorityChange change,
            CancellationToken token = default);

        /// <summary>
        /// Sets the file priority.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="fileId">The file identifier.</param>
        /// <param name="priority">The priority.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task SetFilePriorityAsync(
            [NotNull] string hash,
            int fileId,
            TorrentContentPriority priority,
            CancellationToken token = default);

        /// <summary>
        /// Deletes the torrent.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="deleteDownloadedData"><see langword="true"/> to delete the downloaded data.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task DeleteAsync(
            [NotNull] string hash,
            bool deleteDownloadedData = false,
            CancellationToken token = default);

        /// <summary>
        /// Deletes the torrents.
        /// </summary>
        /// <param name="hashes">The torrent hashes.</param>
        /// <param name="deleteDownloadedData"><see langword="true"/> to delete the downloaded data.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task DeleteAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes,
            bool deleteDownloadedData = false,
            CancellationToken token = default);

        /// <summary>
        /// Sets the location of torrent.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="newLocation">The new location.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task SetLocationAsync(
            [NotNull] string hash,
            [NotNull] string newLocation,
            CancellationToken token = default);

        /// <summary>
        /// Sets the location of the torrents.
        /// </summary>
        /// <param name="hashes">The torrent hashes.</param>
        /// <param name="newLocation">The new location.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task SetLocationAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes,
            [NotNull] string newLocation,
            CancellationToken token = default);

        /// <summary>
        /// Renames the torrent.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="newName">The new name.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task RenameAsync(
            [NotNull] string hash,
            [NotNull] string newName,
            CancellationToken token = default);

        /// <summary>
        /// Adds the tracker to the torrent.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="tracker">The tracker.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task AddTrackerAsync(
            [NotNull] string hash,
            [NotNull] Uri tracker,
            CancellationToken token = default);

        /// <summary>
        /// Adds the trackers to the torrent.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="trackers">The trackers.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task AddTrackersAsync(
            [NotNull] string hash,
            [NotNull, ItemNotNull] IEnumerable<Uri> trackers,
            CancellationToken token = default);

        /// <summary>
        /// Rechecks the torrent.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task RecheckAsync(
            [NotNull] string hash,
            CancellationToken token = default);

        /// <summary>
        /// Gets the server log.
        /// </summary>
        /// <param name="severity">The severity of log entries to return. <see cref="TorrentLogSeverity.All"/> by default.</param>
        /// <param name="afterId">Return the entries with the ID greater than the specified one.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task<IEnumerable<TorrentLogEntry>> GetLogAsync(
            TorrentLogSeverity severity = TorrentLogSeverity.All,
            int afterId = -1,
            CancellationToken token = default);

        /// <summary>
        /// Gets the value indicating whether the alternative speed limits are enabled.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task<bool> GetAlternativeSpeedLimitsEnabledAsync(
            CancellationToken token = default);

        /// <summary>
        /// Toggles the alternative speed limits.
        /// </summary>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task ToggleAlternativeSpeedLimitsAsync(
            CancellationToken token = default);

        /// <summary>
        /// Sets the automatic torrent management.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="enabled"></param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task SetAutomaticTorrentManagementAsync(
            [NotNull] string hash,
            bool enabled,
            CancellationToken token = default);

        /// <summary>
        /// Sets the automatic torrent management.
        /// </summary>
        /// <param name="hashes">The torrent hashes.</param>
        /// <param name="enabled"></param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task SetAutomaticTorrentManagementAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes,
            bool enabled,
            CancellationToken token = default);

        /// <summary>
        /// Sets the force start.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="enabled"></param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task SetForceStartAsync(
            [NotNull] string hash,
            bool enabled,
            CancellationToken token = default);

        /// <summary>
        /// Sets the force start.
        /// </summary>
        /// <param name="hashes">The torrent hashes.</param>
        /// <param name="enabled"></param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task SetForceStartAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes,
            bool enabled,
            CancellationToken token = default);

        /// <summary>
        /// Sets the super seeding.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="enabled"></param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task SetSuperSeedingAsync(
            [NotNull] string hash,
            bool enabled,
            CancellationToken token = default);

        /// <summary>
        /// Sets the super seeding asynchronous.
        /// </summary>
        /// <param name="hashes">The torrent hashes.</param>
        /// <param name="enabled"></param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task SetSuperSeedingAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes,
            bool enabled,
            CancellationToken token = default);

        /// <summary>
        /// Toggles the first and last piece priority.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task ToggleFirstLastPiecePrioritizedAsync(
            [NotNull] string hash,
            CancellationToken token = default);

        /// <summary>
        /// Toggles the first and last piece priority.
        /// </summary>
        /// <param name="hashes">The torrent hashes.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task ToggleFirstLastPiecePrioritizedAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes,
            CancellationToken token = default);

        /// <summary>
        /// Toggles the sequential download.
        /// </summary>
        /// <param name="hash">The torrent hash.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task ToggleSequentialDownloadAsync(
            [NotNull] string hash,
            CancellationToken token = default);

        /// <summary>
        /// Toggles the sequential download.
        /// </summary>
        /// <param name="hashes">The torrent hashes.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task ToggleSequentialDownloadAsync(
            [NotNull, ItemNotNull] IEnumerable<string> hashes,
            CancellationToken token = default);
    }
}