using MySettings;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Crawler {
    public class ChromeImageExplorer {

        public static IWebDriver chromeDriver_;

        public ChromeImageExplorer() {
            chromeDriver_ = InitializeDriver(); //동적 실행으로 변경
        }

        //크롬 드라이버 초기화
        public IWebDriver InitializeDriver() {
            //드라이버 옵션
            string driverpath = GlobalSetting.chromedriverPath;
            string drivername = GlobalSetting.chromedriverName;
            ChromeDriverService service = ChromeDriverService.CreateDefaultService(driverpath, drivername);
            service.HideCommandPromptWindow = true;

            ChromeOptions options = new ChromeOptions();
            options.AddArguments(new List<string>() {
                "--silent-launch",
                "--blink-settings=imagesEnabled=true", //이미지 로드 함
                "--no-startup-window",
                "no-sandbox", //gpu 없는 환경
                "disable-gpu", //gpu 없는 환경
                "headless",}); //headless 모드

            IWebDriver driver = new ChromeDriver(service, options);

            // find 할때 찾을때까지 기다리는 seconds 설정
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);
            driver.Manage().Window.FullScreen(); //풀스크린

            return driver;
        }

        //이미지 크롤링 부분
        public string GetImageUrl(string word, int imageIndex = 1) {

            Console.WriteLine($"searching for {word}");

            string searchUrl = $"https://www.google.co.kr/search?q={word}&hl=ko&tbm=isch&source=hp";

            Console.WriteLine($"searching {searchUrl}");

            //잼난 커스텀
            if((word.Contains("보끔") || word.Contains("재욱") || word.Contains("전재") || word.Contains("아저씨") || word.Contains("틀딱") || word.Contains("화석")) && 
                (word.Contains("여자친구") || word.Contains("애인")|| word.Contains("그녀") || word.Contains("아내") || word.Contains("부인") || word.Contains("여친"))
            ){
                return $"{word} 는 존재하지 않습니다.";
            }

            try {

                chromeDriver_.Url = searchUrl;

                var imageSetction = chromeDriver_.FindElement(By.CssSelector("#islrg"));
                var imageCards = imageSetction.FindElements(By.CssSelector("div[data-ved]"));

                var targetImageCard = imageCards[0];
                if (imageIndex > 1) {
                    targetImageCard = imageCards[imageIndex - 1];
                }
                targetImageCard.Click();

                var sideSection = chromeDriver_.FindElement(By.CssSelector("#Sva75c"));
                var sideImageSection = sideSection.FindElement(By.CssSelector(".v4dQwb img[src^='http'"));
                var image_url = sideImageSection.GetAttribute("src");

                Console.WriteLine($"found url for {word}, {imageIndex}: {image_url}");

                return image_url;
            } catch (Exception ex) {
                Console.WriteLine($"failed to searching {word} , {imageIndex}");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
            return null;
        }

    }
}
