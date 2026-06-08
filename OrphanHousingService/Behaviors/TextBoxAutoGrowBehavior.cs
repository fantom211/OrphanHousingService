using System.Windows;
using System.Windows.Controls;

namespace OrphanHousingService.Behaviors
{
    public static class TextBoxAutoGrowBehavior
    {
        public static readonly DependencyProperty MinHeightProperty =
            DependencyProperty.RegisterAttached(
                "MinHeight",
                typeof(double),
                typeof(TextBoxAutoGrowBehavior),
                new PropertyMetadata(34d, OnSizingChanged));

        public static readonly DependencyProperty MaxHeightProperty =
            DependencyProperty.RegisterAttached(
                "MaxHeight",
                typeof(double),
                typeof(TextBoxAutoGrowBehavior),
                new PropertyMetadata(140d, OnSizingChanged));

        public static void SetMinHeight(DependencyObject obj, double value) =>
            obj.SetValue(MinHeightProperty, value);

        public static double GetMinHeight(DependencyObject obj) =>
            (double)obj.GetValue(MinHeightProperty);

        public static void SetMaxHeight(DependencyObject obj, double value) =>
            obj.SetValue(MaxHeightProperty, value);

        public static double GetMaxHeight(DependencyObject obj) =>
            (double)obj.GetValue(MaxHeightProperty);

        private static void OnSizingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not TextBox textBox)
                return;

            textBox.TextChanged -= OnTextChanged;
            textBox.TextChanged += OnTextChanged;
            textBox.Loaded -= OnLoaded;
            textBox.Loaded += OnLoaded;

            if (textBox.IsLoaded)
                AdjustHeight(textBox);
        }

        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
                AdjustHeight(textBox);
        }

        private static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
                AdjustHeight(textBox);
        }

        private static void AdjustHeight(TextBox textBox)
        {
            var minHeight = GetMinHeight(textBox);
            var maxHeight = GetMaxHeight(textBox);

            textBox.Height = double.NaN;
            textBox.Measure(new Size(textBox.ActualWidth > 0 ? textBox.ActualWidth : textBox.MinWidth, double.PositiveInfinity));

            var desired = Math.Max(minHeight, textBox.DesiredSize.Height);
            textBox.Height = Math.Min(desired, maxHeight);

            textBox.VerticalScrollBarVisibility =
                textBox.DesiredSize.Height > maxHeight
                    ? ScrollBarVisibility.Auto
                    : ScrollBarVisibility.Disabled;
        }
    }
}
