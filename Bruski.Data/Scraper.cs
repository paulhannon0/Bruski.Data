using System;
using System.Net.Http;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Bruski.Data
{
    internal class Scraper
    {
        public string GetSingleNodeText(HtmlDocument document, string xPath)
        {
            return document.DocumentNode.SelectSingleNode(xPath).InnerText;
        }

        public HtmlNodeCollection GetMultipleNodes(HtmlDocument document, string xPath)
        {
            return document.DocumentNode.SelectNodes(xPath);
        }

        public async Task<HtmlDocument> ScrapeAsync(Uri uri)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(uri.AbsoluteUri);
            var pageDocument = new HtmlDocument();

            pageDocument.LoadHtml(await response.Content.ReadAsStringAsync());

            return pageDocument;
        }
    }
}
