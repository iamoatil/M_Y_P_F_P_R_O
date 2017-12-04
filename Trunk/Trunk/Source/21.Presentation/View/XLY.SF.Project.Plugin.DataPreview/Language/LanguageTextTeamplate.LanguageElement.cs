
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.SF.Project.Plugin.DataPreview
{

	internal class Languagekeys
	{
		
        /// <summary>
        /// 音频播放
        /// </summary>
		public static readonly Languagekeys AudioView = "LanguageResource/AudioView";
				
        /// <summary>
        /// 基本信息
        /// </summary>
		public static readonly Languagekeys BasicView = "LanguageResource/BasicView";
				
        /// <summary>
        /// 网页查看
        /// </summary>
		public static readonly Languagekeys HtmlView = "LanguageResource/HtmlView";
				
        /// <summary>
        /// 图片查看
        /// </summary>
		public static readonly Languagekeys ImageView = "LanguageResource/ImageView";
				
        /// <summary>
        /// 文本查看
        /// </summary>
		public static readonly Languagekeys TextView = "LanguageResource/TextView";
				
        /// <summary>
        /// 视频播放
        /// </summary>
		public static readonly Languagekeys VedioView = "LanguageResource/VedioView";
				
        /// <summary>
        /// Office文件
        /// </summary>
		public static readonly Languagekeys OfficeView = "LanguageResource/OfficeView";
				
        /// <summary>
        /// 开始
        /// </summary>
		public static readonly Languagekeys Start = "LanguageResource/Start";
				
        /// <summary>
        /// 暂停
        /// </summary>
		public static readonly Languagekeys Pause = "LanguageResource/Pause";
				
        /// <summary>
        /// 继续
        /// </summary>
		public static readonly Languagekeys Continue = "LanguageResource/Continue";
				
        /// <summary>
        /// 停止
        /// </summary>
		public static readonly Languagekeys Stop = "LanguageResource/Stop";
				
        /// <summary>
        /// 文件名
        /// </summary>
		public static readonly Languagekeys FileName = "LanguageResource/FileName";
				
        /// <summary>
        /// 全路径
        /// </summary>
		public static readonly Languagekeys FullPath = "LanguageResource/FullPath";
				
        /// <summary>
        /// 大小
        /// </summary>
		public static readonly Languagekeys FileSize = "LanguageResource/FileSize";
				
        /// <summary>
        /// 创建时间
        /// </summary>
		public static readonly Languagekeys CreateTime = "LanguageResource/CreateTime";
				
        /// <summary>
        /// 修改时间
        /// </summary>
		public static readonly Languagekeys ModifyTime = "LanguageResource/ModifyTime";
				
        /// <summary>
        /// 访问时间
        /// </summary>
		public static readonly Languagekeys AccessTime = "LanguageResource/AccessTime";
				
        /// <summary>
        /// 属性
        /// </summary>
		public static readonly Languagekeys Attributes = "LanguageResource/Attributes";
				
        /// <summary>
        /// 只读
        /// </summary>
		public static readonly Languagekeys ZhiDu = "LanguageResource/ZhiDu";
				
        /// <summary>
        /// 字节
        /// </summary>
		public static readonly Languagekeys Bytes = "LanguageResource/Bytes";
				
        /// <summary>
        /// 16进制
        /// </summary>
		public static readonly Languagekeys Hex = "LanguageResource/Hex";
		
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

