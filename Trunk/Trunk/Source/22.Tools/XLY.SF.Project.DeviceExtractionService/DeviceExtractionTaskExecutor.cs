using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.DataExtract;
using XLY.SF.Project.Domains;
using XLY.SF.Project.IsolatedTaskEngine.Common;
using XLY.SF.Project.Plugin.Adapter;

namespace XLY.SF.Project.DeviceExtractionService
{
    public class DeviceExtractionTaskExecutor : TaskExecutor
    {
        #region Fields

        private readonly DataExtractControler _controler;

        #endregion

        #region Constructors

        public DeviceExtractionTaskExecutor()
        {
            TaskReporterAggregation reporter = new TaskReporterAggregation();
            reporter.ProgressChanged += _reporter_ProgressChanged;
            reporter.Terminated += _reporter_Terminated;
            reporter.TaskStateChanged += Reporter_TaskStateChanged;
            _controler = new DataExtractControler(reporter);
        }

        #endregion

        #region Properties

        public Pump Pump
        {
            get;
            private set;
        }

        #endregion

        #region Methods

        #region Public

        public override Boolean Launch()
        {
            Logger.Info($"Activator {ExecurtorId} launched");
            return true;
        }

        #endregion

        #region Protected

        protected override void CloseCore()
        {
            _controler.Stop();
            Pump = null;
            Logger.Info($"Excutor {ExecurtorId} closed");
        }

        protected override void OnReceive(Message message)
        {
            ExtractionCode code = (ExtractionCode)message.Code;
            switch (code)
            {
                case ExtractionCode.Init:
                    Init(message);
                    break;
                case ExtractionCode.Start:
                    Start(message);
                    break;
                case ExtractionCode.Stop:
                    _controler.Stop();
                    break;
                default:
                    Logger.Info($"Unrecognizable command:[Activator]{ExecurtorId}--[Command]{code}");
                    break;
            }
        }

        #endregion

        #region Private

        private void _reporter_Terminated(object sender, Framework.Core.Base.ViewModel.TaskTerminateEventArgs e)
        {
            SendMessage(ExtractionCode.ItemTerminate, e);
            if (e.IsFailed)
            {
                Logger.Error($"Exector {ExecurtorId}--Task {e.TaskId}: {e.Exception}");
            }
            Logger.Info($"Exector {ExecurtorId}--Task {e.TaskId} terminated: [IsCompleted]{e.IsCompleted},[IsFailed]{e.IsFailed}");
            if (_controler.IsBusy) return;
            OnExecutorTerminate(new IsolatedTaskEngine.Common.TaskTerminateEventArgs());
        }

        private void _reporter_ProgressChanged(object sender, TaskProgressChangedEventArgs e)
        {
            SendMessage(ExtractionCode.ProgressChanged, e);
        }

        private void Reporter_TaskStateChanged(object sender, TaskStateChangedEventArgs e)
        {
            SendMessage(ExtractionCode.StateChanged, e);
        }

        private void Init(Message message)
        {
            if (_controler.IsBusy) return;
            Pump pump = message.GetContent<Pump>();
            if (pump != null)
            {
                Pump = pump;
                Logger.Info($"Pump:[EnumOSType]{pump.OSType},[SavePath]{pump.SavePath},[Type]{pump.Type},[ScanModel]{pump.ScanModel},[Solution]{String.Format("0x{0:X}", (Int32)pump.Solution)}");
                ExtractItem[] items;
                if ((pump.Solution & PumpSolution.Downgrading) == PumpSolution.Downgrading)
                {
                    items = ExtractItemConfigurationReader.GetDowngradingExtractItems();
                }
                else
                {
                    items = PluginAdapter.Instance.GetAllExtractItems(pump).ToArray();
                }
                Send(message.CreateResponse(items));
            }
            else
            {
                throw new ArgumentException("Invalid command parameter");
            }
        }

        private void Start(Message message)
        {
            if (_controler.IsBusy) return;
            if (Pump == null)
            {
                throw new InvalidOperationException("Executor need initialize");
            }
            ExtractItem[] items = message.GetContent<ExtractItem[]>();
            if (items == null)
            {
                throw new ArgumentException("Invalid command parameter");
            }
            _controler.Start(Pump, items);
            Send(message.CreateResponse(true));
        }

        private void SendMessage(ExtractionCode code,Object obj)
        {
            Message message = new Message((Int32)code, obj);
            Logger.DebugFormat("Send message:[Code]{0},[Token]{1}", message.Code, message.Token);
            Send(message);
        }

        #endregion

        #endregion
    }
}
