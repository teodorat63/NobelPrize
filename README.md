# NobelPrizeApp

NobelPrizeApp is a console application written in C# that allows users to fetch and display information about Nobel Prize laureates from various categories using the Nobel Prize API. The app also determines which month has the most awards based on the data fetched. The application uses Reactive Extensions (Rx) to process and filter the laureate data asynchronously.


## Features

- Fetch and display Nobel Prize laureates by category (Chemistry, Economy, Literature, Peace, Physics, Medicine).
- Display detailed information about each laureate, including their name, award year, date awarded, and motivation.
- Identify and display the month with the most awards.


### Main Program

The main program (`Program.cs`) contains the logic for:
- Displaying prompts to the user.
- Fetching data from the Nobel Prize API.
- Processing and displaying the fetched data.
- Determining and displaying the month with the most awards.

### Reactive Extensions

The application uses Reactive Extensions (Rx) to process and filter the laureate data asynchronously.

## Acknowledgements

- [Nobel Prize API](https://api.nobelprize.org/)
- [Reactive Extensions](https://github.com/dotnet/reactive)
