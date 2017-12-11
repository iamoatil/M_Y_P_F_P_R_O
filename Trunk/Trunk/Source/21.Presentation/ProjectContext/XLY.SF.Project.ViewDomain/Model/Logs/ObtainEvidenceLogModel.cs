using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;

namespace XLY.SF.Project.ViewDomain.Model
{
    /// <summary>
    /// 操作日志参数
    /// </summary>
    public class ObtainEvidenceLogModel : NotifyPropertyBase
    {
        #region 序号

        private int _index;

        public int Index
        {
            get
            {
                return this._index;
            }

            set
            {
                this._index = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 操作员

        private string _operator;
        /// <summary>
        /// 操作员【显示用】
        /// </summary>
        public string DisplayOperator
        {
            get
            {
                return this._operator;
            }

            set
            {
                this._operator = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 对应模块

        private string _operationModel;
        /// <summary>
        /// 对应的操作模块
        /// </summary>
        public string OperationModel
        {
            get
            {
                return this._operationModel;
            }

            set
            {
                this._operationModel = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 操作内容

        private string _operationContent;
        /// <summary>
        /// 操作内容
        /// </summary>
        public string OpContent
        {
            get
            {
                return this._operationContent;
            }

            set
            {
                this._operationContent = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 操作时间

        private DateTime _operationTime;
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OpTime
        {
            get
            {
                return this._operationTime;
            }

            set
            {
                this._operationTime = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 截图

        /// <summary>
        /// 是否有截图
        /// </summary>
        public bool HasScreenShot
        {
            get
            {
                return !string.IsNullOrWhiteSpace(ImageNameForScreenShot);
            }
        }

        private string _imageNameForScreenShot;
        /// <summary>
        /// 截图保存的文件名【暂保存全路径】
        /// </summary>
        public string ImageNameForScreenShot
        {
            get
            {
                return this._imageNameForScreenShot;
            }

            set
            {
                this._imageNameForScreenShot = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

    }
}
