using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Vlc.DotNet.Wpf;

namespace XLY.SF.Project.PreviewFilesView.UI
{
    /// <summary>
    /// AudioUserControl.xaml 的交互逻辑
    /// </summary>
    public partial class PlayerUserControlVLC : UserControl
    {
        Uri _Openningfile;
        private Vlc.DotNet.Forms.VlcControl _player;
        private VlcControl _wpfVlcControl;

        public PlayerUserControlVLC()
        {
            InitializeComponent();
            TimeSlider.LargeChange = 0.1;
            this.VerticalAlignment = VerticalAlignment.Stretch;
            this.Unloaded += PlayerUserControlVLC_Unloaded;

            _wpfVlcControl = new VlcControl();
            _player = _wpfVlcControl.MediaPlayer;
            AppDir appDir = new AppDir();
            _player.VlcLibDirectory = new DirectoryInfo(Path.Combine(appDir.MainDir + @"RefDlls\Libvlc\"));
            _player.EndInit();
            //设置新的一个
            MediaElementContainer.Children.Add(_wpfVlcControl);
            
            _player.LengthChanged += Player_LengthChanged;
            _player.MediaChanged += Player_MediaChanged; ;
            _player.TimeChanged += Player_TimeChanged;
            _player.PositionChanged += Player_PositionChanged;
        }

        private void Player_LengthChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerLengthChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() => { TotalTime.Text = TimeSpan.FromMilliseconds(_player.Length).ToString(@"hh':'mm':'ss"); });
        }

        private void Player_MediaChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerMediaChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() => { Title.Text = e.NewMedia.Title; });
        }

        private void Player_PositionChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerPositionChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() => { TimeSlider.Value = TimeSlider.Maximum * e.NewPosition; });
        }

        private void Player_TimeChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerTimeChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() => { StartTime.Text = TimeSpan.FromMilliseconds(e.NewTime).ToString("hh':'mm':'ss"); });
        }

        private void _player_TitleChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerTitleChangedEventArgs e)
        {
            this.Dispatcher.Invoke(() => { Title.Text = e.NewTitle; });
        }        

        private void PlayerUserControlVLC_Unloaded(object sender, RoutedEventArgs e)
        {
            //停止上一个
            if (_player != null)
            {
                _player.Stop();
            }
        }   

        private void TimeSlider_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(TimeSlider);
            _player.Position = (float)(p.X / TimeSlider.ActualWidth);
        }

        public void Open(string path)
        {
            _Openningfile = new Uri(path);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            _player.Play(_Openningfile);
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            Pause.Visibility = Visibility.Collapsed;
            Continue.Visibility = Visibility.Visible;
            _player.Pause();
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            Pause.Visibility = Visibility.Visible;
            Continue.Visibility = Visibility.Collapsed;
            _player.Pause();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            _player.Stop();
            TimeSlider.Value = 0;
        }
    }
}
