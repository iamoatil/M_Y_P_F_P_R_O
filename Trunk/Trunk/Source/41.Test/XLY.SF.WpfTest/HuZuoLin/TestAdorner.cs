using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace XLY.SF.WpfTest.HuZuoLin
{
    public class TestAdorner : Adorner
    {
        public Border _br;

        public TestAdorner(UIElement adornedElement) 
            : base(adornedElement)
        {
            this.IsHitTestVisible = false;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {

        }
    }
}
