using System;

/* ==============================================================================
* Description：MyReporter  
* Author     ：litao
* Create Date：2017/11/17 16:27:50
* ==============================================================================*/

namespace XLY.SF.Project.MirrorView
{
    /// <summary>
    /// 状态通知器
    /// </summary>
    internal interface IStateReporter<T>
    {
        void Report(T t);
    }

    /// <summary>
    /// 状态通知器
    /// </summary>

    internal class StateReporter : IStateReporter<CmdString>
    {

        public void Report(CmdString value)
        {
            if (Reported != null)
            {
                Reported(value);
            }
        }

        public event Action<CmdString> Reported;
    }    
}
