using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace XLY.SF.Framework.Core.Base.ModuleBase
{
    /// <summary>
    /// 用于外部WPF模块是适配器基类。
    /// </summary>
    public abstract class WpfExternalModuleAdapter : ExternalModuleAdapter
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 XLY.SF.Framework.Core.Base.ModuleBase.WpfExternalModuleAdapter 实例。
        /// </summary>
        protected WpfExternalModuleAdapter()
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// WPF中的UI元素。
        /// </summary>
        public UIElement Element { get; private set; }

        #endregion

        #region Methods

        #region Protected

        /// <summary>
        /// 创建模块的UI元素并加载模块。
        /// </summary>
        /// <param name="parameter">加载参数。</param>
        protected override void LoadCore(Object parameter)
        {
            Element = GenerateUIElement();
        }

        /// <summary>
        /// 卸载模块并销毁模块的UI元素。
        /// </summary>
        protected override void UnloadCore()
        {
            Element = null;
        }

        /// <summary>
        /// 生成UI元素。
        /// </summary>
        /// <returns>UI元素。</returns>
        protected abstract UIElement GenerateUIElement();

        #endregion

        #endregion
    }
}
