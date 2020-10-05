using System;
using System.Collections.Generic;
using System.IO;
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
            var fermentablesDocument = await scraper.ScrapeAsync(new Uri("https://www.brewersfriend.com/fermentables/"));
            var rows = scraper.GetMultipleNodes(fermentablesDocument, "/html/body/div[4]/div/div[2]/table/tbody").Elements();

            foreach (var row in rows)
            {
                var fermentable = new Fermentable
                {
                    Name = scraper.GetSingleNodeText(fermentablesDocument, row.ChildNodes[0].XPath).Trim(),
                    Country = scraper.GetSingleNodeText(fermentablesDocument, row.ChildNodes[1].XPath).Trim(),
                    Category = scraper.GetSingleNodeText(fermentablesDocument, row.ChildNodes[2].XPath).Trim(),
                    Type = scraper.GetSingleNodeText(fermentablesDocument, row.ChildNodes[3].XPath).Trim(),
                    Color = scraper.GetSingleNodeText(fermentablesDocument, row.ChildNodes[4].XPath).Trim(),
                    PPG = scraper.GetSingleNodeText(fermentablesDocument, row.ChildNodes[5].XPath).Trim()
                };

                fermentables.Add(fermentable);
            }

            File.WriteAllText($@"{absoluteRepositoryPath}\fermentables.json", JsonSerializer.Serialize(fermentables));
        }
    }
}
