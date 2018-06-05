//using System;
//using System.Collections.Generic;
//
//namespace fw_monitor
//{
//    public interface IOutputProvider
//    {
//        IEnumerable<string> Errors { get; }
//        IEnumerable<string> Output { get; }
//        string LastError { get; }
//        string LastOutput { get; }
//
//        event Action<IOutputProvider, string> ErrorAdded;
//        event Action<IOutputProvider, string> OutputAdded;
//    }
//    
//}