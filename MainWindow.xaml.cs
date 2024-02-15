using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WpfAppAssignments {
    public partial class MainWindow : Window {
        bool isDrawing = false;
        Point startPoint;
        Pen pen = new (Brushes.White, 2);
        PathFigure currentPathFigure;
        PathGeometry pathGeometry;
        Path path;
        public MainWindow () {
            InitializeComponent ();
            mThick.Visibility = Visibility.Collapsed;
        }
        private void Canvas_MouseDown (object sender, MouseButtonEventArgs e) {
            if (e.ButtonState == MouseButtonState.Pressed) {
                isDrawing = true;
                startPoint = e.GetPosition (canvas);
                currentPathFigure = new PathFigure { StartPoint = startPoint };
                pathGeometry = new PathGeometry ();
                pathGeometry.Figures.Add (currentPathFigure);

                path = new Path {
                    Stroke = pen.Brush,
                    StrokeThickness = pen.Thickness,
                    Data = pathGeometry
                };
                canvas.Children.Add (path);
            }
        }

        private void Canvas_MouseMove (object sender, MouseEventArgs e) {
            if (isDrawing && e.LeftButton == MouseButtonState.Pressed) {
                Point currentPoint = e.GetPosition (canvas);
                LineSegment segment = new LineSegment (currentPoint, true);
                currentPathFigure.Segments.Add (segment);
                mThick.Visibility=Visibility.Collapsed;
            }
        }

        private void Canvas_MouseUp (object sender, MouseButtonEventArgs e) {
            isDrawing = false;
        }

        private void mThick_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e) {
            pen.Thickness = e.NewValue;
            mCircle.Height = mCircle.Width = pen.Thickness;
            mNThick.Text = pen.Thickness.ToString ("N1");
        }

        private void mSelect_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            switch (mSelect.SelectedIndex) {
                case 0: pen = new (Brushes.White, pen.Thickness); mThick.Background = Brushes.White; break;
                case 1: pen = new (Brushes.Green, pen.Thickness); mThick.Background = Brushes.Green; break;
                case 2: pen = new (Brushes.Orange, pen.Thickness); mThick.Background = Brushes.Orange; break;
                case 3: pen = new (Brushes.Red, pen.Thickness); mThick.Background = Brushes.Red; break;
                case 4: pen = new (Brushes.Yellow, pen.Thickness); mThick.Background = Brushes.Yellow; break;
            }
        }

        private void Button_Click (object sender, RoutedEventArgs e) {
            mThick.Visibility = Visibility.Visible;
        }

        private void Button_Click_Erase (object sender, RoutedEventArgs e) {
            pen = new (Brushes.Black, pen.Thickness);
        }

        private void Button_Click_Clear (object sender, RoutedEventArgs e) {
            canvas.Children.Clear ();
        }
    }
}