using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Jump.View
{
    public class CustomButton
    {
        private readonly string pathpic = $"{Directory.GetCurrentDirectory()}\\Picture\\";
        private readonly string pathsound = $"{Directory.GetCurrentDirectory()}\\Sound\\";

        public void CreateButton(string text, ref Button button)
        {
            TextBlock txt = new()
            {
                FontSize = 25,
                Text = text,

                Foreground = Brushes.LightSkyBlue,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid stack = new()
            {
                Height = 50,
                Width = 300,
                Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new(pathpic + "buttonimg.jpg")),
                    Stretch = Stretch.Fill,
                }
            };

            stack.Children.Add(txt);

            button.Height = 50;
            button.Width = 300;
            button.HorizontalAlignment = HorizontalAlignment.Center;
            button.Content = stack;
        }

        public void CreateButton(string text, ref Button button, double width, double height)
        {
            TextBlock txt = new()
            {
                FontSize = 25,
                Text = text,

                Foreground = Brushes.LightSkyBlue,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            Grid stack = new()
            {
                Height = height,
                Width = width,
                Background = new ImageBrush
                {
                    ImageSource = new BitmapImage(new(pathpic + "buttonimg.jpg")),
                    Stretch = Stretch.Fill,
                }
            };

            stack.Children.Add(txt);

            button.Height = height;
            button.Width = width;
            button.HorizontalAlignment = HorizontalAlignment.Center;
            button.Content = stack;
        }
    }
}
