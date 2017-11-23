using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Assembly   ：	XLY.SF.Framework.Core.Base.IStartupArgment
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/11/13 19:18:08
* ==============================================================================*/

namespace XLY.SF.Framework.Core.Base
{
    /// <summary>
    /// 命令行启动参数解析
    /// </summary>
    public class StartupArgment
    {
        public StartupArgment()
        {
            Properties = new Dictionary<string, string>();

            var cmd = Environment.GetCommandLineArgs();
            if(cmd == null || cmd.Length < 2)
            {
                return;
            }
            DecodeCommond(cmd[1]);
        }

        private static StartupArgment _instance = null;
        public static StartupArgment Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new StartupArgment();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string Get(string key, string defaultValue = "")
        {
            return Properties.ContainsKey(key) ? Properties[key] : defaultValue; 
        }

        /// <summary>
        /// 属性字典
        /// </summary>
        public Dictionary<string, string> Properties { get; set; }

        /// <summary>
        /// 原始的Json对象
        /// </summary>
        public JObject SourceObject { get; set; }
        /// <summary>
        /// 原始的Json字符串
        /// </summary>
        public string SourceJson { get; set; }

        private void DecodeCommond(string cmd)
        {
            //base64解码
            SourceJson = "";
            byte[] bytes = Convert.FromBase64String(cmd);
            try
            {
                SourceJson = Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return;
            }

            JsonReader reader = new JsonTextReader(new System.IO.StringReader(SourceJson));

            SourceObject = JObject.Load(reader);
            foreach (var j in SourceObject.Properties())    //读取所有的json属性
            {
                Properties[j.Name] = j.Value.ToObject<string>();
            }
        }
    }
}
