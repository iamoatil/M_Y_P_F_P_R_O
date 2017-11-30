using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.CommandWpf;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.Models;
using XLY.SF.Project.Models.Entities;
using XLY.SF.Project.Models.Logical;
using XLY.SF.Project.ViewDomain.MefKeys;
using System.Linq;
using XLY.SF.Framework.Core.Base.CoreInterface;

namespace XLY.SF.Project.ViewModels.Export
{
    [Export(ExportKeys.ExportDataViewViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ExportDataViewModel: ViewModelBase
    {
        protected override void InitLoad(object parameters)
        {
            
        }
    }

}

