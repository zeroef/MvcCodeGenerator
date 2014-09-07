using System;

namespace Ttg.MvcCodeGenerator.Domain
{
    public class TableColumn
    {
        public string Name { get; set; }
        public Type Type { get; set; }
        public int ColumnLength { get; set; }
    }
}
