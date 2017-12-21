using AForge.Video.DirectShow;
using System.Collections.Generic;
using System.Linq;

namespace XLY.SF.Project.CameraView
{
    /// <summary>
    ///  摄像头设备管理
    /// </summary>
    class CameraDeviceManager
    {
        /// <summary>
        /// 检测到的所有摄像头
        /// </summary>
        public readonly List<CameraDevice> CameraDeviceList = new List<CameraDevice>();
        
        /// <summary>
        /// 默认摄像头设备
        /// </summary>
        public CameraDevice DefaultCameraDevice { get; private set; }

        /// <summary>
        /// 检测状态变化
        /// </summary>
        /// <returns></returns>
        public bool DetectState()
        {
            bool isChanged = InnerDetectState();
            return isChanged;
        }

        private bool InnerDetectState()
        {
            CameraDeviceList.Clear();
            //获取所有设备
            FilterInfoCollection _filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            if (_filterInfoCollection.Count > 0)
            {
                foreach (FilterInfo filterInfo in _filterInfoCollection)
                {
                    CameraDevice cameraDevice = new CameraDevice(filterInfo.MonikerString);
                    CameraDeviceList.Add(cameraDevice);
                }
            }
            //设置DefaultCameraDevice并返回状态是否变化
            if (DefaultCameraDevice == null)
            {
                if (CameraDeviceList.Count > 0)
                {
                    DefaultCameraDevice = CameraDeviceList[0];
                    return true ;
                }
                return false;
            }
            else
            {
                foreach (CameraDevice device in CameraDeviceList)
                {
                    if (device.Name == DefaultCameraDevice.Name)
                    {
                        return false;
                    }
                }
                DefaultCameraDevice = null;
                return true;
            }
        }
        
    }
}
