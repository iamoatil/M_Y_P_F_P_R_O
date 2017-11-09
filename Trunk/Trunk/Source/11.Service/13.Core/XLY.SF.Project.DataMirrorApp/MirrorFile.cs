using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Description：MirrorFile  
*               表示镜像文件，主要实现字节数组写入文件的功能
* Author     ：litao
* Create Date：2017/11/8 15:58:33
* ==============================================================================*/

namespace XLY.SF.Project.DataMirrorApp
{
    class MirrorFile:IDisposable
    {
        /// <summary>
        /// 镜像文件的路径
        /// </summary>
        private readonly string _path;

        /// <summary>
        /// 写文件的流
        /// </summary>
        FileStream _fileStream;

        /// <summary>
        /// 缓冲区大小为12M。但是数据满10M的时候就入硬盘
        /// </summary>
        readonly byte[] _buffer = new byte[M12];
        const int M10 = 10 * 1024 * 1024;
        const int M12 = 12 * 1024 * 1024;
        int _curWriteIndex = 0;

        /// <summary>
        /// 已经写入文件的大小
        /// </summary>
        public int WritedSize { get; private set; }

        public MirrorFile(string filePath)
        {
            _path = filePath;
            _fileStream = new FileStream(filePath,FileMode.Create);
        }

        /// <summary>
        /// 先把数据写到缓冲区，缓冲区要满的时候写入磁盘
        /// </summary>
        /// <param name="bytes"></param>
        public void Write(byte[] bytes)
        {
            bytes.CopyTo(_buffer,_curWriteIndex);
            _curWriteIndex += bytes.Length;
            WritedSize+= bytes.Length;
            if (_curWriteIndex > M10)
            {
                _fileStream.Write(_buffer,0, _curWriteIndex);
                _fileStream.Flush();
               _curWriteIndex = 0;               
            }
        }

        /// <summary>
        /// 关闭文件的时候，先把缓冲区的数据写到文件缓冲区
        /// </summary>
        public void Close()
        {
            _fileStream.Write(_buffer, 0, _curWriteIndex);
            _curWriteIndex = 0;
            _fileStream.Flush();
            _fileStream.Close();
        }

        #region IDisposable
        //是否回收完毕
        bool _disposed;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~MirrorFile()
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
                _fileStream.Dispose();
                //TODO:释放那些实现IDisposable接口的托管对象
            }
            //TODO:释放非托管资源，设置对象为null
            _disposed = true;
        }
    #endregion
    }
}
