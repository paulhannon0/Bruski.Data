using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Bruski.Data.Models;

namespace Bruski.Data
{
    class Program
    {
        private const string absoluteRepositoryPath = @"C:\Code\Bruski.Data";

        public static async Task Main(string[] args)
        {
            var scraper = new Scraper();

            var fermentables = new List<Fermentable>();
            var hops = new List<Hop>();
            var yeasts = new List<Yeast>();
            var otherIngredients = new List<OtherIngredient>();
            var beerStyles = new List<BeerStyle>();

            var fermentablesDocument = await scraper.ScrapeAsync(new Uri("https://www.brewersfriend.com/fermentables/"));
            var hopsDocument = await scraper.ScrapeAsync(new Uri("https://www.brewersfriend.com/hops/"));
            var yeastsDocument = await scraper.ScrapeAsync(new Uri("https://www.brewersfriend.com/yeasts/"));
            var otherIngredientsDocument = await scraper.ScrapeAsync(new Uri("https://www.brewersfriend.com/other/"));
            var beerStylesDocument = await scraper.ScrapeAsync(new Uri("https://www.brewersfriend.com/styles/"));

            var fermentableRows = scraper.GetMultipleNodes(fermentablesDocument, "/html/body/div[4]/div/div[2]/table/tbody").Elements();
            var hopRows = scraper.GetMultipleNodes(hopsDocument, "/html/body/div[4]/div/div[2]/table/tbody").Elements();
            var yeastRows = scraper.GetMultipleNodes(yeastsDocument, "/html/body/div[4]/div/div[2]/table/tbody").Elements();
            var otherIngredientRows = scraper.GetMultipleNodes(otherIngredientsDocument, "/html/body/div[4]/div/div[2]/table/tbody").Elements();
            var beerStyleRows = scraper.GetMultipleNodes(beerStylesDocument, "/html/body/div[4]/div/div[2]/table/tbody").Elements().Where(r => r.Name == "tr");

            foreach (var row in fermentableRows)
            {
                var fermentable = new Fermentable
                {
                    Name = scraper.GetSingleNodeText(fermentablesDocument, row.ChildNodes[0].XPath).Trim(),
                    Country = scraper.GetSingleNodeText(fermentablesDocument, row.ChildNodes[1].XPath).Trim(),
                    Category = scraper.GetSingleNodeText(fermentablesDocument, row.ChildNodes[2].XPath).Trim(),
                    Type = scraper.GetSingleNodeText(fermentablesDocument, row.ChildNodes[3].XPath).Trim(),
                    ColorLovibond = scraper.GetSingleNodeText(fermentablesDocument, row.ChildNodes[4].XPath).Trim(),
                    PPG = scraper.GetSingleNodeText(fermentablesDocument, row.ChildNodes[5].XPath).Trim()
                };

                fermentables.Add(fermentable);
            }

            foreach (var row in hopRows)
            {
                var hop = new Hop
                {
                    Name = scraper.GetSingleNodeText(hopsDocument, row.ChildNodes[0].XPath).Trim(),
                    Type = scraper.GetSingleNodeText(hopsDocument, row.ChildNodes[1].XPath).Trim(),
                    AverageAlphaAcid = double.Parse(scraper.GetSingleNodeText(hopsDocument, row.ChildNodes[2].XPath).Trim()),
                    Use = scraper.GetSingleNodeText(hopsDocument, row.ChildNodes[3].XPath).Trim()
                };

                hops.Add(hop);
            }

            foreach (var row in yeastRows)
            {
                var yeast = new Yeast
                {
                    Name = scraper.GetSingleNodeText(yeastsDocument, row.ChildNodes[0].XPath).Trim(),
                    Laboratory = scraper.GetSingleNodeText(yeastsDocument, row.ChildNodes[1].XPath).Trim(),
                    Code = scraper.GetSingleNodeText(yeastsDocument, row.ChildNodes[2].XPath).Trim(),
                    Type = scraper.GetSingleNodeText(yeastsDocument, row.ChildNodes[3].XPath).Trim(),
                    AlcoholTolerance = scraper.GetSingleNodeText(yeastsDocument, row.ChildNodes[4].XPath).Trim(),
                    Flocculation = scraper.GetSingleNodeText(yeastsDocument, row.ChildNodes[5].XPath).Trim(),
                    Attenuation = scraper.GetSingleNodeText(yeastsDocument, row.ChildNodes[6].XPath).Trim(),
                    MinimumTemp = scraper.GetSingleNodeText(yeastsDocument, row.ChildNodes[7].XPath).Trim(),
                    MaximumTemp = scraper.GetSingleNodeText(yeastsDocument, row.ChildNodes[8].XPath).Trim()
                };

                yeasts.Add(yeast);
            }

            foreach (var row in otherIngredientRows)
            {
                var otherIngredient = new OtherIngredient
                {
                    Name = scraper.GetSingleNodeText(otherIngredientsDocument, row.ChildNodes[0].XPath).Trim(),
                    Type = scraper.GetSingleNodeText(otherIngredientsDocument, row.ChildNodes[1].XPath).Trim(),
                    Use = scraper.GetSingleNodeText(otherIngredientsDocument, row.ChildNodes[2].XPath).Trim()
                };

                otherIngredients.Add(otherIngredient);
            }

            foreach (var row in beerStyleRows)
            {
                var selectedCells = row.ChildNodes.Where(n => n.Name == "td").ToArray();
                var beerStyle = new BeerStyle
                {
                    Name = scraper.GetSingleNodeText(beerStylesDocument, selectedCells[0].XPath).Trim(),
                    Category = scraper.GetSingleNodeText(beerStylesDocument, selectedCells[1].XPath).Trim(),
                    IBU = scraper.GetSingleNodeText(beerStylesDocument, selectedCells[3].XPath).Trim(),
                    ABV = scraper.GetSingleNodeText(beerStylesDocument, selectedCells[4].XPath).Trim(),
                    OG = scraper.GetSingleNodeText(beerStylesDocument, selectedCells[5].XPath).Trim(),
                    FG = scraper.GetSingleNodeText(beerStylesDocument, selectedCells[6].XPath).Trim(),
                    CO2Volume = scraper.GetSingleNodeText(beerStylesDocument, selectedCells[7].XPath).Trim(),
                    ColorLovibond = scraper.GetSingleNodeText(beerStylesDocument, selectedCells[8].XPath).Trim()
                };

                beerStyles.Add(beerStyle);
            }

            File.WriteAllText($@"{absoluteRepositoryPath}\fermentables.json", JsonSerializer.Serialize(fermentables));
            File.WriteAllText($@"{absoluteRepositoryPath}\hops.json", JsonSerializer.Serialize(hops));
            File.WriteAllText($@"{absoluteRepositoryPath}\yeast.json", JsonSerializer.Serialize(yeasts));
            File.WriteAllText($@"{absoluteRepositoryPath}\otherIngredients.json", JsonSerializer.Serialize(otherIngredients));
            File.WriteAllText($@"{absoluteRepositoryPath}\beerStyles.json", JsonSerializer.Serialize(beerStyles));
        }
    }
}
