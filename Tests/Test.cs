using System.Net;
using System.Text;
using ELPais.Pages;
using Google.Cloud.Translation.V2;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using static System.Net.Mime.MediaTypeNames;

namespace ELPais.Tests
{
    public class Tests : commonMethod
    {
        
        public String Locator_By_Xpath_Opinion =
            "//a[@cmp-ltrk-idx='1' and contains(text(),'Opinión')]";
        public String Locator_By_Xpath_AcceptCookies = "//button[@id='didomi-notice-agree-button']";
        public String Locator_By_Xpath_ArticlesHeaders = "//article/header/h2/a";
        public String Locator_By_Xpath_Articles_Content = "//article/p";
        public String Locator_By_Xpath_Articles_Images = "//article";

        [Test]
        public void Test1()
        {
            // Create a new instance of selenium web driver
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--disable-popup-blocking");
            chromeOptions.AddArgument("--no-sandbox");
            chromeOptions.AddArgument("--disable-infobars");
            chromeOptions.AddArgument("--start-maximized");
            IWebDriver driver = new ChromeDriver(chromeOptions);
            driver.Manage().Cookies.DeleteAllCookies();
            StringBuilder builder = new StringBuilder();
            string dir = Directory.GetCurrentDirectory();
            string downloadPath = dir.Replace("bin\\Debug\\net8.0", "DownloadedImages");

            

            // navigate to URL
            driver.Navigate().GoToUrl("https://elpais.com/");

            //Accept Cookies
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(
                ExpectedConditions.ElementToBeClickable(By.XPath(Locator_By_Xpath_AcceptCookies))
            );
            driver.FindElement(By.XPath(Locator_By_Xpath_AcceptCookies)).Click();

            //clicking on opinion

            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath(Locator_By_Xpath_Opinion)));
            driver.FindElement(By.XPath(Locator_By_Xpath_Opinion)).Click();

            // Get the website title
            string title = driver.Title;
            Console.WriteLine($"Website Title: {title}\n");

            var articleTitiles = driver.FindElements(By.XPath(Locator_By_Xpath_ArticlesHeaders));
            var articleContent = driver.FindElements(By.XPath(Locator_By_Xpath_Articles_Content));
            var articleImage = driver.FindElements(By.XPath(Locator_By_Xpath_Articles_Images));

            for (int i = 0; i <= 4; i++)
            {
                int count = i + 1;

                // fetch images


                string header = articleTitiles[i].Text;
                Console.WriteLine($"Article {i + 1} (Spanish): {header}", '\n');

                string Content = articleContent[i].Text;
                Console.WriteLine($"Article {i + 1} (Spanish): {Content}", '\n');

                // Translate the header
                var client = TranslationClient.Create();
                var translatedHeader = client.TranslateText(header, "en");
                var translatedContent = client.TranslateText(Content, "en");

                Console.WriteLine(
                    $"Translated Article Header {i + 1} (English): {translatedHeader.TranslatedText}",
                    '\n'
                );

                Console.WriteLine(
                    $"Translated Article Content {i + 1} (English): {translatedContent.TranslatedText}",
                    '\n'
                );

                builder.AppendLine(translatedHeader.TranslatedText);

                try
                {
                    var imageElement = articleImage[i].FindElement(By.XPath(".//img"));
                    string imageURL = imageElement.GetAttribute("src");
                    string ImageName = imageElement.GetAttribute("alt");
                    //Download image
                    WebClient downloader = new WebClient();

                    downloader.DownloadFile(imageURL, downloadPath + "\\\\" + "Image" + i + ".jpg");
                    Console.WriteLine("Image is downloaded to : " + downloadPath);
                }
                catch (NoSuchElementException)
                {
                    Console.WriteLine("No Image available for this element");
                }

                Console.WriteLine(new string('-', 500));
            }
            Console.WriteLine("\nAll Translated Headers:");
            Console.WriteLine(builder);

            Console.WriteLine(new string('-', 500));

            // Find and count repeated words (more than 2)
            var wordCounts = CountRepeatedWords(builder.ToString());
            Console.WriteLine("\nRepeated Words and Their Counts:");
            foreach (var word in wordCounts)
            {
                Console.WriteLine($"{word.Key}: {word.Value}");
            }
        }
    }
}
