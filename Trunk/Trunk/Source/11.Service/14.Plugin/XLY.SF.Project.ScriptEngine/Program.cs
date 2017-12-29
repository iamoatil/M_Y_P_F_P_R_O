using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.ScriptEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (null == args || 0 == args.Length)
                {
                    Console.WriteLine("start arguments is null");
                    Console.ReadKey();
                    return;
                }

                var argfile = args[0];
                if (!System.IO.File.Exists(argfile))
                {
                    Console.WriteLine("start arguments file is not exist");
                    Console.ReadKey();
                    return;
                }
                var ser = System.IO.File.ReadAllText(argfile, Encoding.UTF8);
                var arg = JsonDeserializeIO<EngineArg>(ser);

                string result = "";
                if(arg.Language == "JavaScript")
                {
                    //1.获取源文件
                    if (!System.IO.File.Exists(arg.ResultFile))
                    {
                        return;
                    }

                    //2.获取源代码
                    string sourcecode = System.IO.File.ReadAllText(arg.ResultFile, Encoding.UTF8);

                    //3.清空源文件
                    System.IO.File.WriteAllText(arg.ResultFile, "", System.Text.Encoding.UTF8);

                    var context = new JavaScriptContext() { CharatorBasePath = arg.ScriptDir };
                    Console.WriteLine("插件特征库文件：" + context.CharatorBasePath);
                    //4.执行源代码
                    result = context.Execute(sourcecode, null)?.ToString();
                }
                else if (arg.Language == "Python36")
                {

                }
               
                Console.Write("执行结束：" + arg.ResultFile);
                //5.存储执行结果
                System.IO.File.WriteAllText(arg.ResultFile, result, System.Text.Encoding.UTF8);
                Console.Write("return");
            }
            catch (Exception ex)
            {
                Console.Write("执行错误：" + ex.Message + ex.StackTrace);
            }
            //Console.ReadKey();
        }

        public static T JsonDeserializeIO<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                T obj = (T)ser.ReadObject(ms);
                return obj;
            }
        }
    }

    [Serializable]
    class EngineArg
    {
        /// <summary>
        /// 插件的语言，是js/python/html/C#
        /// </summary>
        public string Language { get; set; }
        /// <summary>
        /// 插件解压后临时存放路径
        /// </summary>
        public string ScriptDir { get; set; }
        /// <summary>
        /// 插件对象，如果是js插件，则表示插件的文本内容；如果是Python/Html插件，则表示为插件的路径；如果是C#插件，则为null
        /// </summary>
        public string ScriptObject { get; set; }
        /// <summary>
        /// 结果文件保存的路径，如果是js，则会先将执行的脚本导入该文件
        /// </summary>
        public string ResultFile { get; set; }
    }
}
