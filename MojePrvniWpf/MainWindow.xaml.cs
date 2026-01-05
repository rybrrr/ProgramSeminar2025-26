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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (txtPozdrav.Text == "")
            {
                MessageBox.Show("Vstupni pole je prazdne!!!!!", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ((Button)sender).Content = txtPozdrav.Text;
        }

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
        }
    }
}