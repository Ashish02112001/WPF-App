using System.Windows;

namespace WpfAppAssignments {
    abstract class Widget {
        public Widget (UIElement eventSource) => mEventSource = eventSource;
        protected readonly UIElement mEventSource;
    }
    abstract class MouseDragger : Widget {
        protected MouseDragger (UIElement eventSource) : base (eventSource) { }
        enum DragStates { DragBegin, DrawOn, DragEnd }
        protected abstract void DragBegin (Point ptStart);
        protected abstract void DrawOn (Point pt);
        protected abstract void DrawEnd (Point pt);
    }
    class RectSelecter : MouseDragger {
        public RectSelecter (UIElement uI) : base (uI) {
            
        }
        protected override void DragBegin (Point ptStart) {
            throw new System.NotImplementedException ();
        }

        protected override void DrawEnd (Point pt) {
            throw new System.NotImplementedException ();
        }

        protected override void DrawOn (Point pt) {
            throw new System.NotImplementedException ();
        }
    }
}