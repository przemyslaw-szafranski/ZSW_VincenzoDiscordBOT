using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VincenzoDiscordBot.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace VincenzoDiscordBot.Services
{
    class LiveBroadcastService
    {
        private List<LiveBroadcastModel> _broadcasts;
 
        public void CreateCollection(string json)
        {
            _broadcasts = new List<LiveBroadcastModel>();
            JObject broadcastsAllInfo = JObject.Parse(json);
            List<JToken> liveChatIds = broadcastsAllInfo["items"].Children()["snippet"]["liveChatId"].ToList();
            List<JToken> titles = broadcastsAllInfo["items"].Children()["snippet"]["title"].ToList();
            List<JToken> thumbnailsURLs = broadcastsAllInfo["items"].Children()["snippet"]["thumbnails"]["default"]["url"].ToList();
            for(int i = 0;i< liveChatIds.Count;i++)
                _broadcasts.Add(new LiveBroadcastModel(liveChatIds[i].ToString(), titles[i].ToString(), thumbnailsURLs[i].ToString()));
            int g = 0;
        }
    }
}
