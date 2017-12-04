using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Project.Models.Entities;

namespace XLY.SF.Project.Models.Logical
{
    /// <summary>
    /// 提取方案
    /// </summary>
    public class ExtractionPlanModel : LogicalModelBase<ExtractionPlan>
    {
        #region Constructors

        public ExtractionPlanModel(ExtractionPlan entity)
            : base(entity)
        {
        }

        public ExtractionPlanModel()
        {
        }

        #endregion

        #region Properties

        [Required]
        public Int32 ID => Entity.ID;

        /// <summary>
        /// 方案名称。
        /// </summary>
        [Required]
        [StringLength(50)]
        public String Name
        {
            get => Entity.Name;
            set => Entity.Name = value;
        }

        /// <summary>
        /// 提取项的Token列表。
        /// </summary>
        public IEnumerable<String> ExtractItemTokens
        {
            get
            {
                if (Entity.ExtractItems != null)
                {
                    return Entity.ExtractItems.Split(';');
                }
                return new String[0];
            }
            set
            {
                if (value != null)
                {
                    Entity.ExtractItems = String.Join(";", value);
                }
                else
                {
                    Entity.ExtractItems = null;
                }
            }
        }

        #endregion
    }
}
