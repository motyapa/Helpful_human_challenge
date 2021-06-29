using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class ColorModel
    {
        public ColorModel(int red, int green, int blue)
        {
            if (red == -1)
            {
                red = 0;
            }

            if (green == -1)
            {
                green = 0;
            }

            if (blue == -1)
            {
                blue = 0;
            }
            
            this.red = red;
            this.green = green;
            this.blue = blue;

            HexCode = red.ToString("X2") + green.ToString("X2") + blue.ToString("X2");
        }
        [JsonProperty("red")]
        public int red { get; set; }
        [JsonProperty("green")]
        public int green { get; set; }
        [JsonProperty("blue")]
        public int blue { get; set; }
        [JsonProperty("HexCode")]
        public string HexCode { get; set; }

    }
}
