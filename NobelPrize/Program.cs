using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace NobelPrizeApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Dictionary<string, string> categories = new Dictionary<string, string>()
            {
                {"chemistry","che"},
                {"economy","eco"},
                {"literature","lit"},
                {"peace","pea"},
                {"physics","phy"},
                {"medicine","med"},
                {"exit","exit"}
            };

            while (true) 
            {
                Console.WriteLine("Enter the category you are looking for (or type 'exit' to quit):");
                string category = Console.ReadLine().ToLower();

                if (!categories.ContainsKey(category))
                {
                    Console.WriteLine("Invalid category");
                    continue;
                }

                string apiCategory = categories[category];

                if (category == "exit")
                    break;

                var httpClient = new HttpClient();
                var endpoint = $"https://api.nobelprize.org/2.1/nobelPrizes?nobelPrizeCategory={apiCategory}";

                try
                {
                    var response = await httpClient.GetFromJsonAsync<NobelPrizesResponse>(endpoint);

                    if (response != null && response.Prizes != null && response.Prizes.Count > 0)
                    {
                        // Use Reactive Extensions to process and save the data
                        var observable = response.Prizes.ToObservable();

                        var laureatesData = new List<(string Name, string Year, string DateAwarded, string Motivation)>();

                        observable
                            .SelectMany(prize => prize.Laureates ?? Enumerable.Empty<Laureate>(), (prize, laureate) => new
                            {
                                Year = prize.Year,
                                DateAwarded = prize.DateAwarded,
                                Motivation = laureate?.Motivation?.EnglishMotivation, 
                                Laureate = laureate?.KnownName?.EnglishName
                            })
                            .Subscribe(data =>
                            {
                                if (!string.IsNullOrEmpty(data.Laureate) && !string.IsNullOrEmpty(data.DateAwarded)) 
                                {
                                    laureatesData.Add((
                                        data.Laureate,
                                        data.Year,
                                        data.DateAwarded,
                                        data.Motivation ?? "Motivation not available"
                                    ));
                                }
                            });

                        foreach (var laureateData in laureatesData)
                        {
                            Console.WriteLine($"Name: {laureateData.Name}\nYear: {laureateData.Year}\nDate Awarded: {laureateData.DateAwarded}\nMotivation: {laureateData.Motivation}\n");
                        }

                        var monthCounts = laureatesData
                            .Where(data => !string.IsNullOrEmpty(data.DateAwarded)) 
                            .GroupBy(data => DateTime.Parse(data.DateAwarded).Month)
                            .Select(group => new { Month = group.Key, Count = group.Count() })
                            .OrderByDescending(x => x.Count)
                            .FirstOrDefault();

                        if (monthCounts != null)
                        {
                            var monthName = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthCounts.Month);
                            Console.WriteLine($"\nThe month with the most awards is {monthName} with {monthCounts.Count} awards.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("No data found for the specified category.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to fetch data: {ex.Message}");
                }
            }
        }
    }
}
