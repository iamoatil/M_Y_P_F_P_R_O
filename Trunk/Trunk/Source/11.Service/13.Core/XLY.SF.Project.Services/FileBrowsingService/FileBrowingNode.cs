/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/10/23 19:22:58 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Linq;
using System.Collections.Generic;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;
using XLY.SF.Project.Domains.Contract;

namespace XLY.SF.Project.Services
{
    /// <summary>
    /// 文件节点
    /// </summary>
    public abstract class FileBrowingNode
    {
        private readonly static string[] _LsTxt = { ".txt", ".xml", ".rtf", ".doc", ".wps", ".wpt", ".dot", ".docx", ".dotx", ".docm", ".dotm", ".et", ".ett", ".xls", ".xlt" ,
          ".xlsx", ".xltx", ".xltm", ".dps", ".dpt", ".ppt", ".pot" , ".pptm", ".potx", ".potm", ".pptx", ".pps", ".ppsx", ".ppsm", ".pdf", ".epub", ".mobi", ".chm" };
        private readonly static string[] _LsImage = { ".bmp", ".gif", ".jpeg", ".png", ".tif", ".dng", ".jpg", ".cr2", ".nef", ".arw" };
        private readonly static string[] _LsVoice = { ".mp3", ".amr", ".slk", ".aud", ".wma", ".wav", ".ape", ".flac", ".ogg", ".aac", ".mmf", ".m4r", ".m4a", ".m4b", ".m4p",
            ".midi", ".aiff", ".aaif" };
        private readonly static string[] _LsVideo = { ".avi", ".mp4", ".m4v", ".mov", ".3gp", ".wmv", ".afs", ".asx", ".rm", ".rmvb", ".mpg", ".mpeg", ".mpe", ".dat", ".flv", ".vob" };
        private readonly static string[] _LsRAR = { ".rar", ".zip", ".cab", ".lzh", ".ace", ".7z", ".tar", ".gzip", ".gz", ".uue", ".bz2", ".jar", ".iso", ".z" };
        private readonly static string[] _LsDB = { ".db", ".sqlite", ".sqlitedb" };

        /// <summary>
        /// 父节点
        /// </summary>
        public FileBrowingNode Parent { get; internal set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// 是否是文件 如果不是文件，则是文件夹
        /// </summary>
        public bool IsFile
        {
            get { return NodeType == FileBrowingNodeType.File; }
        }

        /// <summary>
        /// 节点类型
        /// </summary>
        internal FileBrowingNodeType NodeType { get; set; }

        /// <summary>
        /// 是否是删除文件、文件夹
        /// </summary>
        public bool IsDelete
        {
            get { return NodeState == EnumDataState.Deleted || NodeState == EnumDataState.Fragment; }
        }

        private EnumFileType? _FileType = null;

        /// <summary>
        /// 文件类型
        /// 根据后缀名判断
        /// </summary>
        public EnumFileType FileType
        {
            get
            {
                if (_FileType == null)
                {
                    if (IsFile)
                    {
                        var ext = System.IO.Path.GetExtension(Name).ToLower();
                        if (_LsTxt.Contains(ext))
                        {
                            _FileType = EnumFileType.Txt;
                        }
                        else if (_LsImage.Contains(ext))
                        {
                            _FileType = EnumFileType.Image;
                        }
                        else if (_LsVoice.Contains(ext))
                        {
                            _FileType = EnumFileType.Voice;
                        }
                        else if (_LsVideo.Contains(ext))
                        {
                            _FileType = EnumFileType.Video;
                        }
                        else if (_LsRAR.Contains(ext))
                        {
                            _FileType = EnumFileType.Rar;
                        }
                        else if (_LsDB.Contains(ext))
                        {
                            _FileType = EnumFileType.DB;
                        }
                        else
                        {
                            _FileType = EnumFileType.Other;
                        }
                    }
                    else
                    {
                        _FileType = EnumFileType.Directory;
                    }
                }
                return _FileType.Value;
            }
        }

        /// <summary>
        /// 节点状态
        /// </summary>
        public EnumDataState NodeState { get; set; } = EnumDataState.Normal;

        /// <summary>
        /// 文件大小
        /// </summary>
        public UInt64 FileSize { get; internal set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? LastWriteTime { get; internal set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; internal set; }

        /// <summary>
        /// 最后访问时间
        /// </summary>
        public DateTime? LastAccessTime { get; internal set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<FileBrowingNode> ChildNodes { get; internal set; }

        private string _FullPath;

        /// <summary>
        /// 全路径
        /// </summary>
        public string FullPath
        {
            get
            {
                if (null == _FullPath)
                {
                    if (null == Parent)
                    {
                        _FullPath = Name;
                    }
                    else
                    {
                        _FullPath = FileHelper.ConnectPath(Parent.FullPath, Name);
                    }
                }
                return _FullPath;
            }
        }

        /// <summary>
        /// 是否是本地文件
        /// </summary>
        public virtual bool IsLocalFile { get; set; } = false;

        /// <summary>
        /// 本地文件路径
        /// </summary>
        public virtual string LocalFilePath { get; set; }

    }

    /// <summary>
    /// 文件节点类型
    /// </summary>
    public enum FileBrowingNodeType
    {
        /// <summary>
        /// 文件
        /// </summary>
        File,
        /// <summary>
        /// 文件夹
        /// </summary>
        Directory,
        /// <summary>
        /// 根节点，虚拟的，没有实际意义
        /// </summary>
        Root,
        /// <summary>
        /// 分区，安卓镜像文件节点专用
        /// </summary>
        Partition,
    }

    /// <summary>
    /// 文件类型
    /// </summary>
    public enum EnumFileType
    {
        /// <summary>
        /// 所有类型
        /// </summary>
        All = 0,
        /// <summary>
        /// 文件夹
        /// </summary>
        Directory,
        /// <summary>
        /// 文本文件
        /// </summary>
        Txt,
        /// <summary>
        /// 图片文件
        /// </summary>
        Image,
        /// <summary>
        /// 音频文件
        /// </summary>
        Voice,
        /// <summary>
        /// 视频
        /// </summary>
        Video,
        /// <summary>
        /// 压缩文件
        /// </summary>
        Rar,
        /// <summary>
        /// 数据库文件
        /// </summary>
        DB,
        /// <summary>
        /// 其他文件
        /// </summary>
        Other,
    }

    public class FilterByEnumFileTypeArgs : FilterArgs
    {
        public FilterByEnumFileTypeArgs(EnumFileType ft)
        {
            FileType = ft;
        }

        public EnumFileType FileType { get; set; }
    }

}
