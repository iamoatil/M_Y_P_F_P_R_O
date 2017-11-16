using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;

namespace XLY.SF.Project.PreviewFilesView.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            NextCommand = new RelayCommand(Next);
            PreviousCommand = new RelayCommand(Previous);
            SetDirectoryCommand = new RelayCommand(new System.Action(() =>
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                DialogResult dr=dialog.ShowDialog();
                if(System.Windows.Forms.DialogResult.OK == dr)
                {
                    string[] files = Directory.GetFiles(dialog.SelectedPath, "*.*");
                    _fileCollection.Reset();
                    _fileCollection.AddPaths(files);
                    if(files.Length > 0)
                    {
                        previewControl.ReplaceContent(files[0]);
                    }                    
                }
            }));

            InitializeComponent();

            string dir = @"C:\Test\Pictures\视频音频";
            if (!Directory.Exists(dir))
            {
                dir = @"./";
            }
            string[] filesPath = Directory.GetFiles(dir, "*.*");
            _fileCollection.AddPaths(filesPath);
            if(filesPath.Length > 0)
            {
                previewControl.ReplaceContent(filesPath[0]);
            }            
        }        

        public ICommand NextCommand { get; private set; }

        public ICommand PreviousCommand { get; private set; }

        /// <summary>
        /// 设置目录
        /// </summary>
        public ICommand SetDirectoryCommand { get; private set; }

        private readonly PathCollection _fileCollection = new PathCollection();

        private void Next()
        {
            string filePath = _fileCollection.GetNextPath();
            if(filePath != null)
            {
                previewControl.ReplaceContent(filePath);
                this.Title = filePath;
            }           
        }

        private void Previous()
        {
            string filePath = _fileCollection.GetPreviousPath();
            if(filePath != null)
            {
                previewControl.ReplaceContent(filePath);
                this.Title = filePath;
            }            
        }
    }
}
