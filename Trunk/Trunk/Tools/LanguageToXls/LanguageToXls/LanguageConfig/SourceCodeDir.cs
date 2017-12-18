using System.Collections.Generic;
using System.IO;

namespace LanguageToXls
{
    class SourceCodeDir
    {
        private const string SouceCodeTopDir = @"\Trunk\Source\";
        
        /// <summary>
        /// 是否已经初始化。只有初始化成功之后才执行其他逻辑
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 源代码主目录 绝对路径
        /// </summary>
        public string AbsolutePath { get; private set; }

        /// <summary>
        /// 相对路径在绝对路径中的起始索引
        /// </summary>
        private int _relativePathStartedIndex = 0;

        /// <summary>
        /// 源代码主目录 相对路径
        /// </summary>
        public string RelativePath
        {
            get
            {
                if (!IsInitialized)
                {
                    return string.Empty;
                }
                else
                {
                    return AbsolutePath.Substring(_relativePathStartedIndex);
                }
            }
        }

        /// <summary>
        /// 语言目录列表
        /// </summary>
        public Dictionary<string,LanguageDir> LanguageDirs { get { return _languageDirs; } }
        private Dictionary<string, LanguageDir> _languageDirs = new Dictionary<string, LanguageDir>();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public bool Initialize(string dir)
        {
            IsInitialized = InnerInitialize(dir);
            return IsInitialized;
        }

        private bool InnerInitialize(string dir)
        {
            //验证源代码目录的有效性
            dir = UtilityPath.ToStandardDirectory(dir);

            if (!Directory.Exists(dir))
            {
                return false;
            }

            AbsolutePath = dir;
            _relativePathStartedIndex = dir.IndexOf(SouceCodeTopDir);
            if(_relativePathStartedIndex < 1)
            {
                return false;
            }

            List<string> langDirs = GetLanguageDirs(dir);
            foreach (var langDir in langDirs)
            {
                LanguageDir languageDir = new LanguageDir();
                languageDir.Initialize(langDir,_relativePathStartedIndex);
                if (languageDir.IsInitialized)
                {
                    LanguageDirs.Add(langDir,languageDir);
                }
            }

            return true;
        }

        /// <summary>
        /// 获取源代码目录下所有包含多语言的文件夹
        /// </summary>
        internal List<string> GetLanguageDirs(string dir)
        {
            string[] projFiles = Directory.GetFiles(dir, "*.csproj", SearchOption.AllDirectories);
            List<string> lanDirs = new List<string>();
            foreach (var projFile in projFiles)
            {
                string projDir = Path.GetDirectoryName(projFile);
                projDir = Path.GetFullPath(projDir);
                if (!projDir.EndsWith("\\"))
                {
                    projDir += "\\";
                }
                string lanDir = projDir + "Language\\";
                if (!Directory.Exists(lanDir))
                {
                    continue;
                }

                lanDirs.Add(lanDir);
            }
            return lanDirs;
        }
        
        public void Read()
        {
            foreach (var item in LanguageDirs.Values)
            {
                item.Read();
            }
        }
        public void Write()
        {
            foreach (var item in LanguageDirs.Values)
            {
                item.Write();
            }
        }
    }
}
