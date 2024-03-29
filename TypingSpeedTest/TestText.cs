using System.Diagnostics;

namespace TypingSpeedTest
{
    public class TextTest
    {
        private static int _startHight = Console.CursorTop;
        private static int _printHight = Console.CursorTop + 1;
        private int _indexText = 0;
        private int _indexConsoleWidth = 0;
        private double _secondsPassed = 0;
        private Stack<bool> _charWrong = new Stack<bool>();
        public int AllErrors { get; private set; } = 0;
        private int uncorrectedErrors;
        public int AllEntries { get; private set; } = 0;
        private string Text { get; init; }
        public TextTest(string text)
        {
            this.Text = text;
            this._indexConsoleWidth = (Console.WindowWidth - Text.Length) / 2;
        }
        private void PrintText()
        {
            _charWrong.Push(false);
            Console.SetCursorPosition(_indexConsoleWidth, _startHight);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(Text);
            Console.SetCursorPosition(_indexConsoleWidth, _startHight);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write(Text[_indexText]);
            Console.SetCursorPosition(_indexConsoleWidth, _startHight);
        }
        private void PressCharNow(bool changePrevCharColor)
        {
            object buf = new();
            lock (buf)
            {
                if (changePrevCharColor)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(Text[_indexText]);
                }
                _indexText++;
                _indexConsoleWidth++;
                AllEntries++;
                if (_indexText != Text.Length)
                {
                    Console.SetCursorPosition(_indexConsoleWidth, _startHight);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(Text[_indexText]);
                    Console.SetCursorPosition(_indexConsoleWidth, _startHight);
                }
            }
        }
        private void PressCharWrong()
        {
            object buf = new();
            lock (buf)
            {
                _charWrong.Push(true);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(Text[_indexText]);
                _indexText++;
                _indexConsoleWidth++;
                AllErrors++;
                AllEntries++;
                uncorrectedErrors++;
                if (_indexText != Text.Length)
                {
                    Console.SetCursorPosition(_indexConsoleWidth, _startHight);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(Text[_indexText]);
                    Console.SetCursorPosition(_indexConsoleWidth, _startHight);
                }
            }
        }
        private void PressBackspace()
        {
            object buf = new();
            lock (buf)
            {
            Console.SetCursorPosition(_indexConsoleWidth, _startHight);
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(Text[_indexText]);
            _indexText--;
            _indexConsoleWidth--;
            _indexText--;
            _indexConsoleWidth--;
            Console.SetCursorPosition(_indexConsoleWidth, _startHight);
            }
            PressCharNow(false);
        }
        public void StartWithoutTimer()
        {
            Stopwatch sw = new();
            PrintText();
            System.Timers.Timer timer = new System.Timers.Timer(1000);
            timer.Elapsed += (sender, e) => PrintAccuracyWpmCpm();
            void HandleTimer()
            {
                _secondsPassed = (double)sw.ElapsedMilliseconds / (double)1000;
            }
            System.Timers.Timer timerPassedSeconds = new System.Timers.Timer(300);
            timerPassedSeconds.Elapsed += (sender, e) => HandleTimer();
            PrintAccuracyWpmCpm();
            while (_indexText != Text.Length)
            {
                bool ind = true;
                bool wrong = _charWrong.Peek();
                do
                {
                    var key = Console.ReadKey(true);
                    sw.Start();
                    _secondsPassed = (double)sw.ElapsedMilliseconds / (double)1000;
                    timer.Start();
                    timerPassedSeconds.Start();
                    switch (key.Key, wrong)
                    {
                        case (ConsoleKey.Backspace, true):
                            uncorrectedErrors--;
                            PressBackspace();
                            _charWrong.Pop();
                            ind = false;
                            break;
                        case (ConsoleKey.Backspace, _):
                            break;
                        case (ConsoleKey.Escape, _):
                            sw.Stop();
                            timer.Stop();
                            timer.Dispose();
                            timerPassedSeconds.Stop();
                            timerPassedSeconds.Dispose();
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine();
                            Console.WriteLine();
                            return;
                        case (ConsoleKey.Enter, _):
                            break;
                        case (ConsoleKey.Tab, _):
                            break;
                        case (_, _):
                            char input = key.KeyChar;
                            if (input == Text[_indexText])
                            {
                                _charWrong.Push(false);
                                PressCharNow(true);
                            }
                            else
                            {
                                PressCharWrong();
                            }
                            ind = false;
                            break;
                    }
                } while (ind);
                Console.ForegroundColor = ConsoleColor.White;
            }
            sw.Stop();
            timer.Stop();
            timer.Dispose();
            timerPassedSeconds.Stop();
            timerPassedSeconds.Dispose();
            _secondsPassed = (double)sw.ElapsedMilliseconds / (double)1000;
            PrintAccuracyWpmCpm();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Press anything to continue");
            Console.ReadKey(true);
            return;
        }
        private void PrintAccuracyWpmCpm()
        {
            int curLeft = Console.CursorLeft;
            int curTop = Console.CursorTop;
            string accuracy = "             Accuracy percent: " + Math.Round(Accuracy(), 2);
            string wpm = "             WPM: " + Math.Round(NetWPM());
            string cpm = "             CPM: " + Math.Round(CPM());
            object buf = new();
            lock (buf)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(Console.WindowLeft, _printHight);
                Console.WriteLine();
                Console.SetCursorPosition(Console.WindowWidth - accuracy.Length, _printHight + 1);
                Console.WriteLine(accuracy);
                Console.SetCursorPosition(Console.WindowWidth - wpm.Length, _printHight + 2);
                Console.WriteLine(wpm);
                Console.SetCursorPosition(Console.WindowWidth - cpm.Length, _printHight + 3);
                Console.WriteLine(cpm);
                Console.SetCursorPosition(curLeft, curTop);
            }
        }
        private double NetWPM()
        {
            double netWPM = (double)(((double)AllEntries / 5) - (double)uncorrectedErrors) / (_secondsPassed / (double)60);
            if (double.IsNaN(netWPM)) netWPM = 0;
            if (double.IsInfinity(netWPM)) netWPM = 0;
            if (netWPM >= 1000) netWPM = 1000;
            return netWPM;
        }
        private double Accuracy()
        {
            double accuracy = (1 - (double)AllErrors / (double)Text.Length) * 100;
            if (double.IsNaN(accuracy)) accuracy = 0;
            if (double.IsInfinity(accuracy)) accuracy = 0;
            return accuracy;
        }
        private double CPM()
        {
            double cpm = ((double)(AllEntries) - (double)uncorrectedErrors) / (_secondsPassed / (double)60);
            if (double.IsNaN(cpm)) cpm = 0;
            if (double.IsInfinity(cpm)) cpm = 0;
            if (cpm >= 1000) cpm = 1000;
            return cpm;
        }
        public void StartWithTimer(double seconds)
        {
            bool stop = false;
            bool escPressed = false;
            Stopwatch sw = new();
            System.Timers.Timer timer = new System.Timers.Timer(1000);
            System.Timers.Timer timerPassedSeconds = new System.Timers.Timer(300);
            Thread test = new Thread(() => {
                PrintText();
                timer.Elapsed += (sender, e) => PrintAccuracyWpmCpm();
                void HandleTimer()
                {
                    _secondsPassed = (double)sw.ElapsedMilliseconds / (double)1000;
                }
                timerPassedSeconds.Elapsed += (sender, e) => HandleTimer();
                PrintAccuracyWpmCpm();
                try
                {
                    while (_indexText != Text.Length)
                    {
                        bool ind = true;
                        bool wrong = _charWrong.Peek();
                        do
                        {
                            if (stop)
                                return;
                            var key = Console.ReadKey(true);
                            sw.Start();
                            _secondsPassed = (double)sw.ElapsedMilliseconds / (double)1000;
                            timer.Start();
                            timerPassedSeconds.Start();
                            switch (key.Key, wrong)
                            {
                                case (ConsoleKey.Backspace, true):
                                    uncorrectedErrors--;
                                    PressBackspace();
                                    _charWrong.Pop();
                                    ind = false;
                                    break;
                                case (ConsoleKey.Backspace, _):
                                    break;
                                case (ConsoleKey.Escape, _):
                                    sw.Stop();
                                    timer.Stop();
                                    timer.Dispose();
                                    timerPassedSeconds.Stop();
                                    timerPassedSeconds.Dispose();
                                    Console.WriteLine();
                                    Console.WriteLine();
                                    Console.WriteLine();
                                    Console.WriteLine();
                                    escPressed = true;
                                    return;
                                case (ConsoleKey.Enter, _):
                                    break;
                                case (ConsoleKey.Tab, _):
                                    break;
                                case (_, _):
                                    char input = key.KeyChar;
                                    if (input == Text[_indexText])
                                    {
                                        _charWrong.Push(false);
                                        PressCharNow(true);
                                    }
                                    else
                                    {
                                        PressCharWrong();
                                    }
                                    ind = false;
                                    break;
                            }
                        } while (ind);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                catch
                {
                    return;
                }
                
                sw.Stop();
                timer.Stop();
                timer.Dispose();
                timerPassedSeconds.Stop();
                timerPassedSeconds.Dispose();
                _secondsPassed = (double)sw.ElapsedMilliseconds / (double)1000;
                PrintAccuracyWpmCpm();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
                return;
            });
            test.IsBackground = true;
            System.Timers.Timer timerStop = new System.Timers.Timer(300);
            timer.Elapsed += (sender, e) =>
            {
                try
                {
                    if (_secondsPassed - seconds < 1 && _secondsPassed - seconds > 0) throw new Exception();
                }
                catch
                {
                    PrintAccuracyWpmCpm();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    Console.WriteLine();
                    sw.Stop();
                    timer.Stop();
                    timer.Dispose();
                    timerPassedSeconds.Stop();
                    timerPassedSeconds.Dispose();
                    timerStop.Stop();
                    timer.Dispose();
                    stop = true;
                }
            };
            timerStop.Start();
            test.Start();
            while (true)
            {
                Task.Delay(100);
                if (stop)
                {
                    Console.WriteLine("Press anything to continue");
                    break;
                }
                if (test.IsAlive == false)
                {
                    Console.WriteLine("Press anything to continue");
                    if (!escPressed)
                    {
                        Console.ReadKey(true);
                    }
                    break;
                }  
            }
            test.Join();
            return;
        }
    }
}
