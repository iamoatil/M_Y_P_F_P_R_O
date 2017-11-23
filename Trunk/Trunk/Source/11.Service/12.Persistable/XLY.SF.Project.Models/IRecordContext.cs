using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Models.Entities;

namespace XLY.SF.Project.Models
{
    /// <summary>
    /// 记录上下文。
    /// </summary>
    /// <typeparam name="TEntity">记录的类型。</typeparam>
    public interface IRecordContext<TEntity>
        where TEntity : IEntity
    {
        #region Properties

        /// <summary>
        /// 所有记录。
        /// </summary>
        IQueryable<TEntity> Records { get; }

        #endregion

        #region Methods

        /// <summary>
        /// 添加一条记录。
        /// </summary>
        /// <param name="record">记录。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        Boolean Add(TEntity record);

        /// <summary>
        /// 添加一组记录。
        /// </summary>
        /// <param name="records">记录列表。</param>
        void AddRange(params TEntity[] records);

        /// <summary>
        /// 移除一条记录。
        /// </summary>
        /// <param name="record">记录。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        Boolean Remove(TEntity record);

        /// <summary>
        /// 移除一组记录。
        /// </summary>
        /// <param name="records">记录列表。</param>
        void RemoveRange(params TEntity[] records);

        /// <summary>
        /// 更新一条记录。
        /// </summary>
        /// <param name="record">记录。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        Boolean Update(TEntity record);

        #endregion
    }
}
