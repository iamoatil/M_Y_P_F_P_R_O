using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Vlc.DotNet.Wpf;

namespace XLY.SF.Project.UserControls.PreviewFile.UserControls.PlayerControl
{
    /// <summary>
    /// PlayerUserControlVLC.xaml 的交互逻辑
    /// </summary>
    public partial class PlayerUserControlVLC : UserControl
    {
        public PlayerUserControlVLC()
        {
            InitializeComponent();
            TimeSlider.LargeChange = 0.1;
            this.VerticalAlignment = VerticalAlignment.Stretch;
            this.Unloaded += PlayerUserControlVLC_Unloaded;
            this.TimeSlider.ValueChanged += Slider_ValueChanged;

            //VlcMediaPlayer初始化
            this.MyControl.MediaPlayer.VlcLibDirectory = new DirectoryInfo(Environment.CurrentDirectory + @"\RefDlls\libvlc");
            var options = new string[]
           {
               // VLC options can be given here. Please refer to the VLC command line documentation.
           };
            this.MyControl.MediaPlayer.VlcMediaplayerOptions = options;
            // Load libvlc libraries and initializes stuff. It is important that the options (if you want to pass any) and lib directory are given before calling this method.
            this.MyControl.MediaPlayer.EndInit();

            MyControl.MediaPlayer.TimeChanged += MediaPlayer_TimeChanged;
            MyControl.MediaPlayer.Playing += MediaPlayer_Playing;
        }        

        private void MediaPlayer_Playing(object sender, Vlc.DotNet.Core.VlcMediaPlayerPlayingEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                TotalTime.Text = TimeSpan.FromMilliseconds(MyControl.MediaPlayer.Length).ToString("hh':'mm':'ss");
                StartTime.Text = TimeSpan.FromMilliseconds(this.MyControl.MediaPlayer.Time).ToString("hh':'mm':'ss");

                TimeSlider.Maximum = MyControl.MediaPlayer.Length;
                TimeSlider.Minimum = 0;
            }));            
        }

        private void MediaPlayer_TimeChanged(object sender, Vlc.DotNet.Core.VlcMediaPlayerTimeChangedEventArgs e)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                StartTime.Text = TimeSpan.FromMilliseconds(this.MyControl.MediaPlayer.Time).ToString("hh':'mm':'ss");
                TimeSlider.Value = this.MyControl.MediaPlayer.Time;
            }));
        }

        /// <summary>
        /// 装载文件到播放器
        /// </summary>
        /// <param name="path"></param>
        public void LoadFile(string path)
        {
            this.MyControl.MediaPlayer.SetMedia(new Uri(path));
            this.Title.Text = path;
            TotalTime.Text = "00:00:00";
            StartTime.Text = "00:00:00";

            TimeSlider.Maximum = 0;
            TimeSlider.Minimum = 0;
        }

        private void PlayerUserControlVLC_Unloaded(object sender, RoutedEventArgs e)
        {
            //停止上一个
            if (MyControl != null)
            {
                MyControl.MediaPlayer.Stop();
            }
        }   

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            MyControl.MediaPlayer.Time =(long)TimeSlider.Value;
        }
        
        private void TimeSlider_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point p = e.GetPosition(TimeSlider);
            TimeSlider.Value = p.X / TimeSlider.ActualWidth * TimeSlider.Maximum;
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            MyControl.MediaPlayer.Play();
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            MyControl.MediaPlayer.Pause();
        }

        private void Stop_Click(object sender, RoutedEventArgs e)
        {
            MyControl.MediaPlayer.Stop();
        }        
    }
}
