using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.ViewModel;

namespace XLY.SF.Project.ViewDomain.Model.SelectControlElement
{
    public class FolderElement : NotifyPropertyBase
    {
        private FolderElement()
        {
            SubFolders = new ObservableCollection<FolderElement>();
            Files = new ObservableCollection<FolderElement>();
        }

        /// <summary>
        /// 创建文件夹元素
        /// </summary>
        /// <param name="parentFolder">当前文件夹</param>
        public FolderElement(DirectoryInfo curDir) : this()
        {
            if (curDir == null)
                throw new NullReferenceException("文件夹不能为Null");
            else if (!curDir.Exists)
                throw new NullReferenceException("文件夹不存在");

            IsFolder = true;
            //Parent = parent;
            Name = curDir.Name;
            FullPath = curDir.FullName;
        }

        /// <summary>
        /// 创建文件元素
        /// </summary>
        /// <param name="parentFolder">当前文件</param>
        public FolderElement(FileInfo file) : this()
        {
            if (file == null)
                throw new NullReferenceException("文件不能为Null");
            else if (!file.Exists)
                throw new NullReferenceException("文件不存在");

            //Parent = parent;
            Name = file.Name;
            FullPath = file.FullName;
        }

        #region 名称

        private string _name;
        /// <summary>
        /// 文件夹名
        /// </summary>
        public string Name
        {
            get
            {
                return this._name;
            }

            set
            {
                this._name = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 路径

        private string _fullPath;
        /// <summary>
        /// 路径
        /// </summary>
        public string FullPath
        {
            get
            {
                return this._fullPath;
            }

            set
            {
                this._fullPath = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 父节点

        private FolderElement _parent;
        /// <summary>
        /// 父节点
        /// </summary>
        public FolderElement Parent
        {
            get
            {
                var tmpParent = Directory.GetParent(FullPath);
                if (tmpParent != null)
                    _parent = new FolderElement(tmpParent);
                return _parent;
            }
            private set
            {
                _parent = value;
            }
        }

        #endregion

        #region 文件类型

        private bool _isFolder;
        /// <summary>
        /// 是否为文件夹
        /// </summary>
        public bool IsFolder
        {
            get
            {
                return this._isFolder;
            }

            set
            {
                this._isFolder = value;
                base.OnPropertyChanged();
            }
        }

        #endregion

        #region 子文件夹

        /// <summary>
        /// 子文件夹
        /// </summary>
        public ObservableCollection<FolderElement> SubFolders { get; set; }

        #endregion

        #region 文件

        /// <summary>
        /// 包含的文件
        /// </summary>
        public ObservableCollection<FolderElement> Files { get; set; }

        #endregion

        /// <summary>
        /// 加载子文件夹和文件
        /// </summary>
        public void LoadSubFolderAndFiles(string filter, bool hasFile)
        {
            DirectoryInfo tmpDir = new DirectoryInfo(FullPath);
            LoadSubFolders(tmpDir);
            if (hasFile)
                LoadFiles(tmpDir, filter);
        }

        private void LoadSubFolders(DirectoryInfo dir)
        {
            SubFolders.Clear();
            var securityTmp = dir.GetAccessControl();
            if (!securityTmp.AreAccessRulesProtected || !securityTmp.AreAuditRulesProtected)
            {
                var subDirs = dir.GetDirectories();
                if (subDirs.Length > 0)
                {
                    for (int i = 0; i < subDirs.Length; i++)
                    {
                        if ((subDirs[i].Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                        {
                            FolderElement tmpFolder = new FolderElement(new DirectoryInfo(subDirs[i].FullName));
                            tmpFolder.Parent = this;
                            SubFolders.Add(tmpFolder);
                        }
                    }
                }
            }
        }

        private void LoadFiles(DirectoryInfo dir, string filter)
        {
            Files.Clear();
            var securityTmp = dir.GetAccessControl();
            if (!securityTmp.AreAccessRulesProtected || !securityTmp.AreAuditRulesProtected)
            {
                var subDirs = dir.GetFiles(filter);
                if (subDirs.Length > 0)
                {
                    for (int i = 0; i < subDirs.Length; i++)
                    {
                        if ((subDirs[i].Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
                        {
                            FolderElement fileTmp = new FolderElement(new FileInfo(subDirs[i].FullName));
                            fileTmp.Parent = this;
                            Files.Add(fileTmp);
                        }
                    }
                }
            }
        }
    }
}
