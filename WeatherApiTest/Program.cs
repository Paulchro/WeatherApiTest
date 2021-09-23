using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherApiTest
{
    public class ConsolidatedWeather
    {
        public object id { get; set; }
        public string weather_state_name { get; set; }
        public string weather_state_abbr { get; set; }
        public string wind_direction_compass { get; set; }
        public DateTime created { get; set; }
        public string applicable_date { get; set; }
        public double min_temp { get; set; }
        public double max_temp { get; set; }
        public double the_temp { get; set; }
        public double wind_speed { get; set; }
        public double wind_direction { get; set; }
        public double air_pressure { get; set; }
        public int humidity { get; set; }
        public double visibility { get; set; }
        public int predictability { get; set; }
    }

    public class Parent
    {
        public string title { get; set; }
        public string location_type { get; set; }
        public int woeid { get; set; }
        public string latt_long { get; set; }
    }

    public class Source
    {
        public string title { get; set; }
        public string slug { get; set; }
        public string url { get; set; }
        public int crawl_rate { get; set; }
    }

    public class Root
    {
        public List<ConsolidatedWeather> consolidated_weather { get; set; }
        public DateTime time { get; set; }
        public DateTime sun_rise { get; set; }
        public DateTime sun_set { get; set; }
        public string timezone_name { get; set; }
        public Parent parent { get; set; }
        public List<Source> sources { get; set; }
        public string title { get; set; }
        public string location_type { get; set; }
        public int woeid { get; set; }
        public string latt_long { get; set; }
        public string timezone { get; set; }
    }
    public class WeatherDirect
    {
        public long id { get; set; }
        public string weather_state_name { get; set; }
        public string weather_state_abbr { get; set; }
        public string wind_direction_compass { get; set; }
        public DateTime created { get; set; }
        public string applicable_date { get; set; }
        public double? min_temp { get; set; }
        public double? max_temp { get; set; }
        public double? the_temp { get; set; }
        public double wind_speed { get; set; }
        public double wind_direction { get; set; }
        public double? air_pressure { get; set; }
        public int? humidity { get; set; }
        public double? visibility { get; set; }
        public int predictability { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Welcome to my weather application, type a city");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("\nA list with matching cities and their codes will appear,\nselect a code to get the weather for the next 5 days.\n");
            Console.ResetColor();
            Dictionary<int, string> citycodes = FindCode();
            int valid = ValidCode(citycodes);
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"-Your choice: {citycodes[valid]}-");
            Console.ResetColor();
            Console.WriteLine();
            Console.Write("If you want to get 5 days weather forecast press");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(" 1 ");
            Console.ResetColor();
            Console.Write("or\nif you want to get weather for a specific day (2013 - "
                + DateTime.Now.ToString("dd/MM/yyyy") + " +5-10 days " + ") press");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(" 2"); 
            Console.ResetColor();
            Console.Write(".\n\n");
            WeatherChoose(citycodes, valid);

        }

        private static void WeatherChoose(Dictionary<int, string> citycodes, int choosencity)
        {
            string forecastselect = Console.ReadLine();
            switch (forecastselect)
            {
                case "2":
                    GetWeatherSpec(citycodes, choosencity);
                    break;
                case "1":
                    GetWeather(citycodes, choosencity);
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Wrong answer, try again");
                    Console.ResetColor();
                    WeatherChoose(citycodes, choosencity);
                    break;
            }
        }

        private static void GetWeather(Dictionary<int, string> citycodes, int choosencity)
        {
            int d = 0;
            var json = new WebClient().DownloadString($"https://www.metaweather.com/api/location/{choosencity}/");
            var root = JsonConvert.DeserializeObject<Root>(json);
            Console.WriteLine("Weather on: " + root.title + " for the next 5 days");
            foreach (var item in root.consolidated_weather)
            {
                Console.WriteLine(DateTime.Now.AddDays(d).ToString("dd/MM/yyyy") + " Max temp: " + String.Format("{0:0}", item.max_temp) 
                    + ". Min temp: " + String.Format("{0:0}", item.min_temp) + " Conditions: " + item.weather_state_name);
                d++;
            }
        }
        private static void GetWeatherSpec(Dictionary<int, string> citycodes, int choosencity)
        {
            Console.WriteLine("Choose a date (dd/MM/yyyy)");
            DateTime date;

            if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Wrong input, try again");
                Console.ResetColor();
                GetWeatherSpec(citycodes, choosencity);
            }
            var json = new WebClient().DownloadString($"https://www.metaweather.com/api/location/{choosencity}/" 
                + date.Year.ToString() + "/" + date.Month.ToString() + "/" + date.Day.ToString() + "/");
            List<WeatherDirect> root = JsonConvert.DeserializeObject<List<WeatherDirect>>(json);
            Console.WriteLine("Weather on: " + citycodes.Values.ElementAt(0) + " for the given day");

            Console.WriteLine(date.ToString("dd/MM/yyyy") + ": Max temp: " + String.Format("{0:0}", root[0].max_temp) 
                + ". Min temp: " + String.Format("{0:0}", root[0].min_temp) + " Conditions: " + root[0].weather_state_name);
             
        }

        private static Dictionary<int, string> FindCode()
        {
            
            Dictionary<int, string> citycodes = new Dictionary<int, string>();
            int i = 1;
            string json;
            do
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Please enter a valid location!");
                Console.ResetColor();
                string query = Console.ReadLine();
                json = new WebClient().DownloadString("https://www.metaweather.com/api/location/search/?query=locationfinder".Replace("locationfinder", query));
                //if (json == "[]")
                //{
                //    Console.ForegroundColor = ConsoleColor.Red;
                //    Console.WriteLine("Sorry, no city found with this name. Try again.");
                //    Console.ResetColor();
                //    citycodes = null;
                //}
            } while (json == "[]");
            List<Root> root = JsonConvert.DeserializeObject<List<Root>>(json);
                if (root.Count == 1)
                {
                    citycodes.Add(root[0].woeid, root[0].title);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nFound only 1 city: " + root[0].title + "\n");
                    Console.ResetColor();
                }
                else if (root.Count > 1)
                {
                Thread thread = new Thread(Dots);
                thread.Start();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("Retrieving cities...Wait until its done.");
                Console.ResetColor();

                foreach (var item in root)
                    {
                        citycodes.Add(item.woeid, item.title);
                        if (root.Count > 1)
                        {
                            var json1 = new WebClient().DownloadString($"https://www.metaweather.com/api/location/{item.woeid}/");
                            var root1 = JsonConvert.DeserializeObject<Root>(json1);
                            //Console.WriteLine(i.ToString() + ": " + item.title + " " + root1.parent.title);
                            //i++;
                        }
                    }
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nDone!");
                    thread.Abort();
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine($"Found {citycodes.Count} matchings.\n");
                    Console.ResetColor();
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                foreach (var item in citycodes)
                    {
                    
                    Console.WriteLine(i.ToString() + ": " + item.Value);
                        i++;
                    }
                Console.WriteLine();
                Console.ResetColor();
            }
           
            return citycodes;
        }

        public static int ValidCode(Dictionary<int, string> list)
        {
            int selection;
            if (list.Count > 1)
            {
                Console.Write("Choose a city\n");
                while (!int.TryParse(Console.ReadLine(), out selection) || selection > list.Count)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write("Wrong value, try again!\n(Choose from the list above)\n");
                    Console.ResetColor();
                }
                bool isInList = list.Keys.ElementAt(selection - 1) == -1;
                if (isInList)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Theres no city with this code! Try again...");
                    Console.ResetColor();
                    ValidCode(list);
                }
                return list.Keys.ElementAt(selection - 1);
            }
            return list.Keys.ElementAt(0);
        }
        public static void Dots()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            while (true)
            {
                for (int i = 0; i < 3; i++)
                {
                    Console.Write('.');
                    System.Threading.Thread.Sleep(1000);
                    if (i == 2)
                    {
                        Console.Write("\r   \r");
                        i = -1;
                        System.Threading.Thread.Sleep(1000);
                    }
                }
            }
           
        }
    }
}
