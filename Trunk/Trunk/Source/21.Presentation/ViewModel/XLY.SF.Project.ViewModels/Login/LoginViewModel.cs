using GalaSoft.MvvmLight.Command;
using ProjectExtend.Context;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.CoreInterface;
using XLY.SF.Framework.Core.Base.MessageAggregation;
using XLY.SF.Framework.Core.Base.MessageBase;
using XLY.SF.Framework.Core.Base;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.Models.Logical;
using XLY.SF.Project.ViewDomain.MefKeys;
using System.Threading.Tasks;
using XLY.SF.Project.ProxyService;
using System.Windows;
using System.Collections.ObjectModel;
using System.Security.Cryptography;
using System;
using System.Text;
using XLY.SF.Framework.Language;
using XLY.SF.Project.Plugin.Adapter;


/*************************************************
 * 创建人：Bob
 * 创建时间：2017/2/24 17:36:58
 * 类功能说明：
 *
 *************************************************/

namespace XLY.SF.Project.ViewModels.Login
{
    [Export(ExportKeys.ModuleLoginViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(System.ComponentModel.Composition.CreationPolicy.NonShared)]
    public class LoginViewModel : ViewModelBase
    {
        #region Constructors

        [ImportingConstructor]
        public LoginViewModel(IMessageBox messageBox)
        {
            //_dbService = dbService;
            MessageBox = messageBox;
            LoginCommand = new ProxyRelayCommand(ExecuteLoginCommand, GetViewContainer, null);
            ExitSysCommand = new RelayCommand(ExeucteExitSysCommand);
        }

        #endregion

        #region Commands

        protected override void InitLoad(object parameters)
        {
            //执行加载
            ExecuteSysLoad(parameters.ToString());
        }

        #region 执行系统加载

        /// <summary>
        /// 加载
        /// </summary>
        /// <param name="solutionContentFromXml">推荐方案内容，XML文件格式</param>
        /// <returns></returns>
        private bool ConcreateLoadOfTask(object solutionContentFromXml)
        {
            //初始化系统上下文
            bool opertionResult = SystemContext.Instance.InitSysInfo() &&
                SystemContext.Instance.LoadProposedSolution(solutionContentFromXml.ToString());

            //开启设备监听服务
            ProxyFactory.DeviceMonitor.OpenDeviceService();

            //加载插件
            PluginAdapter.Instance.Initialization(null);

            //加载一次数据库
            _dbService.UserInfos.FirstOrDefault();
            AllUser =new ObservableCollection<UserInfo>(_dbService.UserInfos.OrderByDescending(p=>p.LoginTime).Take(5)); //获取本地前5条用户记录

            return opertionResult;
        }

        private async void ExecuteSysLoad(string solutionContentFromXml)
        {
           
            var opertionResult = await Task<bool>.Factory.StartNew(ConcreateLoadOfTask, solutionContentFromXml);

            if (!opertionResult)
            {
                //加载信息失败，关闭程序
                MessageBox.ShowDialogErrorMsg("系统加载失败，即将关闭程序。");
                SysCommonMsgArgs<string> args = new SysCommonMsgArgs<string>(SystemKeys.ShutdownProgram);
                base.MessageAggregation.SendSysMsg(args);
            }
            IsLoadingVisibility = Visibility.Collapsed;
            IsLoadVisibility = Visibility.Visible;
            //完成加载，进入登录界面
            //base.CloseView();
            //base.NavigationForNewWindow(ExportKeys.ModuleLoginView);
        }

        #endregion
        /// <summary>
        /// 登录
        /// </summary>
        public ProxyRelayCommand LoginCommand { get; set; }

        /// <summary>
        /// 退出程序
        /// </summary>
        public ICommand ExitSysCommand { get; set; }

        #endregion


        private ObservableCollection<UserInfo> _AllUser;

        public ObservableCollection<UserInfo> AllUser
        {
            get { return _AllUser; }
            set { _AllUser = value;
                base.OnPropertyChanged();
            }
        }



        #region Model

        private UserInfoEntityModel _curLoginUser;
        /// <summary>
        /// 当前登录用户
        /// </summary>
        public UserInfoEntityModel CurLoginUser
        {
            get {
                if (_curLoginUser == null)
                {
                    _curLoginUser = new UserInfoEntityModel();
                }
                return _curLoginUser;
            }
            set {
                _curLoginUser = value;
                base.OnPropertyChanged();
            }
        }

        //public UserInfoEntityModel CurLoginUser { get; set; }

        #endregion

        #region 数据定义

        /// <summary>
        /// 数据库服务
        /// </summary>
        [Import(typeof(IDatabaseContext))]
        private IDatabaseContext _dbService { get; set; }

        /// <summary>
        /// 消息服务
        /// </summary>
        //[Import(typeof(IMessageBox))]
        private IMessageBox MessageBox { get; set; }

        #endregion

        private Visibility _IsLoadingVisibility;
        /// <summary>
        /// loading是否显示
        /// </summary>
        public Visibility IsLoadingVisibility
        {
            get { return _IsLoadingVisibility; }
            set {
                _IsLoadingVisibility = value;
                base.OnPropertyChanged();
            }
        }

        private Visibility _IsLoadVisibility= Visibility.Collapsed;
        /// <summary>
        /// 登录是否显示
        /// </summary>
        public Visibility IsLoadVisibility
        {
            get { return _IsLoadVisibility; }
            set
            {
                _IsLoadVisibility = value;
                base.OnPropertyChanged();
            }
        }


        protected override void Closed()
        {

        }

        #region 登录操作

        private string ExecuteLoginCommand()
        {
            string operationLog = string.Empty;

            MD5CryptoServiceProvider md5Psd = new MD5CryptoServiceProvider();
            String newPsd = BitConverter.ToString(md5Psd.ComputeHash(Encoding.ASCII.GetBytes(CurLoginUser.LoginPassword)));
            var loginUser = _dbService.UserInfos.FirstOrDefault((t) => t.LoginUserName == CurLoginUser.LoginUserName && t.LoginPassword== newPsd).ToModel<UserInfo, UserInfoEntityModel>();
            if (loginUser == default(UserInfoEntityModel))
            {
                //登录失败
                MessageBox.ShowDialogErrorMsg("登录失败，密码或帐号错误！");
            }
            else
            {
                SystemContext.Instance.SetLoginSuccessUser(loginUser);

                //关闭界面
                //由于此处导航是持续导航（第一个界面完成后，直接进入下个界面，无导航消息发送）
                //所以需要自己关闭
                base.CloseView();
                operationLog = "登录用户成功";

                //登录成功
                SysCommonMsgArgs sysArgs = new SysCommonMsgArgs(SystemKeys.LoginComplete);
                MsgAggregation.Instance.SendSysMsg(sysArgs);
            }

            return operationLog;
        }

        #endregion

        #region 退出程序

        private void ExeucteExitSysCommand()
        {
            SysCommonMsgArgs<string> args = new SysCommonMsgArgs<string>(SystemKeys.ShutdownProgram);
            base.MessageAggregation.SendSysMsg(args);
        }

        #endregion
    }
}
