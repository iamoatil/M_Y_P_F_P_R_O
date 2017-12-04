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
        /// 是否已经初始化
        /// </summary>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// 在Initialize方法中打开设备，并且初始化
        /// </summary>
        public void Initialize(string deviceSerialnumber,int isHtc,string path)
        {
            IsInitialized = false;
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
                return;
            }

            IsInitialized = true;
        }

        public void Start(string block,long startedPos)
        {
            if(IsInitialized == true)
            {
                try
                {
                    var result = AndroidMirrorAPI.ImageDataZone(_deviceHandle, block, startedPos/512, -1, ImageDataCallBack);
                    if (0 != result)
                    {
                        Exception(string.Format("安卓手机镜像出错！ImageDataZone失败，设备ID:{0} 错误码:{1}", _deviceSerialnumber, result));
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MirrorFile.Close();
                    Console.WriteLine("{0}|{1}", CmdStrings.Progress, MirrorFile.WritedSize.ToString());
                    Exception(string.Format("镜像异常，设备ID:{0} 错误码:{1}", _deviceSerialnumber, ex));
                    return;
                }
                
                MirrorFile.Close();
                MirrorFile.CreateMD5File();
                Console.WriteLine(CmdStrings.FinishState);
            }
        }

        /// <summary>
        /// 发送异常状态到调用端
        /// </summary>
        /// <param name="msg"></param>
        private void Exception(string msg)
        {
            MirrorFile.Close();
            Console.WriteLine("{0}|{1}",CmdStrings.Exception, msg);
            IsInitialized = false;
        }

        /// <summary>
        /// 保存镜像数据，并且发送进度信息到调用端
        /// </summary>
        /// <returns></returns>
        private int ImageDataCallBack(IntPtr data, int datasize, ref int stop)
        {
            var buff = new byte[datasize];
            Marshal.Copy(data, buff, 0, datasize);

            MirrorFile.Write(buff);
            Console.WriteLine("{0}|{1}", CmdStrings.Progress, MirrorFile.WritedSize.ToString());
            return 0;
        }
    }    
}
