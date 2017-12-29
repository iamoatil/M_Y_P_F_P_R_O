using System;
using System.Runtime.InteropServices;

/* ==============================================================================
* ==============================================================================*/

namespace XLY.SF.Project.DataMirrorApp
{
    /// <summary>
    /// Android 镜像API
    /// </summary>
    public static class Android9008MirrorAPI
    {
        // Android镜像
        private const string _AdroidImg9008 = @"Lib\vcdll\Android_Img9008\AdroidImg9008.dll";

        /// <summary>
        /// 装载参数
        /// </summary>
        /// <param name="pcomstr">手机连接的com口字符串，例如COM10</param>
        /// <param name="pphoneModel">手机的型号，型号是自定义的数据详见支持的型号参数表</param>
        /// <param name="pmbnfileDir">资源文件的目录绝对路径，res9008目录</param>
        /// <param name="pQSaharaServerfilepath">QCUSBXLY.exe的绝对路径</param>
        /// <param name="handle">句柄</param>
        /// <returns>0为成功 非0为错误码</returns>
        [DllImport(_AdroidImg9008, EntryPoint = "android9008img_Mount_ret")]
        public static extern int Android_9008_Img_Mount(string pcomstr, string pphoneModel, string pmbnfileDir, string pQSaharaServerfilepath, ref IntPtr handle);

        /// <summary>
        /// 获取手机镜像扇区数
        /// </summary>
        /// <param name="handle9008">句柄</param>
        /// <param name="sectorlen">引用扇区数</param>
        /// <returns>是否成功，0-成功，反之亦然</returns>
        [DllImport(_AdroidImg9008, EntryPoint = "android9008img_getdiskSectors")]
        public static extern int Android_9008_Img_TetdiskSectors(IntPtr handle9008, ref Int64 sectorlen);

        /// <summary>
        /// 数据镜像、断点续传镜像
        /// </summary>
        /// <param name="handle9008">句柄</param>
        /// <param name="start">开始扇区数（默认0）：镜像文件字节总数/512</param>
        /// <param name="count">镜像总数（默认-1）：扇区长度-开始扇区数</param>
        /// <param name="callBack">数据回调，同androidimg的dll中数据回调一致</param>
        /// <returns>是否成功，0-成功，反之亦然</returns>
        [DllImport(_AdroidImg9008, EntryPoint = "android9008img_imageDataZone")]
        public static extern int Android_9008_Img_ImageDataZone(IntPtr handle9008, Int64 start, Int64 count, ImageDataCallBack callBack);

        /// <summary>
        /// 镜像回调函数
        /// </summary>
        /// <param name="data">每次返回数据</param>
        /// <param name="datasize">每次返回字节数</param>
        /// <param name="stop">是否停止，0-继续，1-停止</param>
        /// <returns></returns>
        public delegate int ImageDataCallBack(IntPtr data, int datasize, ref int stop);

        /// <summary>
        /// 卸载
        /// </summary>
        /// <param name="handle9008">句柄</param>
        /// <returns>是否成功，0-成功，反之亦然</returns>
        [DllImport(_AdroidImg9008, EntryPoint = "android9008img_unMount")]
        public static extern int Android_9008_Img_UNMount(ref IntPtr handle9008);
    }
}
