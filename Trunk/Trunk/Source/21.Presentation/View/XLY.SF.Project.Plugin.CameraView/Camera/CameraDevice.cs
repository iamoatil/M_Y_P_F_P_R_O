using System;
using AForge.Controls;
using AForge.Video.DirectShow;


namespace XLY.SF.Project.CameraView
{
    class CameraDevice
    {
        /// <summary>
        /// 设备
        /// </summary>
        private VideoCaptureDevice _device;

        private bool _isInitialized;

        /// <summary>
        /// 是否已经连接到了Player
        /// </summary>
        public bool IsConnectedToPlayer { get; private set; }

        /// <summary>
        /// 设备名字
        /// </summary>
        public string Name { get; private set; }

        public CameraDevice(string deviceName)
        {
            Name = deviceName;
            _device = new VideoCaptureDevice(deviceName);
            _isInitialized = true;
        }

        /// <summary>
        /// 把Player连接到指定的摄像头设备上
        /// </summary>
        /// <param name="device"></param>
        public void ConnnectDevice(VideoSourcePlayer videoSourcePlayer)
        {
            if(!_isInitialized)
            {
                return;
            }
            videoSourcePlayer.VideoSource = _device;
            IsConnectedToPlayer = true;
        }

        public void DisconnnectDevice(VideoSourcePlayer videoSourcePlayer)
        {
            if(!_isInitialized)
            {
                return;
            }
            videoSourcePlayer.VideoSource = null;
            IsConnectedToPlayer = false;
        }
    }
}
