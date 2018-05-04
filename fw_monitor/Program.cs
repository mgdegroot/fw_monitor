using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using CommandLine;

namespace fw_monitor
{
    class Program
    {
        
        static void Main(string[] args)
        {
            
            NFTManager manager = new NFTManager();
            manager.ManageLists(null, null, true);
            Console.WriteLine("done");
        }

        class CmddLineOptions
        {
            [Value(0)]
            public string Listname { get; set; }
            
            [Value(1)]
            public string Hostname { get; set; }
        }

        private static void parseArgs(string[] args)
        {
            
            foreach (string s in args)
            {
                Console.WriteLine(s);
            }

            ParserResult<CmddLineOptions> result = Parser.Default.ParseArguments<CmddLineOptions>(args)
                .WithParsed(options => { Console.WriteLine($"options.Hostname"); })
                .WithNotParsed(errors =>
                {
                    foreach (Error error in errors)
                    {
                        Console.WriteLine(error.ToString());
                    }
                });

//            Console.WriteLine(result.);

        }
    }
}