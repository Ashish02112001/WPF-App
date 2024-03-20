using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
}