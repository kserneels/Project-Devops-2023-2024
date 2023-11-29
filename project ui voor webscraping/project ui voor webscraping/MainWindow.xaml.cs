using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace YourNamespace
{
    public partial class MainWindow : Window
    {
        private async void ScrapeData_Click(object sender, RoutedEventArgs e)
        {
            // Get season and patch number from input fields
            if (!int.TryParse(SeasonTextBox.Text, out int seasonNumber) ||
                !int.TryParse(PatchTextBox.Text, out int patchNumber))
            {
                ResultText.Text = "Invalid season or patch number. Please enter valid numbers.";
                return;
            }

            // Additional check for valid season number
            if (seasonNumber <= 0)
            {
                ResultText.Text = "Invalid season number. Please enter a valid positive number.";
                return;
            }

            // Display a loading message
            ResultText.Text = "Scraping data...";

            try
            {
                // Scraping logic using Selenium and HtmlAgilityPack
                string scrapedData = await ScrapingLogic.ScrapeData(seasonNumber, patchNumber, ResultText);

                // Display scraped data
                ResultText.Text = scrapedData;

                // Save to CSV and JSON files
                string csvFilePath = SaveToFile(scrapedData, $"Patch_{seasonNumber}_{patchNumber}.csv");
                string jsonFilePath = SaveToFile(scrapedData, $"Patch_{seasonNumber}_{patchNumber}.json");

                // Print the file paths
                ResultText.Text += $"\nData downloaded to CSV file: {csvFilePath}";
                ResultText.Text += $"\nData downloaded to JSON file: {jsonFilePath}";
            }
            catch (Exception ex)
            {
                if (ResultText.Text == "No more patches available.")
                {
                    // Display scraped data
                    ResultText.Text = "No more patches available.";
                }
                else
                {
                    // Handle exceptions and display an error message
                    ResultText.Text = $"Error: {ex.Message}";
                }
            }
        }

        private string SaveToFile(string data, string fileName)
        {
            string directoryPath = @"H:\school\school 2023-2024\devops\patchscraper";
            string filePath = Path.Combine(directoryPath, fileName);

            // Check if the folder structure exists
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Write data to file
            File.WriteAllText(filePath, data);

            return filePath;
        }
    }

    public static class ScrapingLogic
    {
        public static async Task<string> ScrapeData(int seasonNumber, int patchNumber, System.Windows.Controls.TextBlock ResultText)
        {
            // Configure Selenium
            var chromeOptions = new ChromeOptions();
            //chromeOptions.AddArgument("--headless"); // Run Chrome in headless mode (no GUI)

            using (var driver = new ChromeDriver(chromeOptions))
            {
                // Navigate to the League of Legends patch notes website
                driver.Navigate().GoToUrl($"https://www.leagueoflegends.com/en-us/news/tags/patch-notes/");

                // Wait for the page to load or for elements to appear
                // You might need to adjust the wait time based on your website
                await Task.Delay(5000);

                // Use a loop to keep trying until the element is found or no more "Load More" button
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30)); // Adjust the timeout as needed

                while (true)
                {
                    try
                    {
                        // Find the specific patch notes based on the season and patch number
                        var patchXPath = $"//h2[contains(text(), 'Patch {seasonNumber}.{patchNumber}')]";
                        var patchNotesNode = driver.FindElement(By.XPath(patchXPath));

                        // Use JavaScript to click on the title element
                        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                        js.ExecuteScript("arguments[0].click();", patchNotesNode);

                        // Wait for the patch notes page to load
                        await Task.Delay(5000);

                        // Get the patch notes page source
                        var patchNotesPageSource = driver.PageSource;

                        // Use HtmlAgilityPack to parse the patch notes page HTML
                        var patchNotesHtmlDocument = new HtmlDocument();
                        patchNotesHtmlDocument.LoadHtml(patchNotesPageSource);

                        // Extract the data you need from the patch notes page
                        var patchNotesData = patchNotesHtmlDocument.DocumentNode
                            .SelectSingleNode("//div[@class='style__Content-sc-17x3yhp-1 hAcEIj']")
                            ?.InnerText;

                        return patchNotesData ?? "No data found in the patch notes page.";
                    }
                    catch (NoSuchElementException)
                    {
                        // Handle the case where the element is not found
                        Console.WriteLine($"Element not found for Patch {seasonNumber}.{patchNumber}");

                        // Check if "Load More" button is available and visible
                        var loadMoreButtons = driver.FindElements(By.XPath("//button[contains(@class, 'style__LoadMoreButton-sc-1ynvx8h-5') and contains(@class, 'is-visible')]"));

                        if (loadMoreButtons.Count > 0)
                        {
                            // Use JavaScript to click on the first visible "Load More" button
                            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                            js.ExecuteScript("arguments[0].click();", loadMoreButtons[0]);

                            // Wait for the page to load or for elements to appear after clicking "Load More"
                            await Task.Delay(5000);
                        }
                        else
                        {
                            // If no visible "Load More" button is found, break out of the loop
                            ResultText.Text = "No more patches available.";
                            break;
                        }
                    }
                    catch (ElementNotInteractableException)
                    {
                        // Handle the case where the element is not interactable (not clickable)
                        // Add a delay and try again
                        await Task.Delay(2000);
                    }
                    catch (StaleElementReferenceException)
                    {
                        // Handle the case where the element is no longer attached to the DOM
                        await Task.Delay(2000);
                    }
                }

                return $"No patch notes found for Patch {seasonNumber}.{patchNumber}.";
            }
        }
    }
}
