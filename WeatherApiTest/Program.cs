using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
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
    public class Root2
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
            Console.WriteLine("Welcome to my weather application, type a city");
            Console.WriteLine("\nA list with matching cities and their codes will appear,\nselect a code to get the weather for the next 5 days.\n");
            List<int> citycodes = FindCode();
            Console.WriteLine($"If you want to get 5 days weather forecast press -1- or\nif you want to get weather for a specific day (2013 - " + DateTime.Now.ToString("dd/MM/yyyy") +" +5-10 days " + ") press -2-");
            WeatherChoose(citycodes);

        }

        private static void WeatherChoose(List<int> citycodes)
        {
            string forecastselect = Console.ReadLine();
            switch (forecastselect)
            {
                case "2":
                    GetWeatherSpec(citycodes);
                    break;
                case "1":
                    GetWeather(citycodes);
                    break;
                default:
                    Console.WriteLine("Wrong answer, try again");
                    WeatherChoose(citycodes);
                    break;
            }
        }

        private static void GetWeather(List<int> citycodes)
        {
            int d = 0;
            var json = new WebClient().DownloadString($"https://www.metaweather.com/api/location/{ValidCode(citycodes)}/");
            var root = JsonConvert.DeserializeObject<Root>(json);
            Console.WriteLine("Weather on: " + root.title + " for the next 5 days");
            foreach (var item in root.consolidated_weather)
            {
                Console.WriteLine(DateTime.Now.AddDays(d).ToString("dd/MM/yyyy") + " Max temp: " + String.Format("{0:0}", item.max_temp) + ". Min temp: " + String.Format("{0:0}", item.min_temp) + " Conditions: " + item.weather_state_name);
                d++;
            }
        }
        private static void GetWeatherSpec(List<int> citycodes)
        {
            int d = 0;
            Console.WriteLine("Choose a date (dd/MM/yyyy)");
            DateTime date;

            if (!DateTime.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                Console.WriteLine("Wrong input, try again");
                GetWeatherSpec(citycodes);
            }
            var json = new WebClient().DownloadString($"https://www.metaweather.com/api/location/{ValidCode(citycodes)}/" + date.Year.ToString() + "/" + date.Month.ToString() + "/" + date.Day.ToString() + "/");
            List<Root2> root = JsonConvert.DeserializeObject<List<Root2>>(json);
            //Console.WriteLine("Weather on: " + root. + " for the next 5 days");
            
             Console.WriteLine(date.ToString("dd/MM/yyyy") + ": Max temp: " + String.Format("{0:0}", root[0].max_temp) + ". Min temp: " + String.Format("{0:0}", root[0].min_temp) + " Conditions: " + root[0].weather_state_name);
             
        }

        private static List<int> FindCode()
        {
            List<int> citycodes = new List<int>();
            int i = 1;
            string query = Console.ReadLine();
            var json = new WebClient().DownloadString("https://www.metaweather.com/api/location/search/?query=locationfinder".Replace("locationfinder", query));
            List<Root> root = JsonConvert.DeserializeObject<List<Root>>(json);
            if(root.Count == 1)
            {
                citycodes.Add(root[0].woeid);
                Console.WriteLine("Found only 1 city: " + root[0].title +"\n");
            }
            else if (root.Count > 1)
            {
                Console.WriteLine("Retrieving cities...Wait until its done.");
                foreach (var item in root)
                {
                    citycodes.Add(item.woeid);
                    if (root.Count > 1)
                    {
                        var json1 = new WebClient().DownloadString($"https://www.metaweather.com/api/location/{item.woeid}/");
                        var root1 = JsonConvert.DeserializeObject<Root>(json1);
                        Console.WriteLine(i.ToString() + ": " + item.title + " " + root1.parent.title);
                        i++;
                    }
                }
                Console.WriteLine("\nDone!");
                Console.WriteLine($"Found {citycodes.Count} matchings.");
            }
            else
            {
                Console.WriteLine("Sorry, no city found with this name. Try again.");
                FindCode();
            }

            return citycodes;
        }

        public static int ValidCode(List<int> list)
        {
            int selection;
            int mycode;
            if (list.Count > 1)
            {
                Console.Write("Choose a city\n");
                while (!int.TryParse(Console.ReadLine(), out selection))
                    Console.Write("The value must be of integer type, try again: ");
                bool isInList = list.IndexOf(list[selection - 1]) == -1;
                if (isInList)
                {
                    Console.WriteLine("Theres no city with this code! Try again...");
                    ValidCode(list);
                }
                return list[selection - 1];
            }
            return list[0]; 
        }
    }
}
