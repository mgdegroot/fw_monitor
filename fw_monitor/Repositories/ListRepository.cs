using System;
using System.Collections.Generic;
using System.IO;
using fw_monitor.DataObjects;

namespace fw_monitor
{
    public class ListRepository : Repository, IRepository
    {
        private Dictionary<string, IContentList> _repository = new Dictionary<string, IContentList>();
        public override ICreator Creator { get; set; } = new ContentListFromStdInCreator();
        
        public ListRepository()
        {
            filenamePrefix = "list";
            
        }

        public ListRepository(IUtil util) : this()
        {
            this._util = util;
        }

        public override IRepositoryItem this[string key]
        {
            get => Get(key);
            set => Set(value);
        }

        public IRepositoryItem Get(string key)
        {
            _repository.TryGetValue(key, out IContentList content);
            if (content == null)
            {
                string path = getFilename(key);

                string listStr = _util.ReadFromFile(path, false);
                IContentList contentList = new ContentList().Deserialize(listStr);
                _repository.Add(contentList.Name, contentList);
            }
            return content;
        }

        public override void Set(IRepositoryItem item)
        {
            if (item is ContentList contentList)
            {
                _repository[contentList.Name] = contentList;

                if (SerializeToFile)
                {
                    string strContent = contentList.Serialize();
                    writeToFile(getFilename(contentList.Name), strContent);
                }
            }
            
        }
    }

    public class ContentListFromStdInCreator : ICreator
    {
        public IRepositoryItem Create(string name)
        {
            return readFromSTDIN(name);
        }
        
        
        private ContentList readFromSTDIN(string name)
        {
            ContentList contentList = new ContentList() { Name=name, };
            contentList.Name = ConsoleHelper.ReadInput("name", contentList.Name);
            contentList.Version = ConsoleHelper.ReadInput("version", contentList.Version);
            contentList.IsSubList = ConsoleHelper.ReadInputAsBool("is sublist (y/n)", contentList.IsSubList ? "y" : "n");
            bool addElems = ConsoleHelper.ReadInputAsBool("enter elements (y/n)", "y");
            
            while (addElems)
            {
                string elem = ConsoleHelper.ReadInput("element (when done hit <enter>)");
                addElems = !string.IsNullOrEmpty(elem);
                if (!string.IsNullOrEmpty(elem))
                {
                    contentList.Add(elem);
                }
            }

            return contentList;
        }
    }
}