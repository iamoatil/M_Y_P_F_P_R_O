using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Description：ConstDefinition  
* Author     ：litao
* Create Date：2017/11/27 9:52:15
* ==============================================================================*/

namespace XLY.SF.Project.EarlyWarningView
{
    class ConstDefinition
    {
        public static readonly string RootName = "Root";

        public static readonly string CountrySafety = "CountrySafety";
        public static readonly string PublicSafety = "PublicSafety";
        public static readonly string EconomySafety = "EconomySafety";
        public static readonly string Livehood = "Livehood";
        public static readonly string Custom = "Custom";

        public static readonly List<string> Categorys=new List<string>(){ CountrySafety, PublicSafety, EconomySafety , Livehood, Custom };

        public static readonly string CallCollection = "AppCollection";
        public static readonly string FileMd5Collection = "FileMd5Collection";
        public static readonly string KeyWordCollection = "KeyWordCollection";
        public static readonly string UrlCollection = "UrlCollection";
    }
}
