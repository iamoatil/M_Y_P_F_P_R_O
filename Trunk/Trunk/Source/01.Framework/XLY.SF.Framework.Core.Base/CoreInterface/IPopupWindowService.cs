using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Framework.Core.Base.CoreInterface
{
    public interface IPopupWindowService
    {
        #region 打开文件，选择路径，保存文件服务

        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="filter">筛选，【标签|*.后缀;】</param>
        /// <returns></returns>
        string OpenFileDialog(string filter = "案例项目文件|*.cp");

        /// <summary>
        /// 选择路径
        /// </summary>
        /// <returns></returns>
        string SelectFolderDialog();

        #endregion

        #region 弹窗服务

        /// <summary>
        /// 显示窗体
        /// </summary>
        /// <param name="exportKey">ViewKey</param>
        /// <param name="parameters">参数</param>
        object ShowDialogWindow(string exportKey, object parameters, bool showInTaskBar = false);

        #endregion
    }
}
