using System;
using System.Diagnostics;
using System.IO;
using XLY.SF.Framework.Log4NetService;

/* ==============================================================================
* Description：MirrorBackgroundProcess  
* Author     ：litao
* Create Date：2017/11/9 13:38:25
* ==============================================================================*/

namespace XLY.SF.Project.MirrorView
{
    class MirrorBackgroundProcess
    {
        Process curProcess;
        string _backgroundAppPath = AppDir.BackgroundAppDir+@"XLY.SF.Project.DataMirrorApp.exe";        

        public bool Initialize()
        {
            ProcessStartInfo start = new ProcessStartInfo(_backgroundAppPath);
            start.CreateNoWindow = true;                //不显示dos命令行窗口   
            start.RedirectStandardOutput = true;
            start.RedirectStandardInput = true;
            start.RedirectStandardError = true;
            start.UseShellExecute = false;              //是否指定操作系统外壳进程启动程序，这里需为false   

            if (!File.Exists(_backgroundAppPath))
            {
                OnCallBack(string.Format("{0}|file:{1} is not existed!", CmdStrings.Exception, _backgroundAppPath));
                return false;
            }
            curProcess = Process.Start(start);
            curProcess.BeginErrorReadLine();
            curProcess.BeginOutputReadLine();
            curProcess.OutputDataReceived += (o, e) => { OnCallBack(e.Data); };
            return true;
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
            try
            {
                curProcess.StandardInput.WriteLine(cmd.ToString());
            }
            catch (Exception ex)
            {
                OnCallBack(string.Format("{0}|{1}",CmdStrings.Exception ,ex.Message));
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
