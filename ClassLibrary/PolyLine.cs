﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ClassLibrary.ShapeType;

namespace ClassLibrary {
    public class PolyLine : Sketch {
        public PolyLine () => sType = PLINES;
    }
}
