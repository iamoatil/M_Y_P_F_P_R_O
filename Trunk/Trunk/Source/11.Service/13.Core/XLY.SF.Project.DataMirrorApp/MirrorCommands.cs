/* ==============================================================================
* Description：MirrorCommands  
* Author     ：litao
* Create Date：2017/11/16 20:02:32
* ==============================================================================*/

using System;

namespace XLY.SF.Project.DataMirrorApp
{

    /// <summary>
    /// 命令字符串集合
    /// </summary>
    class CmdString
    {
        private string _cmdString;
        public CmdString(string cmd)
        {
            _cmdString = cmd;
        }    
        /// <summary>
        ///命令中符串的长度。eg：“Start”的长度
        /// </summary>
        public int CmdLength
        {
            get { return _cmdString.Length; }
        }

        /// <summary>
        /// 命令区域的长度。eg：“Start|”的长度
        /// </summary>
        public int CmdAreaLength
        {
            get { return _cmdString.Length + 1; }
        }

        public override string ToString()
        {
            return _cmdString;
        }

        /// <summary>
        ///命令字符串是否相匹配
        /// </summary>
        internal bool Match(CmdString cmd)
        {
            if (_cmdString.Equals(cmd._cmdString, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }        
        
        /// <summary>
        /// 命令是不是cmd类型
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        internal bool IsType(CmdString cmd)
        {
            if (_cmdString.StartsWith(cmd._cmdString, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        internal CmdString Substring(CmdString cmd)
        {
            string str=_cmdString.Substring(cmd.CmdAreaLength);
            return new CmdString(str);
        }
    }

   /// <summary>
   /// 命令字符串集合
   /// </summary>
    class CmdStrings
    {
        //此段定义Background反馈上来的状态。
        //主要有3类，State，Progress，Exception
        //其中State又分为StopState,PauseState.
        //StopState 又分为SuccessStopState，UserStopedState
        public static readonly CmdString State = new CmdString("State");
        public static readonly CmdString StopState = new CmdString("StopState");       
        public static readonly CmdString SuccessStopState = new CmdString("SuccessStopState");
        public static readonly CmdString UserStopedState = new CmdString("UserStopedState");
        public static readonly CmdString Progress = new CmdString("Progress");
        public static readonly CmdString Exception = new CmdString("Exception");

        // 此段定义操作命令
        public static readonly CmdString StartMirror = new CmdString("StartMirror");
        public static readonly CmdString StopMirror = new CmdString("StopMirror");
        public static readonly CmdString ContinueMirror = new CmdString("ContinueMirror");
        public static readonly CmdString PauseMirror = new CmdString("PauseMirror");
    }
}
