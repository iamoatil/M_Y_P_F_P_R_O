using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using XLY.SF.Framework.Language;
using XLY.SF.Framework.Log4NetService;
using XLY.SF.Project.Models.Logical;
using XLY.SF.Project.ViewDomain.Model;

namespace ProjectExtend.Context
{
    public partial class SystemContext
    {
        #region 初始化信息

        /// <summary>
        /// 初始化
        /// </summary>
        public bool InitSysInfo()
        {
            SysStartDateTime = DateTime.Now;
            //加载系统DPI
            LoadCurrentScreenDPI();
            return LoadConfig();
        }

        private void CreateDirectory(String directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        #endregion

        #region 添加操作日志

        /// <summary>
        /// 添加操作日志到数据库
        /// </summary>
        /// <param name="opEmt">操作内容</param>
        public void AddOperationLog(ObtainEvidenceLogModel opEmt)
        {
            OperationLogEntityModel log = new OperationLogEntityModel()
            {
                OperationContent = opEmt.OpContent,
                OperationDateTime = opEmt.OpTime,
                ScreenShotPath = opEmt.ImageNameForScreenShot,
                OperationUser = this.CurUserInfo,
                OperationModel = opEmt.OperationModel
            };
            _dbService.Add(log);
        }

        /// <summary>
        /// 保存操作图片，整个窗体
        /// </summary>
        /// <param name="control">截图目标</param>
        /// <returns>截图成功后的绝对路径</returns>
        public string SaveOperationImageByWindow(FrameworkElement control)
        {
            if (control != null)
            {
                var curWin = Window.GetWindow(control);
                RenderTargetBitmap rtb = new RenderTargetBitmap((int)curWin.Width, (int)curWin.Height, this.DpiX, this.DpiY, PixelFormats.Pbgra32);
                string imageFullPath = Path.Combine(OperationImagePath, GetOperationImageSaveName());
                using (FileStream fs = new FileStream(imageFullPath, FileMode.Create))
                {
                    rtb.Render(curWin);
                    PngBitmapEncoder pbe = new PngBitmapEncoder();
                    var bitTmp = BitmapFrame.Create(rtb);
                    pbe.Frames.Add(bitTmp);
                    pbe.Save(fs);
                    fs.Flush();
                }
                return imageFullPath;
            }
            return string.Empty;
        }

        #endregion

        #region 推荐方案

        /// <summary>
        /// 加载所有推荐方案
        /// </summary>
        /// <param name="solutionContentFromXml">推荐方案内容</param>
        public bool LoadProposedSolution(string solutionContentFromXml)
        {
            List<StrategyElement> result = new List<StrategyElement>();
            try
            {
                if (!string.IsNullOrWhiteSpace(solutionContentFromXml))
                {
                    var xml = new System.Xml.Serialization.XmlSerializer(typeof(List<StrategyElement>));
                    using (TextReader tr = new StringReader(solutionContentFromXml))
                    {
                        SolutionProposed = xml.Deserialize(tr) as List<StrategyElement>;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggerManagerSingle.Instance.Error(ex, "加载推荐方案失败");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取所有推荐方案
        /// </summary>
        /// <returns></returns>
        public StrategyElement[] GetAllProposedSolution()
        {
            return SolutionProposed.ToArray();
        }

        #endregion
    }
}
