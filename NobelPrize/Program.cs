using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reactive.Linq;
using System.Text;
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
            };

            HttpClient httpClient = new HttpClient();
            var loggingEndpoint = "http://localhost:5000/";

            while (true)
            {
                Console.WriteLine("Enter the category you are looking for (or type 'exit' to quit):");
                string category = Console.ReadLine().ToLower();

                if (category == "exit")
                    break;

                if (!categories.ContainsKey(category))
                {
                    Console.WriteLine("Invalid category");
                    await LogMessage(loggingEndpoint, "Failed to fetch data for: " + category + " Invalid category");
                    continue;
                }

                string apiCategory = categories[category];
                var endpoint = $"https://api.nobelprize.org/2.1/nobelPrizes?nobelPrizeCategory={apiCategory}";

                try
                {
                    var response = await httpClient.GetFromJsonAsync<NobelPrizesResponse>(endpoint);

                    if (response != null && response.Prizes != null && response.Prizes.Count > 0)
                    {
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

                        string[] monthNames = {"January", "February", "March", "April", "May", "June","July", "August", "September", "October", "November", "December"};

                        if (monthCounts != null)
                        {
                            string monthName = monthNames[monthCounts.Month - 1];
                            Console.WriteLine($"\nThe month with the most awards is {monthName} with {monthCounts.Count} awards.");
                        }

                        await LogMessage(loggingEndpoint, "Successfully fetched and processed data for: " + category);
                    }
                    else
                    {
                        Console.WriteLine("No data found for the following category: " + category);
                        await LogMessage(loggingEndpoint, "No data found for the following category: " + category);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to fetch data: {ex.Message}");
                    await LogMessage(loggingEndpoint, $"Failed to fetch data: {ex.Message}");
                }
            }
        }

        static async Task LogMessage(string endpoint, string message)
        {
            using (HttpClient client = new HttpClient())
            {
                var content = new StringContent(message, Encoding.UTF8, "application/json");
                await client.PostAsync(endpoint, content);
            }
        }
    }
}
