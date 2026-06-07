using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace OrphanHousingService.Behaviors
{
    public static class TextBoxAutoGrowBehavior
    {
        public static readonly DependencyProperty MaxHeightProperty =
            DependencyProperty.RegisterAttached(
                "MaxHeight",
                typeof(double),
                typeof(TextBoxAutoGrowBehavior),
                new PropertyMetadata(double.PositiveInfinity, OnMaxHeightChanged));

        public static void SetMaxHeight(DependencyObject obj, double value)
            => obj.SetValue(MaxHeightProperty, value);

        public static double GetMaxHeight(DependencyObject obj)
            => (double)obj.GetValue(MaxHeightProperty);

        private static void OnMaxHeightChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox tb)
            {
                tb.TextChanged -= Tb_TextChanged;
                tb.TextChanged += Tb_TextChanged;
            }
        }

        private static void Tb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox tb) return;

            tb.Height = double.NaN;
            tb.Measure(new Size(tb.ActualWidth, double.PositiveInfinity));

            var max = GetMaxHeight(tb);
            tb.Height = Math.Min(tb.DesiredSize.Height + 10, max);
        }
    }
}
