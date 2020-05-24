using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PlexureTest.Exercise1
{
    public class Exercise1
    {
        public async Task<long> GetContentLength(IEnumerable<string> urls, CancellationToken cancellationToken)
        {
            var httpClient = new HttpClient();
            var tasks = urls.Select(url => httpClient.GetAsync((string)url, cancellationToken)).ToList();
            await Task.WhenAll(tasks);
            return tasks.Sum(x => x.Result.Content.Headers.ContentLength ?? 0);
        }
    }
}
