/* ==============================================================================
* Description：Mirror  
*               1，对AndroidMirrorAPI中的静态方法的调用
*               2，实现镜像功能：打开设备，初始化设备，镜像并且保存文件
* Author     ：litao
* Create Date：2017/11/8 15:52:18
* ==============================================================================*/

using System;
using System.Runtime.InteropServices;

namespace XLY.SF.Project.DataMirrorApp
{
    class Mirror
    {
        /// <summary>
        /// 设备句柄
        /// </summary>
        IntPtr _deviceHandle;
        
        /// <summary>
        ///设备序列号 
        /// </summary>
        string _deviceSerialnumber;

        /// <summary>
        /// 镜像文件
        /// </summary>
        public MirrorFile MirrorFile { get; private set; }

        /// <summary>
        /// 是否有错误
        /// </summary>
        bool _haveErrors = false;        

        /// <summary>
        /// 在Initialize方法中打开设备，并且初始化
        /// </summary>
        public void Initialize(string deviceSerialnumber,int isHtc,string path)
        {            
            try
            {
                MirrorFile = new MirrorFile(path);
                _deviceSerialnumber = deviceSerialnumber;
                _deviceHandle = AndroidMirrorAPI.OpenDevice(deviceSerialnumber);
                if (IntPtr.Zero == _deviceHandle)
                {
                    Exception(string.Format("安卓手机镜像出错！OpenDevice失败，设备ID:{0}", deviceSerialnumber));
                    return;
                }
                var result = AndroidMirrorAPI.Initialize(_deviceHandle, 61440, (IntPtr)isHtc);
                if (0 != result)
                {
                    Exception(string.Format("安卓手机镜像出错！Initialize失败，设备ID:{0} 错误码:{1}", deviceSerialnumber, result));
                    return;
                }
            }
            catch (Exception ex)
            {
                Exception(string.Format("安卓手机镜像出错！设备ID:{0} {1}", deviceSerialnumber, ex));
            }            
        }

        public void Start(string block)
        {
            if(_haveErrors == false)
            {
                var result = AndroidMirrorAPI.ImageDataZone(_deviceHandle, block, 0, -1,ImageDataCallBack);
                if (0 != result)
                {
                    Exception(string.Format("安卓手机镜像出错！ImageDataZone失败，设备ID:{0} 错误码:{1}", _deviceSerialnumber, result));
                    return;
                }
                Stop(CmdStrings.SuccessStopState);
            }
        }

        public void Stop(CmdString cmd)
        {
            MirrorFile.Close();
            if(cmd.Match(CmdStrings.SuccessStopState))
            {
                MirrorFile.CreateMD5File();
            }
            Console.WriteLine(string.Format("{0}|{1}|{2}",CmdStrings.State, CmdStrings.StopState,cmd));
        }

        private void Exception(string msg)
        {
            MirrorFile.Close();
            Console.WriteLine("{0}|{1}",CmdStrings.Exception, msg);
            _haveErrors = true;
        }

        private int ImageDataCallBack(IntPtr data, int datasize, ref int stop)
        {
            var buff = new byte[datasize];
            Marshal.Copy(data, buff, 0, datasize);

            MirrorFile.Write(buff);
            Console.WriteLine("{0}|{1}", CmdStrings.Progress,MirrorFile.WritedSize.ToString()) ;
            return 0;
        }
    }
}
