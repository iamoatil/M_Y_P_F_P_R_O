// ***********************************************************************
// Assembly:XLY.SF.Project.DataMirror
// Author:Songbing
// Created:2017-04-05 14:17:56
// Description:
// ***********************************************************************
// Last Modified By:
// Last Modified On:
// ***********************************************************************

using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataMirror
{
    /// <summary>
    /// 镜像服务接口
    /// </summary>
    public interface IMirrorService
    {
        /// <summary>
        /// 执行镜像
        /// </summary>
        /// <param name="mirror">镜像源</param>
        /// <param name="asyn">异步通知</param>
        void Execute(Mirror mirror, DefaultAsyncTaskProgress asyn);

        /// <summary>
        /// 停止镜像
        /// </summary>
        void Stop();

        /// <summary>
        /// 是否可以暂停
        /// </summary>
        bool EnableSuspend { get; }

        /// <summary>
        /// 暂停
        /// </summary>
        void Suspend();

        /// <summary>
        /// 继续
        /// </summary>
        void Continue();
    }
}
