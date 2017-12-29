
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.SF.Project.Plugin.DataView
{

	internal class Languagekeys
	{
		
        /// <summary>
        /// 表格视图
        /// </summary>
		public static readonly Languagekeys GridView = "LanguageResource/GridView";
				
        /// <summary>
        /// 联系人视图
        /// </summary>
		public static readonly Languagekeys ContactView = "LanguageResource/ContactView";
				
        /// <summary>
        /// 消息视图
        /// </summary>
		public static readonly Languagekeys MessageView = "LanguageResource/MessageView";
				
        /// <summary>
        /// 对话模式
        /// </summary>
		public static readonly Languagekeys ConversionMode = "LanguageResource/ConversionMode";
				
        /// <summary>
        /// 默认布局
        /// </summary>
		public static readonly Languagekeys DefaultLayout = "LanguageResource/DefaultLayout";
				
        /// <summary>
        /// 手机布局
        /// </summary>
		public static readonly Languagekeys PhoneLayout = "LanguageResource/PhoneLayout";
				
        /// <summary>
        /// 标记
        /// </summary>
		public static readonly Languagekeys Bookmark = "LanguageResource/Bookmark";
				
        /// <summary>
        /// 页面列表
        /// </summary>
		public static readonly Languagekeys PhonePageList = "LanguageResource/PhonePageList";
				
        /// <summary>
        /// 主页
        /// </summary>
		public static readonly Languagekeys PhoneHome = "LanguageResource/PhoneHome";
				
        /// <summary>
        /// 后退
        /// </summary>
		public static readonly Languagekeys PhoneBack = "LanguageResource/PhoneBack";
				
        /// <summary>
        /// 微信
        /// </summary>
		public static readonly Languagekeys WeChat = "LanguageResource/WeChat";
				
        /// <summary>
        /// 登录
        /// </summary>
		public static readonly Languagekeys DengLu = "LanguageResource/DengLu";
				
        /// <summary>
        /// 正在执行操作,请稍后...
        /// </summary>
		public static readonly Languagekeys OperatingTip = "LanguageResource/OperatingTip";
		
		public string Key { get; set; }

        public static implicit operator string(Languagekeys d)
        {
            return LanguageHelper.Get(d.Key);
        }

        public static implicit operator Languagekeys(string d)
        {
            return new Languagekeys() { Key = d };
        }

        public override string ToString()
        {
            return this;
        }
	}
}

