using System.Globalization;
using CsvHelper;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Webscraper
{
    public class VideoInfo
    {
        public string Title { get; set; }
        public string Uploader { get; set; }
        public string Views { get; set; }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Verkrijg de zoekterm van de gebruiker
            Console.Write("Enter the YouTube search term: ");
            string searchTerm = Console.ReadLine();

            // Initialiseren van de ChromeDriver
            IWebDriver driver = new ChromeDriver();

            // Navigeer naar de Youtube zoek resultaat pagina
            driver.Navigate().GoToUrl($"https://www.youtube.com/results?search_query={searchTerm}");

            // Wacht voor tot dat de pagina helemaal geladen is
            System.Threading.Thread.Sleep(5000); // Adjust the delay as needed

            //  Pas een filter toe voor uploads van vandaag en scrape de  informatie
            var videoInfoList = ApplyFilterForToday(driver);

            // Sluit de browser
            driver.Quit();


            // Slaag de data op naar een CSV file
            string csvFilePath = SaveToCsv(videoInfoList, "videos.csv");

            // Slaag de data op naar een JSON file 
            string jsonFilePath = SaveToJson(videoInfoList, "videos.json");

            // Print het bericht over de opgeslagen files
            Console.WriteLine($"Data downloaded to CSV file: {csvFilePath}");
            Console.WriteLine($"Data downloaded to JSON file: {jsonFilePath}");
        }

        static List<VideoInfo> ApplyFilterForToday(IWebDriver driver)
        {
            // Zoek de "Filters" knop
            var filtersButton = driver.FindElement(By.CssSelector("button.yt-spec-button-shape-next.yt-spec-button-shape-next--text.yt-spec-button-shape-next--mono.yt-spec-button-shape-next--size-m.yt-spec-button-shape-next--icon-trailing"));

            // Klik op de "Filters" knop met JavaScript
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", filtersButton);

          
            // Zoek en klik op de "Vandaag" link in de pop-up
            var vandaagLink = driver.FindElement(By.CssSelector("yt-formatted-string.style-scope.ytd-search-filter-renderer"));
            vandaagLink.Click();

            // Wacht tot de filter wordt uitgevoerd
            System.Threading.Thread.Sleep(5000);

            // Verkrijg de pagina source nadat de pagina geladen is
            string html = driver.PageSource;

            // Scrape de informatie van de geladen pagina 
            var htmlDocument = new HtmlAgilityPack.HtmlDocument();
            htmlDocument.LoadHtml(html);

            // Verkrijg alle video containers in de zoek resultaten
            var videoContainers = htmlDocument.DocumentNode.SelectNodes("//div[@id='contents']//ytd-video-renderer");

            // Maak een lijst waar alle video informatie wordt bijgehouden
            List<VideoInfo> videoInfoList = new List<VideoInfo>();

            // Verwerk elke video container
            int counter = 1;
            foreach (var videoContainer in videoContainers.Take(5))
            {
                // Scrape de informatie van elke video container
                var titleElement = videoContainer.SelectSingleNode(".//a[@id='video-title']");
                var title = titleElement?.InnerText.Trim() ?? "Title not found";

                var uploaderElement = videoContainer.SelectSingleNode(".//a[@class='yt-simple-endpoint style-scope yt-formatted-string']");
                var uploader = uploaderElement?.InnerText.Trim() ?? "Uploader not found";

                var viewsElement = videoContainer.SelectSingleNode(".//span[contains(@class, 'style-scope ytd-video-meta-block')]");
                var views = CleanUpViews(viewsElement?.InnerText.Trim());

                // Print de informatie
                Console.WriteLine($"***** Video {counter} *****");
                Console.WriteLine($"Title: {title}");
                Console.WriteLine($"Uploader: {uploader}");
                Console.WriteLine($"Views: {views}");
                Console.WriteLine();

                counter++;

                // Creer een VideoInfo object en voeg het toe aan de lijst 
                var videoInfo = new VideoInfo
                {
                    Title = title,
                    Uploader = uploader,
                    Views = views
                };

                videoInfoList.Add(videoInfo);
            }

            return videoInfoList;
        }

        static string CleanUpViews(string rawViews)
        {
            if (rawViews == null)
                return "Views not found";

            // Verwijder "nbsp;" en "weergaven" van de views, maar behou "mln."
            string cleanedViews = rawViews.Replace("&nbsp;", "").Replace("weergaven", "").Trim();

            // Voeg spacie toe acter "mln" voor consistent formatting
            cleanedViews = cleanedViews.Replace("mln", "mln ");

            return cleanedViews;
        }

        static string SaveToCsv(List<VideoInfo> videoInfoList, string fileName)
        {
            string filePath = GetFilePath(fileName);

            using (var writer = new StreamWriter(filePath))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(videoInfoList);
                }
            }

            return filePath;
        }

        static string SaveToJson(List<VideoInfo> videoInfoList, string fileName)
        {
            string filePath = GetFilePath(fileName);

            File.WriteAllText(filePath, JsonConvert.SerializeObject(videoInfoList, Formatting.Indented));

            return filePath;
        }

        static string GetFilePath(string fileName)
        {
            string directoryPath = @"H:\school\school 2023-2024\devops\yt scraper files";
            string filePath = Path.Combine(directoryPath, fileName);

            // Check of the mappen structuur bestaat
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            return filePath;
        }
    }
}