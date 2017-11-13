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
    /// IStartupArgment
    /// </summary>
    public class StartupArgment
    {
        public StartupArgment()
        {
            Properties = new Dictionary<string, string>();

            var cmd = Environment.GetCommandLineArgs();
            if(cmd == null || cmd.Length < 1)
            {
                return;
            }
            DecodeCommond(cmd[0]);
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

        public Dictionary<string, string> Properties { get; set; }

        public JObject Source { get; set; }

        private void DecodeCommond(string cmd)
        {
            //base64解码
            string decode = "";
            byte[] bytes = Convert.FromBase64String(cmd);
            try
            {
                decode = Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return;
            }

            JsonReader reader = new JsonTextReader(new System.IO.StringReader(decode));

            Source = JObject.Load(reader);
            foreach (var j in Source.Properties())    //读取所有的json属性
            {
                Properties[j.Name] = j.Value.ToObject<string>();
            }
        }
    }
}
