namespace NotFishyAtAll
{
    class Config
    {
        private readonly string configFileString =
            "in-game min: 100, in-game max: 100,000\n" +
            "initialBet=200\n\n" +

            "in-game min: 100, in-game max: 100,000\n" +
            "maximumBet=100000\n\n" +

            "wins needed until current bet is checked against winnings\n" +
            "minimumWinsUntilProfitFailsafe=8\n\n" +

            "set to 0 to disable. when winnings reach this, immediately exit\n" +
            "cashOutAt=1600\n\n";

        public static Dictionary<string, int> ConfigValues = new Dictionary<string, int>();

        private void CreateConfigFile(string filePath)
        {
            StreamWriter sw = new(filePath);
            sw.WriteLine(this.configFileString);
            sw.Close();
        }

        public Config()
        {
            string configFilePath = AppDomain.CurrentDomain.BaseDirectory + "\\config.txt";

            if (!File.Exists(configFilePath))
            {
                CreateConfigFile(configFilePath);
            }

            StreamReader sr = new(configFilePath);
            string line;
            // Read and display lines from the file until the end of
            // the file is reached.
            while ((line = sr.ReadLine()) != null)
            {
                int indexOfEqualSign = line.IndexOf('=');
                if (indexOfEqualSign != -1)
                {
                    string key = line.Substring(0, indexOfEqualSign);
                    string value = line.Substring(indexOfEqualSign + 1);
                    ConfigValues.Add(key, Convert.ToInt32(value));
                }
            }
            sr.Close();
        }
    }
}
