using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VincenzoBot.Models
{
    class LiveBroadcastModel
    {
        private string liveChatId { get; set; }
        private string title { get; set; }
        private string thumbnailURL { get; set; }

        public LiveBroadcastModel(string id, string title, string thumbnailURL)
        {
            this.liveChatId = id;
            this.title = title;
            this.thumbnailURL = thumbnailURL;
        }


    }
}
