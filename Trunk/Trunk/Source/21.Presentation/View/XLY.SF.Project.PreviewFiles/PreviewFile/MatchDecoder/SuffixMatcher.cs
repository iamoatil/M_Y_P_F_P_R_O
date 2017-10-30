using System.Collections.Generic;

/* ==============================================================================
* Description：SuffixSelector  
* Author     ：litao
* Create Date：2017/10/26 10:40:25
* ==============================================================================*/

namespace XLY.SF.Project.UserControls.PreviewFile.MatchDecoder
{
    /// <summary>
    /// 后缀匹配器。他根据文件的后缀来选择使用解码器
    /// </summary>
    public class SuffixMatcher: IMatched
    {
        private readonly List<string> _suffixList=new List<string>(); 

        public SuffixMatcher(string suffixExp)
        {
            string[] suffixes = suffixExp.Split('|');
            foreach (var suffix in suffixes)
            {
                string lowerSuffix = suffix.ToLower();
                _suffixList.Add(lowerSuffix);
            }
        }

        /// <summary>
        /// 是否匹配
        /// </summary>
        /// <returns></returns>
        public bool Match(string suffix)
        {
            if (_suffixList.Contains(suffix))
            {
                return true;
            }
            return false;
        }
    }
}
