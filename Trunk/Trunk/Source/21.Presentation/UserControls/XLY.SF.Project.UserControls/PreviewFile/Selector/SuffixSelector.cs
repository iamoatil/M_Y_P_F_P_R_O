using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Description：SuffixSelector  
* Author     ：litao
* Create Date：2017/10/26 10:40:25
* ==============================================================================*/

namespace XLY.SF.Project.UserControls.PreviewFile.Selector
{
    /// <summary>
    /// 后缀选择器。他根据文件的后缀来选择使用解码器
    /// </summary>
    public class SuffixSelector:ISuffixSelector
    {
        private readonly List<string> _suffixList=new List<string>(); 

        public SuffixSelector(string suffixExp)
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
        public bool IsMatch(string suffix)
        {
            if (_suffixList.Contains(suffix))
            {
                return true;
            }
            return false;
        }
    }
}
