using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace LanguageToXls
{
    class ViewModel:INotifyPropertyChanged
    {
        public ViewModel()
        {
            _readCommand = new RelayCommand(Read);
            _writeCommand = new RelayCommand(Write);
            _setCodeDirCommand = new RelayCommand(SetCodeDir);
            _setLoadXlsCommand=new RelayCommand(SetLoadXls);
        }        

        public ICommand ReadCommand{ get { return _readCommand; } }
        private ICommand _readCommand ;

        public ICommand WriteCommand { get { return _writeCommand; } }
        private ICommand _writeCommand;

        public ICommand SetCodeDirCommand { get { return _setCodeDirCommand; } }

        private ICommand _setCodeDirCommand;

        public ICommand SetLoadXlsCommand { get { return _setLoadXlsCommand; } }

        private ICommand _setLoadXlsCommand;

        ConfigToXls _configToXls = new ConfigToXls();
        XlsToConfig _xlsToConfig = new XlsToConfig();

        public string XlsSavePath
        {
            get
            {
                return _xlsSavePath;
            }
            set
            {
                _xlsSavePath = value;
                OnPropertyChanged();
            }
        }
        private string _xlsSavePath = @"d:\test.xls";

        public string XlsLoadPath
        {
            get
            {
                return _xlsLoadPath;
            }
            set
            {
                _xlsLoadPath = value;
                OnPropertyChanged();
            }
        }
        private string _xlsLoadPath = @"d:\test.xls";
        public string CodeDir
        {
            get { return _codeDir; }
            set
            {
                _codeDir = value;
                OnPropertyChanged();
            }
        }
        private string _codeDir= @"D:\SourceCode\XLY\SPF-PRO\Trunk\Trunk\Source\";

        private void Read()
        {
            if(Directory.Exists(CodeDir))
            {
                _configToXls.Initialize(CodeDir, XlsSavePath);
                _configToXls.Read();
            }  
            else
            {
                MessageBox.Show($"源代码目录：“{CodeDir}” 不存在！");
            }
        }

        private void Write()
        {
            if (Directory.Exists(CodeDir))
            {
                _xlsToConfig.Initialize(CodeDir, XlsLoadPath);
                _xlsToConfig.Write();
            }
            else
            {
                MessageBox.Show($"{CodeDir} 不存在！");
            }
        }

        private void SetCodeDir()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();           
            DialogResult dr = folderBrowserDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                CodeDir = folderBrowserDialog.SelectedPath;
            }
        }

        private void SetLoadXls()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.RestoreDirectory = true;
            DialogResult dr = openFileDialog.ShowDialog();
            if (dr == DialogResult.OK)
            {
                XlsLoadPath = openFileDialog.FileName;
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// 属性更新（不用给propertyName赋值）
        /// </summary>
        /// <param name="propertyName"></param>
        public void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
