using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColorDisplay.Models
{
    public class IndexViewModel
    {
        public IndexViewModel(List<ColorModel> _colors, int total)
        {
            this._colors = _colors;
            this.total = total;
        }
        public List<ColorModel> _colors { get; set; }
        public int total { get; set; }
    }
}
