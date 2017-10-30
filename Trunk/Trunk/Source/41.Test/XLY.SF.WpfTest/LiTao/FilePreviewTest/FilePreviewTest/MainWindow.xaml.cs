using System.IO;
using System.Windows;
using System.Windows.Input;
using TestHelper;
using XLY.SF.Project.PreviewFiles.Language;
using XLY.SF.Project.UserControls.PreviewFile;

namespace FilePreviewTest
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

            InitializeComponent();

            string dir = @"C:\Test\Pictures\";
            if (!Directory.Exists(dir))
            {
                dir = @"./";
            }
            string[] filesPath = Directory.GetFiles(dir, "*.*");
            _fileCollection.AddPaths(filesPath);
            previewControl.ReplaceContent(filesPath[0]);
        }        

        public ICommand NextCommand { get; private set; }

        public ICommand PreviousCommand { get; private set; }

        private readonly PathCollection _fileCollection = new PathCollection();

        private void Next()
        {
            string filePath = _fileCollection.GetNextPath();
            previewControl.ReplaceContent(filePath);
            this.Title = filePath;
        }

        private void Previous()
        {
            string filePath = _fileCollection.GetPreviousPath();
            previewControl.ReplaceContent(filePath);
            this.Title = filePath;
        }

        private bool _isChinese;
        private void ChangeLanguage_Click(object sender, RoutedEventArgs e)
        {
            if (_isChinese)
            {
                LanguageHelperSingle.Instance.SwitchLanguage((LanguageType)1);
            }
            else
            {
                LanguageHelperSingle.Instance.SwitchLanguage((LanguageType)0);
            }
            _isChinese = !_isChinese;
        }
    }
}
