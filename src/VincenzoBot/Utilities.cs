using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace VincenzoBot
{
    class Utilities
    {
        private static Dictionary<string,string> _phrases;
        /// <summary>
        /// Utilities Constructor
        /// </summary>
        static Utilities()
        {
<<<<<<< HEAD
=======
            try
            {
                //string json = File.ReadAllText("Phrases/phrases.json");
                //var data = JsonConvert.DeserializeObject<dynamic>(json);
                //_phrases = data.ToObject <Dictionary<string, string>>();
            }
            catch(Exception e)
            {
                throw e;
            }
>>>>>>> Fix Exception
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



        public static float GetCpuUsage()
        {
            if (!ShellHelper.IsLinux())
                return -1;

            float usage;

            var output = ShellHelper.Bash("LC_ALL=C top -bn2 | grep \"Cpu(s)\" | tail -n1 | sed \"s/.*, *\\([0-9.]*\\)%* id.*/\\1/\" | awk \'{print 100 - $1}\'");
            try
            {
                usage = float.Parse(output);
            }
            catch(Exception)
            {
                usage = -1;
            }
            return usage;
        }

        public static float GetRamUsage()
        {
            if (!ShellHelper.IsLinux())
                return -1;

            float usage;

            var output = ShellHelper.Bash("free -m | awk '/Mem:/ { printf(\"%3.1f\", $3/$2*100) }'");
            try
            {
                usage = float.Parse(output);
            }
            catch (Exception)
            {
                usage = -1;
            }
            return usage;
        }

        public static float GetDiskUsage()
        {
            if (!ShellHelper.IsLinux())
                return -1;

            float usage;

            var output = ShellHelper.Bash("df -h / | awk '/\\// {print $(NF-1)}' | tr =d '%'");
            try
            {
                usage = float.Parse(output);
            }
            catch (Exception)
            {
                usage = -1;
            }
            return usage;
        }

    }
}
