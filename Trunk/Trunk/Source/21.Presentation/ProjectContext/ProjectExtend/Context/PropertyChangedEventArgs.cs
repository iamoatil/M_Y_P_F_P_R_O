using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectExtend.Context
{
    public class PropertyChangedEventArgs<T> : EventArgs
    {
        #region Constructors

        public PropertyChangedEventArgs(T oldValue,T newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        #endregion

        #region Properties

        public T OldValue { get; }

        public T NewValue { get; }

        #endregion
    }
}
