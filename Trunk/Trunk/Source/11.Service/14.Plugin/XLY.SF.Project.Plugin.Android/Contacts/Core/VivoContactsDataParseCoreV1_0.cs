/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/30 15:45:30 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Persistable.Primitive;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// VIVO备份联系人数据解析核心类
    /// </summary>
    internal class VivoContactsDataParseCoreV1_0
    {
        /// <summary>
        /// contact.vcf 文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// VIVO备份联系人数据解析核心类
        /// </summary>
        /// <param name="mainDbPath">contact.vcf文件路径</param>
        public VivoContactsDataParseCoreV1_0(string mainDbPath)
        {
            MainDbPath = mainDbPath;
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="datasource"></param>
        public void BuildData(ContactDataSource datasource)
        {
            if (!FileHelper.IsValid(MainDbPath))
            {
                return;
            }

            try
            {
                string allText = System.IO.File.ReadAllText(MainDbPath);
                var arrData = allText.Split(new string[] { "BEGIN:VCARD", "END:VCARD" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var d in arrData)
                {
                    var datas = d.Trim('\r', '\n').Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                    if (datas.IsInvalid())
                    {
                        continue;
                    }

                    Contact contact = new Contact();
                    contact.DataState = EnumDataState.Normal;

                    string temp = datas.FirstOrDefault(s => s.StartsWith("TEL;TYPE=CELL:"));
                    if (temp.IsValid())
                    {
                        contact.Number = temp.TrimStart("TEL;TYPE=CELL:").Trim();
                    }

                    temp = datas.FirstOrDefault(s => s.StartsWith("FN:"));
                    if (temp.IsValid())
                    {
                        contact.Name = temp.TrimStart("FN:").Trim();
                    }

                    if (contact.Name.IsValid() || contact.Number.IsValid())
                    {
                        datasource.Items.Add(contact);
                    }
                }
            }
            catch
            {
            }
        }
    }
}
