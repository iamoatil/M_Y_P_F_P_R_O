using System;
using System.Diagnostics;
using System.IO;

/* ==============================================================================
* Description：MirrorBackgroundProcess  
* Author     ：litao
* Create Date：2017/11/9 13:38:25
* ==============================================================================*/

namespace XLY.SF.Project.MirrorView
{
    class MirrorBackgroundProcess
    {
        Process _curProcess;
        string _backgroundAppPath = AppDir.BackgroundAppDir+@"XLY.SF.Project.DataMirrorApp.exe";
        bool _isInitialized;

        /// <summary>
        /// 初始化。此为关键方法。初始化失败时，此类大多数功能将无法执行
        /// </summary>
        /// <returns></returns>
        public bool Initialize()
        {
            _isInitialized = false;

            ProcessStartInfo start = new ProcessStartInfo(_backgroundAppPath);
            start.CreateNoWindow = true;                //不显示dos命令行窗口   
            start.RedirectStandardOutput = true;
            start.RedirectStandardInput = true;
            start.RedirectStandardError = true;
            start.UseShellExecute = false;              //是否指定操作系统外壳进程启动程序，这里需为false   

            if (!File.Exists(_backgroundAppPath))
            {
                OnCallBack(string.Format("{0}|file:{1} is not existed!", CmdStrings.Exception, _backgroundAppPath));
                _isInitialized = false;
                return _isInitialized;
            }
            try
            {
                _curProcess = Process.Start(start);
                _curProcess.BeginErrorReadLine();
                _curProcess.BeginOutputReadLine();
                _curProcess.OutputDataReceived += (o, e) =>
                {
                    //当镜像进程关闭的时候，会触发一个null的e.Data数据过来。此处用_curProcess.HasExited排除掉
                    if (!((Process)o).HasExited)
                    {
                        OnCallBack(e.Data);
                    }   
                    else
                    {

                    }
                };
            }
            catch (Exception ex)
            {
                OnCallBack(string.Format("{0}|:{1}", CmdStrings.Exception, ex.Message));
                return _isInitialized;
            }

            _isInitialized = true;
            return _isInitialized;
        }

        private void OnCallBack(string msg)
        {
            if(CallBack != null )
            {
                CallBack(msg);
            }            
        }

        /// <summary>
        /// 反馈的数据
        /// </summary>
        public event Action<string> CallBack;

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="cmd"></param>
        public void ExcuteCmd(CmdString cmd)
        {
            if(_isInitialized)
            {
                _curProcess.StandardInput.WriteLine(cmd.ToString());
            }           
        }

        public void Close()
        {
            Process[] pros=Process.GetProcessesByName(Path.GetFileNameWithoutExtension(_backgroundAppPath));
            foreach (var item in pros)
            {
                try
                {
                    item.Kill();
                }
                catch (Exception)
                {
                    
                }                
            }
        }
    }
}
