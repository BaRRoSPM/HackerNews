using Microsoft.AspNetCore.Routing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Api
{
    public static class HackerNewsClient
    {
        private const string baseEndpoint = "https://hacker-news.firebaseio.com/v0"; // this should be placed in a config file.

        public async static Task<IEnumerable<HackerNewsStory>> GetHackerNewsStories(int numberOfStories = 0)
        {
            var result = new List<HackerNewsStory>();

            var stories = await getBestStories();

            if (numberOfStories > 0) stories = stories.Take(numberOfStories);

            foreach (var s in stories)
            {
                var d = await getStoryDetails(s);
                var story = new HackerNewsStory()
                {
                    title = d.title,
                    uri = d.url,
                    postedBy = d.by,
                    time = DateTimeOffset.FromUnixTimeSeconds(d.time).DateTime, // HackerNews API returns dates in UnixTime
                    score = d.score,
                    commentCount = d.descendants
                };
                result.Add(story);
            }
            return result;
        }

        private async static Task<IEnumerable<int>> getBestStories()
        {
            string endpoint = $"{baseEndpoint}/beststories.json"; // this should be placed in a config file.
            var result = await getFromEndpoint<IEnumerable<int>>(endpoint);
            return result;
        }

        private async static Task<StoryDetail> getStoryDetails(int storyID)
        {
            string endpoint = $"{baseEndpoint}/item/{storyID}.json"; // this should be placed in a config file.
            var result = await getFromEndpoint<StoryDetail>(endpoint);
            return result;
        }

        /// <summary>
        /// Generic method for calling a REST endpoint and returning a T result
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        private async static Task<T> getFromEndpoint<T>(string endpoint)
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetStringAsync(endpoint);
                var result = JsonConvert.DeserializeObject<T>(response);
                return result;
            }
        }

        // HackerNews Item details contract/proxy class
        private class StoryDetail
        {
            public int id { get; set; }
            public string deleted { get; set; }
            public string type { get; set; }
            public string by { get; set; }
            public long time { get; set; }
            public string text { get; set; }
            public string dead { get; set; }
            public int parent { get; set; }
            public int pool { get; set; }
            public IEnumerable<int> kids { get; set; }
            public string url { get; set; }
            public int score { get; set; }
            public string title { get; set; }
            public IEnumerable<int> parts { get; set; }
            public int descendants { get; set; }
        }
    }
}
