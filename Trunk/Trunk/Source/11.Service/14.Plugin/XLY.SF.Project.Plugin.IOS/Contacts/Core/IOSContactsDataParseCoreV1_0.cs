/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/30 9:55:00 
 * explain :  
 *
*****************************************************************************/

using System.Linq;
using System.Text;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Persistable.Primitive;
using XLY.SF.Project.Services;

namespace XLY.SF.Project.Plugin.IOS
{
    /// <summary>
    /// IOS联系人解析
    /// </summary>
    internal class IOSContactsDataParseCoreV1_0
    {
        /// <summary>
        /// AddressBook.sqlitedb文件路径
        /// </summary>
        private string MainDbPath { get; set; }

        /// <summary>
        /// IOS联系人数据解析核心类
        /// </summary>
        /// <param name="mainDbPath">AddressBook.sqlitedb文件路径</param>
        public IOSContactsDataParseCoreV1_0(string mainDbPath)
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
                var rMainDbFile = SqliteRecoveryHelper.DataRecovery(MainDbPath, @"chalib\IOS_Contact\AddressBook.sqlitedb_V7.charactor", "ABPerson,ABMultiValue,ABGroupMembers,ABGroup", true);
                mainContext = new SqliteContext(rMainDbFile);

                string groupString = "select member_id,Name from ABGroupMembers m left join ABGroup g on m.group_id == g.ROWID";
                var groups = mainContext.Find(groupString);

                mainContext.UsingSafeConnection("select p.*,v.record_id,v.property,v.label,v.[value] from ABPerson p,ABMultiValue v WHERE p.ROWID = v.record_id", r =>
                 {
                     Contact contact;
                     dynamic contactObj;

                     while (r.Read())
                     {
                         contactObj = r.ToDynamic();
                         contact = new Contact();

                         contact.DataState = DynamicConvert.ToEnumByValue(contactObj.XLY_DataType, EnumDataState.Normal);

                         //正常的联系人，目前只处理电话号码的信息，如亲属关系，社交等暂不处理。
                         if (contact.DataState == EnumDataState.Normal)
                         {
                             int propertyId = DynamicConvert.ToSafeInt(contactObj.property);
                             if (propertyId != 3)
                             {
                                 continue;
                             }
                         }

                         contact.Name = DynamicConvert.ToSafeString(contactObj.Last) + DynamicConvert.ToSafeString(contactObj.First);
                         contact.Name = FragmentHelper.RemoveNullityDataNew(contact.Name);
                         contact.Number = DataParseHelper.NumberToStu(DynamicConvert.ToSafeString(contactObj.value));
                         // 号码过滤,验证号码长度
                         if (!DataParseHelper.ValidateNumber(contact.Number))
                         {
                             continue;
                         }

                         //创建时间（最后修改时间）
                         contact.CreateDate = DynamicConvert.ToSafeDateTime(contactObj.ModificationDate, 2001);

                         //联系人分组
                         int contactId = DynamicConvert.ToSafeInt(contactObj.ROWID);
                         var groupObj = groups.FirstOrDefault(g => DynamicConvert.ToSafeInt(g.member_id) == contactId);
                         if (groupObj != null)
                         {
                             contact.GroupName = DynamicConvert.ToSafeString(groupObj.Name);
                         }

                         //基础备注
                         contact.Remark = BuildRemark(contactObj).ToString().TrimStart('；');

                         datasource.Items.Add(contact);
                     }
                 });
            }
            finally
            {
                mainContext?.Dispose();
                mainContext = null;
            }
        }

        private StringBuilder BuildRemark(dynamic contractObj)
        {
            var remarkBuilder = new StringBuilder();

            //号码类型，工作电话、传真电话等。
            string labelId = DynamicConvert.ToSafeString(contractObj.label);
            if (!string.IsNullOrWhiteSpace(labelId))
            {
                remarkBuilder.Append("号码类型:" + GetNumberType(labelId));
            }

            string nickname = DynamicConvert.ToSafeString(contractObj.Nickname);
            if (!string.IsNullOrWhiteSpace(nickname))
            {
                remarkBuilder.Append("; 昵称:" + nickname);
            }

            string organization = DynamicConvert.ToSafeString(contractObj.Organization);
            if (!string.IsNullOrWhiteSpace(organization))
            {
                remarkBuilder.Append("; 组织名称:" + organization);
            }

            string department = DynamicConvert.ToSafeString(contractObj.Department);
            if (!string.IsNullOrWhiteSpace(department))
            {
                remarkBuilder.Append("; 部门名称:" + department);
            }

            string note = DynamicConvert.ToSafeString(contractObj.Note);
            if (!string.IsNullOrWhiteSpace(note))
            {
                remarkBuilder.Append("; 关键备注:" + note);
            }

            string birthday = DynamicConvert.ToSafeString(DynamicConvert.ToSafeDateTime(contractObj.Birthday));
            if (!string.IsNullOrWhiteSpace(birthday))
            {
                remarkBuilder.Append("; 生日:" + birthday);
            }

            string jobTitle = DynamicConvert.ToSafeString(contractObj.JobTitle);
            if (!string.IsNullOrWhiteSpace(jobTitle))
            {
                remarkBuilder.Append("; 工作职务:" + jobTitle);
            }
            return remarkBuilder;
        }

        private string GetNumberType(string labelId)
        {
            switch (labelId)
            {
                case "1":
                    return "移动电话";
                case "2":
                    return "住宅电话";
                case "3":
                    return "工作电话";
                case "4":
                    return "iphone电话";
                case "5":
                    return "主要电话";
                case "6":
                    return "住宅传真";
                case "8":
                    return "工作传真";
                case "10":
                    return "其他传真";
                case "9":
                    return "传呼";
                default:
                    return string.Empty;
            }
        }

    }
}
