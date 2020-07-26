using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api
{
    public class HackerNewsStory
    {
        public string title { get; set; }
        public string uri { get; set; }
        public string postedBy { get; set; }
        public DateTime time { get; set; }
        public int score { get; set; }
        public int commentCount { get; set; }
    }


}
