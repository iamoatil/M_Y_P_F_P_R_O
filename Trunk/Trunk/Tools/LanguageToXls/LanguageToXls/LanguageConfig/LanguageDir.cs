using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageToXls
{
    class LanguageDir
    {
        /// <summary>
        /// 是否已经初始化。只有初始化成功之后才执行其他逻辑
        /// </summary>
        public bool IsInitialized { get; private set; }
        
        /// <summary>
        /// 绝对路径
        /// </summary>
        public string AbsolutePath { get; private set; }

        /// <summary>
        /// 相对路径在绝对路径中的起始索引
        /// </summary>
        private int _relativePathStartedIndex = 0;

        /// <summary>
        ///  相对路径
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
        /// 源代码中所有的语言文件
        /// </summary>
        public Dictionary<string,LanguageFile> LanguageFileDic
        {
            get { return _languageFileDic; }
        }
        private Dictionary<string, LanguageFile> _languageFileDic = new Dictionary<string, LanguageFile>();        

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public bool Initialize(string dir,int startedIndex)
        {
            _relativePathStartedIndex = startedIndex;
            IsInitialized = InnerInitialize(dir);
            AbsolutePath = dir;
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

            //获取中文配置文件
            string file = GetCnFile(dir);
            LanguageFile languageFile = new LanguageFile(LanguageFile.CnName);
            languageFile.Initialize(file, _relativePathStartedIndex);
            if (languageFile.IsInitialized)
            {
                LanguageFileDic.Add(languageFile.Name, languageFile);
            }
            //获取英文配置文件
            file = GetEnFile(dir);
            languageFile = new LanguageFile(LanguageFile.EnName);
            languageFile.Initialize(file, _relativePathStartedIndex);
            if (languageFile.IsInitialized)
            {
                LanguageFileDic.Add(languageFile.Name, languageFile);
            }

            return true;
        }

        /// <summary>
        /// 获取中文描述文件
        /// </summary>
        internal string GetCnFile(string dir)
        {
            string cnFile = dir + LanguageFile.CnName;
            if(!File.Exists(cnFile))
            {
                return "Unknow";
            }
            return cnFile;
        }

        /// <summary>
        /// 获取英文描述文件
        /// </summary>
        internal string GetEnFile(string dir)
        {
            string enFile = dir + LanguageFile.EnName;
            if (!File.Exists(enFile))
            {
                return "Unknow";
            }
            return enFile;
        }

        internal void Read()
        {
            if(_languageFileDic.Keys.Contains(LanguageFile.EnName)
                && _languageFileDic.Keys.Contains(LanguageFile.CnName))
            {
                LanguageFile cnFile = _languageFileDic[LanguageFile.CnName];
                LanguageFile foreignFile = _languageFileDic[LanguageFile.EnName];
                cnFile.ReadAllWord();
                foreignFile.ReadAllWord();
                foreignFile.Update(cnFile);
            }
        }

        internal void Write()
        {
            if (_languageFileDic.Keys.Contains(LanguageFile.EnName))
            {
                LanguageFile foreignFile = _languageFileDic[LanguageFile.EnName];
                foreignFile.WriteAllWord();
            }
        }
    }

}
