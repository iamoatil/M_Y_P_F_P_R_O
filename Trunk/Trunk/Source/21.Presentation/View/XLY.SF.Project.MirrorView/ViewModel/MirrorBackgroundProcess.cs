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
        public MirrorBackgroundProcess()
        {
            ProcessStartInfo start = new ProcessStartInfo(_backgroundAppPath);
            start.CreateNoWindow = true;                //不显示dos命令行窗口   
            start.RedirectStandardOutput = true;
            start.RedirectStandardInput = true;
            start.RedirectStandardError = true;
            start.UseShellExecute = false;              //是否指定操作系统外壳进程启动程序，这里需为false   
           
            if(!File.Exists(_backgroundAppPath))
            {
                LoggerManagerSingle.Instance.Error("file:"+_backgroundAppPath+" is not existed!");
                return;
            }
            curProcess = Process.Start(start);
            curProcess.BeginErrorReadLine();
            curProcess.BeginOutputReadLine();
            curProcess.OutputDataReceived += new DataReceivedEventHandler(OnOutputDataReceived);
        }

        private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if(CallBack != null )
            {
                CallBack(e.Data);
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
        public void ExcuteCmd(string cmd)
        {
            curProcess.StandardInput.WriteLine(cmd) ;
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
