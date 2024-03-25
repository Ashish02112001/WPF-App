using System.Windows;
using System.Windows.Controls;
using ClassLibrary;

namespace WpfAppAssignments {
    #region Class MainWindow ----------------------------------------------------------------------
    /// <summary>Implements main window events</summary>
    public partial class MainWindow : Window {
        #region Constructor -----------------------------------------
        public MainWindow () {
            InitializeComponent ();
            Title = DocManager.Name;
        }
        #endregion

        #region Implementations -------------------------------------

        private void ButtonClear (object sender, RoutedEventArgs e) {
            mScribble.Clear ();
        }

        private void ButtonLoad (object sender, RoutedEventArgs e) {
            mScribble.LoadSketch ();
            Title = DocManager.Name;
        }

        private void ButtonSave (object sender, RoutedEventArgs e) {
            mScribble.SaveSketch ();
            Title = DocManager.Name;
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