using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32;
using System.IO;
using System.Text;

namespace WpfAppAssignments {
    public partial class MainWindow : Window {
        public MainWindow () {
            InitializeComponent ();
        }

        private void Button_Click_Erase (object sender, RoutedEventArgs e) {
            mScribble.Erase ();
        }

        private void Button_Click_Clear (object sender, RoutedEventArgs e) {
            mScribble.Clear ();
        }
        private void mSelect_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            mScribble.ColorChange (mSelect.SelectedIndex);
        }

        private void Button_Click_Save (object sender, RoutedEventArgs e) {
            SaveFileDialog saveFile = new ();
            saveFile.Filter = "Binary files (*.bin)|*.bin";
            if (saveFile.ShowDialog () == true) {
                mScribble.Save (saveFile.FileName);
            }
        }
        private void Button_Click_Load (object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new ();
            openFileDialog.Filter = "Binary files (*.bin)|*.bin";
            if (openFileDialog.ShowDialog () is true) {
                var fileName = openFileDialog.FileName;
                mScribble.Load (fileName);
            }
        }
        private void Button_Click_Undo (object sender, RoutedEventArgs e) {
            mScribble.OnClickUndo ();
        }

        private void Button_Click_Redo (object sender, RoutedEventArgs e) {
            mScribble.OnClickRedo ();
        }

        private void Button_Click_Line (object sender, RoutedEventArgs e) {
            ScribblePad.mCurrentShape = ShapeType.LINE;
        }

        private void Button_Click_Rect (object sender, RoutedEventArgs e) {
            ScribblePad.mCurrentShape = ShapeType.RECTANGLE;
        }

        private void Button_Click_Scribble (object sender, RoutedEventArgs e) {
            ScribblePad.mCurrentShape = ShapeType.SCRIBBLE;
        }

        private void Button_Click_Ellipse (object sender, RoutedEventArgs e) {
            ScribblePad.mCurrentShape = ShapeType.ELLIPSE;

        }

        private void Button_Click_Circle (object sender, RoutedEventArgs e) {
            ScribblePad.mCurrentShape = ShapeType.CIRCLE;
        }

        private void Button_Click_Pline (object sender, RoutedEventArgs e) {
            ScribblePad.mCurrentShape = ShapeType.PLINES;
        }
    }
}

