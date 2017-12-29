using System;
using System.Collections.Generic;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.EarlyWarningView
{
    class FileMd5DbWriter
    {
        public FileDownloader FileDownloader { get { return _fileDownloader; } }
        private FileDownloader _fileDownloader = new FileDownloader();

        private FileMd5DataSource _fileMd5DataSource;

        /// <summary>
        ///  是否已经初始化。
        /// </summary>
        protected bool IsInitialize;

        /// <summary>
        ///提取目录
        /// </summary>
        private string _extractDir;

        public bool Initialize(string extractDir)
        {
            IsInitialize = false;

            _extractDir = extractDir;
            _fileMd5DataSource = new FileMd5DataSource(_extractDir + "data.db");

            IsInitialize = true;
            return IsInitialize;
        }

        internal void TestDeserialize()
        {
            if (!IsInitialize)
            {
                return;
            }
            FileMd5DataSource ds=Serializer.DeSerializeFromBinary<FileMd5DataSource>($@"{_extractDir}\Result\{_fileMd5DataSource.TableName}.ds");
        }

        /// <summary>
        /// 把DataSource对象序列化为ds文件
        /// </summary>
        internal void GenerateDsFile()
        {
            if (!IsInitialize)
            {
                return;
            }
            Serializer.SerializeToBinary(_fileMd5DataSource, $@"{_extractDir}\Result\{_fileMd5DataSource.TableName}.ds");
        }

        /// <summary>
        /// 把数据写入数据库
        /// </summary>
        internal void WriteDb(List<FileMd5DataItem> fileMd5List)
        {
            if (!IsInitialize)
            {
                return;
            }
            foreach (var fileMd5 in fileMd5List)
            {
                _fileMd5DataSource.Items.Add(fileMd5);
            }
            _fileMd5DataSource.Items.Commit();           
        }
    }
}
