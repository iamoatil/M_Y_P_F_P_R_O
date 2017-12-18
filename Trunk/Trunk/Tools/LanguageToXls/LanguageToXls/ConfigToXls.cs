using System.Collections.Generic;
using System.Linq;

namespace LanguageToXls
{
    class ConfigToXls
    {
        string _codeDir;
        string _xlsPath;
        SourceCodeDir _sourceCodeDir= new SourceCodeDir();

        public void Initialize(string codeDir,string xlsPath)
        {
            _codeDir = codeDir;
            _xlsPath = xlsPath;
        }

        public void Read()
        {
            _sourceCodeDir.Initialize(_codeDir);
            _sourceCodeDir.Read();
            XlsFile xlsFile = new XlsFile(_xlsPath);
            xlsFile.Write(_sourceCodeDir);
        }

    }
}
