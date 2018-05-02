using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using fw_monitor.DataObjects;

namespace fw_monitor
{
    public class ListFetcher
    {
        public string CombinedListName { get; set; } = "COMBINED";


        public ListConfig Config { get; set; } = new ListConfig("empty");
        public Uri URL => Config.URL;
        public void setURL(string url) => Config.URL = new Uri(url);

        public string Raw { get; set; } = string.Empty;
        public List<string> RawLines { get; private set; } = new List<string>();
        public List<string> Lines { get; private set; } = new List<string>();
        
        public Dictionary<string, List<string>> Lists { get; private set; } = new Dictionary<string, List<string>>();
                
        public string LastError { get; private set; } = string.Empty;
        
        
        public ListFetcher() {}

        public ListFetcher(ListConfig listConfig)
        {
            Config = listConfig;
        }
        
        public ListFetcher(string url)
        {
            setURL(url);
        }

        public ListFetcher(Uri url)
        {
            this.Config.URL = url;
        }

        public async Task FetchAndParse()
        {
            await orchestrateActions();
        }
        
        public void SaveRawToFile(string path)
        {
            if (string.IsNullOrEmpty(Raw))
            {
                LastError = "Content (Raw) is empty. Not writing empty file...";
                return;
            }

            File.WriteAllText(path, Raw);
        }
        
        public void SaveListToFile(string path)
        {
            if (Lines.Count == 0)
            {
                LastError = "List is empty. Not writing empty file";
            }

            File.WriteAllLines(path, Lines);
        }
        
        

        private async Task fetch()
        {
            
            if (URL == null)
            {
                throw new ArgumentException("URL is not set");
            }

            HttpClient client = new HttpClient();
            try
            {
                Raw = await client.GetStringAsync(URL);
            }
            catch (HttpRequestException hre)
            {
                LastError = hre.Message;
            }
            finally
            {
                client.Dispose();
            }

            convertRawToList();
        }

        private async Task orchestrateActions()
        {
            await fetch();
            convertRawToList();
            cleanupList();
        }

        private void convertRawToList()
        {
            if (string.IsNullOrEmpty(Raw))
            {
                LastError = "Content (Raw) is empty. Not converting...";
                return;
            }

            RawLines = Raw.Split(Config.LineSeparator).Where(i => string.IsNullOrEmpty(i) == false).ToList();

//            RawLines = new List<string>(lines);
            
        }

        private void cleanupList()
        {
            if (string.IsNullOrEmpty(Raw))
            {
                LastError = "Content (Raw) is empty. Not cleaning...";
            }

            Lines = RawLines.Where(i => i.StartsWith('#') == false).ToList();
        }

        public void splitListInParts(string separator)
        {
//            Dictionary<string, List<string>> lists = new Dictionary<string, List<string>>();
            
            // Remove all comment fluff -->
            List<string> intermediate = RawLines.Where(i => (i.Trim() == "#") == false).ToList();


            //string strMatchListRevision = @"^# Rev (\d*)$";
            int listRevision = 0;
            bool inBody = false;
            string listName = "noname";
            for (int i = 0; i < intermediate.Count; i++)
            {
                if (!inBody)
                {
                    Match match = Config.RevisionRegex.Match(intermediate[i]);
//                    Match match = Regex.Match(intermediate[i], strMatchListRevision);
                    if (match.Success)
                    {
                        int.TryParse(match.Groups[1].Value, out listRevision);
                        inBody = true;
//                        continue;
                    }
//                    Console.WriteLine($"Match.value: {match.Value}\r\nMatch.Groups[1].Value: {match.Groups[1].Value}");
//                    
//                    if (intermediate[i].Contains("# Rev "))
//                    {
//                        int.TryParse(intermediate[i].Substring(6), out listRevision);
//                        inBody = true;
//                        // jump to start of loop to start processing content -->
//                        continue;
//                    }
                }
                else
                {
                    Match subsetMatch = Config.SubsetHeader.Match(intermediate[i]);
                    if (subsetMatch.Success)
//                    if (intermediate[i].StartsWith('#'))
                    {
                        listName = subsetMatch.Groups[1].Value;
//                        listName = intermediate[i].Substring(1).Trim();
                        // Limit listname to alphanumerical -->
//                        string regExInvalidChars = @"[^A-Za-z0-9\-_]";
                        listName = Config.InvalidListnameChars.Replace(listName, Config.InvalidCharReplacement);
//                        listName = Regex.Replace(listName, regExInvalidChars, "_");
                        Lists[listName] = new List<string>();
                    }
                    else
                    {
                        Lists[listName].Add(intermediate[i]);
                    }
                }
            }

            CombinedListName = "dd";
            Lists[CombinedListName] = Lines;

        }

        public void DisplayLists()
        {
            foreach (string listName in Lists.Keys)
            {
                Console.WriteLine($"\r\n==============================\r\n");
                Console.WriteLine($"\tLIST {listName}\r\n\r\n");
                foreach (string entry in Lists[listName])
                {
                    Console.WriteLine($"\t\t\t{entry}");
                }
            }
        }
    }
}