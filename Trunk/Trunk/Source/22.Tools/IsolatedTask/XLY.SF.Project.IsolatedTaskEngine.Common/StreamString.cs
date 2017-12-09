using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.IsolatedTaskEngine.Common
{
    /// <summary>
    /// 读写字符串流。
    /// </summary>
    public class StreamString
    {
        #region Fields

        private readonly StreamReader _reader;

        private readonly StreamWriter _writer;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Project.IsolatedTaskEngine.Common.StreamString  实例。
        /// </summary>
        /// <param name="ioStream">流。</param>
        public StreamString(Stream ioStream)
        {
            _reader = new StreamReader(ioStream, Encoding.UTF8);
            _writer = new StreamWriter(ioStream, Encoding.UTF8);
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 从流中读取以换行符结尾字符串。
        /// </summary>
        /// <returns>字符串。</returns>
        public String ReadString()
        {
            return _reader.ReadLine();
        }

        /// <summary>
        /// 向流中写入字符串并以换行符结尾。
        /// </summary>
        /// <param name="str">字符串。</param>
        public void WriteString(String str)
        {
            _writer.Write(str);
            _writer.WriteLine();
            _writer.Flush();
        }

        #endregion

        #endregion
    }
}
