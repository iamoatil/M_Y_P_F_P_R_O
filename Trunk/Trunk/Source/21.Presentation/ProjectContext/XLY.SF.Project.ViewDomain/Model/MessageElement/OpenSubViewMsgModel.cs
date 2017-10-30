using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.ViewDomain.Model.MessageElement
{
    /// <summary>
    /// 子界面消息模型
    /// </summary>
    public class SubViewMsgModel
    {
        /// <summary>
        /// 创建子界面消息
        /// </summary>
        /// <param name="needStoryboard">是否需要动画</param>
        /// <param name="isExpandSubView">是否为展开子界面</param>
        public SubViewMsgModel(bool needStoryboard, bool isExpandSubView)
        {
            NeedStoryboard = needStoryboard;
            IsExpandSubView = isExpandSubView;
        }

        /// <summary>
        /// 是否需要动画
        /// </summary>
        public bool NeedStoryboard { get; private set; }

        /// <summary>
        /// 是否为展开子界面
        /// </summary>
        public bool IsExpandSubView { get; private set; }
    }
}
