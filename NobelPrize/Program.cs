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
            while (true) // Keep the program running indefinitely
            {
                Console.WriteLine("Enter the category you are looking for (or type 'exit' to quit):");
                string category = Console.ReadLine();

                if (string.IsNullOrEmpty(category))
                {
                    Console.WriteLine("Category cannot be empty.");
                    continue; // Skip the rest of the loop iteration and prompt again
                }

                if (category.ToLower() == "exit")
                    break; // Exit the loop and end the program

                var httpClient = new HttpClient();
                var endpoint = $"https://api.nobelprize.org/v1/prize.json?category={category.ToLower()}";

                try
                {
                    // Fetch data from the Nobel Prize API
                    var response = await httpClient.GetFromJsonAsync<NobelPrizesResponse>(endpoint);

                    if (response != null && response.Prizes != null && response.Prizes.Count > 0)
                    {
                        // Use Reactive Extensions to process and save the data
                        var observable = response.Prizes.ToObservable();

                        var laureatesData = new List<(string Name, string Year, string Motivation)>();

                        observable
                            .SelectMany(prize => prize.Laureates ?? Enumerable.Empty<Laureate>(), (prize, laureate) => new
                            {
                                Year = prize.Year,
                                Motivation = laureate?.Motivation, // Null-conditional operator used here
                                Laureate = laureate
                            })
                            .Subscribe(data =>
                            {
                                if (data.Laureate != null) // Check if Laureate is not null
                                {
                                    // Adding data to laureatesData
                                    laureatesData.Add((
                                        $"{data.Laureate.Firstname} {data.Laureate.Surname}",
                                        data.Year,
                                        data.Motivation ?? "Motivation not available" // Providing a default value if Motivation is null
                                    ));
                                }
                            });

                        // Now 'laureatesData' contains the desired information
                        foreach (var laureateData in laureatesData)
                        {
                            Console.WriteLine($"Name: {laureateData.Name}\nYear: {laureateData.Year}\nMotivation: {laureateData.Motivation}\n");
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
