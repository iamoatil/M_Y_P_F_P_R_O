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
    class Mirror9008:IMirror
    {
        /// <summary>
        /// 设备句柄
        /// </summary>
        IntPtr _deviceHandle;

        /// <summary>
        ///设备的Com名字
        /// </summary>
        string ComName;

        /// <summary>
        /// 手机型号
        /// </summary>
        string PhoneType;

        /// <summary>
        /// 镜像文件
        /// </summary>
        public MirrorFile MirrorFile { get; private set; }

        /// <summary>
        /// 是否已经初始化
        /// </summary>
        public bool IsInitialized { get; private set; }

        //镜像是否已经关闭
        private bool _isClosed;
        private object _closeLocker = new object();

        /// <summary>
        /// 在Initialize方法中打开设备，并且初始化
        /// </summary>
        public void Initialize(string com, string phoneType, string pmbnfileDir, string pQSaharaServerfilepath)
        {
            IsInitialized = false;
            try
            {
                //todo pmbnfileDir是否正确？
                MirrorFile = new MirrorFile(pmbnfileDir);
                int ret = Android9008MirrorAPI.Android_9008_Img_Mount(com, phoneType, pmbnfileDir, pQSaharaServerfilepath, ref _deviceHandle);
                if (IntPtr.Zero == _deviceHandle)
                {
                    Exception($"安卓9008镜像出错！Mount失败，设备信息:{com} {phoneType} {pmbnfileDir} {pQSaharaServerfilepath}");
                    return;
                }
                ComName = com;
                PhoneType = phoneType;
            }
            catch (Exception ex)
            {
                Exception($"安卓9008镜像出错！Mount失败，设备信息:{com} {phoneType} {pmbnfileDir} {pQSaharaServerfilepath} exception:{ex.Message}");
                return;
            }

            IsInitialized = true;
        }

        public void Start(int count, long startedPos)
        {
            if (IsInitialized == true)
            {
                try
                {
                    var result = Android9008MirrorAPI.Android_9008_Img_ImageDataZone(_deviceHandle, startedPos / 512, count, ImageDataCallBack);
                    if (0 != result)
                    {
                        Exception(string.Format("安卓9008镜像出错！ImageDataZone失败，设备信息:{1}", ComName, result));
                        return;
                    }
                }
                catch (Exception ex)
                {
                    MirrorFile.Close();
                    Console.WriteLine("{0}|{1}", CmdStrings.Progress, MirrorFile.WritedSize.ToString());
                    Exception(string.Format("镜像异常，设备ID:{0} 错误码:{1}", ComName, ex));
                    return;
                }

                MirrorFile.Close();
                MirrorFile.CreateMD5File();
                Console.WriteLine(CmdStrings.FinishState);
            }
        }

        /// <summary>
        /// 获取扇区总数
        /// </summary>
        public void GetDiskSector()
        {
            long sectorCount = 0;
            Android9008MirrorAPI.Android_9008_Img_TetdiskSectors(_deviceHandle,ref sectorCount);
            
        }

        /// <summary>
        /// 关闭镜像
        /// </summary>
        public void Close()
        {
            lock (_closeLocker)
            {
                _isClosed = true;
            }
            int ret=Android9008MirrorAPI.Android_9008_Img_UNMount(ref _deviceHandle);
            MirrorFile.Close();
        }

        /// <summary>
        /// 发送异常状态到调用端
        /// </summary>
        /// <param name="msg"></param>
        private void Exception(string msg)
        {
            MirrorFile.Close();
            Console.WriteLine("{0}|{1}", CmdStrings.Exception, msg);
            IsInitialized = false;
        }

        /// <summary>
        /// 保存镜像数据，并且发送进度信息到调用端
        /// </summary>
        /// <returns></returns>
        private int ImageDataCallBack(IntPtr data, int datasize, ref int stop)
        {
            lock (_closeLocker)
            {
                if (!_isClosed)
                {
                    var buff = new byte[datasize];
                    Marshal.Copy(data, buff, 0, datasize);

                    MirrorFile.Write(buff);
                    Console.WriteLine("{0}|{1}", CmdStrings.Progress, MirrorFile.WritedSize.ToString());
                }
            }
            return 0;
        }
    }
}
