using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* ==============================================================================
* Assembly   ：	XLY.SF.Project.DataDisplayView.AsyncOperator
* Description：	  
* Author     ：	fhjun
* Create Date：	2017/11/20 16:03:25
* ==============================================================================*/

namespace XLY.SF.Project.DataDisplayView
{
    /// <summary>
    /// AsyncOperator
    /// </summary>
    public class AsyncOperator
    {
        #region 可以用于异步操作的对象
        /// <summary>
        /// 可以用于异步操作的对象，主要用于非UI线程中替换dispatcher
        /// </summary>
        public static AsyncOperation AsyncOperation { get; private set; }
        /// <summary>
        /// 加载异步操作对象
        /// </summary>
        public static void LoadAsyncOperation(object owner)
        {
            AsyncOperation = AsyncOperationManager.CreateOperation(owner);
        }

        /// <summary>
        /// 执行异步方法
        /// </summary>
        /// <param name="method"></param>
        public static void Execute(Action method)
        {
            AsyncOperation.Post(t =>
            {
                method();
            }, null);
        }
        #endregion
    }
}
