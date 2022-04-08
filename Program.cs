namespace NotFishyAtAll
{
    class Program
    {
        // Config vars
        static int initialBet;
        static int maximumBet;
        static int minimumWinsUntilProfitFailsafe;
        static int cashOutAt;
        // End Config vars

        static DateTime timeStarted = DateTime.Now;

        static Random rand = new Random();

        static Point? fixedMousePoint = null;

        static int currentBet = 100;

        static int moneyGained = 0;
        static int moneyLost = 0;

        static int timesRan = 0;

        static void Main(string[] args)
        {
            // Set config vars
            new Config();
            initialBet = Config.ConfigValues["initialBet"];
            maximumBet = Config.ConfigValues["maximumBet"];
            minimumWinsUntilProfitFailsafe = Config.ConfigValues["minimumWinsUntilProfitFailsafe"];
            cashOutAt = Config.ConfigValues["cashOutAt"];
            // End set config vars

            Console.WriteLine("initialized! 10 second timer starts now");
            Thread.Sleep(10000);

            currentBet = initialBet;
            maximumBet = maximumBet <= 100000 ? maximumBet : 100000;

            while (currentBet <= maximumBet)
            {
                Console.WriteLine("do the thing!");

                // complex failsafe
                if ((moneyGained >= initialBet*minimumWinsUntilProfitFailsafe && currentBet+moneyLost >= moneyGained)
                    || (cashOutAt > 0 && moneyGained-moneyLost >= cashOutAt))
                    break;

                timesRan++;

                // send tab
                SendKeys.SendWait("{TAB}");
                Thread.Sleep(GetRandomSleepTime(1, 100));

                // write currentBet
                SendKeys.SendWait(currentBet.ToString());
                Thread.Sleep(GetRandomSleepTime(1, 100));

                // send enter
                SendKeys.SendWait("{ENTER}");
                Thread.Sleep(GetRandomSleepTime(1000, 4000));
                
                // Set mouse to fixed point if not null
                if (fixedMousePoint != null)
                    Cursor.Position = (Point)fixedMousePoint;
                else
                    fixedMousePoint = Cursor.Position;

                // press left click
                LeftMouseClick(Cursor.Position);
                Thread.Sleep(GetRandomSleepTime(2000, 3000));

                // get color at mouse position
                Color colOfMouseLoc = GetPixelColorAt(Cursor.Position);

                // if color is not red, you LOST
                if (colOfMouseLoc.R < 100)
                {
                    moneyLost += currentBet;
                    currentBet *= 2;
                }
                // if you WON
                else
                {
                    moneyGained += initialBet;
                    moneyLost = 0;
                    currentBet = initialBet;
                }

                Thread.Sleep(GetRandomSleepTime(1, 100));
            }

            DateTime timeStopped = DateTime.Now;

            moneyGained -= moneyLost;

            Console.WriteLine("\nchange in money: " + moneyGained.ToString());

            Console.WriteLine("\nstarted at " + timeStarted.ToString("MM/dd/yyyy h:mm tt"));
            Console.WriteLine("failsafe engaged at " + timeStopped.ToString("MM/dd/yyyy h:mm tt"));

            int secondsBetween = (int)(timeStopped - timeStarted).TotalSeconds;
            Console.WriteLine("\nelapsed time: " + GetTimeString(secondsBetween));

            Console.WriteLine("\ntimes ran: " + timesRan.ToString());

            Console.Read();
        }

        static string GetTimeString(int seconds)
        {
            int minutes = seconds / 60;
            int remainingSeconds = seconds % 60;

            return minutes.ToString() + " minute(s) and " + remainingSeconds.ToString() + " second(s)";
        }

        static int GetRandomSleepTime(int min, int max)
        {
            return rand.Next(min, max);
        }

        static Color GetPixelColorAt(Point position)
        {
            using (var bitmap = new Bitmap(1, 1))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    graphics.CopyFromScreen(position, new Point(0, 0), new Size(1, 1));
                }
                return bitmap.GetPixel(0, 0);
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);
        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;
        //This simulates a left mouse click
        public static void LeftMouseClick(Point position)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, position.X, position.Y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, position.X, position.Y, 0, 0);
        }
    }
}