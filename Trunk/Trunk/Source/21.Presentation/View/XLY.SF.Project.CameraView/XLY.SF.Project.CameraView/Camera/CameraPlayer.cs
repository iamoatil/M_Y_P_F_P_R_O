using AForge.Controls;
using System;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Media.Imaging;

namespace XLY.SF.Project.CameraView
{
    /// <summary>
    /// 摄像头播放器控件
    /// 当播放器为拍照时，显示默认的照片；否则显示摄像头的内容
    /// </summary>
    class CameraPlayer:Border,IDisposable
    {     
        /// <summary>
        /// 视频播放器
        /// </summary>
        private VideoSourcePlayer _videoSourcePlayer;

        /// <summary>
        /// 视频播放器的 WPF承载器
        /// </summary>
        private WindowsFormsHost _videoSourcePlayerHost;

        /// <summary>
        /// 默认图片
        /// </summary>
        private Image _defaultImage;

        /// <summary>
        /// 当前摄像头设备
        /// </summary>
        private CameraDevice _currentCameraDevice;

        Thread _thread;

        /// <summary>
        /// 是否已经完成初始化
        /// </summary>
        public bool IsInitialized2 { get; private set; }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            IsInitialized2 = InnerInitialize();
        }        

        private bool InnerInitialize()
        {
            if(IsInitialized2)
            {
                return true ;
            }
            _videoSourcePlayer = new VideoSourcePlayer();
            _videoSourcePlayerHost = new WindowsFormsHost();
            _videoSourcePlayerHost.Child = _videoSourcePlayer;

            _defaultImage = new Image();
            _defaultImage.Source = new BitmapImage(new Uri(@"/Resource/no-camera.png", UriKind.Relative));

            _thread = new Thread(state => { RefreshDevice(); Thread.Sleep(1000); });
            _thread.IsBackground = true;
            _thread.Start();

            RefreshDevice();
            return true;
        }

        /// <summary>
        ///  刷新界面
        /// </summary>
        public void RefreshDevice()
        {
            CameraDeviceManager cameraDeviceManager = new CameraDeviceManager();
            bool isChanged = cameraDeviceManager.DetectState();
            if (isChanged)
            {
                if (cameraDeviceManager.DefaultCameraDevice != null)
                {
                    _currentCameraDevice = cameraDeviceManager.DefaultCameraDevice;

                    AppThread.Instance.Invoke(() =>
                    {
                        _currentCameraDevice.ConnnectDevice(_videoSourcePlayer);
                        this.Child = _videoSourcePlayerHost;
                        Start();
                    });                    
                }
                else
                {
                    _currentCameraDevice = null;
                    AppThread.Instance.Invoke(() => { this.Child = _defaultImage; });
                }
            }
        }

        /// <summary>
        /// 摄像头开始捕获
        /// </summary>
        public void Start()
        {
            if(_currentCameraDevice != null
                && _currentCameraDevice.IsConnectedToPlayer)
            {
                _videoSourcePlayer.Start();
            }
        }

        /// <summary>
        /// 摄像头停止捕获
        /// </summary>
        public void Stop()
        {
            //停止摄像头设备
            //呈现默认图片
        }

        #region IDisposable
        //是否回收完毕
        bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~CameraPlayer()
        {
            Dispose(false);
        }

        //这里的参数表示示是否需要释放那些实现IDisposable接口的托管对象
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return; //如果已经被回收，就中断执行
            }
            if (disposing)
            {
                IsInitialized2 = false;
                //TODO:释放那些实现IDisposable接口的托管对象
            }
            //TODO:释放非托管资源，设置对象为null
            _disposed = true;
        }
        #endregion
    }
}
