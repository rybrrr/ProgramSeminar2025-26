using Malovani;
using System.Drawing;
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
using Color = System.Windows.Media.Color;

namespace MyFirstResponsiveWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool _isDrawing;
        int _circleRadius;
        Color _color = new Color
        {
            A = 255,
            R = 0,
            G = 0,
            B = 0,
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenItemMenu_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".png"; // Default file extension
            dialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
            }
        }

        private void SaveItemMenu_Click(object sender, RoutedEventArgs e)
        {
            // Configure open file dialog box
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.FileName = "Document"; // Default file name
            dialog.DefaultExt = ".png"; // Default file extension
            dialog.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                "Portable Network Graphic (*.png)|*.png"; // Filter files by extension

            // Show open file dialog box
            bool? result = dialog.ShowDialog();

            // Process open file dialog box results
            if (result == true)
            {
                // Open document
                string filename = dialog.FileName;
            }
        }

        private void btnColor_Click(object sender, RoutedEventArgs e)
        {
            RGBColorPicker rGBColorPicker = new RGBColorPicker();
            rGBColorPicker.ShowDialog();

            if (rGBColorPicker.Success == true)
            {
                btnColor.Background = new SolidColorBrush(rGBColorPicker.Color);
                _color = rGBColorPicker.Color;
            }
        }

        private void DrawingCanvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDrawing)
            {
                // Získání aktuální pozice myši
                System.Windows.Point currentPoint = e.GetPosition(DrawingCanvas);

                // Vytvoření kruhu (Ellipse)
                Ellipse circle = new Ellipse
                {
                    Width = _circleRadius,
                    Height = _circleRadius,
                    Fill = new SolidColorBrush(_color) // Barva kruhu
                };

                // Nastavení pozice kruhu
                Canvas.SetLeft(circle, currentPoint.X - _circleRadius);
                Canvas.SetTop(circle, currentPoint.Y - _circleRadius);

                // Přidání kruhu na Canvas
                DrawingCanvas.Children.Add(circle);
            }
        }

        private void DrawingCanvas_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = true;
        }

        private void DrawingCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _isDrawing = false;
        }

        private void sldrWidth_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _circleRadius = (int)((Slider)sender).Value;
        }
    }
}