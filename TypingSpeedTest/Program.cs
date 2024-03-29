namespace TypingSpeedTest
{
    class Program
    {

        public static void Main(string[] args)
        {
            Console.CursorVisible = true;
            Console.Title = "Typing Speed Test";
            while (true)
            {
                string[] text = TextGenerator.GenerateLines(5, 5);
                Console.Clear();
                Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
                Console.WriteLine("Press 1 to start without timer");
                Console.WriteLine("Press 2 to start with timer");
                Console.WriteLine("Press Esc to exit");
                var key = Console.ReadKey(true);
                switch (key.Key)
                {
                    case (ConsoleKey.D1):
                        var textTestWithoutTimer = new TextTestManyLines(text);
                        textTestWithoutTimer.StartWithoutTimer();
                        break;
                    case (ConsoleKey.D2):
                        Console.Clear();
                        Console.SetBufferSize(Console.WindowWidth, Console.WindowHeight);
                        Console.WriteLine("Press 1 to set timer to 15");
                        Console.WriteLine("Press 2 to set timer to 30");
                        Console.WriteLine("Press 3 to set timer to 60");
                        Console.WriteLine("Press 4 to set timer to 120");
                        Console.WriteLine("Press Esc to exit");
                        bool breakFromTimerLoop = false;
                        do
                        {
                            key = Console.ReadKey(true);
                            switch (key.Key)
                            {
                                case (ConsoleKey.D1):
                                    var textTestWithTimer = new TextTestManyLines(text);
                                    textTestWithTimer.StartWithTimer(15);
                                    breakFromTimerLoop = true;
                                    break;
                                case (ConsoleKey.D2):
                                    textTestWithTimer = new TextTestManyLines(text);
                                    textTestWithTimer.StartWithTimer(30);
                                    breakFromTimerLoop = true;
                                    break;
                                case (ConsoleKey.D3):
                                    textTestWithTimer = new TextTestManyLines(text);
                                    textTestWithTimer.StartWithTimer(60);
                                    breakFromTimerLoop = true;
                                    break;
                                case (ConsoleKey.D4):
                                    textTestWithTimer = new TextTestManyLines(text);
                                    textTestWithTimer.StartWithTimer(120);
                                    breakFromTimerLoop = true;
                                    break;
                                case (ConsoleKey.Escape):
                                    goto Next;
                            }
                        } while (!breakFromTimerLoop);
                        break;
                    case (ConsoleKey.Escape):
                        goto Stop;

                }
            Next:
                continue;
            Stop:
                break;
            }
        }
    }
}