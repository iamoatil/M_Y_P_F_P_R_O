/* ==============================================================================
* Description：MirrorCommands  
* Author     ：litao
* Create Date：2017/11/16 20:02:32
* ==============================================================================*/

namespace XLY.SF.Project.MirrorView
{

    /// <summary>
    /// 镜像中使用的命令定义
    /// </summary>
    class MirrorCmdDefinition
    {
        private string _cmdString;
        public MirrorCmdDefinition(string cmd)
        {
            _cmdString = cmd;
        }
        
        /// <summary>
        /// 命令和字符串是否相匹配
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        public bool Match(string cmd)
        {
            if(_cmdString.Equals(cmd, System.StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///命令中字符串的长度。eg：“Start”的长度
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
    }

   /// <summary>
   /// 命令集合
   /// </summary>
    class MirrorCommands
    {
        public readonly MirrorCmdDefinition Operate = new MirrorCmdDefinition("Operate");
        public readonly MirrorCmdDefinition Start = new MirrorCmdDefinition("Start");
        public readonly MirrorCmdDefinition Stop = new MirrorCmdDefinition("Stop");       
        public readonly MirrorCmdDefinition Success = new MirrorCmdDefinition("Success");
        public readonly MirrorCmdDefinition UserStoped = new MirrorCmdDefinition("UserStoped");
        public readonly MirrorCmdDefinition Progress = new MirrorCmdDefinition("Progress");
        public readonly MirrorCmdDefinition Exception = new MirrorCmdDefinition("Exception");
        public readonly MirrorCmdDefinition StopMirror = new MirrorCmdDefinition("StopMirror");
        public readonly MirrorCmdDefinition ContinueMirror = new MirrorCmdDefinition("ContinueMirror");
        public readonly MirrorCmdDefinition PauseMirror = new MirrorCmdDefinition("PauseMirror");
    }
}
