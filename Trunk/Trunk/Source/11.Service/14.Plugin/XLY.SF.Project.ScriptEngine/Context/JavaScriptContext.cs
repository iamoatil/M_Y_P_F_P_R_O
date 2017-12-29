using Noesis.Javascript;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.ScriptEngine.Engine;

/* ==============================================================================
* Description：JavascriptContext  
* Author     ：Fhjun
* Create Date：2017/2/28 15:52:09
* ==============================================================================*/

namespace XLY.SF.Project.ScriptEngine
{
    /// <summary>
    /// 执行JavaScript脚本
    /// </summary>
    public class JavaScriptContext : IScriptContext
    {
        /// <summary>
        /// 特征库文件的基本路径，为脚本文件的目录
        /// </summary>
        public string CharatorBasePath { get; set; }

        /// <summary>
        /// 执行JavaScript脚本，该脚本同源SPF脚本格式，在后续只兼容执行，不再添加新脚本
        /// </summary>
        /// <param name="content">脚本内容（替换了$source之后的内容)</param>
        /// <param name="asyn">消息通知</param>
        /// <param name="argrument">传入main函数的参数，JavaScript脚本应该为空</param>
        /// <param name="paramValues">其它需要设置到脚本的动态参数</param>
        /// <param name="isThrowExeception">如果执行出现错误，是否抛出异常</param>
        /// <returns></returns>
        public object Execute(string content, IAsyncTaskProgress asyn, object[] argrument = null, Dictionary<string, object> paramValues = null, bool isThrowExeception = true)
        {
            try
            {
                using (JavascriptContext context = new JavascriptContext())
                {
                    XLYEngine engine = new XLYEngine();
                    engine.Sqlite.CharatorBasePath = CharatorBasePath;

                    context.SetParameter("XLY", engine);
                    var ac = new Action<object>(s => { engine.Debug.Write(s); });
                    context.SetParameter("log", ac);
                    if (paramValues != null)
                    {
                        foreach (var pk in paramValues)
                        {
                            context.SetParameter(pk.Key, pk.Value);
                        }
                    }

                    //所有脚本均从main函数开始执行。
                    //content += string.Format(@"
                    //        var ___result = main({0});
                    //        ___result;
                    //", argrument);
                    var obj = context.Run(content);
                    return obj;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("JavaScriptContext Error! " + ex.Message + ex.StackTrace);
                if (isThrowExeception)
                {
                    throw ex;
                }
                return null;
            }
        }
    }
}
