using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading;
using System.Threading.Tasks;

namespace fw_monitor
{
    class Program
    {
        
        static void Main(string[] args)
        {
            parseArgs(args);
            
            NFTManager manager = new NFTManager();
            manager.ManageLists();
        }

        private static void parseArgs(string[] args)
        {
            
        }
    }
}