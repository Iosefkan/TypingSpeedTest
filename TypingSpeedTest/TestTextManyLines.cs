using System.Diagnostics;

namespace TypingSpeedTest
{
    public class TextTestManyLines
    {
        private object _locker = new object();
        private int[] _startHight;
        private static int _printHight;
        private int[] _indexText;
        private int[] _indexConsoleWidth;
        private double _secondsPassed = 0;
        private static int _allTextLength;
        private Stack<bool> _charWrong = new Stack<bool>();
        public int AllErrors { get; private set; } = 0;
        private int uncorrectedErrors;
        public int AllEntries { get; private set; } = 0;
        private string[] Text { get; init; }
        public TextTestManyLines(string[] text)
        {
            Text = text;
            _indexText = new int[text.Length];
            for (int i = 0; i < _indexText.Length; i++)
            {
                _indexText[i] = 0;
            }
            _startHight = new int[text.Length];
            for (int i = 0; i < _startHight.Length; i++)
            {
                _startHight[i] = Console.CursorTop + i;
            }
            _printHight = Console.CursorTop + _startHight.Length;
            _indexConsoleWidth = new int[text.Length];
            for (int i = 0; i < _indexConsoleWidth.Length; i++)
            {
                _indexConsoleWidth[i] = (Console.WindowWidth - Text[i].Length) / 2;
            }
            foreach (string line in Text)
            {
                _allTextLength += line.Length;
            }
        }
        private void PrintText()
        {
            _charWrong.Push(false);
            for (int i = 0; i < Text.Length; i++)
            {
                Console.SetCursorPosition(_indexConsoleWidth[i], _startHight[i]);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(Text[i]);
                Console.SetCursorPosition(_indexConsoleWidth[i], _startHight[i]);
            }
        }
        private void PressCharNow(bool changePrevCharColor, int textIndex)
        {
            lock (_locker)
            {
                if (changePrevCharColor)
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.Write(Text[textIndex][_indexText[textIndex]]);
                }
                _indexText[textIndex]++;
                _indexConsoleWidth[textIndex]++;
                AllEntries++;
                if (_indexText[textIndex] != Text[textIndex].Length)
                {
                    Console.SetCursorPosition(_indexConsoleWidth[textIndex], _startHight[textIndex]);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(Text[textIndex][_indexText[textIndex]]);
                    Console.SetCursorPosition(_indexConsoleWidth[textIndex], _startHight[textIndex]);
                }
            }
        }
        private void PressCharWrong(int textIndex)
        {
            lock (_locker)
            {
                _charWrong.Push(true);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(Text[textIndex][_indexText[textIndex]]);
                _indexText[textIndex]++;
                _indexConsoleWidth[textIndex]++;
                AllErrors++;
                AllEntries++;
                uncorrectedErrors++;
                if (_indexText[textIndex] != Text[textIndex].Length)
                {
                    Console.SetCursorPosition(_indexConsoleWidth[textIndex], _startHight[textIndex]);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(Text[textIndex][_indexText[textIndex]]);
                    Console.SetCursorPosition(_indexConsoleWidth[textIndex], _startHight[textIndex]);
                }
            }
        }
        private void PressBackspace(int textIndex)
        {
            lock (_locker)
            {
                Console.SetCursorPosition(_indexConsoleWidth[textIndex], _startHight[textIndex]);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(Text[textIndex][_indexText[textIndex]]);
                _indexText[textIndex]--;
                _indexConsoleWidth[textIndex]--;
                _indexText[textIndex]--;
                _indexConsoleWidth[textIndex]--;
                Console.SetCursorPosition(_indexConsoleWidth[textIndex], _startHight[textIndex]);
            }
            PressCharNow(false, textIndex);
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
            for (int i = 0; i < Text.Length; i++)
            {
                lock (_locker)
                {
                    Console.SetCursorPosition(_indexConsoleWidth[i], _startHight[i]);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write(Text[i][_indexText[i]]);
                    Console.SetCursorPosition(_indexConsoleWidth[i], _startHight[i]);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                while (_indexText[i] != Text[i].Length)
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
                                PressBackspace(i);
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
                                Console.SetCursorPosition(Console.WindowLeft, _printHight + 4);
                                return;
                            case (ConsoleKey.Enter, _):
                                break;
                            case (ConsoleKey.Tab, _):
                                break;
                            case (_, _):
                                char input = key.KeyChar;
                                if (input == Text[i][_indexText[i]])
                                {
                                    if (_charWrong.Peek())
                                        _charWrong.Push(false);
                                    PressCharNow(true, i);
                                }
                                else
                                {
                                    PressCharWrong(i);
                                }
                                ind = false;
                                break;
                        }
                    } while (ind);
                    lock (_locker)
                    {
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
                _charWrong.Clear();
                _charWrong.Push(false);
                if (i != Text.Length - 1)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        AllEntries++;
                    }
                    else
                    {
                        AllEntries++;
                        AllErrors++;
                    }
                }
            }
            sw.Stop();
            timer.Stop();
            timer.Dispose();
            timerPassedSeconds.Stop();
            timerPassedSeconds.Dispose();
            _secondsPassed = (double)sw.ElapsedMilliseconds / (double)1000;
            PrintAccuracyWpmCpm();
            Console.SetCursorPosition(Console.WindowLeft, _printHight + 4);
            Console.WriteLine("Press esc to continue");
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape)
                    break;
            }
            return;
        }
        private void PrintAccuracyWpmCpm()
        {
            int curLeft = Console.CursorLeft;
            int curTop = Console.CursorTop;
            string accuracy = "             Accuracy percent: " + Math.Round(Accuracy(), 2);
            string wpm = "             WPM: " + Math.Round(NetWPM());
            string cpm = "             CPM: " + Math.Round(CPM());
            lock (_locker)
            {
                Console.ForegroundColor = ConsoleColor.White;
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
            double netWPM = (double)(((double)AllEntries / 5) - (double)uncorrectedErrors) / (_secondsPassed / 60.0);
            if (double.IsNaN(netWPM)) netWPM = 0;
            if (double.IsInfinity(netWPM)) netWPM = 0;
            if (netWPM >= 1000) netWPM = 1000;
            return netWPM;
        }
        private double Accuracy()
        {
            double accuracy = (1 - (double)AllErrors / (double)_allTextLength) * 100;
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
                    for (int i = 0; i < Text.Length; i++)
                    {
                        lock (_locker)
                        {
                            Console.SetCursorPosition(_indexConsoleWidth[i], _startHight[i]);
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.Write(Text[i][_indexText[i]]);
                            Console.SetCursorPosition(_indexConsoleWidth[i], _startHight[i]);
                            Console.ForegroundColor = ConsoleColor.White;
                        }
                        while (_indexText[i] != Text[i].Length)
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
                                        PressBackspace(i);
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
                                        Console.SetCursorPosition(Console.WindowLeft, _printHight + 4);
                                        return;
                                    case (ConsoleKey.Enter, _):
                                        break;
                                    case (ConsoleKey.Tab, _):
                                        break;
                                    case (_, _):
                                        char input = key.KeyChar;
                                        if (input == Text[i][_indexText[i]])
                                        {
                                            if (_charWrong.Peek())
                                                _charWrong.Push(false);
                                            PressCharNow(true, i);
                                        }
                                        else
                                        {
                                            PressCharWrong(i);
                                        }
                                        ind = false;
                                        break;
                                }
                            } while (ind);
                            lock (_locker)
                            {
                                Console.ForegroundColor = ConsoleColor.White;
                            }
                        }
                        _charWrong.Clear();
                        _charWrong.Push(false);
                        if (i != Text.Length - 1)
                        {
                            var key = Console.ReadKey(true);
                            if (key.Key == ConsoleKey.Enter)
                            {
                                AllEntries++;
                            }
                            else
                            {
                                AllEntries++;
                                AllErrors++;
                            }
                        }
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
                Console.SetCursorPosition(Console.WindowLeft, _printHight + 4);
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
                    sw.Stop();
                    timer.Stop();
                    timer.Dispose();
                    timerPassedSeconds.Stop();
                    timerPassedSeconds.Dispose();
                    timerStop.Stop();
                    timer.Dispose();
                    Console.SetCursorPosition(Console.WindowLeft, _printHight + 4);
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
                    Console.WriteLine("Press esc to continue");
                    while (true)
                    {
                        var key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Escape)
                            break;
                    }
                    break;
                }
                if (test.IsAlive == false)
                {
                    Console.WriteLine("Press esc to continue");
                    while (true)
                    {
                        var key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.Escape)
                            break;
                    }
                    break;
                }
            }
            test.Join();
            return;
        }
    }
}
