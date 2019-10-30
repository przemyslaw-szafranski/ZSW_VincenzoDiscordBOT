using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace VicenzoDiscordBot
{
    class Utilities
    {
        private static Dictionary<string,string> _phrases;
        /// <summary>
        /// Utilities Constructor
        /// </summary>
        static Utilities()
        {
            try
            {
                string json = File.ReadAllText("Phrases/phrases.json");
                var data = JsonConvert.DeserializeObject<dynamic>(json);
                _phrases = data.ToObject <Dictionary<string, string>>();
            }catch(Exception e)
            {
                throw e;
            }
        }
        public static string GetPhrase(string key)
        {
            if (_phrases.ContainsKey(key)) return _phrases[key];
            return "";
        }
        public static int Random(int begin, int end)
        {
            Random r = new Random();
            int rInt = r.Next(begin, end); //for ints
            return rInt;
        }
        public static string GetFormattedPhrase(string key, params object[] parameter)
        {
            if (_phrases.ContainsKey(key))
            {
                return String.Format(_phrases[key], parameter);
            }
            return "";
        }
    }
}
