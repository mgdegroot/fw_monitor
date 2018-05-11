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
    public class ListFetcher : IListFetcher
    {
        public string CombinedListName { get; set; } = Util.MAINLISTNAME;

        public ListConfig ListConfig { get; set; } = new ListConfig();

        public Dictionary<string, List<string>> Lists { get; set; } = new Dictionary<string, List<string>>();
                
        public string LastError { get; private set; } = string.Empty;
        
        public IList<string> this[string key]
        {
            get => Get(key); 
            set => Set(key, value as List<string>);
        }

        public IList<string> Get(string name)
        {
            Lists.TryGetValue(name, out List<string> list);
            return list;
        }

        public void Set(string name, List<string> list)
        {
            Lists[name] = list;
        }

        private string Raw { get; set; } = string.Empty;
        private List<string> RawLines { get; set; } = new List<string>();
        private List<string> Lines { get; set; } = new List<string>();
        
        public ListFetcher() {}

        
        public ListFetcher(ListConfig listListConfig)
        {
            ListConfig = listListConfig;
        }

        public async Task FetchAndParse()
        {
            await orchestrateActions();
        }
        
        private void SaveRawToFile(string path)
        {
            if (string.IsNullOrEmpty(Raw))
            {
                LastError = "Content (Raw) is empty. Not writing empty file...";
                return;
            }
            
            File.WriteAllText(path, Raw);
        }
        
        private void SaveListToFile(string path)
        {
            if (Lines.Count == 0)
            {
                LastError = "List is empty. Not writing empty file";
            }
            
            File.WriteAllLines(path, Lines);
        }
        
        private async Task fetch()
        {
            
            if (ListConfig.URL == null)
            {
                throw new ArgumentException("URL is not set");
            }

            HttpClient client = new HttpClient();
            try
            {
                Raw = await client.GetStringAsync(ListConfig.URL);
            }
            catch (HttpRequestException hre)
            {
                LastError = hre.Message;
            }
            finally
            {
                client.Dispose();
            }
        }

        private async Task orchestrateActions()
        {
            await fetch();
            convertRawToList();
            cleanupList();
            splitListInParts();
        }

        private void convertRawToList()
        {
            if (string.IsNullOrEmpty(Raw))
            {
                LastError = "Content (Raw) is empty. Not converting...";
                return;
            }

            RawLines = Raw.Split(ListConfig.LineSeparator).Where(i => string.IsNullOrEmpty(i) == false).ToList();
        }

        private void cleanupList()
        {
            if (string.IsNullOrEmpty(Raw))
            {
                LastError = "Content (Raw) is empty. Not cleaning...";
            }

            Lines = RawLines.Where(i => i.StartsWith('#') == false).ToList();
        }

        private void splitListInParts()
        {
            // Remove all comment fluff -->
            List<string> intermediate = new List<string>();

            intermediate.AddRange(RawLines.Where(i => ListConfig.EmptyLineIndicators.IsMatch(i) == false));

            int listRevision = 0;
            bool inBody = false;
            string listName = "noname";
            for (int i = 0; i < intermediate.Count; i++)
            {
                if (!inBody)
                {
                    Match match = ListConfig.RevisionRegex.Match(intermediate[i]);

                    if (match.Success)
                    {
                        int.TryParse(match.Groups[1].Value, out listRevision);
                        ListConfig.Version = listRevision.ToString();
                        inBody = true;
                    }
                }
                else
                {
                    Match subsetMatch = ListConfig.SubsetHeader.Match(intermediate[i]);
                    if (subsetMatch.Success)
                    {
                        listName = subsetMatch.Groups[1].Value;

                        listName = ListConfig.InvalidListnameChars.Replace(listName, ListConfig.InvalidCharReplacement);
                        Lists[listName] = new List<string>();
                    }
                    else
                    {
                        Lists[listName].Add(intermediate[i]);
                    }
                }
            }

            Lists[CombinedListName] = Lines;

        }

        private void DisplayLists()
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