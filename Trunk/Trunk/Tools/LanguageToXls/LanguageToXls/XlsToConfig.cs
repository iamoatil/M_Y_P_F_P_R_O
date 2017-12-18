namespace LanguageToXls
{
    class XlsToConfig
    {
        string _codeDir;
        string _xlsPath;
        SourceCodeDir _sourceCodeDir = new SourceCodeDir();

        public void Initialize(string codeDir,string xlsPath)
        {
            _codeDir = codeDir;
            _xlsPath = xlsPath;
        }

        public void Write()
        {
            _sourceCodeDir.Initialize(_codeDir);
            _sourceCodeDir.Read();

            XlsFile xlsFile = new XlsFile(_xlsPath);
            xlsFile.Read(_sourceCodeDir);
        }

    }
}
