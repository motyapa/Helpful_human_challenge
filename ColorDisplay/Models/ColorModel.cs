using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ColorDisplay.Models
{
    public class ColorModel
    {
        //Used during the script generation
        //A little bit awkward during the generation. Assuming RGBStep = 32 we need to add elements
        //with values like 0, 31, 63... 
        //The one out of place is 0, so I decided to add elements of the form -1 + RGBstep * n
        //and increment if the value is -1.
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

        //Used when retrieving data from DB
        public ColorModel(int red, int green, int blue, string HexCode) {
            this.red = red;
            this.green = green;
            this.blue = blue;
            this.HexCode = HexCode;
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
