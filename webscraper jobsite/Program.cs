using System.Globalization;
using CsvHelper;
using HtmlAgilityPack;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Webscraperictjobs
{
    class Program
    {
        static void Main(string[] args)
        {
            // Krijg de zoekterm van de gebruiker
            Console.Write("Enter the job search term: ");
            string searchTerm = Console.ReadLine();

            // Set up van de  ChromeDriver
            using (IWebDriver driver = new ChromeDriver())
            {
                // Maak de url met de zoekterm van de gebruiker
                string url = $"https://www.ictjob.be/nl/it-vacatures-zoeken?keywords_options=OR&SortOrder=DESC&SortField=RANK&From=0&To=19&keywords={searchTerm}";

                // Navigeer naar de bepaalde URL
                driver.Navigate().GoToUrl(url);

                // Wacht tot dat de hele pagina geladen is
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                wait.Until(driver => ((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState").Equals("complete"));

                // Get the job links and information from the search results
                var jobInfoList = GetJobInformation(driver, 5); // Get the first 5 jobs

                // Print de informatie naar de console
                PrintJobInformation(jobInfoList);

                // Sla de data op naar een CSV-bestand
                string csvFilePath = SaveToCsv(jobInfoList, "jobs.csv");

                // Sla de data op naar een JSON-bestand
                string jsonFilePath = SaveToJson(jobInfoList, "jobs.json");

                // Print het bericht over de opgeslagen files
                Console.WriteLine($"Data downloaded to CSV file: {csvFilePath}");
                Console.WriteLine($"Data downloaded to JSON file: {jsonFilePath}");
            }
        }

        static List<JobInfo> GetJobInformation(IWebDriver driver, int numberOfJobs)
        {
            List<JobInfo> jobInfoList = new List<JobInfo>();

            // Verkrijg de job informatie van de zoek resultaten
            var jobInfoNodes = driver.FindElements(By.CssSelector("li.search-item.clearfix"));

            if (jobInfoNodes != null && jobInfoNodes.Any())
            {
                // Ga over elke vacature en scrape de informatie ervan
                foreach (var jobNode in jobInfoNodes.Take(numberOfJobs))
                {
                    // Gebruik HtmlAgilityPack om de vacature node te laden
                    var jobNodeHtmlDocument = new HtmlDocument();
                    jobNodeHtmlDocument.LoadHtml(jobNode.GetAttribute("outerHTML"));

                    // Extract de informatie van de vacature node
                    var title = GetInnerText(jobNodeHtmlDocument, "//h2[@class='job-title']");
                    var company = GetInnerText(jobNodeHtmlDocument, "//span[@class='job-company']");
                    var location = GetInnerText(jobNodeHtmlDocument, "//span[@itemprop='addressLocality']");
                    var keywords = GetInnerText(jobNodeHtmlDocument, "//span[@class='job-keywords']");
                    var jobLink = jobNode.FindElement(By.CssSelector("a.job-title.search-item-link")).GetAttribute("href");

                    // Voeg de informatie toe aan de lijst
                    jobInfoList.Add(new JobInfo
                    {
                        Title = title,
                        Company = company,
                        Location = location,
                        Keywords = keywords,
                        JobLink = jobLink
                    });
                }
            }

            return jobInfoList;
        }

        static void PrintJobInformation(List<JobInfo> jobInfoList)
        {
            foreach (var jobInfo in jobInfoList)
            {
                Console.WriteLine($"Title: {jobInfo.Title}");
                Console.WriteLine($"Company: {jobInfo.Company}");
                Console.WriteLine($"Location: {jobInfo.Location}");
                Console.WriteLine($"Keywords: {jobInfo.Keywords}");
                Console.WriteLine($"Link: {jobInfo.JobLink}");
                Console.WriteLine();
            }
        }

        static string GetInnerText(HtmlDocument document, string xpath)
        {
            var element = document.DocumentNode.SelectSingleNode(xpath);
            return element?.InnerText.Trim() ?? "Not found";
        }

        static string SaveToCsv(List<JobInfo> jobInfoList, string fileName)
        {
            string filePath = GetFilePath(fileName);

            using (var writer = new StreamWriter(filePath))
            {
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(jobInfoList);
                }
            }

            return filePath;
        }

        static string SaveToJson(List<JobInfo> jobInfoList, string fileName)
        {
            string filePath = GetFilePath(fileName);

            File.WriteAllText(filePath, JsonConvert.SerializeObject(jobInfoList, Formatting.Indented));

            return filePath;
        }

        static string GetFilePath(string fileName)
        {
            string directoryPath = @"H:\school\school 2023-2024\devops\ictjobs scraper";
            string filePath = Path.Combine(directoryPath, fileName);

            // Check of de mappen structuur bestaat
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            return filePath;
        }
    }

    public class JobInfo
    {
        public string Title { get; set; }
        public string Company { get; set; }
        public string Location { get; set; }
        public string Keywords { get; set; }
        public string JobLink { get; set; }
    }
}
