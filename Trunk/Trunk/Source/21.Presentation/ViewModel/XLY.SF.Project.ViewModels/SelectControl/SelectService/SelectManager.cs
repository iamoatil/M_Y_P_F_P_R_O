using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.Enums;
using XLY.SF.Project.ViewDomain.Model.SelectControlElement;

namespace XLY.SF.Project.ViewModels.SelectControl.SelectService
{
    public class SelectManager : NotifyPropertyBase
    {
        #region Properties

        /// <summary>
        /// 当前选择管理器类型
        /// </summary>
        private SelectControlType _curStatus;

        ///// <summary>
        ///// 文件过滤
        ///// </summary>
        //private string _fileFilter;

        #region 当前路径【文件夹层级】

        private FolderElement _curFolderLevel;
        /// <summary>
        /// 当前文件夹层级
        /// </summary>
        public FolderElement CurFolderLevel
        {
            get
            {
                return this._curFolderLevel;
            }

            set
            {
                this._curFolderLevel = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #endregion

        #region 公用方法

        /// <summary>
        /// 创建文件夹选择管理器
        /// </summary>
        /// <param name="status"></param>
        public SelectManager(SelectControlType status)
        {
            _curStatus = status;
        }

        ///// <summary>
        ///// 创建文件选择管理器
        ///// </summary>
        ///// <param name="status"></param>
        ///// <param name="filter"></param>
        //public SelectManager(SelectControlType status, string filter)
        //    : this(status)
        //{
        //    _fileFilter = filter;
        //}

        /// <summary>
        /// 进入文件夹
        /// </summary>
        /// <param name="inFolder"></param>
        public List<FolderElement> InFolderAndUpdateLevel(FolderElement inFolder, string filter = "*.*")
        {
            List<FolderElement> result = new List<FolderElement>();
            if (inFolder != null && inFolder.IsFolder)
            {
                //刷新当前目录
                inFolder.LoadSubFolderAndFiles(filter, _curStatus == SelectControlType.SelectFile);
                //刷新子目录
                foreach (var item in inFolder.SubFolders)
                {
                    item.LoadSubFolderAndFiles(filter, _curStatus == SelectControlType.SelectFile);
                }
                result = GetFolderItems(inFolder);
                //记录层级
                CurFolderLevel = inFolder;
            }
            return result;
        }

        /// <summary>
        /// 初始化文件夹
        /// </summary>
        /// <returns></returns>
        public List<FolderElement> InitFolders(string filter = "*.*")
        {
            List<FolderElement> result = new List<FolderElement>();
            //桌面
            result.Add(new FolderElement(new System.IO.DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.Desktop))));
            result.Last().LoadSubFolderAndFiles(filter, _curStatus == SelectControlType.SelectFile);
            if (SystemContext.LanguageManager.Type == Framework.Language.LanguageType.Cn)
                result.Last().Name = "桌面";
            //我的文档
            result.Add(new FolderElement(new System.IO.DirectoryInfo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))));
            result.Last().LoadSubFolderAndFiles(filter, _curStatus == SelectControlType.SelectFile);
            if (SystemContext.LanguageManager.Type == Framework.Language.LanguageType.Cn)
                result.Last().Name = "我的文档";

            foreach (var item in DriveInfo.GetDrives())
            {
                DirectoryInfo dirTmp = new System.IO.DirectoryInfo(item.Name);
                if (dirTmp.Exists)
                {
                    FolderElement tmpEmt = new FolderElement(dirTmp);
                    tmpEmt.LoadSubFolderAndFiles(filter, _curStatus == SelectControlType.SelectFile);
                    tmpEmt.Name = string.Format("{0}（{1}）", string.IsNullOrWhiteSpace(item.VolumeLabel) ? "本地磁盘" : item.VolumeLabel, item.Name);
                    result.Add(tmpEmt);
                }
            }
            return result;
        }

        /// <summary>
        /// 进入指定的路径
        /// </summary>
        /// <param name="inputPath"></param>
        private void InFolderByInput(string inputPath)
        {
            if (!string.IsNullOrWhiteSpace(inputPath) && Directory.Exists(inputPath))
            {
                FolderElement inputFolder = new FolderElement(new DirectoryInfo(inputPath));
            }
        }

        /// <summary>
        /// 获取新文件夹的名称
        /// </summary>
        /// <param name="curPath">当前新建文件夹的路径</param>
        /// <returns></returns>
        public string GetNewFolderName(string curPath)
        {
            string newFolderName = string.Empty;
            if (!string.IsNullOrWhiteSpace(curPath))
            {
                newFolderName = Path.Combine(curPath, "新建文件夹");
                int folderIndex = 1;
                while (Directory.Exists(newFolderName))
                {
                    newFolderName = Path.Combine(curPath, string.Format("新建文件夹（{0}）", folderIndex));
                    folderIndex++;
                }
            }

            return newFolderName;
        }

        #endregion

        #region Tools

        /// <summary>
        /// 刷新界面显示
        /// </summary>
        /// <param name="curFolder"></param>
        private List<FolderElement> GetFolderItems(FolderElement curFolder)
        {
            List<FolderElement> result = new List<FolderElement>();
            //加载文件夹
            foreach (var item in curFolder.SubFolders)
            {
                result.Add(item);
            }
            if (_curStatus == SelectControlType.SelectFile)
            {
                //加载文件
                foreach (var item in curFolder.Files)
                {
                    result.Add(item);
                }
            }

            return result;
        }

        #endregion
    }
}
