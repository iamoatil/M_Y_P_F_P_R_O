using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.EntityBase;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.Models.Logical;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/3/22 17:07:56
 * 类功能说明：
 * 1. 实现数据库，增、删、改、查
 * 2. 单独多连接查询，需要自己添加实现方法
 *************************************************/

namespace XLY.SF.Project.Persistable
{
    /// <summary>
    /// 数据库管理器【单例模式】
    /// </summary>
    [Export(typeof(IRecordContext<OperationLog>))]
    [Export(typeof(IRecordContext<UserInfo>))]
    [Export(typeof(IRecordContext<RecentCase>))]
    [Export(typeof(IRecordContext<ExtractionPlan>))]
    [Export(typeof(ILogicalDataContext))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class DbService : DbContext, ILogicalDataContext
    {
        #region Constructors

        public DbService()
            : base("System")
        {
            Database.SetInitializer<DbService>(null);
        }

        #endregion

        #region Properties

        public DbSet<OperationLog> OperationLogs { get; set; }

        public DbSet<UserInfo> UserInfos { get; set; }

        public DbSet<RecentCase> RecentCases { get; set; }

        public DbSet<ExtractionPlan> ExtractionPlans { get; set; }

        IQueryable<OperationLog> IRecordContext<OperationLog>.Records => OperationLogs;

        IQueryable<UserInfo> IRecordContext<UserInfo>.Records => UserInfos;

        IQueryable<RecentCase> IRecordContext<RecentCase>.Records => RecentCases;

        IQueryable<ExtractionPlan> IRecordContext<ExtractionPlan>.Records => ExtractionPlans;

        #endregion

        #region Methods

        #region IDatabaseContext

        public Boolean Add<TModel>(TModel model)
            where TModel : LogicalModelBase
        {
            DbSet set = Set(model.Entity.GetType());
            set?.Add(model.Entity);
            return SaveChanges() == 1;
        }

        public void AddRange<TModel>(params TModel[] models)
            where TModel : LogicalModelBase
        {
            if (models.Length == 0) return;
            DbSet set = Set(models[0].Entity.GetType());
            if (set == null) return;
            set.AddRange(models);
            SaveChanges();
        }

        public Boolean Remove<TModel>(TModel model)
            where TModel : LogicalModelBase
        {
            DbSet set = Set(model.Entity.GetType());
            if (set == null) return false;
            Object attached = set.Attach(model.Entity);
            if (attached == null) return false;
            set.Remove(attached);
            return SaveChanges() == 1;
        }

        public void RemoveRange<TModel>(params TModel[] models)
            where TModel : LogicalModelBase
        {
            if (models.Length == 0) return;
            DbSet set = Set(models[0].Entity.GetType());
            if (set == null) return;
            Object[] attaches = new Object[models.Length];
            Object attached = null;
            for (Int32 i = 0; i < models.Length; i++)
            {
                attached = set.Attach(models[i].Entity);
                if (attached == null) return;
                attaches[i] = attached;
            }
            set.RemoveRange(attaches);
            SaveChanges();
        }

        public Boolean Update<TModel>(TModel model)
            where TModel : LogicalModelBase
        {
            DbSet set = Set(model.Entity.GetType());
            if (set == null) return false;
            Object attached = set.Attach(model.Entity);
            if (attached == null) return false;
            Entry(attached).State = EntityState.Modified;
            return SaveChanges() == 1;
        }

        #endregion

        #region IRecordContext

        public Boolean Add(OperationLog record)
        {
            if (record == null) return false;
            OperationLogs.Add(record);
            try
            {
                return SaveChanges() == 1;
            }
            catch (DbEntityValidationException)
            {
                return false;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public Boolean Add(UserInfo record)
        {
            if (record == null) return false;
            UserInfos.Add(record);
            try
            {
                return SaveChanges() == 1;
            }
            catch (DbEntityValidationException)
            {
                return false;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public Boolean Add(RecentCase record)
        {
            if (record == null) return false;
            RecentCases.Add(record);
            try
            {
                return SaveChanges() == 1;
            }
            catch (DbEntityValidationException)
            {
                return false;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public Boolean Add(ExtractionPlan record)
        {
            if (record == null) return false;
            ExtractionPlans.Add(record);
            try
            {
                return SaveChanges() == 1;
            }
            catch (DbEntityValidationException)
            {
                return false;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public void AddRange(params OperationLog[] records)
        {
            if (records.Length == 0) return;
            OperationLogs.AddRange(records);
            try
            {
                SaveChanges();
            }
            catch (DbEntityValidationException)
            {
            }
            catch (DbUpdateException)
            {
            }
        }

        public void AddRange(params UserInfo[] records)
        {
            if (records.Length == 0) return;
            UserInfos.AddRange(records);
            try
            {
                SaveChanges();
            }
            catch (DbEntityValidationException)
            {
            }
            catch (DbUpdateException)
            {
            }
        }

        public void AddRange(params RecentCase[] records)
        {
            if (records.Length == 0) return;
            RecentCases.AddRange(records);
            try
            {
                SaveChanges();
            }
            catch (DbEntityValidationException)
            {
            }
            catch (DbUpdateException)
            {
            }
        }

        public void AddRange(params ExtractionPlan[] records)
        {
            if (records.Length == 0) return;
            ExtractionPlans.AddRange(records);
            try
            {
                SaveChanges();
            }
            catch (DbEntityValidationException)
            {
            }
            catch (DbUpdateException)
            {
            }
        }

        public Boolean Remove(OperationLog record)
        {
            if (record == null) return false;
            var attached = OperationLogs.Attach(record);
            if (attached == null) return false;
            OperationLogs.Remove(attached);
            try
            {
                return SaveChanges() == 1;
            }
            catch (DbEntityValidationException)
            {
                return false;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public Boolean Remove(UserInfo record)
        {
            if (record == null) return false;
            var attached = UserInfos.Attach(record);
            if (attached == null) return false;
            UserInfos.Remove(attached);
            try
            {
                return SaveChanges() == 1;
            }
            catch (DbEntityValidationException)
            {
                return false;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public Boolean Remove(RecentCase record)
        {
            if (record == null) return false;
            var attached = RecentCases.Attach(record);
            if (attached == null) return false;
            RecentCases.Remove(attached);
            try
            {
                return SaveChanges() == 1;
            }
            catch (DbEntityValidationException)
            {
                return false;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public Boolean Remove(ExtractionPlan record)
        {
            if (record == null) return false;
            var attached = ExtractionPlans.Attach(record);
            if (attached == null) return false;
            ExtractionPlans.Remove(attached);
            try
            {
                return SaveChanges() == 1;
            }
            catch (DbEntityValidationException)
            {
                return false;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public void RemoveRange(params OperationLog[] records)
        {
            if (records.Length == 0) return;
            OperationLogs.RemoveRange(records);
            try
            {
                SaveChanges();
            }
            catch (DbEntityValidationException)
            {
            }
            catch (DbUpdateException)
            {
            }
        }

        public void RemoveRange(params UserInfo[] records)
        {
            if (records.Length == 0) return;
            UserInfos.RemoveRange(records);
            try
            {
                SaveChanges();
            }
            catch (DbEntityValidationException)
            {
            }
            catch (DbUpdateException)
            {
            }
        }

        public void RemoveRange(params RecentCase[] records)
        {
            if (records.Length == 0) return;
            RecentCases.RemoveRange(records);
            try
            {
                SaveChanges();
            }
            catch (DbEntityValidationException)
            {
            }
            catch (DbUpdateException)
            {
            }
        }

        public void RemoveRange(params ExtractionPlan[] records)
        {
            if (records.Length == 0) return;
            ExtractionPlans.RemoveRange(records);
            try
            {
                SaveChanges();
            }
            catch (DbEntityValidationException)
            {
            }
            catch (DbUpdateException)
            {
            }
        }

        public Boolean Update(OperationLog record)
        {
            if (record == null) return false;
            var found = OperationLogs.Local.FirstOrDefault(x => x.OperationID == record.OperationID);
            if (found == null) return false;
            var entry = Entry(found);
            entry.CurrentValues.SetValues(record);
            try
            {
                return SaveChanges() == 1;
            }
            catch (DbEntityValidationException)
            {
                return false;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public Boolean Update(UserInfo record)
        {
            if (record == null) return false;
            var found = UserInfos.Local.FirstOrDefault(x=>x.UserID == record.UserID);
            if (found == null) return false;
            var entry = Entry(found);
            entry.CurrentValues.SetValues(record);
            try
            {
                return SaveChanges() == 1;
            }
            catch (DbEntityValidationException ex)
            {
                return false;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public Boolean Update(RecentCase record)
        {
            if (record == null) return false;
            var found = RecentCases.Local.FirstOrDefault(x => x.ID == record.ID);
            if (found == null) return false;
            var entry = Entry(found);
            entry.CurrentValues.SetValues(record);
            try
            {
                return SaveChanges() == 1;
            }
            catch (DbEntityValidationException)
            {
                return false;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        public Boolean Update(ExtractionPlan record)
        {
            if (record == null) return false;
            var found = ExtractionPlans.Local.FirstOrDefault(x => x.ID == record.ID);
            if (found == null) return false;
            var entry = Entry(found);
            entry.CurrentValues.SetValues(record);
            try
            {
                return SaveChanges() == 1;
            }
            catch (DbEntityValidationException)
            {
                return false;
            }
            catch (DbUpdateException)
            {
                return false;
            }
        }

        #endregion

        #endregion
    }
}
