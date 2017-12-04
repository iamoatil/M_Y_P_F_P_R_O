using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vlc.DotNet.Wpf;

namespace XLY.SF.Project.Plugin.DataPreview.View
{
    /// <summary>
    /// MediaPlayerControl.xaml 的交互逻辑
    /// </summary>
    public partial class MediaPlayerControl : UserControl, IDataPreviewRelease
    {
        public MediaPlayerControl()
        {
            InitializeComponent();

            TimeSlider.LargeChange = 0.1;
            this.VerticalAlignment = VerticalAlignment.Stretch;
            this.Unloaded += PlayerUserControlVLC_Unloaded;

            _wpfVlcControl = new VlcControl();
            _player = _wpfVlcControl.MediaPlayer;
            string assemblyPath = Path.GetDirectoryName(this.GetType().Assembly.Location);  //当前程序集路径，C++库和程序集同一级目录
            _player.VlcLibDirectory = new DirectoryInfo(Path.Combine(assemblyPath, @"Libvlc\"));
            _player.EndInit();
            //设置新的一个
            MediaElementContainer.Children.Add(_wpfVlcControl);

            _player.LengthChanged += Player_LengthChanged;
            _player.MediaChanged += Player_MediaChanged; ;
            _player.TimeChanged += Player_TimeChanged;
            _player.PositionChanged += Player_PositionChanged;
        }

        private bool IsUserControl_Loaded = false;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsUserControl_Loaded)
            {//只加载一次
                IsUserControl_Loaded = true;

                DataPreviewPluginArgument arg = this.DataContext as DataPreviewPluginArgument;
                if (arg != null && arg.CurrentData is string fileName)
                {
                    if (File.Exists(fileName))
                    {
                        try
                        {
                            imgAudio.Visibility = IsAudioFormat ? Visibility.Visible : Visibility.Collapsed;
                            MediaElementContainer.Visibility = !IsAudioFormat ? Visibility.Visible : Visibility.Collapsed;
                            Open(fileName);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }
        }

        /// <summary>
        /// true为音频，false为视频
        /// </summary>
        public bool IsAudioFormat { get; set; }

        Uri _Openningfile;
        private Vlc.DotNet.Forms.VlcControl _player;
        private VlcControl _wpfVlcControl;

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

        private void TimeSlider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(TimeSlider);
            _player.Position = (float)(p.X / TimeSlider.ActualWidth);
        }

        public void Open(string path)
        {
            _Openningfile = new Uri(path);
            _isPause = false;
            //_player.Play(_Openningfile);
            //_player.Stop();
        }
        bool _isPause = false;
        private void Start_Click(object sender, RoutedEventArgs e)
        {
            if (_isPause)
                _player.Play();
            else
                _player.Play(_Openningfile);

            btnStart.Visibility = Visibility.Collapsed;
            btnPause.Visibility = Visibility.Visible;
            btnStop.Visibility = Visibility.Visible;
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            btnStart.Visibility = Visibility.Visible;
            btnPause.Visibility = Visibility.Collapsed;
            btnStop.Visibility = Visibility.Visible;
            _player.Pause();
            _isPause = true;
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            btnStart.Visibility = Visibility.Visible;
            btnPause.Visibility = Visibility.Collapsed;
            btnStop.Visibility = Visibility.Collapsed;

            _player.Stop();
            TimeSlider.Value = 0;
            _isPause = false;
        }

        public void Release()
        {
            _Openningfile = null;

            _player?.Stop();
        }
    }
}
