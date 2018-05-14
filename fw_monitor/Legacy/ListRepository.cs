//using System;
//using System.Collections.Generic;
//using System.IO;
//using fw_monitor.DataObjects;
//
//namespace fw_monitor
//{
//    public class ListRepository : Repository, IRepository
//    {
//        private Dictionary<string, IContentList> _repository = new Dictionary<string, IContentList>();
//        public override ICreator Creator { get; set; } = new ContentListFromStdInCreator();
//        
//        public ListRepository()
//        {
//            filenamePrefix = "list";
//        }
//
//        public ListRepository(IUtil util) : this()
//        {
//            this._util = util;
//        }
//
//        public override IRepositoryItem this[string key]
//        {
//            get => Get(key);
//            set => Set(value);
//        }
//
//        public IRepositoryItem Get(string key)
//        {
//            _repository.TryGetValue(key, out IContentList content);
//            if (content == null)
//            {
//                string path = getFilename(key);
//
//                string listStr = _util.ReadFromFile(path, false);
//                IContentList contentList = new ContentList().Deserialize(listStr);
//                _repository.Add(contentList.Name, contentList);
//            }
//            return content;
//        }
//
//        public override void Set(IRepositoryItem item)
//        {
//            if (item is ContentList contentList)
//            {
//                _repository[contentList.Name] = contentList;
//
//                if (SerializeToFile)
//                {
//                    string strContent = contentList.Serialize();
//                    writeToFile(getFilename(contentList.Name), strContent);
//                }
//            }
//            
//        }
//    }
//}