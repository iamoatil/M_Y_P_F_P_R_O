using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace XLY.SF.Project.Themes
{
    /// <summary>
    /// 针对数据装饰属性的绑定
    /// </summary>
    public class DecorationBinding : MarkupExtension
    {
        public PropertyPath Path { get; set; }
        public dynamic DP { get; set; }
        public IValueConverter Converter { get; set; }
        public object ConverterParameter { get; set; }
        public BindingMode Mode { get; set; }

        private List<FrameworkElement> _listTargets = new List<FrameworkElement>();
        private FrameworkElement _targetObject;
        private DependencyProperty _targetProperty;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            IProvideValueTarget target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
            if (target == null)
                return null;
            if (target.TargetObject.GetType().FullName == "System.Windows.SharedDp")
                return this;
            _targetObject = target.TargetObject as FrameworkElement;
            _targetProperty = target.TargetProperty as DependencyProperty;
            if (_targetObject == null)
                return null;

            _listTargets.Add(_targetObject);

            if (_targetObject.DataContext == null)
            {
                _targetObject.DataContextChanged -= TargetObject_DataContextChanged;
                _targetObject.DataContextChanged += TargetObject_DataContextChanged;
                return null;
            }
            else
            {
                Binding binding = CreateBinding();
                return binding.ProvideValue(serviceProvider);
            }
        }

        private void TargetObject_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_targetObject.DataContext == null)
                return;
            foreach (var item in _listTargets)
            {
                Binding binding = new Binding("BindProperty");
                binding.Mode = this.Mode;
                binding.Converter = this.Converter;
                binding.ConverterParameter = this.ConverterParameter;
                if (Path == null)
                {
                    var bdobj = item.DataContext;
                    binding.Source = new DecorationBindingItem(bdobj, DP);
                }
                else
                {
                    var bdobj = item.DataContext.GetType().GetProperty(Path.Path).GetValue(item.DataContext);
                    binding.Source = new DecorationBindingItem(bdobj, DP);
                }
                BindingOperations.SetBinding(item, _targetProperty, binding);
            }
        }

        private Binding CreateBinding() // 创建绑定类型实例
        {
            Binding binding = new Binding("BindProperty");
            binding.Mode = this.Mode;
            binding.Converter = this.Converter;
            binding.ConverterParameter = this.ConverterParameter;
            if (Path == null)
            {
                var bdobj = _targetObject.DataContext;
                binding.Source = new DecorationBindingItem(bdobj, DP);
            }
            else
            {
                var bdobj = _targetObject.DataContext.GetType().GetProperty(Path.Path).GetValue(_targetObject.DataContext);
                binding.Source = new DecorationBindingItem(bdobj, DP);
            }

            return binding;
        }
    }
}
