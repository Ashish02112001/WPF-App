using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace WpfAppAssignments {
    #region Class MainWindow ----------------------------------------------------------------------
    /// <summary>Implements main window events</summary>
    public partial class MainWindow : Window {
        #region Constructor -----------------------------------------
        public MainWindow () {
            InitializeComponent ();
        }
        #endregion

        #region Implementations -------------------------------------

        private void ClrChange (object sender, SelectionChangedEventArgs e) {
            mScribble.ColorChange (mSelect.SelectedIndex);
        }
        private void ButtonClear (object sender, RoutedEventArgs e) {
            mScribble.Clear ();
        }

        private void ButtonLoad (object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new ();
            openFileDialog.Filter = "Binary files (*.bin)|*.bin";
            if (openFileDialog.ShowDialog () is true) {
                var fileName = openFileDialog.FileName;
                mScribble.Load (fileName);
            }
        }

        private void ButtonSave (object sender, RoutedEventArgs e) {
            SaveFileDialog saveFile = new ();
            saveFile.Filter = "Binary files (*.bin)|*.bin";
            if (saveFile.ShowDialog () == true) {
                mScribble.Save (saveFile.FileName);
            }
        }
       
        private void ButtonUndo (object sender, RoutedEventArgs e) {
            mScribble.OnClickUndo ();
        }

        private void ButtonRedo (object sender, RoutedEventArgs e) {
            mScribble.OnClickRedo ();
        }

        private void ButtonScrib (object sender, RoutedEventArgs e) {
            ScribblePad.sCurrentShape = ShapeType.SCRIBBLE;
        }

        private void ButtonLine (object sender, RoutedEventArgs e) {
            ScribblePad.sCurrentShape = ShapeType.LINE;
        }

        private void ButtonPline (object sender, RoutedEventArgs e) {
            ScribblePad.sCurrentShape = ShapeType.PLINES;
        }
        private void ButtonRect (object sender, RoutedEventArgs e) {
            ScribblePad.sCurrentShape = ShapeType.RECTANGLE;
        }

        private void ButtonEllipse (object sender, RoutedEventArgs e) {
            ScribblePad.sCurrentShape = ShapeType.ELLIPSE;
        }

        private void ButtonCircle (object sender, RoutedEventArgs e) {
            ScribblePad.sCurrentShape = ShapeType.CIRCLE;
        }
        #endregion
    }
    #endregion
}