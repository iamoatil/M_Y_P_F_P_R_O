/***************************************************************************
 * 
 * author :  Songbing
 * time: 2017/12/15 14:02:13 
 * explain :  
 *
*****************************************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using XLY.SF.Framework.BaseUtility;
using XLY.SF.Project.BaseUtility.Helper;
using XLY.SF.Project.Domains;

namespace XLY.SF.Project.Services
{
    /// <summary>
    /// 文件提取辅助类
    /// </summary>
    public static class FileDataParser
    {
        public static void GetAndroidPhoneTreeFiles(Device phone, string databasesPath, string fileSavePath, FileTreeDataSource dataSource, string dataItemsFilePath, EnumColumnType columnType)
        {
            if (null == phone || !FileHelper.IsValidDictory(databasesPath) || null == dataSource)
            {
                return;
            }

            TreeNode node = CreateFileTreeNode("所有", dataItemsFilePath);
            TreeNode normalNode = CreateFileTreeNode("正常", dataItemsFilePath);
            TreeNode deleteNode = CreateFileTreeNode("删除", dataItemsFilePath);
            TreeNode classifyNode = CreateFileTreeNode("扩展名分类", dataItemsFilePath);
            TreeNode fileNode = CreateFileTreeNode("文件夹分类", dataItemsFilePath);

            node.IsIncludeChildrenInTotal = false;
            node.TreeNodes.Add(normalNode);
            node.TreeNodes.Add(deleteNode);

            dataSource.TreeNodes.Add(node);
            dataSource.TreeNodes.Add(classifyNode);
            dataSource.TreeNodes.Add(fileNode);

            var dbFiles = Directory.GetFiles(databasesPath, "external*.db", SearchOption.TopDirectoryOnly);
            if (dbFiles.IsInvalid())
            {
                return;
            }

            using (var context = new SqliteContext(dbFiles.First()))
            {
                IEnumerable<dynamic> dataInfos = null;

                switch (columnType)
                {
                    case EnumColumnType.Image:
                        dataInfos = context.FindByName("images");
                        break;
                    case EnumColumnType.Audio:
                        dataInfos = context.FindByName("audio");
                        break;
                    case EnumColumnType.Video:
                        dataInfos = context.FindByName("video");
                        break;
                    case EnumColumnType.Word:
                        dataInfos = context.Find(new SQLiteString("SELECT * FROM files WHERE mime_type LIKE 'text%' OR mime_type LIKE 'application%'"));
                        break;
                    default:
                        return;
                }

                if (dataInfos.IsInvalid())
                {
                    return;
                }

                //获取SDCard根目录
                var res = context.Find(new SQLiteString("select * from files where _data like '%DCIM' limit 0,1"));
                var sdcard = string.Empty;
                if (res.IsValid())
                {
                    sdcard = res.First().xly_data.ToString();
                    sdcard = sdcard.TrimEnd("DCIM", StringComparison.OrdinalIgnoreCase);
                }
                bool replace = sdcard != phone.SDCardPath;

                string source, localFile, condition;
                FileX fileX;
                foreach (var data in dataInfos)
                {
                    source = DynamicConvert.ToSafeString(data.xly_data);

                    //sdcard路径处理
                    if (replace)
                    {
                        source = source.Replace(sdcard, phone.SDCardPath);
                    }

                    localFile = phone.CopyFile(source, fileSavePath, null);

                    if (FileHelper.IsExist(localFile))
                    {
                        fileX = CreateFlieX(data, localFile, columnType);
                        node.Items.Add(fileX);

                        if (fileX.Name.StartsWith("Del_"))
                        {
                            fileX.DataState = EnumDataState.Deleted;
                            deleteNode.Items.Add(fileX);
                        }
                        else
                        {
                            normalNode.Items.Add(fileX);
                        }

                        condition = FileHelper.GetExtension(localFile);

                        IsExitNode(classifyNode, fileX, condition, dataItemsFilePath);

                        IsExitNode(fileNode, fileX, new FileInfo(localFile).Directory.Parent.Parent.Name, dataItemsFilePath);
                    }
                }
            }
        }

        public static void GetImageFiles(FileTreeDataSource dataSource, string dataItemsFilePath, string folderPath, string[] extensions = null)
        {
            if (extensions == null)
            {
                extensions = new[] { ".jpg", ".jpeg", ".bmp", ".png", ".exif", ".dxf", ".pcx", ".fpx", ".ufo", ".tiff", ".svg", ".eps", ".gif", ".psd", ".ai", ".cdr", ".tga", ".pcd", ".hdri", ".map" };
            }

            GetTreeFiles(dataSource, dataItemsFilePath, folderPath, EnumColumnType.Image, extensions);
        }

        public static void GetAudioFiles(FileTreeDataSource dataSource, string dataItemsFilePath, string folderPath, string[] extensions = null)
        {
            if (extensions == null)
            {
                extensions = new[] { ".m4a", ".mpeg-4", ".mp3", ".wma", ".wav", ".ape", ".acc", ".ogg", ".amr", ".3ga" };
            }

            GetTreeFiles(dataSource, dataItemsFilePath, folderPath, EnumColumnType.Audio, extensions);
        }

        public static void GetVideoFiles(FileTreeDataSource dataSource, string dataItemsFilePath, string folderPath, string[] extensions = null)
        {
            if (extensions == null)
            {
                extensions = new[] { ".mp4", ".mpeg", ".mpg", ".dat", ".avi", ".m4v", ".mov", ".3gp", ".rm", ".flv", ".wmv", ".asf", ".navi", "mkv", "f4v", "rmvb", ".webm", ".real video" };
            }

            GetTreeFiles(dataSource, dataItemsFilePath, folderPath, EnumColumnType.Video, extensions);
        }

        public static void GetWordFiles(FileTreeDataSource dataSource, string dataItemsFilePath, string folderPath, string[] extensions = null)
        {
            if (extensions == null)
            {
                extensions = new[] { ".txt", ".rtf", ".doc", ".wps", ".wpt", ".doc", ".dot", ".docx", ".dotx", ".docm", ".dotm", ".et", ".ett", ".xls", ".xlt", ".xlsx", ".xlsm", ".xltx", ".xltm", ".dps", ".dpt", ".ppt", ".pot", ".pptm", ".potx", ".potm", ".pptx", ".pps", ".ppsx", ".ppsm", ".pdf", ".epub", ".mobi", ".ch", ".zip", ".rar" };
            }

            GetTreeFiles(dataSource, dataItemsFilePath, folderPath, EnumColumnType.Word, extensions);
        }

        private static void GetTreeFiles(FileTreeDataSource dataSource, string dataItemsFilePath, string folderPath, EnumColumnType columnType, string[] extensions)
        {
            if (null == dataSource || !FileHelper.IsValidDictory(folderPath) || !extensions.Any())
            {
                return;
            }

            var allSuitableFiles = FileHelper.GetFiles(folderPath, extensions);
            if (allSuitableFiles.IsInvalid())
            {
                return;
            }

            TreeNode node = CreateFileTreeNode("所有", dataItemsFilePath);
            TreeNode normalNode = CreateFileTreeNode("正常", dataItemsFilePath);
            TreeNode deleteNode = CreateFileTreeNode("删除", dataItemsFilePath);
            TreeNode classifyNode = CreateFileTreeNode("扩展名分类", dataItemsFilePath);
            TreeNode fileNode = CreateFileTreeNode("文件夹分类", dataItemsFilePath);

            node.IsIncludeChildrenInTotal = false;
            node.TreeNodes.Add(normalNode);
            node.TreeNodes.Add(deleteNode);

            dataSource.TreeNodes.Add(node);
            dataSource.TreeNodes.Add(classifyNode);
            dataSource.TreeNodes.Add(fileNode);

            foreach (var file in allSuitableFiles)
            {
                string oldPath = file.Directory.FullName;
                var fileX = new FileX();
                fileX.Name = file.Name;
                fileX.Type = columnType;
                fileX.ParentDirectory = file.Directory.FullName;//path;//
                fileX.CreationDate = file.CreationTime;
                fileX.LastAccessDate = file.LastAccessTime;
                fileX.LastWriteData = file.LastWriteTime;
                fileX.Size = file.Length;

                node.Items.Add(fileX);

                if (file.Name.StartsWith("Del_"))
                {
                    fileX.DataState = EnumDataState.Deleted;
                    deleteNode.Items.Add(fileX);
                }
                else
                {
                    normalNode.Items.Add(fileX);
                }

                IsExitNode(classifyNode, fileX, FileHelper.GetExtension(fileX.Name), dataItemsFilePath);

                IsExitNode(fileNode, fileX, file.Directory.Parent.Parent.Name, dataItemsFilePath);

            }
        }

        private static FileX CreateFlieX(dynamic data, string path, EnumColumnType fileType)
        {
            FileX file = new FileX();
            file.DataState = EnumDataState.Normal;
            file.Name = DynamicConvert.ToSafeString(data.xly_display_name);
            if (file.Name.IsInvalid())
            {
                file.Name = FileHelper.GetFileName(path);
            }
            file.Type = fileType;
            file.Size = DynamicConvert.ToSafeLong(data.xly_size);
            file.CreationDate = DynamicConvert.ToSafeDateTime(data.date_added) ?? DateTime.MinValue;
            file.LastWriteData = DynamicConvert.ToSafeDateTime(data.date_modified) ?? DateTime.MinValue;
            file.LastAccessDate = file.LastWriteData;
            var finfo = new FileInfo(path);
            file.ParentDirectory = finfo.Directory.FullName;
            return file;
        }

        private static void IsExitNode(TreeNode node, FileX file, string condition, string dataItemsFilePath)
        {
            TreeNode IsExitFileNode = node.TreeNodes.Find(v => v.Text == condition);//查找是否有该扩展结点
            if (IsExitFileNode != null)
            {
                IsExitFileNode.Items.Add(file);
            }
            else
            {
                TreeNode parentfilenode = CreateFileTreeNode(condition, dataItemsFilePath);

                parentfilenode.Items.Add(file);

                node.TreeNodes.Add(parentfilenode);
            }
        }

        private static TreeNode CreateFileTreeNode(string text, string dataItemsFilePath)
        {
            TreeNode node = new TreeNode();
            node.Text = text;
            node.Type = typeof(FileX);
            node.Items = new DataItems<FileX>(dataItemsFilePath);

            return node;
        }
    }
}
