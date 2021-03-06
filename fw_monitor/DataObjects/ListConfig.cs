﻿using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace fw_monitor.DataObjects
{
    [DataContract]
    public class ListConfig : Config, IRepositoryItem
    {
        private ICreator _creator = new ListConfigFromStdInCreator();
        
        [DataMember]
        private string _revisionRegexStr = @"^# Rev (\d*)$";
        [DataMember]
        private string _subsetHeaderRegexStr = @"^#\s*(.*)\s*$";
        [DataMember]
        private string _invalidListnameCharsRegexStr = @"[^A-Za-z0-9\-_]";
        [DataMember]
        private string _emptyLineIndicatorRegexStr = @"^[#\s]*$";
        
        // TODO: serialization has issues with RegEx. Implement custom (de)serialisation for complex types --> 
        private Regex _invalidListnameChars;
        private Regex _emptyLineIndicators;
        private Regex _subsetHeader;
        private Regex _revisionRegex;

        [DataMember(Order = 2)] public Uri Url { get; set; }
        [DataMember(Order = 3)] public Uri UrlVersion { get; set; }
        [DataMember(Order = 4)] public bool IsComposite { get; set; } = false;
        [DataMember(Order = 5)] public bool IsRevisioned { get; set; } = false;
        [DataMember(Order = 6)] public string Version { get; set; }
        [DataMember(Order = 9)] public string InvalidCharReplacement { get; set; } = "_";
        [DataMember(Order = 10)] public string LineSeparator { get; set; } = Environment.NewLine;

        public IEnumerable<string> Subsets { get; set; } = new List<string>();
        
        public Regex RevisionRegex
        {
            get => _revisionRegex ?? (_revisionRegex = new Regex(_revisionRegexStr));
            set
            {
                _revisionRegexStr = value.ToString();
                _revisionRegex = value;
            }
        }

        public Regex SubsetHeader
        {
            get => _subsetHeader ?? (_subsetHeader = new Regex(_subsetHeaderRegexStr));
            set
            {
                _subsetHeaderRegexStr = value.ToString();
                _subsetHeader = value;
            }
        }

        public Regex InvalidListnameChars
        {
            get => _invalidListnameChars ?? (_invalidListnameChars = new Regex(_invalidListnameCharsRegexStr));
            set
            {
                _invalidListnameCharsRegexStr = value.ToString();
                _invalidListnameChars = value;
            }
        }

        public Regex EmptyLineIndicators
        {
            get => _emptyLineIndicators ?? (_emptyLineIndicators = new Regex(_emptyLineIndicatorRegexStr));
            set
            {
                _emptyLineIndicatorRegexStr = value.ToString();
                _emptyLineIndicators = value;
            }
        }

        public string GetFormattedConfig(bool incSensitive = false)
        {
//            return $@"[Name: {Name};
//Description: {Description};";
            return $@"[Name: {Name};
Description: {Description};
URL: {Url};
IsComposite: {IsComposite.ToString()};
SubsetHeader: {_subsetHeaderRegexStr};
IsRevisioned: {IsRevisioned.ToString()};
RevisionMatch: {_revisionRegexStr};
InvalidChars: {_invalidListnameCharsRegexStr};
EmptyLineIndicators: {_emptyLineIndicatorRegexStr};
LineSeparator: {LineSeparator};]";
        }

        public override ICreator Creator { 
            get => _creator;
            set => _creator = value;
        }
        public override string ToString() => GetFormattedConfig(false);
        public override bool Equals(object obj) => obj?.ToString() == ToString();
        public override int GetHashCode() => GetFormattedConfig(true).GetHashCode();

        public bool IsUpdateAvailable()
        {
            throw new NotImplementedException("nog niet");
        }

    }
    
    public class ListConfigFromStdInCreator : ICreator
    {
        public IRepositoryItem Create(string name)
        {
            return readFromSTDIN(name);
        }
        
        private ListConfig readFromSTDIN(string name=null)
        {
            ListConfig listConfig = new ListConfig();
            
            listConfig.Name = ConsoleHelper.ReadInput("name", name);
            listConfig.Description = ConsoleHelper.ReadInput("description");
            listConfig.Url = new Uri(ConsoleHelper.ReadInput("URL"));
            listConfig.IsComposite = ConsoleHelper.ReadInputAsBool("contains sublists (y/n)");
            if (listConfig.IsComposite)
            {
                listConfig.SubsetHeader = new Regex(ConsoleHelper.ReadInput("regex for subset name"));
            }
            listConfig.IsRevisioned = ConsoleHelper.ReadInputAsBool("is versioned (y/n");
            if (listConfig.IsRevisioned)
            {
                listConfig.RevisionRegex = new Regex(ConsoleHelper.ReadInput("regex for version number"));
                listConfig.UrlVersion = new Uri(ConsoleHelper.ReadInput("Version URL"));
            }

            return listConfig;
        }
    }

}