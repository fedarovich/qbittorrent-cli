using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace QBittorrent.Client.Extensions
{
    internal static class HttpClientExtensions
    {
        public static async Task<string> GetStringAsync(this HttpClient client, Uri uri, CancellationToken token)
        {
            using (var response = await client.GetAsync(uri, token).ConfigureAwait(false))
            {
                response.EnsureSuccessStatusCode();
                HttpContent content = response.Content;
                if (content != null)
                {
                    return await content.ReadAsStringAsync().ConfigureAwait(false);
                }

                return string.Empty;
            }
        }
    }
}
