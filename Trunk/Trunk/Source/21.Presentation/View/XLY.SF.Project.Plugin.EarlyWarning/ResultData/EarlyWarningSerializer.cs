/* ==============================================================================
* Description：EarlyWarningSerializer  
* Author     ：litao
* Create Date：2017/12/4 19:53:38
* ==============================================================================*/

using System;
using System.IO;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.EarlyWarningView
{
    /// <summary>
    /// 预警序列化
    /// </summary>
    class EarlyWarningSerializer : IDisposable
    {
        /// <summary>
        /// 是否已经初始化。对象要初始化后才能使用
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// 序列化文件的路径
        /// </summary>
        private string _dir;

        public EarlyWarningSerializer()
        {
            _dir = Path.GetFullPath(@"EarlyWarning\智能提取\Result\");
        }

        /// <summary>
        /// 对象要初始化后才能使用
        /// </summary>
        /// <returns></returns>
        internal bool Initialize()
        {
            _isInitialized = false;
            //清空_dir
            try
            {
                if (Directory.Exists(_dir))
                {
                    Directory.Delete(_dir, true);
                }
                Directory.CreateDirectory(_dir);
            }
            catch (Exception)
            {
                return _isInitialized;
            }
            _isInitialized = true;
            return _isInitialized;
        }

        /// <summary>
        /// 序列化
        /// </summary>
        internal void Serialize(IDataSource dataSource)
        {
            string fileName = string.Format("{0}{1}_{2}.ds", _dir, dataSource.PluginInfo.Guid.Trim(new []{ '{','}'}), dataSource.PluginInfo.Name);
            if(!File.Exists(fileName))
            {
                Serializer.SerializeToBinary(dataSource, fileName);
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
        ~EarlyWarningSerializer()
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
                _isInitialized = false;
                
                //TODO:释放那些实现IDisposable接口的托管对象
            }
            //TODO:释放非托管资源，设置对象为null
            _disposed = true;
        }
        #endregion
    }
}
