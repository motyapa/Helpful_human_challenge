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
        private static int RGBStep = 32; //Can error check this for values < 256
        private static List<ColorModel> colors = new List<ColorModel>();
        public ActionResult Index()
        {
            string pathToData = Path.Combine(Environment.CurrentDirectory, RGBStep + "RGBValues.json");
            if (!System.IO.File.Exists(pathToData))
            {
                using (StreamWriter sw = new StreamWriter(pathToData))
                {
                    int loopValue = 256 / RGBStep;
                    for (int blue = 0; blue <= loopValue; blue++)
                    {
                        for (int green = 0; green <= loopValue; green++)
                        {
                            for (int red = 0; red <= loopValue; red++)
                            {
                                ColorModel cm = new ColorModel(red * RGBStep - 1, green * RGBStep - 1, blue * RGBStep - 1);

                                
                                colors.Add(cm);
                            }
                        }
                    }
                    Shuffle(colors, RGBStep);
                    var allData = JsonConvert.SerializeObject(colors, Formatting.Indented);
                    sw.Write(allData);
                }
            }
            else
            {
                colors = JsonConvert.DeserializeObject<List<ColorModel>>(System.IO.File.ReadAllText(pathToData));
            }

            IndexViewModel data = new IndexViewModel(colors.GetRange(0, 12), colors.Count());
            return View(data);
        }

        [HttpPost]
        public ActionResult GetColors(int pageNumber)
        {
            int remainder = Math.Min(12, colors.Count - (pageNumber - 1) * 12);
            List<ColorModel> data = colors.GetRange((pageNumber - 1) * 12, remainder);
            return PartialView("_ColorGrid", data);
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
