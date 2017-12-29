// ***********************************************************************
// Assembly:XLY.SF.Project.DataMirror
// Author:Songbing
// Created:2017-04-05 14:26:50
// Description:
// ***********************************************************************
// Last Modified By:
// Last Modified On:
// ***********************************************************************

using System;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.DataMirror
{
    /// <summary>
    /// 镜像服务抽象类
    /// </summary>
    public abstract class AbstractMirrorService : IMirrorService
    {
        /// <summary>
        /// 执行镜像
        /// </summary>
        /// <param name="mirror"></param>
        /// <param name="asyn"></param>
        public abstract void Execute(Mirror mirror, DefaultAsyncTaskProgress asyn);

        /// <summary>
        /// 停止
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// 是否可以暂停
        /// 默认不可暂停
        /// 如果可以暂停，请重写Suspend和Continue方法
        /// </summary>
        public virtual bool EnableSuspend
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 暂停
        /// </summary>
        public virtual void Suspend()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 继续
        /// </summary>
        public virtual void Continue()
        {
            throw new NotImplementedException();
        }

    }
}
