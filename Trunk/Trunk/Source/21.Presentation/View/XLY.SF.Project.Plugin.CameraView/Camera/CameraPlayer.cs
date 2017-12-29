using AForge.Controls;
using ProjectExtend.Context;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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

        /// <summary>
        /// 是否刷新设备停止
        /// </summary>
        private bool _isRefreshStop;
        
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

            CameraDeviceManager cameraDeviceManager = new CameraDeviceManager();
            Task.Run((() => { while (!_isRefreshStop) { RefreshDevice(cameraDeviceManager); Thread.Sleep(1000); } }));

            return true;
        }

        /// <summary>
        ///  刷新界面
        /// </summary>
        public void RefreshDevice(CameraDeviceManager cameraDeviceManager)
        {
            
            bool isChanged = cameraDeviceManager.DetectState();
            if (isChanged)
            {
                if (cameraDeviceManager.DefaultCameraDevice != null)
                {
                    _currentCameraDevice = cameraDeviceManager.DefaultCameraDevice;

                    //SystemContext.Instance.AsyncOperation.Post(state =>
                    //{
                    //    this.Child = _videoSourcePlayerHost;
                    //    Start();
                    //},null);
                    AppThread.Instance.Invoke(()=>
                    {
                        this.Child = _videoSourcePlayerHost;
                        Start();
                    });                    
                }
                else
                {
                    _currentCameraDevice = null;
                    //SystemContext.Instance.AsyncOperation.Post(state =>
                    //{
                    //    this.Child = _defaultImage;
                    //}, null);
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
                && !_currentCameraDevice.IsConnectedToPlayer)
            {
                _currentCameraDevice.ConnnectDevice(_videoSourcePlayer);
                _videoSourcePlayer.Start();
            }
        }

        /// <summary>
        /// 摄像头停止捕获
        /// </summary>
        public void Stop()
        {
            _isRefreshStop = true;

            if (_currentCameraDevice != null
               && _currentCameraDevice.IsConnectedToPlayer)
            {
                _videoSourcePlayer.Stop();
                _currentCameraDevice.DisconnnectDevice(_videoSourcePlayer);
            }           
        }

        /// <summary>
        /// 拍照
        /// </summary>
        /// <param name="dir"></param>
        public void TakePhoto(string path)
        {
            string dir = Path.GetDirectoryName(path);
            //修整目录
            if(!dir.EndsWith("\\"))
            {
                dir += "\\";
            }
            if(!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            try
            {
                if ((_videoSourcePlayer != null) && (_videoSourcePlayer.IsRunning))
                {
                    BitmapSource image = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                _videoSourcePlayer.GetCurrentVideoFrame().GetHbitmap(),
                                IntPtr.Zero,
                                Int32Rect.Empty,
                                BitmapSizeOptions.FromEmptyOptions());

                    BitmapEncoder encoder = new JpegBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    MemoryStream ms = new MemoryStream();
                    encoder.Save(ms);
                    // 剪切图片

                    System.Drawing.Image initImage = System.Drawing.Image.FromStream(ms, true);

                    //对象实例化
                    System.Drawing.Bitmap pickedImage = new System.Drawing.Bitmap((int)image.PixelWidth, (int)image.PixelHeight);
                    System.Drawing.Graphics pickedG = System.Drawing.Graphics.FromImage(pickedImage);
                    //设置质量
                    pickedG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    pickedG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    //定位
                    System.Drawing.Rectangle fromR = new System.Drawing.Rectangle(0, 0, (int)image.PixelWidth, (int)image.PixelHeight);
                    System.Drawing.Rectangle toR = new System.Drawing.Rectangle(0, 0, (int)image.PixelWidth, (int)image.PixelHeight);
                    //画图
                    pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);

                    pickedImage.Save(path);

                    // 释放资源 

                    ms.Close();
                    pickedImage.Dispose();
                    pickedG.Dispose();

                }
            }
            catch (Exception ex)
            {
            }
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
