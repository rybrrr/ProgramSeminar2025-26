using System.Diagnostics.Eventing.Reader;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MojePrvniWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /*
        private void btnPozdrav_MouseEnter(object sender, MouseEventArgs e)
        {
            Random rnd = new Random();
            byte r = (byte)rnd.Next(0, 255);
            byte g = (byte)rnd.Next(0, 255);
            byte b = (byte)rnd.Next(0, 255);

            byte diff = 40;

            // btnPozdrav.Background = new SolidColorBrush(Color.FromRgb(r, g, b));
            btnPozdrav.Background = new LinearGradientBrush(
                Color.FromRgb(r, g, b), // start
                Color.FromRgb(
                    BitConverter.GetBytes((r + diff) % 255)[0],
                    BitConverter.GetBytes((g + diff) % 255)[0],
                    BitConverter.GetBytes((b + diff) % 255)[0]
                ),
                90
            );
        }*/

        private void updateColor()
        {
            if (!IsLoaded)
                return;

            int r = int.Parse(redTxt.Text);
            int g = int.Parse(greenTxt.Text);
            int b = int.Parse(blueTxt.Text);
            string hexR = r.ToString("X2");
            string hexG = g.ToString("X2");
            string hexB = b.ToString("X2");

            colorRect.Fill = new SolidColorBrush(Color.FromRgb((byte)r, (byte)g, (byte)b));
            hexLabel.Content = $"#{hexR}{hexG}{hexB}";
        }

        private void Integer(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !e.Text.All(cc => Char.IsNumber(cc));
            base.OnPreviewTextInput(e);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int.TryParse(((TextBox)sender).Text, out int value);
            if (value < 0 || value > 255)
            {
                ((TextBox)sender).Text = Math.Clamp(value, 0, 255).ToString();
                MessageBox.Show("Vstupní číslo musí být mezi 0 a 255!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            updateColor();
        }

        private void redSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            redTxt.Text = Math.Round(redSlider.Value).ToString();
            updateColor();
        }

        private void greenSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            greenTxt.Text = Math.Round(greenSlider.Value).ToString();
            updateColor();
        }

        private void blueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            blueTxt.Text = Math.Round(blueSlider.Value).ToString();
            updateColor();
        }
    }
}