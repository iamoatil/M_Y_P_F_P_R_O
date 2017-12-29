using System;
using System.Collections.Generic;
using System.Management;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.AndroidImg9008
{
    public class MulHardware
    {
        /// <summary>
        /// WMI取硬件信息
        /// </summary>
        /// <param name="hardType">硬件类型：枚举win32 api</param>
        /// <param name="propKey">属性名称</param>
        /// <returns>硬件信息列表</returns>
        public static string[] GetMulHardwareInfo(HardwareEnum hardType, string propKey)
        {

            List<string> strs = new List<string>();
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("select * from " + hardType))
                {
                    var hardInfos = searcher.Get();
                    foreach (var hardInfo in hardInfos)
                    {
                        if (hardInfo.Properties[propKey].Value != null)
                        {
                            String str = hardInfo.Properties[propKey].Value.ToString();
                            strs.Add(str);
                        }

                    }
                    searcher.Dispose();
                }
                return strs.ToArray();
            }
            catch
            {
                return null;
            }
            finally
            { strs = null; }
        }
    }
}
