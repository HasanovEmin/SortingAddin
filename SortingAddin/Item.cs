using Aveva.Core.Database;

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
