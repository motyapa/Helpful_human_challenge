using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class IndexViewModel
    {
        public IndexViewModel(List<ColorModel> colors, int total)
        {
            this.colors = colors;
            this.total = total;
        }
        public List<ColorModel> colors { get; set; }
        public int total { get; set; }
    }
}
