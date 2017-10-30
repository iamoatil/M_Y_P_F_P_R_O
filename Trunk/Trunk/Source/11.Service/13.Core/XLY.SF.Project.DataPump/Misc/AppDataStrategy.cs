using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;

namespace XLY.SF.Project.DataPump.Misc
{
    public class AppDataStategy : IProcessControllableStrategy
    {
        #region Methods

        #region Public

        public void Process(DataPumpControllableExecutionContext context)
        {
            String destPath = context.GetContextData<String>("destPath");
            String sourcePath = context.GetContextData<String>("sourcePath");
            var apps = context.ExtractItems.Select(s => s.AppName).Distinct().ToList();

            if (apps.Contains("com.tencent.mm"))
            {//安卓微信多帐号
                apps.Add("com.tencen1.mm");
                apps.Add("com.weisin1.mm");
                apps.Add("com.weisin2.mm");
                apps.Add("com.weisin3.mm");
                apps.Add("com.weisin4.mm");
                apps.Add("com.weisin5.mm");
                apps.Add("com.weijin1.mm");
                apps.Add("com.weijin2.mm");
                apps.Add("com.weijin3.mm");
                apps.Add("com.weijin4.mm");
                apps.Add("com.weijin5.mm");
                apps.Add("com.weilin1.mm");
                apps.Add("com.weilin2.mm");
                apps.Add("com.weilin3.mm");
                apps.Add("com.weilin4.mm");
                apps.Add("com.weilin5.mm");
                apps.Add("com.weirui.mm");
                apps.Add("com.lbe.parallel");
            }

            if (apps.Contains("com.apple.MobileAddressBook") || apps.Contains("com.apple.CallHistory") || apps.Contains("com.apple.MobileSMS"))
            {//IOS通讯录 IOS通话记录 IOS短信
                apps.Add("HomeDomain");
                apps.Add("WirelessDomain");
            }

            String configPath = context.Source.Config;
            if (!configPath.IsInvalid() || configPath.Contains("com."))
            {
                var arr = configPath.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (arr.IsValid())
                {
                    var foldname = arr.FirstOrDefault((s) => s.StartsWith("com."));
                    if (foldname.IsValid() && !apps.Contains(foldname))
                    {
                        apps.Add(foldname);
                    }
                }
            }

            int count = apps.Count;

            foreach (var app in apps)
            {
                //asyn.Advance(0, String.Format(LanguageHelper.Get("LANGKEY_ZhengZaiChaZhao_01481"), app));
                //if (FindAppData(app, sourcePath, destPath, context.Reporter))
                //{
                    //asyn.Advance(30.0 / apps.Count, String.Format(LanguageHelper.Get("LANGKEY_HuoQuShuJuWenJianChengGong_01482"), app));
                //}
                //else
                //{
                    //asyn.Advance(30.0 / apps.Count);
                //}
            }

            //删除temp目录
            //FileHelper.DeleteDirectory(destPath.TrimEnd('/', '\\') + @"\temp\");
        }

        #endregion

        #endregion
    }
}
