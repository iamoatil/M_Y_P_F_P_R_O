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

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// 安卓华为备份联系人数据解析
    /// </summary>
    internal class HuaweiContactsDataParseCoreV1_0
    {
        /// <summary>
        /// contact.db文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// 安卓华为备份联系人数据解析
        /// </summary>
        /// <param name="mainDbPath">contact.db文件路径</param>
        public HuaweiContactsDataParseCoreV1_0(string mainDbPath)
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

            SqliteContext mainContext = null;

            try
            {
                var dataList = mainContext.Find(new SQLiteString("SELECT raw_contact_id,data1,mimetype FROM data_tb WHERE data1 NOTNULL ORDER BY raw_contact_id,mimetype"));
                var rawidList = dataList.Select(d => DynamicConvert.ToSafeString(d.raw_contact_id)).Distinct();

                foreach (string rawid in rawidList)
                {
                    var nameData = dataList.FirstOrDefault(d => DynamicConvert.ToSafeString(d.raw_contact_id) == rawid && DynamicConvert.ToSafeString(d.mimetype) == "vnd.android.cursor.item/name");
                    var noteData = dataList.FirstOrDefault(d => DynamicConvert.ToSafeString(d.raw_contact_id) == rawid && DynamicConvert.ToSafeString(d.mimetype) == "vnd.android.cursor.item/note");
                    var emailData = dataList.FirstOrDefault(d => DynamicConvert.ToSafeString(d.raw_contact_id) == rawid && DynamicConvert.ToSafeString(d.mimetype) == "vnd.android.cursor.item/email_v2");
                    //var websiteData = dataList.FirstOrDefault(d => DynamicConvert.ToSafeString(d.raw_contact_id) == rawid && DynamicConvert.ToSafeString(d.mimetype) == "vnd.android.cursor.item/website");
                    var addressData = dataList.FirstOrDefault(d => DynamicConvert.ToSafeString(d.raw_contact_id) == rawid && DynamicConvert.ToSafeString(d.mimetype) == "vnd.android.cursor.item/postal-address_v2");

                    //string name = "", note = "", email = "", website = "", address = "";
                    string name = "", note = "", email = "", address = "";
                    if (null != nameData)
                    {
                        name = DynamicConvert.ToSafeString(nameData.data1);
                    }
                    if (null != noteData)
                    {
                        note = DynamicConvert.ToSafeString(noteData.data1);
                    }
                    if (null != emailData)
                    {
                        email = DynamicConvert.ToSafeString(emailData.data1);
                    }
                    //if (null != websiteData)
                    //{
                    //    website = DynamicConvert.ToSafeString(websiteData.data1);
                    //}
                    if (null != addressData)
                    {
                        address = DynamicConvert.ToSafeString(addressData.data1);
                    }

                    var phones = dataList.Where(d => DynamicConvert.ToSafeString(d.raw_contact_id) == rawid && DynamicConvert.ToSafeString(d.mimetype) == "vnd.android.cursor.item/phone_v2")
                                         .Select(d => DynamicConvert.ToSafeString(d.data1));
                    //var groups = dataList.Where(d => DynamicConvert.ToSafeString(d.raw_contact_id) == rawid && DynamicConvert.ToSafeString(d.mimetype) == "vnd.android.cursor.item/group_membership")
                    //                     .Select(d => DynamicConvert.ToSafeString(d.data1)).Distinct();

                    //string groupinfo = GetGroupsInfo(context, groups);
                    DateTime? lastContactDate = GetLastContactDate(mainContext, rawid);

                    if (phones.IsInvalid())
                    {
                        if (name.IsInvalid())
                        {
                            continue;
                        }

                        Contact contactTemp = new Contact();
                        contactTemp.DataState = EnumDataState.Normal;
                        contactTemp.Name = name;
                        contactTemp.Remark = note;
                        contactTemp.Email = email;
                        //contactTemp.Website = website;
                        contactTemp.PostalAddress = address;
                        //contactTemp.GroupName = groupinfo;
                        contactTemp.LastContactDate = lastContactDate;

                        datasource.Items.Add(contactTemp);
                    }
                    else
                    {
                        foreach (var phonenumber in phones)
                        {
                            Contact contactTemp = new Contact();
                            contactTemp.DataState = EnumDataState.Normal;
                            contactTemp.Number = DynamicConvert.ToSafeString(phonenumber);
                            contactTemp.Name = name;
                            contactTemp.Remark = note;
                            contactTemp.Email = email;
                            //contactTemp.Website = website;
                            contactTemp.PostalAddress = address;
                            //contactTemp.GroupName = groupinfo;
                            contactTemp.LastContactDate = lastContactDate;

                            datasource.Items.Add(contactTemp);
                        }
                    }
                }

            }
            finally
            {
                mainContext?.Dispose();
                mainContext = null;
            }
        }

        private string GetGroupsInfo(SqliteContext context, IEnumerable<dynamic> ids)
        {
            if (ids.IsInvalid())
            {
                return string.Empty;
            }

            var table = context.FindByName("groups_tb");
            var list = table.Where(d => ids.Any(i => i == d.xlyid));
            if (list.IsValid())
            {
                return string.Join(",", list.Select(d => DynamicConvert.ToSafeString(d.title)));
            }

            return string.Empty;
        }

        private DateTime? GetLastContactDate(SqliteContext context, string id)
        {
            var data = context.Find(new SQLiteString(string.Format("SELECT last_time_contacted FROM raw_contacts_tb WHERE _id = '{0}'", id))).FirstOrDefault();

            if (null != data)
            {
                string value = DynamicConvert.ToSafeString(data.last_time_contacted);
                if (value.IsValid() && "0" != value)
                {
                    return new DateTime(1970, 1, 1).AddSeconds(DynamicConvert.ToSafeLong(data.last_time_contacted) / 1000).AddHours(8);
                }
            }

            return null;
        }

    }
}
