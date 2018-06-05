using System;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace fw_monitor
{
    
    public class Util : IUtil
    {
        public static string SerializePath { get; set; } = Environment.GetEnvironmentVariable("HOME") + "/NftHosts/";
        public static Encoding DefaultEncoding = Encoding.UTF8;
        public const string MAINLISTNAME = "COMBINED";

        public struct FilenameExtension
        {
            public string JSON => ".json";
            public string XML => ".xml";
            public string CONFIG => ".config";
            public string LOG => ".log";
            public string TEXT => ".txt";

        }

        public enum BackingStore
        {
            FILE,
            DATABASE,
            SERVICE,
            NONE,
        }
        
        public static FilenameExtension Extension { get; set; }

        public static BackingStore BackingStoreType { get; set; } = BackingStore.FILE;
        
//        public delegate void OutputHandler(IOutputProvider sender, string msg);
        
        public bool WriteToFile(string path, string content, bool append=false)
        {
            FileInfo attributes = new FileInfo(path);
            if (!Directory.Exists(attributes.DirectoryName))
            {
                Directory.CreateDirectory(attributes.DirectoryName);
            }

            try
            {
                if (append)
                {
                    File.AppendAllText(path, content, Encoding.UTF8);
                }
                else
                {
                    File.WriteAllText(path, content, Encoding.UTF8);
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string ReadFromFile(string path, bool throwWhenNotFound=false)
        {
            if (File.Exists(path))
            {
                return File.ReadAllText(path, DefaultEncoding);
            }
            else if (throwWhenNotFound)
            {
                throw new FileNotFoundException($"File {path} not found.");
            }
            
            return null;
        }
    }
}