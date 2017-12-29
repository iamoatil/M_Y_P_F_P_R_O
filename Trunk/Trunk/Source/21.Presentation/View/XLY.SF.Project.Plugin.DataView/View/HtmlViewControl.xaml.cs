using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XLY.SF.Framework.BaseUtility;

namespace XLY.SF.Project.Plugin.DataView
{
    /// <summary>
    /// HtmlViewControl.xaml 的交互逻辑
    /// </summary>
    public partial class HtmlViewControl : UserControl
    {
        public HtmlViewControl(string url, DataViewPluginArgument source)
        {
            InitializeComponent();

            this.Url = url;     //@"C:\Users\fhjun\Desktop\htmp\323\index.html"
            this.DataSource = source;
            SaveDataSource();
            _operator.OnSelectedDataChanged -= OnSelectedDataChanged;
            _operator.OnSelectedDataChanged += OnSelectedDataChanged;

            web1.Navigate(new Uri(this.Url, UriKind.RelativeOrAbsolute));
            web1.ObjectForScripting = _operator;
        }

        private WebOperator _operator = new WebOperator();

        public event DelgateDataViewSelectedItemChanged OnSelectedDataChanged;

        public string Url { get; set; }
        public DataViewPluginArgument DataSource { get; set; }

        /// <summary>
        /// 保存数据源，生成json文件
        /// </summary>
        private void SaveDataSource()
        {
            if (DataSource == null || DataSource.Items == null)
            {
                return;
            }
            FileInfo fi = new FileInfo(Url);
            if (!fi.Exists)
            {
                return;
            }
            if (!Directory.Exists(System.IO.Path.Combine(fi.DirectoryName, "data")))
            {
                Directory.CreateDirectory(System.IO.Path.Combine(fi.DirectoryName, "data"));
            }
            string fileName = System.IO.Path.Combine(fi.DirectoryName, "data/data.js");
            //File.WriteAllText(fileName, $"var __data = {Serializer.JsonSerilize(DataSource)}", Encoding.UTF8);
            using (StreamWriter sw = new StreamWriter(fileName, false, Encoding.UTF8))
            {
                sw.Write("var __data = [");
                int r = 0;
                foreach (var c in DataSource.Items.GetView(0, -1))
                {
                    if (r != 0)
                        sw.Write(",");
                    sw.Write(Serializer.JsonSerilize(c));
                    r++;
                }
                sw.Write("];");
            }
        }
    }

    /// <summary>
    /// 浏览器操作COM口，用于html和cs的交互
    /// </summary>
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]//将该类设置为com可访问  
    public class WebOperator
    {
        public event DelgateDataViewSelectedItemChanged OnSelectedDataChanged;
        public void SelectItem(object obj)
        {
            OnSelectedDataChanged?.Invoke(obj);
            //MessageBox.Show($"you click {obj}");
        }
    }
}
