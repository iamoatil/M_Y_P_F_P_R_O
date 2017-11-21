using System.Collections.Generic;
using System.Threading;

/* ==============================================================================
* Description：MirrorControlerBox  
* Author     ：litao
* Create Date：2017/11/20 16:27:07
* ==============================================================================*/

namespace XLY.SF.Project.MirrorView
{
    /// <summary>
    /// 无它，MirrorControler
    /// 因为：个人觉得这种封装更符合一般认识
    /// </summary>
    internal class MirrorControlerBox
    {
        MirrorBackgroundProcess _mirrorBackgroundProcess;
        string _deviceID;
        int _isHtc;
        IStateReporter<CmdString> _stateReporter;
        CmdString _curState;
        PauseInfo _pauseInfo ;

        public MirrorControlerBox(string deviceID, int isHtc, IStateReporter<CmdString> stateReporter, PauseInfo pauseInfo)
        {
            _deviceID = deviceID;
            _isHtc = isHtc;
            _stateReporter = stateReporter;
            _pauseInfo = pauseInfo;
        }

        //此处封装了关键的方法：Start，Stop，Continue，Pause
        internal void Start(List<MirrorBlockInfo> mirrorBlockInfos, int startedFileIndex = 0, long startedPos = 0)
        {
            if (mirrorBlockInfos.Count < 1)
            {
                _stateReporter.Report(CmdStrings.NoSelectedPartition);
                return;
            }

            _curState = null;
            //设置反馈，以及执行命令
            if (_mirrorBackgroundProcess != null)
            {
                _mirrorBackgroundProcess.CallBack -= OnCallBack;
                _mirrorBackgroundProcess.Close();
            }
            _mirrorBackgroundProcess = new MirrorBackgroundProcess();
            _mirrorBackgroundProcess.CallBack += OnCallBack;

            bool isInitialized = _mirrorBackgroundProcess.Initialize();
            if (!isInitialized)
            {
                return;
            }

            for (int i = startedFileIndex; i < mirrorBlockInfos.Count; i++)
            {
                var item = mirrorBlockInfos[i];
                //以下是构建参数
                string arg = string.Format(@"{0}|{1}|{2}|{3}|{4}|{5}", CmdStrings.StartMirror, _deviceID, _isHtc, item.TargetMirrorFile, item.SourceBlockPath, startedPos);
                CmdString startCmd = new CmdString(arg);
                _mirrorBackgroundProcess.ExcuteCmd(startCmd);
                _pauseInfo.SetPauseFileInfo(i, mirrorBlockInfos);

                while (true)
                {
                    if (_curState != null)
                    {
                        if (_curState.Match(CmdStrings.FinishState))
                        {
                            break;
                        }
                        else if (_curState.Match(CmdStrings.StopMirror)
                        || _curState.Match(CmdStrings.Exception))
                        {
                            return;
                        }
                    }

                    Thread.Sleep(1000);
                }
            }

            _mirrorBackgroundProcess.CallBack -= OnCallBack;
            _mirrorBackgroundProcess.Close();
            _stateReporter.Report(CmdStrings.AllFinishState);
        }

        private void OnCallBack(string info)
        {
            CmdString cmd = CmdStrings.UnknowException;
            if (info != null)
            {
                cmd = new CmdString(info);
            }

            _curState = cmd;
            _stateReporter.Report(cmd);
        }

        internal void Stop()
        {
            if (_mirrorBackgroundProcess != null)
            {
                _mirrorBackgroundProcess.ExcuteCmd(CmdStrings.StopMirror);
            }
        }

        internal void Continue()
        {
            if (_mirrorBackgroundProcess != null)
            {
                var mirrorBlockInfos = _pauseInfo.MirrorBlockInfos;
                var pauseFileIndex = _pauseInfo.PauseFileIndex;
                var pausePos = _pauseInfo.PausePos;

                Start(mirrorBlockInfos, pauseFileIndex,pausePos);
            }
        }

        internal void Pause()
        {
            if (_mirrorBackgroundProcess != null)
            {
                _mirrorBackgroundProcess.ExcuteCmd(CmdStrings.PauseMirror);
            }
        }
    }
}
