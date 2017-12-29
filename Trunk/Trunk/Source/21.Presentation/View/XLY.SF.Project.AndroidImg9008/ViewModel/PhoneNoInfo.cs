using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace XLY.SF.Project.AndroidImg9008
{
    /// <summary>
    /// 支持手机型号信息
    /// </summary>
    public class PhoneNoInfo
    {
        private long _id;

        public long ID
        {
            get { return _id; }
            set { _id = value; }
        }

        private string _phoneNo;

        /// <summary>
        /// 手机型号
        /// </summary>
        public string PhoneNo
        {
            get { return _phoneNo; }
            set { _phoneNo = value; }
        }

        private string _chinese_name;

        /// <summary>
        /// 别名-中文
        /// </summary>
        public string Chinese_Name
        {
            get { return _chinese_name; }
            set { _chinese_name = value; }
        }

        private string _english_name;

        /// <summary>
        /// 别名-英文
        /// </summary>
        public string English_Name
        {
            get { return _english_name; }
            set { _english_name = value; }
        }

        private string _remark;

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get { return _remark; }
            set { _remark = value; }
        }


        /// <summary>
        /// 手机品牌中文
        /// </summary>
        public string PhoneBrand_Cn
        {
            get;
            set;
        }

        /// <summary>
        /// 手机品牌英文
        /// </summary>
        public string PhoneBrand_En
        {
            get;
            set;
        }

        [XmlIgnore]
        public string DisplayPhoneBrand { get; set; }

        /// <summary>
        /// 显示名
        /// </summary>
        public string DisplayName
        { get; set; }
    }
}
