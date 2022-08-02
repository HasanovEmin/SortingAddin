﻿using Aveva.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SortingAddin
{
    class Item
    {
        public DbElement Element { get; set; }
        public double P1bore { get; set; }
        public double P2bore { get; set; }
        public double P3bore { get; set; } = 0;
        public double P4bore { get; set; } = 0;
    }
}