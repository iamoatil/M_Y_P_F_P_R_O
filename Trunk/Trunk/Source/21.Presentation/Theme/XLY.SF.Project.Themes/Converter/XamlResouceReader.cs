using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.Theme.ResouceReader
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/11/16 19:41:16
* ==============================================================================*/

namespace XLY.SF.Project.Themes
{
    /// <summary>
    /// 资源读取
    /// </summary>
    public class XamlResouceReader
    {
        private static Dictionary<string, string> _dic = new Dictionary<string, string>();
        private static Dictionary<string, DataTemplate> _dicRes = new Dictionary<string, DataTemplate>();

        /// <summary>
        /// 将某个资源文件内容读取为字符串
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetResourceAsString(string name)
        {
            if (!_dic.ContainsKey(name))
            {
                if (!name.StartsWith("XLY.SF.Project.Themes"))
                {
                    name = "XLY.SF.Project.Themes." + name;
                }
                var o = Assembly.GetExecutingAssembly().GetManifestResourceStream(name);
                var contentbytes = new byte[o.Length];
                o.Read(contentbytes, 0, contentbytes.Length);
                _dic[name] = Encoding.UTF8.GetString(contentbytes);
            }
            return _dic[name];
        }

        /// <summary>
        /// 将字符串转换为Xaml对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        public static T ToXamlObject<T>(string content) where T:class
        {
            using (Stream sem = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                return XamlReader.Load(sem) as T;
            }
        }

        /// <summary>
        /// 读取某个资源文件中的数据模板，并返回指定key的对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">资源文件名，比如ThemesStyle.DataGridStyle.DataGridColumnTemplate.xaml</param>
        /// <param name="key"></param>
        /// <param name="completeCallback">读取完成后的转换函数，此时可以替换文件内容</param>
        /// <returns></returns>
        public static T ToDataTemplate<T>(string name, Func<string, string> completeCallback = null) where T : class
        {
            var content = GetResourceAsString(name);
            if(completeCallback != null)
            {
                content = completeCallback(content);
            }
            if(_dicRes.ContainsKey(name))
            {
                return _dicRes[name] as T;
            }
            using (Stream sem = new MemoryStream(Encoding.UTF8.GetBytes(content)))
            {
                var rd = XamlReader.Load(sem) as DataTemplate;
                _dicRes[name] = rd;
                return rd as T;
            }
        }
    }
}
