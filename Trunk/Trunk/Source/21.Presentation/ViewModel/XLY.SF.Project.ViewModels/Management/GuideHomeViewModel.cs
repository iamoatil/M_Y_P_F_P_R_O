using GalaSoft.MvvmLight.CommandWpf;
using ProjectExtend.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using XLY.SF.Framework.Core.Base.ViewModel;
using XLY.SF.Framework.Language;
using XLY.SF.Project.ViewDomain.MefKeys;

namespace XLY.SF.Project.ViewModels.Management
{
    [Export(ExportKeys.SettingsGuideHomeViewModel, typeof(ViewModelBase))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GuideHomeViewModel : ViewModelBase
    {
        #region Fields

        private readonly Dictionary<String, StepPair[]> _items = new Dictionary<string, StepPair[]>();

        private readonly String _baseUri = "Pack://application:,,,/XLY.SF.Project.Themes;Component/Resources/Images/Guide/";

        private Int32 _maxIndex =-1;

        #endregion

        #region Constructors

        public GuideHomeViewModel()
        {
            Blocks = Init();
            PreviousPageCommand = new RelayCommand(PreviousPage, () => CurrentIndex > 0);
            NextPageCommand = new RelayCommand(NextPage, () => CurrentIndex < _maxIndex);
        }

        #endregion

        #region Nested Type

        public class StepPair
        {
            public String Summary1 { get; set; }

            public String Uri1 { get; set; }

            public String Summary2 { get; set; }

            public String Uri2 { get; set; }
        }

        #endregion

        #region Properties

        public ICommand PreviousPageCommand { get; }

        public ICommand NextPageCommand { get; }

        public String[] Blocks { get; }

        #region SelectedBlock

        private String _selectedBlock;
        public String SelectedBlock
        {
            get=>_selectedBlock;
            set
            {
                if (_selectedBlock != value)
                {
                    _selectedBlock = value;
                    if (!String.IsNullOrWhiteSpace(value) && _items.ContainsKey(value))
                    {
                        StepPair[] sp = _items[value];
                        _maxIndex = sp.GetUpperBound(0);
                        Steps = sp;
                        CurrentIndex = sp.GetLowerBound(0);
                    }
                    else
                    {
                        _maxIndex = -1;
                        Steps = null;
                        CurrentIndex = -1;
                    }
                    OnPropertyChanged();
                }
            }
        }

        #endregion

        #region CurrentStep

        private StepPair _currentStep;
        public StepPair CurrentStep
        {
            get => _currentStep;
            private set
            {
                _currentStep = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region CurrentIndex

        private Int32 _currentIndex = 0;
        public Int32 CurrentIndex
        {
            get => _currentIndex;
            set
            {
                _currentIndex = value;
                OnPropertyChanged();
                if (Steps == null) return;
                if (value < 0 || value > _maxIndex)
                {
                    CurrentStep = null;
                }
                else
                {
                    CurrentStep = Steps[value];
                }
            }
        }

        #endregion

        #region Steps

        private StepPair[] _steps;
        public StepPair[] Steps
        {
            get => _steps;
            private set
            {
                _steps = value;
                OnPropertyChanged("Steps");
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Protected

        private String[] Init()
        {
            String[] blocks = new String[]
            {
                SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Upgrade],
                SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_IOS_Title],
                SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Android4_0_Title],
                SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Android4_1_Title],
                SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Android5_0_Title],
                SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Developer_Title],
            };
            foreach (String block in blocks)
            {
                _items.Add(block, null);
            }
            return blocks;
        }

        protected override void InitLoad(Object parameters)
        {
            _items[Blocks[0]] = new StepPair[]
           {
           };
            _items[Blocks[1]] = new StepPair[]
            {
                new StepPair
                {
                    Summary1 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_IOS_Step1],
                    Uri1 = _baseUri+"ios_step1.png",
                                   Summary2 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_IOS_Step2],
                    Uri2 = _baseUri+"ios_step2.png"
                }
            };
            _items[Blocks[2]] = new StepPair[]
            {
                new StepPair
                {
                    Summary1 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Android4_0_Step1],
                    Uri1 = _baseUri+"android4_step1.png",
                    Summary2 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Android4_0_Step2],
                    Uri2 = _baseUri+"android4_step2.png"
                },
                 new StepPair
                {
                    Summary1 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Android4_0_Step3],
                    Uri1 = _baseUri+"android4_step3.png",
                    Summary2 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Android4_0_Step4],
                    Uri2 = _baseUri+"android4_step4.png"
                },
                new StepPair
                {
                    Summary1 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Android4_0_Step5],
                    Uri1 = _baseUri+"android4_step5.png",
                    Summary2 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Android4_0_Step6],
                    Uri2 = _baseUri+"android4_step6.png"
                },
           };
            _items[Blocks[3]] = new StepPair[]
           {
                new StepPair
                {
                    Summary1 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Android4_1_Step1],
                    Uri1 = _baseUri+"android4_1_step1.png",
                    Summary2 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Android4_1_Step2],
                    Uri2 = _baseUri+"android4_1_step2.png"
                },
                 new StepPair
                {
                    Summary1 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Android4_1_Step3],
                    Uri1 = _baseUri+"android4_1_step3.png",
                    Summary2 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Android4_1_Step4],
                    Uri2 = _baseUri+"android4_1_step4.png"
                },
                new StepPair
                {
                    Summary1 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Android4_1_Step5],
                    Uri1 = _baseUri+"android4_1_step5.png",
                    Summary2 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Android4_1_Step6],
                    Uri2 = _baseUri+"android4_1_step6.png"
                },
          };
            _items[Blocks[4]] = new StepPair[]
           {
                new StepPair
                {
                    Summary1 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Android5_0_Step1],
                    Uri1 = _baseUri+"android5_0_step1.png",
                    Summary2 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Android5_0_Step2],
                    Uri2 = _baseUri+"android5_0_step2.png"
                },
                 new StepPair
                {
                    Summary1 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Android5_0_Step3],
                    Uri1 = _baseUri+"android5_0_step3.png",
                    Summary2 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Android5_0_Step4],
                    Uri2 = _baseUri+"android5_0_step4.png"
                },
          };
            _items[Blocks[5]] = new StepPair[]
           {
                new StepPair
                {
                    Summary1 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Developer_Step1],
                    Uri1 = _baseUri+"developer_step1.png",
                    Summary2 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Developer_Step2],
                    Uri2 = _baseUri+"developer_step2.png"
                },
                 new StepPair
                {
                    Summary1 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Developer_Step3],
                    Uri1 = _baseUri+"developer_step3.png",
                    Summary2 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Developer_Step4],
                    Uri2 = _baseUri+"developer_step4.png"
                },
                new StepPair
                {
                    Summary1 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Developer_Step5],
                    Uri1 = _baseUri+"developer_step5.png",
                    Summary2 = SystemContext.LanguageManager[Languagekeys.ViewLanguage_Management_Guide_Developer_Step6],
                    Uri2 = _baseUri+"developer_step6.png"
                },
          };
        }

        #endregion

        #region Private

        private void PreviousPage()
        {
            CurrentIndex--;
        }

        private void NextPage()
        {
            CurrentIndex++;
        }

        #endregion

        #endregion
    }
}
