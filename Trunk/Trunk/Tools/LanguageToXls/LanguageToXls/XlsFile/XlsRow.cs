using System;

namespace LanguageToXls
{
    class XlsRow
    {
        public XlsRow(string virtualPath, string chineseString, string foreignString,string langDir)
        {
            VirtualPath = virtualPath;
            ChineseString = chineseString;
            ForeignString = foreignString;
            this.LangDir = langDir;
        }

        /// <summary>
        /// 汉语字符串
        /// </summary>
        public string ChineseString { get; private set; }

        /// <summary>
        /// 外语字符串
        /// </summary>
        public string ForeignString { get; private set; }

        /// <summary>
        /// 虚拟路径
        /// </summary>
        public string VirtualPath { get; private set; }

        /// <summary>
        /// 语言文件的目录
        /// </summary>
        public string LangDir { get; private set; }

        public override string ToString()
        {
            string str= ChineseString + "\t" + (ForeignString==string.Empty?" ": ForeignString )+ "\t" + VirtualPath+"\t"+ LangDir;
            return str;
        }

        /// <summary>
        /// 解析一个字符串生成一个XlsRow对象
        /// </summary>
        internal static XlsRow Parser(string rowStr)
        {
            string[] items=rowStr.Split('\t');
            if(items.Length == 4)
            {
                XlsRow row = new XlsRow(items[0], items[1], items[2], items[3]);
                return row;
            }
            return null;
        }
    }
}
