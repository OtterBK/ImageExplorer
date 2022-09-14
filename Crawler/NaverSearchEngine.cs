using MySettings;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Crawler {
    public class NaverSearchEngine {

        public string clientID;
        public string clientSecret;

        public NaverSearchEngine() {

            clientID = GlobalSetting.naverApiClientId;
            clientSecret = GlobalSetting.naverApiClientSecret;

            Console.WriteLine(GetImageUrl("하츠네미쿠"));

        }
        public string GetImageUrl(string word, int imageIndex = 1) {

            Console.WriteLine($"searching for {word} , {imageIndex}");

            //잼난 커스텀
            if ((word.Contains("보끔") || word.Contains("재욱") || word.Contains("전재") || word.Contains("아저씨") || word.Contains("틀딱") || word.Contains("화석")) &&
                (word.Contains("여자친구") || word.Contains("애인") || word.Contains("그녀") || word.Contains("아내") || word.Contains("부인") || word.Contains("여친"))
            ) {
                return $"{word} 는 존재하지 않습니다.";
            }

            string query = word; // 검색할 문자열
            string url = "https://openapi.naver.com/v1/search/image?&display=10&start=1&sort=sim&filter=medium&query=" + query; // 결과가 JSON 포맷

            try {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Headers.Add("X-Naver-Client-Id", clientID); // 클라이언트 아이디
                request.Headers.Add("X-Naver-Client-Secret", clientSecret);       // 클라이언트 시크릿

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                string status = response.StatusCode.ToString();
                if (status == "OK") {

                    Stream stream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string text = reader.ReadToEnd();

                    var jsonResponse = JObject.Parse(text);
                    var items = jsonResponse["items"];
                    if (items != null) {
                        var item = items.Count() >= imageIndex ? items[imageIndex - 1] : items[items.Count() - 1];
                        if (item != null) {
                            return item["link"].ToString();
                        }
                    }

                } else {
                    Console.WriteLine("HTTP Error 발생=" + status);
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }


            return null;
        }
    }
}
