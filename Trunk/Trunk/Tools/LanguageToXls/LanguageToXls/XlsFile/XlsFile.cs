using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LanguageToXls
{
    class XlsFile
    {
        private List<XlsRow> _xlsRows = new List<XlsRow>();
        
        /// <summary>
        /// Xls文件的路径
        /// </summary>
        public string Path { get;private set; }

        public List<XlsRow> XlsRows { get { return _xlsRows; } }

        public XlsFile(string path)
        {
            Path = path;
        }

        public void Write(SourceCodeDir sourceCodeDir)
        {
            List<LanguageDir> langDirs = sourceCodeDir.LanguageDirs.Values.ToList();
            
            foreach (var langDir in langDirs)
            {
                if (langDir.LanguageFileDic.Keys.Contains(LanguageFile.EnName)
                 && langDir.LanguageFileDic.Keys.Contains(LanguageFile.CnName))
                {
                    LanguageFile cnFile = langDir.LanguageFileDic[LanguageFile.CnName];
                    LanguageFile enFile = langDir.LanguageFileDic[LanguageFile.EnName];                
                    
                    foreach (var word in cnFile.LanguageWordDic.Values)
                    {
                       XlsRow row = new XlsRow(word.VirtualPath, word.Content, enFile.LanguageWordDic[word.VirtualPath].Content, langDir.RelativePath);
                       XlsRows.Add(row);
                    }
                }
            }

            List<string> ls = new List<string>();
            foreach (var item in XlsRows)
            {
                ls.Add(item.ToString());
            }

           File.WriteAllLines(Path,ls, Encoding.Default);
           
        }

        public void Read(SourceCodeDir sourceCodeDir)
        {
            //把xls数据读取成XlsRows列表
            string[] rows=File.ReadAllLines(Path);
            foreach (var row in rows)
            {
                XlsRow xlsRow = XlsRow.Parser(row);
                if(xlsRow != null)
                {
                    XlsRows.Add(xlsRow);
                }
            }
            //把XlsRows列表写入Config中
            List<string> modifyList = new List<string>();
            foreach (var row in XlsRows)
            {
                string lanDir = row.LangDir;
                string virtualPath = row.VirtualPath;
                string chinese = row.ChineseString;
                string foreign = row.ForeignString;              
                //若sourceCodeDir中不存在Xls中的某条LanguageDir
                if (!sourceCodeDir.LanguageDirs.Keys.Contains(lanDir))
                {
                    modifyList.Add(row.ToString()+"\t 语言目录不匹配");
                    continue;
                }
                LanguageDir languageDir = sourceCodeDir.LanguageDirs[lanDir];                
              
                LanguageFile cnFile = languageDir.LanguageFileDic[LanguageFile.CnName];                
                if (!cnFile.LanguageWordDic.Keys.Contains(virtualPath))
                {
                    modifyList.Add(row.ToString() + "\t VirtualPath不匹配");
                    continue;
                }
                LanguageWord cnWord= cnFile.LanguageWordDic[virtualPath];
                if (cnWord.Content != chinese)
                {
                    modifyList.Add(row.ToString() + "\t 中文不匹配");
                    continue;
                }
                LanguageFile enFile = languageDir.LanguageFileDic[LanguageFile.EnName];
                if(!enFile.LanguageWordDic.Keys.Contains(virtualPath))
                {
                    LanguageWord word= new LanguageWord();
                    word.Initialize(virtualPath, row.ForeignString);
                    enFile.LanguageWordDic.Add(virtualPath, word);
                }
                else
                {
                    LanguageWord word= enFile.LanguageWordDic[virtualPath];
                    word.Initialize(virtualPath, row.ForeignString);
                }
            }
            sourceCodeDir.Write();
            //保存修改的数据
            File.WriteAllLines("Modify.xls",modifyList);
            //清空此次数据
            XlsRows.Clear();
            sourceCodeDir.LanguageDirs.Clear();
        }
    }
}
