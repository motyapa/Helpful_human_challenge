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
        private static int _RGBStep = 32; //Can error check this for values < 256
        private static List<ColorModel> _colors = new List<ColorModel>();
        private static double _tintMultiplier = .5;
        private static double _shadeMultiplier = .75;

        public ActionResult Index()
        {
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
                    Shuffle(_colors, _RGBStep);
                    var allData = JsonConvert.SerializeObject(_colors, Formatting.Indented);
                    sw.Write(allData);
                }
            }
            else
            {
                _colors = JsonConvert.DeserializeObject<List<ColorModel>>(System.IO.File.ReadAllText(pathToData));
            }

            IndexViewModel data = new IndexViewModel(_colors.GetRange(0, 12), _colors.Count());
            return View(data);
        }

        [HttpPost]
        public ActionResult Get_colors(int pageNumber)
        {
            int remainder = Math.Min(12, _colors.Count - (pageNumber - 1) * 12);
            List<ColorModel> data = _colors.GetRange((pageNumber - 1) * 12, remainder);
            return PartialView("_ColorGrid", data);
        }

        [HttpPost]
        public ActionResult Details(int red, int green, int blue)
        {
            List<ColorModel> details = new List<ColorModel>();
            ColorModel original = new ColorModel(red, green, blue);
            if (red == -1)
            {
                Random r = new Random();
                original = _colors.ElementAt(r.Next(0, _colors.Count()));
            }
            for (int i = 2; i >= 1; i--)
            {
                double multiplier = Math.Pow(_shadeMultiplier, i);
                details.Add(new ColorModel(Convert.ToInt32(original.red * multiplier), Convert.ToInt32(original.green * multiplier), Convert.ToInt32(original.blue * multiplier)));
            }
            details.Add(original);

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
            
            return PartialView("_Details", details);
        }

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
