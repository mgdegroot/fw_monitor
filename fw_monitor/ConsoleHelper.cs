using System;
using System.Collections;
using System.Collections.Generic;

namespace fw_monitor
{
    
    
    public static class ConsoleHelper
    {
        private static Dictionary<string,bool>defaultBoolMapping = new Dictionary<string,bool>() {{"y", true}, {"n", false}};
        
        public enum ActionKey
        {
            YES = 'y',
            NO = 'n',
            CANCEL = 'c',
            EXIT = 'e',
        }

        public static ActionKey ParseActionKey(string input)
        {
//            if (string.IsNullOrEmpty(input))
//            {
//                return ActionKey.CANCEL;
//            }
            switch (input)
            {
                case string s when s == "y":
                    return ActionKey.YES;
                    
                case string s when s == "n":
                    return ActionKey.NO;
                case string s when s == "e":
                    return ActionKey.EXIT;
                default:
                    return ActionKey.CANCEL;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool ParseInputAsBool(string input, Dictionary<string, bool>mapping = null)
        {
//            defaultBoolMapping = new Dictionary<string,bool>() {{"y", true}, {"n", false}, {null, false}};
            if (mapping == null) mapping = defaultBoolMapping;

            mapping.TryGetValue(input, out bool retVal);                
            return retVal;
        }

        public static bool ReadInputAsBool(string entry, string defaultValue = null, Dictionary<string,bool> mapping=null)
        {
            return ParseInputAsBool(ReadInput(entry, defaultValue), mapping);
        }
        
        public static string ReadInput(string entry, string defaultValue = null)
        {
            
            if (String.IsNullOrEmpty(defaultValue))
            {
                Console.Write($"{entry}: ");
            }
            else
            {
                Console.Write($"{entry} ({defaultValue}): ");    
            }
            
            string input = Console.ReadLine();
            // Sometimes the readline does not flush the input -->
            while (Console.KeyAvailable)
            {
                Console.ReadKey(true);
            }
            
            if (string.IsNullOrEmpty(input))
            {
                return defaultValue;
            }
            else
            {
                return input;
            }
        }
        
    }
}