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
            //mScribble.mPen = new (Brushes.Black, mScribble.mPen.Thickness);
        }

        private void Button_Click_Clear (object sender, RoutedEventArgs e) {
            mScribble.Children.Clear ();
        }
        private void mSelect_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            mScribble.ColorChange (mSelect.SelectedIndex);
        }

        private void Button_Click_Save (object sender, RoutedEventArgs e) {
            SaveFileDialog saveFile = new ();
            saveFile.Filter = "Text files (*.txt)|*.txt|Binary files (*.bin)|*.bin";
            if (saveFile.ShowDialog () == true) {
                mScribble.Save (saveFile.FileName);
            }
        }
        private void Button_Click_Load (object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new ();
            openFileDialog.Filter = "Text files (*.txt)|*.txt|Binary files (*.bin)|*.bin";
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
    }
}

