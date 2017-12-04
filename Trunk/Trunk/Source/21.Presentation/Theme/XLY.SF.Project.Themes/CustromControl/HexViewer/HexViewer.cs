using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XLY.SF.Project.Themes.CustromControl
{
    /// <summary>
    /// 按照步骤 1a 或 1b 操作，然后执行步骤 2 以在 XAML 文件中使用此自定义控件。
    ///
    /// 步骤 1a) 在当前项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:XLY.SF.Project.Themes.CustromControl.HexViewer"
    ///
    ///
    /// 步骤 1b) 在其他项目中存在的 XAML 文件中使用该自定义控件。
    /// 将此 XmlNamespace 特性添加到要使用该特性的标记文件的根 
    /// 元素中: 
    ///
    ///     xmlns:MyNamespace="clr-namespace:XLY.SF.Project.Themes.CustromControl.HexViewer;assembly=XLY.SF.Project.Themes.CustromControl.HexViewer"
    ///
    /// 您还需要添加一个从 XAML 文件所在的项目到此项目的项目引用，
    /// 并重新生成以避免编译错误: 
    ///
    ///     在解决方案资源管理器中右击目标项目，然后依次单击
    ///     “添加引用”->“项目”->[浏览查找并选择此项目]
    ///
    ///
    /// 步骤 2)
    /// 继续操作并在 XAML 文件中使用控件。
    ///
    ///     <MyNamespace:HexViewer/>
    ///
    /// </summary>
    public class HexViewer : Control
    {
        static HexViewer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HexViewer), new FrameworkPropertyMetadata(typeof(HexViewer)));
        }

        #region Public
        /// <summary>
        /// 当前编码方式
        /// </summary>
        public Encoding Encoding { get => _encoding ?? Encoding.Default; }

        /// <summary>
        /// 当前打开的文件路径
        /// </summary>
        public string FileName { get; set; }

        //打开一个文件
        public void Open(string fileName, Encoding defaultEncoding = null)
        {
            if(!File.Exists(fileName))
            {
                throw new Exception($"File {fileName} is not exist");
            }
            FileName = fileName;
            _cursor = 0;
            ReadPage(FileName);
            if(defaultEncoding == null)
            {
                _encoding = GetEncoding(_buff.Buffer) ?? Encoding.Default;
            }
            else
            {
                _encoding = defaultEncoding;
            }
            _cbEncoding.SelectedValue = _encoding;
        }
        #endregion

        #region Private
        CacheBuffer _buff = new CacheBuffer(PageSize + 2 * CachePageSize);

        long _cursor = 0;
        long _fileSize = 0;
        const int PageSize = 1024 * 16;
        const int CachePageSize = 16;
        Encoding _encoding = Encoding.Default;

        #endregion
        Grid _gridLayout;
        TextBox _tbLine;
        TextBox _tbByte;
        TextBox _tbText;
        ScrollBar _scroll;
        ComboBox _cbEncoding;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _gridLayout = GetTemplateChild("PART_Container") as Grid;
            _tbLine = GetTemplateChild("PART_Line") as TextBox;
            _tbByte = GetTemplateChild("PART_Byte") as TextBox;
            _tbText = GetTemplateChild("PART_Text") as TextBox;
            _scroll = GetTemplateChild("PART_Scroll") as ScrollBar;
            _cbEncoding = GetTemplateChild("PART_Encoding") as ComboBox;

            _gridLayout.MouseWheel += _gridLayout_MouseWheel;
            _scroll.ValueChanged += _scroll_ValueChanged;
            _cbEncoding.SelectionChanged += _cbEncoding_SelectionChanged;

            Dictionary<string, Encoding> dic = new Dictionary<string, Encoding>();
            int idx = 0;
            foreach (var ec in Encoding.GetEncodings())
            {
                if (dic.ContainsKey(ec.DisplayName))
                {
                    idx = 1;
                    while (dic.ContainsKey($"{ec.DisplayName}({idx})"))
                    {
                        idx++;
                    }
                    dic[$"{ec.DisplayName}({idx})"] = ec.GetEncoding();
                }
                else
                {
                    dic[ec.DisplayName] = ec.GetEncoding();
                }
            }

            _cbEncoding.ItemsSource = dic;
            _cbEncoding.DisplayMemberPath = "Key";
            _cbEncoding.SelectedValuePath = "Value";
        }

        private void Read()
        {
            ReadPage(FileName);
            RestContent();
        }

        /// <summary>
        /// 读取当前页的数据
        /// </summary>
        /// <param name="fileName"></param>
        private void ReadPage(string fileName)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                _fileSize = fs.Length;
                _scroll.Minimum = 0;
                _scroll.Maximum = fs.Length;
                fs.Seek(_cursor, SeekOrigin.Begin);
                _buff.RealBufferSize = fs.Read(_buff.Buffer, 0, PageSize);
            }
        }

        /// <summary>
        /// 重置数据显示内容
        /// </summary>
        private void RestContent()
        {
            StringBuilder sbByte = new StringBuilder(_buff.RealBufferSize * 2);
            StringBuilder sbLineNo = new StringBuilder(_buff.RealBufferSize / 16);

            int startIndex = (int)_cursor % PageSize;
            for (int i = 0; i < _buff.RealBufferSize; i++)
            {
                if (i != 0 && i % 16 == 0)
                {
                    sbByte.AppendLine();
                    sbLineNo.AppendFormat("{0:X8}", _cursor + i - 16);
                    sbLineNo.AppendLine();
                }

                sbByte.AppendFormat("{0:X2}", _buff[i]);
                if (i % 16 != 15 && i != _buff.RealBufferSize - 1)
                {
                    sbByte.Append(" ");
                }
            }
            if (_buff.RealBufferSize > 0)
            {
                sbByte.AppendLine();
                sbLineNo.AppendFormat("{0:X8}", _cursor + (_buff.RealBufferSize - 1) / 16 * 16);
                sbLineNo.AppendLine();
            }

            _tbLine.Text = sbLineNo.ToString();
            _tbByte.Text = sbByte.ToString();
            _tbText.Text = ConverterToText();
        }

        /// <summary>
        /// 将字节流转换为显示的文本
        /// </summary>
        /// <returns></returns>
        private string ConverterToText()
        {
            StringBuilder sbText = new StringBuilder(_buff.RealBufferSize);
            string txt = Encoding.GetString(_buff.Buffer);  //先全部转换为文本
            int strIndex = 0;
            int byteCount = 1;
            int line = 1;
            string strChar;
            bool isNewLine = false;
            for (int i = 0; i < _buff.RealBufferSize; i += byteCount)
            {
                if (strIndex >= txt.Length)
                {
                    continue;
                }
                strChar = txt[strIndex].ToString();   //然后再每个字符依次转换，主要为解决多字节的问题

                //byteCount = 1;
                //while (strChar != Encoding.GetString(_buff.Buffer, i, byteCount))
                //{
                //    byteCount++;
                //}
                byteCount = Encoding.GetBytes(strChar).Length;
                strIndex++;

                if (strChar == "\r" || strChar == "\n" || strChar == "\t")  //替换掉特殊字符
                    sbText.Append(" ");
                else
                    sbText.Append(strChar);
                isNewLine = false;
                for (int j = 1; j < byteCount; j++)     //如果是多字节，则在后面添加空格，比如汉字为3个字节，则显示为“张  ”
                {
                    if (i + j >= line * 16)
                    {
                        isNewLine = true;
                        line++;
                        sbText.AppendLine();
                    }
                    sbText.Append(" ");
                }

                if (!isNewLine && i + byteCount >= line * 16)
                {
                    line++;
                    sbText.AppendLine();
                }
            }

            return sbText.ToString();
        }

        /// <summary>
        /// 重新修改编码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _cbEncoding_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _encoding = _cbEncoding.SelectedValue as Encoding;
            RestContent();
        }

        /// <summary>
        /// 支持鼠标滚轮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _gridLayout_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (e.Delta < 0)  //下滚
            {
                var line = Math.Min(_tbLine.LineCount - 1, _tbLine.GetLastVisibleLineIndex() + 1);
                _scroll.Value = line * 16 + _cursor / PageSize * PageSize;
                double pos = (line - (_tbLine.GetLastVisibleLineIndex() - _tbLine.GetFirstVisibleLineIndex())) * 16 + _cursor / PageSize * PageSize;
                if (pos >= 0)  //由于下滚的最后一行会自动显示为第一行，没有只滚动一行的效果，所以需要重新设置下
                    _scroll.Value = pos;
            }
            else if (e.Delta > 0)  //上滚
            {
                var line = _cursor > 0 ? _tbLine.GetFirstVisibleLineIndex() - 1 : Math.Max(0, _tbLine.GetFirstVisibleLineIndex() - 1);
                _scroll.Value = line * 16 + _cursor / PageSize * PageSize;
            }
        }

        /// <summary>
        /// 滚动条滚动时，3个文本框同时滚动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _scroll_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            long pos = (long)_scroll.Value;
            if (pos / PageSize != _cursor / PageSize)     //翻页
            {
                _cursor = pos / PageSize * PageSize;
                Read();
            }
            int minLine = Math.Min(_tbLine.LineCount, Math.Min(_tbByte.LineCount, _tbText.LineCount));
            int line = Math.Min(minLine - 1, (int)(pos % PageSize / 16));
            _tbLine.ScrollToLine(line);
            _tbByte.ScrollToLine(line);
            _tbText.ScrollToLine(line);
        }

        #region 自动判断文件编码
        /// <summary>
        /// 通过给定的文件流，判断文件的编码类型
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <returns>文件的编码类型</returns>
        public static System.Text.Encoding GetEncoding(byte[] buffer)
        {
            byte[] Unicode = new byte[] { 0xFF, 0xFE, 0x41 };
            byte[] UnicodeBIG = new byte[] { 0xFE, 0xFF, 0x00 };
            byte[] UTF8 = new byte[] { 0xEF, 0xBB, 0xBF }; //带BOM
            Encoding reVal = Encoding.Default;

            int i;
            int.TryParse(buffer.Length.ToString(), out i);
            byte[] ss = buffer;
            if (IsUTF8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }
            return reVal;
        }


        /// <summary>
        /// 判断是否是不带 BOM 的 UTF8 格式
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUTF8Bytes(byte[] data)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数
            byte curByte; //当前分析的字节.
            for (int i = 0; i < data.Length; i++)
            {
                curByte = data[i];
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }
                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X 
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }
                    charByteCounter--;
                }
            }
            if (charByteCounter > 1)
            {
                return false;
                //throw new Exception("非预期的byte格式");
            }
            return true;
        }
        #endregion

        /// <summary>
        /// 数据的缓存
        /// </summary>
        class CacheBuffer
        {
            public CacheBuffer(int maxBufferSize)
            {
                MaxBufferSize = maxBufferSize;
                Buffer = new byte[MaxBufferSize];
                RealBufferSize = 0;
            }

            public int MaxBufferSize { get; set; }
            public int RealBufferSize { get; set; }
            public byte[] Buffer { get; set; }

            public byte this[int index] => Buffer[index];

            public void Clear()
            {
                RealBufferSize = 0;
            }
        }
    }
}
