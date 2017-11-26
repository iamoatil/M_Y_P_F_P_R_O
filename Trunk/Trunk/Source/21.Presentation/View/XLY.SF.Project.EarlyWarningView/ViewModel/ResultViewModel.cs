/* ==============================================================================
* Description：ResultViewModel  
* Author     ：litao
* Create Date：2017/11/24 15:29:04
* ==============================================================================*/

using System.Windows.Input;

namespace XLY.SF.Project.EarlyWarningView
{
    class ResultViewModel
    {
        public ResultViewModel()
        {
            _earlyWarning.Initialize();
            DetectCommand = new RelayCommand(() => { _earlyWarning.Detect(); });
        }

        /// <summary>
        /// 智能预警
        /// </summary>
        DetectionManager _earlyWarning { get { return DetectionManager.Instance; } }

        /// <summary>
        /// 检测命令
        /// </summary>
        public ICommand DetectCommand { get; private set; }

    }
}
