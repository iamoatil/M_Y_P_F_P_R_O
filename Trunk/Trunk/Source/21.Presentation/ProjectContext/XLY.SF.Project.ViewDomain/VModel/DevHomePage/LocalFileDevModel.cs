using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XLY.SF.Project.ViewDomain.VModel.DevHomePage
{
    /// <summary>
    /// 文件设备
    /// </summary>
    public class LocalFileDevModel : DeviceModel
    {
        public LocalFileDevModel()
        {

        }

        #region 文件类型

        private string _fileTypeName;
        /// <summary>
        /// 文件类型
        /// </summary>
        public string FileTypeName
        {
            get
            {
                return this._fileTypeName;
            }

            set
            {
                this._fileTypeName = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 文件路径

        private string _filePath;
        /// <summary>
        /// 文件路径
        /// </summary>
        public string FilePath
        {
            get
            {
                return this._filePath;
            }

            set
            {
                this._filePath = value;
                base.OnPropertyChanged();
            }
        }

        #endregion
    }
}
