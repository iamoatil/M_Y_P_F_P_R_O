/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/30 15:45:30 
 * explain :  
 *
*****************************************************************************/

using System.Collections.Generic;
using System.Linq;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Persistable.Primitive;
using XLY.SF.Project.Plugin.Language;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.Android
{
    /// <summary>
    /// 安卓联系人数据解析
    /// </summary>
    internal class AndroidContactsDataParseCoreV1_0
    {
        /// <summary>
        /// 查询联系人详情信息sql语句
        /// </summary>
        private const string _SelectDataViewSql = "select A.*,ifnull(substr(B.[mimetype],25,length(B.[mimetype])-24),'') as mimetype from data as A left outer join mimetypes as B on A.[mimetype_id] = B.[_id] where A.[mimetype_id] > 0";

        /// <summary>
        /// 查询RawContacts表sql语句
        /// </summary>
        private const string _SelectRawContactsSql = "select raws.*,con.[name_raw_contact_id] as raw_contact_id from raw_contacts raws left join contacts con on raws.contact_id = con.[_id]";

        /// <summary>
        /// contacts2.db文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// 安卓联系人数据解析核心类
        /// </summary>
        /// <param name="mainDbPath">contacts2.db文件路径</param>
        public AndroidContactsDataParseCoreV1_0(string mainDbPath)
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
                var rMainDbFile = SqliteRecoveryHelper.DataRecovery(MainDbPath, @"chalib\com.android.providers.contacts\contacts2.db.charactor", "raw_contacts,contacts,phone_lookup,data,mimetypes,groups", true);
                mainContext = new SqliteContext(rMainDbFile);

                // raw_contacts表数据集合
                var contactList = mainContext.Find(_SelectRawContactsSql);
                // phone_lookup表数据集合
                var phoneLookupList = mainContext.Find("SELECT * FROM phone_lookup");
                // dataView视图数据集合
                var dataViewList = mainContext.Find(_SelectDataViewSql);
                // 群组列表
                var groupList = mainContext.Find("SELECT * FROM groups");

                //  联系人解析
                var items = new List<Contact>();
                if (contactList.IsValid())
                {
                    items = TryParseItems(contactList);
                    TryParsePhoneLookup(items, phoneLookupList);
                    TryParseDataView(items, dataViewList, groupList);
                }

                foreach (var item in items.Where(c => c.Number.IsValid()))
                {
                    datasource.Items.Add(item);
                }
            }
            finally
            {
                mainContext?.Dispose();
                mainContext = null;
            }
        }

        /// <summary>
        /// 解析联系人列表（raw_contacts表)
        /// </summary>
        /// <param name="contactList">raw_contacts表数据列表</param>
        /// <returns>返回解析后联系人集合</returns>
        private List<Contact> TryParseItems(IEnumerable<dynamic> contactList)
        {
            var items = new List<Contact>();
            Contact item;
            foreach (var sourceitem in contactList)
            {
                try
                {
                    item = new Contact();
                    item.Name = DynamicConvert.ToSafeString(sourceitem.display_name);
                    if (item.Name.IsInvalid())
                    {
                        continue;
                    }

                    int id = DynamicConvert.ToSafeInt(sourceitem.xly_id);
                    if (id < 1)
                    {
                        id = DynamicConvert.ToSafeInt(sourceitem.raw_contact_id);
                    }

                    item.Id = id;
                    item.LastContactDate = DynamicConvert.ToSafeDateTime(sourceitem.last_time_contacted);
                    var times = DynamicConvert.ToSafeInt(sourceitem.times_contacted);
                    item.Number = DataParseHelper.NumberToStu(DynamicConvert.ToSafeString(sourceitem.raw_phone_number));

                    var d = DynamicConvert.ToSafeInt(sourceitem.deleted);
                    var d2 = DynamicConvert.ToEnumByValue<EnumDataState>(sourceitem.XLY_DataType, EnumDataState.Normal);
                    item.DataState = EnumDataState.Normal;
                    if (d == 1 || d2 == EnumDataState.Deleted || d2 == EnumDataState.Fragment)
                    {
                        item.DataState = EnumDataState.Deleted;
                    }

                    if (times > 0) item.Remark = string.Format("{0}:{1} ", LanguageHelper.GetString(Languagekeys.PluginContacts_Times), times);
                    items.Add(item);
                }
                catch
                {
                }
            }

            return items;
        }

        /// <summary>
        /// 根据phone_lookup表，解析联系人号码
        /// </summary>
        /// <param name="items">联系人列表</param>
        /// <param name="phoneLookupList">phone_lookup表集合</param>
        private void TryParsePhoneLookup(List<Contact> items, IEnumerable<dynamic> phoneLookupList)
        {
            if (items.IsInvalid() || phoneLookupList.IsInvalid())
            {
                return;
            }

            foreach (var p in phoneLookupList)
            {
                try
                {
                    string number = DynamicConvert.ToSafeString(p.normalized_number);

                    number = DataParseHelper.NumberToStu(number);

                    // 号码过滤
                    if (!DataParseHelper.ValidateNumber(number))
                    {
                        continue;
                    }

                    //已存在，则跳过
                    //这儿不能过滤，存在两个联系人同一号码的情况
                    //if (items.Any(s => s.Number == number))
                    //{
                    //    continue;
                    //}

                    var c = items.Find(s => s.Id == p.raw_contact_id);
                    if (c == null)
                    {
                        var ncc = new Contact();
                        ncc.Id = DynamicConvert.ToSafeInt(p.raw_contact_id);
                        ncc.Number = number;
                        ncc.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(p.XLY_DataType, EnumDataState.Normal);
                        items.Add(ncc);
                        continue;
                    }

                    if (c.Number.IsInvalid())
                    {
                        c.Number = number;
                        continue;
                    }

                    if (c.Number == number)
                    {
                        continue;
                    }

                    //new
                    var nc = new Contact();
                    nc.Id = c.Id;
                    nc.Name = c.Name;
                    nc.LastContactDate = c.LastContactDate;
                    nc.Remark = c.Remark;
                    var dataState = DynamicConvert.ToEnumByValue<EnumDataState>(p.XLY_DataType, EnumDataState.Normal);
                    if (dataState == EnumDataState.Deleted || dataState == EnumDataState.Fragment)
                    {
                        nc.DataState = EnumDataState.Deleted;
                    }
                    else
                    {
                        nc.DataState = c.DataState;
                    }
                    nc.Number = number;
                    items.Add(nc);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 解析联系人详细信息
        /// </summary>
        /// <param name="items">联系人列表</param>
        /// <param name="dataViewList">dataView视图数据集合</param>
        /// <param name="groupList">群组列表</param>
        private void TryParseDataView(List<Contact> items, IEnumerable<dynamic> dataViewList, IEnumerable<dynamic> groupList)
        {
            if (items.IsInvalid() || dataViewList.IsInvalid())
            {
                return;
            }

            var dataGroup = dataViewList.GroupBy(c => c.raw_contact_id);
            foreach (var view in dataGroup)
            {
                TryParseSingleGroupDataView(view.GetEnumerator().ToList(), items, groupList);
            }
        }

        /// <summary>
        /// 解析一组联系人详细信息
        /// </summary>
        /// <param name="groupDataViewList">联系人详情信息分组列表（同一raw_contact_id）</param>
        /// <param name="contactList">联系人对象列表</param>
        /// <param name="groupList">群组列表</param>
        private void TryParseSingleGroupDataView(List<dynamic> groupDataViewList, List<Contact> contactList, IEnumerable<dynamic> groupList)
        {
            // 查找当前分组中，保存电话号码数据行列表
            //List<dynamic> phoneDataViewList = groupDataViewList.Where(c => c.mimetype.Contains("phone")).ToList();
            List<dynamic> phoneDataViewList = groupDataViewList;
            string number = "";
            foreach (var phoneDataView in phoneDataViewList)
            {
                number = DataParseHelper.NumberToStu(phoneDataView.data1);
                // 号码过滤
                if (!DataParseHelper.ValidateNumber(number))
                {
                    continue;
                }

                Contact contact = contactList.FirstOrDefault(s => s.Id == phoneDataView.raw_contact_id);
                if (contact == null)
                {
                    contact = new Contact();
                    contact.Id = DynamicConvert.ToSafeInt(phoneDataView.raw_contact_id);
                    contact.Number = number;
                    contact.DataState = DynamicConvert.ToEnumByValue<EnumDataState>(phoneDataView.XLY_DataType, EnumDataState.Normal);
                    contactList.Add(contact);
                }

                TryParseSingleDataView(contact, groupDataViewList, groupList);
            }
        }

        /// <summary>
        /// 解析单个联系人详细信息
        /// </summary>
        /// <param name="contact">联系人对象</param>
        /// <param name="groupDataViewList">匹配当前联系人的dataView列表</param>
        /// <param name="groupList">群组列表</param>
        private void TryParseSingleDataView(Contact contact, List<dynamic> groupDataViewList, IEnumerable<dynamic> groupList)
        {
            if (groupDataViewList.IsInvalid())
            {
                // 如果为空，跳过
                return;
            }

            contact.Email = ParseDataViewByKeyword(groupDataViewList, "email");
            contact.ImNumber = ParseDataViewByKeyword(groupDataViewList, "im");
            //contact.NickName = ParseDataViewByKeyword(groupDataViewList, "nickname");
            //contact.SipAddress = ParseDataViewByKeyword(groupDataViewList, "sip_address");
            contact.PostalAddress = ParseDataViewByKeyword(groupDataViewList, "postal-address");
            contact.GroupName = ParseDataViewByKeyword(groupDataViewList, "group");
            //contact.Website = ParseDataViewByKeyword(groupDataViewList, "website");
            contact.Remark = ParseDataViewByKeyword(groupDataViewList, "note");
            if (contact.Name.IsInvalid())
            {
                contact.Name = ParseDataViewByKeyword(groupDataViewList, "name");
            }
            if (contact.Number.IsInvalid())
            {
                contact.Number = ParseDataViewByKeyword(groupDataViewList, "vnd.com.tencent.mm");
            }

            ParseGroupName(contact, contact.GroupName, groupList);
            ParseOrganization(contact, groupDataViewList);
        }

        /// <summary>
        /// 根据关键字，从dataView视图数据中，检索联系人某类信息（如邮箱、及时通讯号等）
        /// </summary>
        /// <param name="matchDataList">匹配当前联系人的dataView列表</param>
        /// <param name="keyword">关键字</param>
        /// <param name="rule">匹配规则（默认1）：1、包含关键字匹配；2、关键字相等匹配</param>
        /// <returns>返回某类信息</returns>
        private string ParseDataViewByKeyword(IEnumerable<dynamic> matchDataList, string keyword, int rule = 1)
        {
            List<dynamic> matchItems;

            if (rule == 1)
            {
                matchItems = matchDataList.Where(c => c.mimetype.Contains(keyword)).ToList();
            }
            else
            {
                matchItems = matchDataList.Where(c => c.mimetype == keyword).ToList();
            }

            List<string> list = new List<string>();
            foreach (var data in matchItems)
            {
                if (!list.Contains(data.data1))
                {
                    list.Add(data.data1);
                }
            }

            return string.Join(";", list).ToSafeString().TrimEnd(';');
        }

        /// <summary>
        /// 解析组织信息
        /// </summary>
        /// <param name="item">短信对象</param>
        /// <param name="matchDataList">消息信息列表</param>
        private void ParseOrganization(Contact item, IEnumerable<dynamic> matchDataList)
        {
            dynamic matchItem = matchDataList.FirstOrDefault(c => c.mimetype.Contains("organization"));
            if (matchItem != null)
            {
                item.Organization = matchItem.data1 + "  " + matchItem.data4;
            }
        }

        /// <summary>
        /// 根据群组ID，解析群组昵称
        /// </summary>
        /// <param name="item">短信对象</param>
        /// <param name="groupid">群组ID</param>
        /// <param name="groupList">群组列表</param>
        private void ParseGroupName(Contact item, string groupid, IEnumerable<dynamic> groupList)
        {
            int id = groupid.ToSafeInt();
            if (id.IsGreaterThanZero())
            {
                var group = groupList.FirstOrDefault(c => c.xly_id == id);
                if (group != null)
                {
                    item.GroupName = group.title;
                }
            }
        }

    }
}
