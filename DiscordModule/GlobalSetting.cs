namespace MySettings {
    public static class GlobalSetting {

        public static string startUpPath = "";
        public static string token = "";
        public static string tokenFileName = "token.txt";
        public static string chromedriverPath = "";
        public static string chromedriverName = "";
        public static string chromedriverPathFile = "chromedriverpath.txt";

        public static void LoadToken() {
            LoadToken(startUpPath + tokenFileName);
        }

        public static void LoadToken(string filePath) {

            using (var tokenFile = File.OpenText(filePath)) {
                if(tokenFile != null) {
                    string tokenString = tokenFile.ReadLine();
                    if(tokenString != string.Empty) {
                        GlobalSetting.token = tokenString;
                    }
                }
            }

        }

        public static void LoadChromeDriverPath() {

            using (var pathFile = File.OpenText(chromedriverPathFile)) {
                if (pathFile != null) {
                    string driverpath = pathFile.ReadLine();
                    if (driverpath != string.Empty) {
                        chromedriverPath = driverpath;
                    }
                    string drivername = pathFile.ReadLine();
                    if (drivername != string.Empty) {
                        chromedriverName = drivername;
                    }
                }
            }

            if (chromedriverPath == null) {
                Console.WriteLine("ChromeDriver 를 찾을 수 없습니다.");
            } else {
                chromedriverPath = chromedriverPath.Trim();
                Console.WriteLine($"ChromeDriver 경로: {chromedriverPath}{chromedriverName}");
            }

        }

    }
}
