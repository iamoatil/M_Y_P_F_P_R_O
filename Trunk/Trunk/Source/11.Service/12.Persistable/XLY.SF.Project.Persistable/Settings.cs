using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;

namespace XLY.SF.Project.Persistable
{
    [Export(typeof(IRecordContext<Basic>))]
    [Export(typeof(IRecordContext<CaseType>))]
    [Export(typeof(IRecordContext<WorkUnit>))]
    [Export(typeof(IRecordContext<Inspection>))]
    [Export(typeof(ISettings))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class Settings : DbContext, ISettings
    {
        #region Constructors

        public Settings()
            : base("System")
        {
        }

        #endregion

        #region Properties

        public DbSet<Basic> Basics { get; set; }

        public DbSet<CaseType> CaseTypes { get; set; }

        public DbSet<WorkUnit> WorkUnits { get; set; }

        public DbSet<Inspection> Inspections { get; set; }

        IQueryable<Basic> IRecordContext<Basic>.Records => Basics;

        IQueryable<CaseType> IRecordContext<CaseType>.Records => CaseTypes;

        IQueryable<WorkUnit> IRecordContext<WorkUnit>.Records => WorkUnits;

        IQueryable<Inspection> IRecordContext<Inspection>.Records => Inspections;

        #endregion

        #region Methods

        public Boolean Add(Basic record)
        {
            if (record == null) return false;
            Basics.Add(record);
            return SaveChanges() == 1;
        }

        public Boolean Add(CaseType record)
        {
            if (record == null) return false;
            CaseTypes.Add(record);
            return SaveChanges() == 1;
        }

        public Boolean Add(WorkUnit record)
        {
            if (record == null) return false;
            WorkUnits.Add(record);
            return SaveChanges() == 1;
        }

        public Boolean Add(Inspection record)
        {
            if (record == null) return false;
            Inspections.Add(record);
            return SaveChanges() == 1;
        }

        public void AddRange(params Basic[] records)
        {
            if (records.Length == 0) return;
            Basics.AddRange(records);
            SaveChanges();
        }

        public void AddRange(params CaseType[] records)
        {
            if (records.Length == 0) return;
            CaseTypes.AddRange(records);
            SaveChanges();
        }

        public void AddRange(params WorkUnit[] records)
        {
            if (records.Length == 0) return;
            WorkUnits.AddRange(records);
            SaveChanges();
        }

        public void AddRange(params Inspection[] records)
        {
            if (records.Length == 0) return;
            Inspections.AddRange(records);
            SaveChanges();
        }

        public Boolean Remove(Basic record)
        {
            if (record == null) return false;
            var attached = Basics.Attach(record);
            if (attached == null) return false;
            Basics.Remove(attached);
            return SaveChanges() == 1;
        }

        public Boolean Remove(CaseType record)
        {
            if (record == null) return false;
            var attached = CaseTypes.Attach(record);
            if (attached == null) return false;
            CaseTypes.Remove(attached);
            return SaveChanges() == 1;
        }

        public Boolean Remove(WorkUnit record)
        {
            if (record == null) return false;
            var attached = WorkUnits.Attach(record);
            if (attached == null) return false;
            WorkUnits.Remove(attached);
            return SaveChanges() == 1;
        }

        public Boolean Remove(Inspection record)
        {
            if (record == null) return false;
            var attached = Inspections.Attach(record);
            if (attached == null) return false;
            Inspections.Remove(attached);
            return SaveChanges() == 1;
        }

        public void RemoveRange(params Basic[] records)
        {
            if (records.Length == 0) return;
            Basics.RemoveRange(records);
            SaveChanges();
        }

        public void RemoveRange(params CaseType[] records)
        {
            if (records.Length == 0) return;
            CaseTypes.RemoveRange(records);
            SaveChanges();
        }

        public void RemoveRange(params WorkUnit[] records)
        {
            if (records.Length == 0) return;
            WorkUnits.RemoveRange(records);
            SaveChanges();
        }

        public void RemoveRange(params Inspection[] records)
        {
            if (records.Length == 0) return;
            Inspections.RemoveRange(records);
            SaveChanges();
        }

        public Boolean Update(Basic record)
        {
            if (record == null) return false;
            var attached = Basics.Attach(record);
            if (attached == null) return false;
            Entry(attached).State = EntityState.Modified;
            return SaveChanges() == 1;
        }

        public Boolean Update(CaseType record)
        {
            if (record == null) return false;
            var attached = CaseTypes.Attach(record);
            if (attached == null) return false;
            Entry(attached).State = EntityState.Modified;
            return SaveChanges() == 1;
        }

        public Boolean Update(WorkUnit record)
        {
            if (record == null) return false;
            var attached = WorkUnits.Attach(record);
            if (attached == null) return false;
            Entry(attached).State = EntityState.Modified;
            return SaveChanges() == 1;
        }

        public Boolean Update(Inspection record)
        {
            if (record == null) return false;
            var attached = Inspections.Attach(record);
            if (attached == null) return false;
            Entry(attached).State = EntityState.Modified;
            return SaveChanges() == 1;
        }

        public String GetValue(String key)
        {
            Basic b = Basics.FirstOrDefault(x => x.Key == key);
            return b?.Value;
        }

        public void SetValue(String key, String value)
        {
            Basic b = Basics.FirstOrDefault(x => x.Key == key);
            if (b != null)
            {
                b.Value = value;
                Update(b);
            }
            else
            {
                b = new Basic
                {
                    Key = key,
                    Value = value
                };
                Add(b);
            }
        }

        #endregion
    }
}
