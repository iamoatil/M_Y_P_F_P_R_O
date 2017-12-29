using GalaSoft.MvvmLight.CommandWpf;
using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Project.ViewDomain.MefKeys;
using XLY.SF.Project.ViewDomain.Model;

namespace XLY.SF.Project.ViewModels.Main.DeviceMain
{
    [Export(ExportKeys.DeviceEditViewModel, typeof(ViewModelBase))]
    public class DeviceEditViewModel : ViewModelBase
    {
        public DeviceEditViewModel()
        {
            OkCommand = new RelayCommand(ExecuteOkCommand, CheckBtn);
            SearchCommand=new RelayCommand(ExecuteSearchCommand);
            Phones=Datas = GetAllData();
        }
        private bool CheckBtn()
        {
            if (PhoneVisibility)
            {
                return Phones.Where(p => p.IsChecked).Count() > 0;
            }
            return Modes.Where(p => p.IsChecked).Count() > 0; ;
        }
            private void ExecuteSearchCommand()
            {
              //查询手机
               Phones = Datas.Where(p => p.PhoneName.Contains(SearchContent)).ToList();
               //查询型号
               Modes = Datas.Where(p => p.IsChecked).FirstOrDefault().Items.Where(o => o.Name.Contains(SearchContent)).ToList();
        }

            private void ExecuteOkCommand()
             {
            if (ModelVisibility == false)
            {
                //显示选择型号界面
                ModelVisibility = true;
                PhoneVisibility = false;
                var tmp= Datas.Where(p => p.IsChecked).FirstOrDefault();
                Modes = tmp.Items;
                PhoneName = tmp.PhoneName;
            }
            else {
                base.DialogResult = true;
                base.CloseView();
            }
        }

        public override object GetResult()
        {
            if (DialogResult) 
                return new DeviceInfoModel() { PhoneName= PhoneName, ModelName = Modes.Where(i=>i.IsChecked).FirstOrDefault().Name};
            return null;
        }
        public string PhoneName { get; set; }


        private bool _ModelVisibility =false;
        /// <summary>
        /// 型号列表是否显示
        /// </summary>
        public bool ModelVisibility
        {
            get { return _ModelVisibility; }
            set {
                _ModelVisibility = value;
                OnPropertyChanged();
            }
        }

        private bool _PhoneVisibility = true;
        /// <summary>
        /// 手机列表是否显示
        /// </summary>
        public bool PhoneVisibility
        {
            get { return _PhoneVisibility; }
            set
            {
                _PhoneVisibility = value;
                OnPropertyChanged();
            }
        }

        private string _SearchContent="";

        public string SearchContent
        {
            get { return _SearchContent.Trim(); }
            set { _SearchContent = value;
                OnPropertyChanged();
            }
        }



        private List<ModelDatas> _Modes;
        /// <summary>
        /// 所有型号，用户前台绑定
        /// </summary>
        public List<ModelDatas> Modes
        {
            get { return _Modes; }
            set {
                _Modes = value;
                OnPropertyChanged();
            }
        }

        private List<PhoneDatas> _Phones;
        /// <summary>
        /// 所有手机，用户前台绑定
        /// </summary>
        public List<PhoneDatas> Phones
        {
            get { return _Phones; }
            set
            {
                _Phones = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// 所有数据，拿来过滤数据
        /// </summary>
        private List<PhoneDatas> Datas;

        private List<PhoneDatas> GetAllData() {
            XmlDocument doc = new XmlDocument();
            doc.Load(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config/PhoneModelConfig.xml"));
            XmlElement rootElement = doc.DocumentElement;
            XmlNodeList nodes = rootElement.GetElementsByTagName("PhoneModelConfigItems");
            List<PhoneDatas> Datas = new List<PhoneDatas>();
            PhoneDatas phone;
            foreach (XmlNode node in nodes)
            {
                phone = new PhoneDatas() { PhoneName = ((XmlElement)node).GetAttribute("PhoneName"), ImageUri = ((XmlElement)node).GetAttribute("ImageUri"),Items = new List<ModelDatas>() };
                foreach (var item in ((XmlElement)node).GetElementsByTagName("Item"))
                {
                    phone.Items.Add(new ModelDatas() { Name = ((XmlElement)item).GetAttribute("Name") });
                }
                Datas.Add(phone);
            }

            return Datas;
            
        }


        public ICommand OkCommand { get; set; }
        public ICommand SearchCommand { get; set; }
    }
    public class PhoneDatas: ViewModelBase
    {
        public string PhoneName { get; set; }
        public string ImageUri { get; set; }
        public new bool IsChecked { get; set; }

        public List<ModelDatas> Items { get; set; }

    }
    public class ModelDatas
    {
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }
}
