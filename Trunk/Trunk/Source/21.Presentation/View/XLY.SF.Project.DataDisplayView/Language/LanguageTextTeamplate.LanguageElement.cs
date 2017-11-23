
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XLY.SF.Project.DataDisplayView
{

	internal class Languagekeys
	{
		
        /// <summary>
        /// 清空搜索条件
        /// </summary>
		public static readonly Languagekeys ClearTip = "LanguageResource/ClearTip";
				
        /// <summary>
        /// 搜索
        /// </summary>
		public static readonly Languagekeys Search = "LanguageResource/Search";
				
        /// <summary>
        /// 所有应用
        /// </summary>
		public static readonly Languagekeys AllApps = "LanguageResource/AllApps";
				
        /// <summary>
        /// 请输入关键词
        /// </summary>
		public static readonly Languagekeys KeyWordWatermark = "LanguageResource/KeyWordWatermark";
				
        /// <summary>
        /// 未提取到任何数据
        /// </summary>
		public static readonly Languagekeys TipNoData = "LanguageResource/TipNoData";
				
        /// <summary>
        /// 请尝试其他方案
        /// </summary>
		public static readonly Languagekeys TipNoData2 = "LanguageResource/TipNoData2";
				
        /// <summary>
        /// 正在查询数据...
        /// </summary>
		public static readonly Languagekeys Searching = "LanguageResource/Searching";
				
        /// <summary>
        /// 所有标记
        /// </summary>
		public static readonly Languagekeys BookmarkAll = "LanguageResource/BookmarkAll";
				
        /// <summary>
        /// 未标记
        /// </summary>
		public static readonly Languagekeys BookmarkNone = "LanguageResource/BookmarkNone";
				
        /// <summary>
        /// 已标记
        /// </summary>
		public static readonly Languagekeys BookmarkYes = "LanguageResource/BookmarkYes";
				
        /// <summary>
        /// 所有状态
        /// </summary>
		public static readonly Languagekeys DataStateAll = "LanguageResource/DataStateAll";
				
        /// <summary>
        /// 正常数据
        /// </summary>
		public static readonly Languagekeys DataStateNormal = "LanguageResource/DataStateNormal";
				
        /// <summary>
        /// 删除数据
        /// </summary>
		public static readonly Languagekeys DataStateDelete = "LanguageResource/DataStateDelete";
				
        /// <summary>
        /// 关键词
        /// </summary>
		public static readonly Languagekeys Keyword = "LanguageResource/Keyword";
				
        /// <summary>
        /// 正则
        /// </summary>
		public static readonly Languagekeys ZhengZe = "LanguageResource/ZhengZe";
		
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

