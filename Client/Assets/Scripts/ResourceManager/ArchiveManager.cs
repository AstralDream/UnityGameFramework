using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

namespace Game.Resource
{

    // 将所有资源结构以xml形式存放起来，name是资源存储路径，type是文件类型
    // 包括音乐，模型，场景等所有资源
    // 他只是对于资源类型与路径的存储
    class ArchiveManager : Singleton<ArchiveManager>
    {
        internal Dictionary<string, Archive> mAllArchives;

        public ArchiveManager()
        {
            mAllArchives = new Dictionary<string, Archive>();
        }

        public void Init()
        {
            StreamReader sr = ResourcesManager.OpenText("Resource");
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sr.ReadToEnd());
            XmlElement root = doc.DocumentElement;
            IEnumerator iter = root.GetEnumerator();
            while (iter.MoveNext())
            {
                XmlElement child_root = iter.Current as XmlElement;
                IEnumerator child_iter = child_root.GetEnumerator();
                if (!mAllArchives.ContainsKey(child_root.Name))
                {
                    Archive arh = new Archive();
                    mAllArchives.Add(child_root.Name, arh);
                }
                while (child_iter.MoveNext())
                {
                    XmlElement file = child_iter.Current as XmlElement;
                    string name = file.GetAttribute("name");
                    string type = file.GetAttribute("type");
                    mAllArchives[child_root.Name].add(name, type);
                }
            }
            sr.Close();
        }

        public string getPath(string archiveName, string fileName)
        {
            if (mAllArchives.ContainsKey(archiveName))
                return mAllArchives[archiveName].getPath(fileName);
            else
                DebugEx.LogError("can not find " + archiveName, ResourceCommon.DEBUGTYPENAME);
            return null;
        }

        //public void dumpAllArchives()
        //{
        //    foreach (KeyValuePair<string, Archive> a in mAllArchives)
        //    {
        //        Debug.Log(" the archive is : " + a.Key);
        //        a.Value.dumpAllFiles();
        //    }
        //}
    }
}

