/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/1 11:41:44
 * 接口功能说明：
 * 当外部组件需要接入到宿主时，用于将外部组件
 *
 *************************************************/

using System;

namespace XLY.SF.Framework.Core.Base.ModuleBase
{
    /// <summary>
    /// 外部模块是适配器基类。
    /// </summary>
    public abstract class ExternalModuleAdapter
    {
        #region Events

        /// <summary>
        /// 适配器加载事件。
        /// </summary>
        public event EventHandler Loaded;

        /// <summary>
        /// 适配器卸载事件。
        /// </summary>
        public event EventHandler Unloaded;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Framework.Core.Base.ModuleBase.ExternalModuleAdapter 实例。
        /// </summary>
        protected ExternalModuleAdapter()
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// 标识适配器是否已经加载。
        /// </summary>
        protected Boolean IsLoaded { get; private set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 加载模块
        /// </summary>
        /// <param name="parameter">加载参数。</param>
        public void Load(Object parameter = null)
        {
            if (IsLoaded) return;
            LoadCore(parameter);
            IsLoaded = true;
            Loaded?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 卸载模块。
        /// </summary>
        public void Unload()
        {
            if (!IsLoaded) return;
            UnloadCore();
            IsLoaded = false;
            Unloaded?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// 获取属性值。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        /// <returns>属性值。</returns>
        public abstract Object GetProperty(String propertyName);

        /// <summary>
        /// 设置属性值。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        /// <param name="value">属性值。</param>
        public abstract void SetProperty(String propertyName, Object value);

        /// <summary>
        /// 调用方法。
        /// </summary>
        /// <param name="methodName">方法名称。</param>
        /// <param name="args">参数列表。</param>
        /// <returns>返回值。</returns>
        public abstract Object Invoke(String methodName, Object args = null);

        /// <summary>
        /// 添加事件处理器。
        /// </summary>
        /// <param name="eventName">事件名称。</param>
        /// <param name="handler">事件处理器。</param>
        public abstract void AddEventHandler(String eventName, Delegate handler);

        /// <summary>
        /// 移除事件处理器。
        /// </summary>
        /// <param name="eventName">事件名称。</param>
        /// <param name="handler">事件处理器。</param>
        public abstract void RemoveEventnHandler(String eventName, Delegate handler);

        #endregion

        #region Protected

        /// <summary>
        /// 加载适配器。
        /// </summary>
        /// <param name="parameter">参数。</param>
        protected virtual void LoadCore(Object parameter) { }

        /// <summary>
        /// 卸载适配器。
        /// </summary>
        protected virtual void UnloadCore() { }

        #endregion

        #endregion
    }
}
