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
    public class DeviceExtractionTask : TaskActivator
    {
        #region Fields

        private readonly DataExtractControler _controler;

        private readonly MultiTaskReportBase _reporter;

        #endregion

        #region Constructors

        public DeviceExtractionTask()
        {
            _reporter = new DefaultMultiTaskReporter();
            _reporter.ProgressChanged += _reporter_ProgressChanged;
            _reporter.Ternimated += _reporter_Ternimated;
            _controler = new DataExtractControler()
            {
                Reporter = _reporter
            };
        }

        #endregion

        #region Methods

        #region Public

        public override Boolean Launch()
        {
            return true;
        }

        #endregion

        #region Protected

        protected override void OnReceive(Message message)
        {
            ExtractionCode code = (ExtractionCode)message.Code;
            switch (code)
            {
                case ExtractionCode.Start:
                    Start(message);
                    break;
                case ExtractionCode.Stop:
                    _controler.Stop();
                    break;
                default:
                    break;
            }
        }

        protected override void Dispose(Boolean isDisposing)
        {
            base.Dispose(isDisposing);
            _controler.Stop();
        }

        #endregion

        #region Private

        private void _reporter_Ternimated(object sender, TaskTerminateEventArgs e)
        {
            OnTaskOver(e.TaskId, e.IsCompleted);
        }

        private void _reporter_ProgressChanged(object sender, TaskProgressEventArgs e)
        {
        }

        private void Start(Message message)
        {
            DataExtractionParams @params = message.GetContent<DataExtractionParams>();
            Pump pump = @params.Pump;
            if (@params != null && pump != null)
            {
                _controler.Start(@params.Pump, @params.Items);
                OnSend(message.CreateResponse(true));
            }
            else
            {
                OnActivatorError(new ArgumentException("Can't convert to type 'DataExtractionParams' "));
            }
        }

        #endregion

        #endregion
    }
}
