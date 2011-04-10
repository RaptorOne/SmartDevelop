using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Windows.Controls;
using WPFCommon.Connectors.Converters.DefaultConverters;

namespace WPFCommon.Connectors.Converters
{
    public static class BindingHelper
    {

        public static MultiBinding CreateCenteredBinding(IConnectable connectable) {
            // Create a multibinding collection and assign an appropriate converter to it
            MultiBinding multiBinding = new MultiBinding();
            multiBinding.Converter = new DefaultCenterBinding();

            if (connectable is FrameworkElement) {
                multiBinding.ConverterParameter = new Point(
                    ((FrameworkElement)connectable).ActualWidth / 2,
                    ((FrameworkElement)connectable).ActualHeight / 2);
            }

            // Create binging #1 to IConnectable to handle Left
            Binding binding = new Binding();
            binding.Source = connectable;
            binding.Path = new PropertyPath(Canvas.LeftProperty);
            multiBinding.Bindings.Add(binding);

            // Create binging #2 to IConnectable to handle Top
            binding = new Binding();
            binding.Source = connectable;
            binding.Path = new PropertyPath(Canvas.TopProperty);
            multiBinding.Bindings.Add(binding);

            return multiBinding;
        }

        public static MultiBinding CreateAngledBinding(IConnectable source, IConnectable target) {
            MultiBinding multiBinding = new MultiBinding();
            multiBinding.Converter = new DefaultAngleCenterConverter();
            multiBinding.ConverterParameter = new Point(
                ((FrameworkElement)source).ActualWidth / 2,
                ((FrameworkElement)source).ActualHeight / 2);

            Binding binding = new Binding();
            binding.Source = source;
            binding.Path = new PropertyPath(Canvas.LeftProperty);
            multiBinding.Bindings.Add(binding);

            binding = new Binding();
            binding.Source = source;
            binding.Path = new PropertyPath(Canvas.TopProperty);
            multiBinding.Bindings.Add(binding);

            binding = new Binding();
            binding.Source = target;
            binding.Path = new PropertyPath(Canvas.LeftProperty);
            multiBinding.Bindings.Add(binding);

            binding = new Binding();
            binding.Source = target;
            binding.Path = new PropertyPath(Canvas.TopProperty);
            multiBinding.Bindings.Add(binding);

            return multiBinding;
        }

        public static MultiBinding CreateBezierBinding(IConnectable source, IConnectable target) {
            MultiBinding multiBinding = new MultiBinding();
            multiBinding.Converter = new ConnectionSplineConverter();

            double[] sizes = new double[] 
            { 
                ((FrameworkElement)source).ActualWidth,
                ((FrameworkElement)source).ActualHeight,
                ((FrameworkElement)target).ActualWidth,
                ((FrameworkElement)target).ActualHeight
            };

            bool reverse = false;

            if (Canvas.GetLeft((UIElement)source) < Canvas.GetLeft((UIElement)target)) {
                reverse = true;
                multiBinding.ConverterParameter = new double[]
                {
                    sizes[0],
                    sizes[1] / 2,
                    0,
                    sizes[3] / 2
                };

            } else {
                reverse = false;
                multiBinding.ConverterParameter = new double[]
                {
                    sizes[0],
                    sizes[1] / 2,
                    0,
                    sizes[3] / 2
                };
            }

            Binding binding = new Binding();
            binding.Source = reverse ? source : target;
            binding.Path = new PropertyPath(Canvas.LeftProperty);
            multiBinding.Bindings.Add(binding);

            binding = new Binding();
            binding.Source = reverse ? source : target;
            binding.Path = new PropertyPath(Canvas.TopProperty);
            multiBinding.Bindings.Add(binding);

            binding = new Binding();
            binding.Source = reverse ? target : source;
            binding.Path = new PropertyPath(Canvas.LeftProperty);
            multiBinding.Bindings.Add(binding);

            binding = new Binding();
            binding.Source = reverse ? target : source;
            binding.Path = new PropertyPath(Canvas.TopProperty);
            multiBinding.Bindings.Add(binding);

            return multiBinding;
        }
    }
}
