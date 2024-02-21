using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Win32;
using System.IO;

namespace WpfAppAssignments {
    public partial class MainWindow : Window {
        public MainWindow () {
            InitializeComponent ();
        }
        private void Button_Click (object sender, RoutedEventArgs e) {
            mThick.Visibility = Visibility.Visible;
        }

        private void Button_Click_Erase (object sender, RoutedEventArgs e) {
            mScribble.mPen = new (Brushes.Black, mScribble.mPen.Thickness);
        }

        private void Button_Click_Clear (object sender, RoutedEventArgs e) {
            mScribble.Children.Clear ();
        }
        private void mSelect_SelectionChanged (object sender, SelectionChangedEventArgs e) {
            SolidColorBrush color = Brushes.White;
            switch (mSelect.SelectedIndex) {
                case 0: color = Brushes.White; mThick.Background = Brushes.White; break;
                case 1: color = Brushes.Green; mThick.Background = Brushes.Green; break;
                case 2: color = Brushes.Orange; mThick.Background = Brushes.Orange; break;
                case 3: color = Brushes.Red; mThick.Background = Brushes.Red; break;
                case 4: color = Brushes.Yellow; mThick.Background = Brushes.Yellow; break;
            }
            mScribble.mPen.Brush = color;
        }
        private void mThick_ValueChanged (object sender, RoutedPropertyChangedEventArgs<double> e) {
            mScribble.mPen.Thickness = e.NewValue;
            mCircle.Height = mCircle.Width = mScribble.mPen.Thickness;
            mNThick.Text = mScribble.mPen.Thickness.ToString ("N1");
        }

        private void Button_Click_Save (object sender, RoutedEventArgs e) {
            SaveFileDialog saveFile = new ();
            if (saveFile.ShowDialog () == true) {
                StreamWriter sw = new (saveFile.FileName, true);
                foreach (var points in ScribblePad.mWayPoints) sw.WriteLine (points);
                sw.Close ();
            }
        }

        private void Button_Click_Load (object sender, RoutedEventArgs e) {
            OpenFileDialog openFileDialog = new ();
            openFileDialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog () is true) {
                var fileName = openFileDialog.FileName;
                mScribble.Load (fileName);
            }
        }
    }
}