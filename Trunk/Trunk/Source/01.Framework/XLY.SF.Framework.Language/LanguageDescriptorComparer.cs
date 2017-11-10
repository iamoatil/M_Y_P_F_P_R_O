using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Framework.Language
{
    internal class LanguageDescriptorComparer : IEqualityComparer<LanguageDescriptorAttribute>
    {
        public Boolean Equals(LanguageDescriptorAttribute x, LanguageDescriptorAttribute y)
        {
            return x.Type == y.Type;
        }

        public Int32 GetHashCode(LanguageDescriptorAttribute obj)
        {
            return obj.Type.GetHashCode();
        }
    }
}
