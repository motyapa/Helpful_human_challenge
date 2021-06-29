using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ColorDisplay.Models;

namespace ColorDisplay.Controllers
{
    public class HomeController : Controller
    {
        private static int _RGBStep = 32; //The difference in RGB values
        private static List<ColorModel> _colors = new List<ColorModel>(); //Where all of the generated colors are stored
        private static double _tintMultiplier = .5; 
        private static double _shadeMultiplier = .75;

        public ActionResult Index()
        {
            //If file doesn't exist, generate rgb colors 
            string pathToData = Path.Combine(Environment.CurrentDirectory, _RGBStep + "RGBValues.json");
            if (!System.IO.File.Exists(pathToData))
            {
                using (StreamWriter sw = new StreamWriter(pathToData))
                {
                    int loopValue = 256 / _RGBStep;
                    for (int blue = 0; blue <= loopValue; blue++)
                    {
                        for (int green = 0; green <= loopValue; green++)
                        {
                            for (int red = 0; red <= loopValue; red++)
                            {
                                ColorModel cm = new ColorModel(red * _RGBStep - 1, green * _RGBStep - 1, blue * _RGBStep - 1);

                                
                                _colors.Add(cm);
                            }
                        }
                    }
                    //Shuffle using the step as a seed - garauntees each time that the order of colors will be the same if the data is lost for whatever reason
                    Shuffle(_colors, _RGBStep);
                    var allData = JsonConvert.SerializeObject(_colors, Formatting.Indented);
                    sw.Write(allData);
                }
            }
            else
            {
                //If the file exists, just retrieve them
                _colors = JsonConvert.DeserializeObject<List<ColorModel>>(System.IO.File.ReadAllText(pathToData));
            }

            //Only use the data we need - in a real world example I would fetch only the first 12 results from a database
            IndexViewModel data = new IndexViewModel(_colors.GetRange(0, 12), _colors.Count());
            return View(data);
        }

        //Given a pagenumber, return the 12 (# of elements displayed) relevent to that page
        [HttpPost]
        public ActionResult Colors(int pageNumber)
        {
            //In the instance of the last page, there can be less than 12 elements
            int remainder = Math.Min(12, _colors.Count - (pageNumber - 1) * 12);

            //Only use the data we need - in a real world example I would fetch only the 12 results needed from a database
            List<ColorModel> data = _colors.GetRange((pageNumber - 1) * 12, remainder);
            return PartialView("_ColorGrid", data);
        }

        //For the details page, pass through the rgb values of the swatch selected
        //Also used for when the user hits random, then rgb values are -1.
        [HttpPost]
        public ActionResult Details(int red, int green, int blue)
        {
            List<ColorModel> details = new List<ColorModel>();
            ColorModel original = new ColorModel(red, green, blue);

            //Random button pressed
            if (red == -1)
            {
                Random r = new Random();
                original = _colors.ElementAt(r.Next(0, _colors.Count()));
            }

            //shade the elements, add at the start of list
            for (int i = 2; i >= 1; i--)
            {
                double multiplier = Math.Pow(_shadeMultiplier, i);
                details.Add(new ColorModel(Convert.ToInt32(original.red * multiplier), Convert.ToInt32(original.green * multiplier), Convert.ToInt32(original.blue * multiplier)));
            }

            //Add originally selected color
            details.Add(original);

            //Tint the elements, add at end
            double lastR = original.red;
            double lastG = original.green;
            double lastB = original.blue;          
            for (int i = 1; i <= 2; i++)
            {
                lastR = lastR + (255 - lastR) * _tintMultiplier;
                lastG = lastG + (255 - lastG) * _tintMultiplier;
                lastB = lastB + (255 - lastB) * _tintMultiplier;
                details.Add(new ColorModel(Convert.ToInt32(lastR), Convert.ToInt32(lastG), Convert.ToInt32(lastB)));
            }      
            
            //The reason for going from darkest -> lightest is due to the way the tinting works. Since it relies on the previous value, it was simpler to make a list going from darkest to lightest.           
            return PartialView("_Details", details);
        }

        //Shuffle the colors list given a seed.
        public static void Shuffle<T>(IList<T> list, int seed)
        {
            var rng = new Random(seed);
            int n = list.Count;

            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
