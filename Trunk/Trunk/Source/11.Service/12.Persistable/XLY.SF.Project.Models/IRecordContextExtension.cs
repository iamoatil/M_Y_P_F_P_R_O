using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Models.Logical;

namespace XLY.SF.Project.Models
{
    /// <summary>
    /// 为上层模型封装底层数据库操作。
    /// </summary>
    public interface IRecordContextExtension
    {
        #region Methods

        /// <summary>
        /// 将模型关联的记录添加到底层数据库中。
        /// </summary>
        /// <typeparam name="TModel">模型类型。</typeparam>
        /// <param name="model">模型。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        Boolean Add<TModel>(TModel model)
            where TModel : LogicalModelBase;

        /// <summary>
        /// 将一组模型关联的记录添加到底层数据库中。
        /// </summary>
        /// <typeparam name="TModel">模型类型。</typeparam>
        /// <param name="models">模型列表。</param>
        void AddRange<TModel>(params TModel[] models)
            where TModel : LogicalModelBase;

        /// <summary>
        /// 将模型关联的记录从底层数据库中移除。
        /// </summary>
        /// <typeparam name="TModel">模型类型。</typeparam>
        /// <param name="model">模型。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        Boolean Remove<TModel>(TModel model)
            where TModel : LogicalModelBase;

        /// <summary>
        /// 将一组模型关联的记录从底层数据库中移除。
        /// </summary>
        /// <typeparam name="TModel">模型类型。</typeparam>
        /// <param name="models">模型列表。</param>
        void RemoveRange<TModel>(params TModel[] models)
            where TModel : LogicalModelBase;

        /// <summary>
        /// 将模型关联的记录更新到底层数据库中。
        /// </summary>
        /// <typeparam name="TModel">模型类型。</typeparam>
        /// <param name="model">模型。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        Boolean Update<TModel>(TModel model)
            where TModel : LogicalModelBase;

        #endregion
    }
}
