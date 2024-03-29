using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TypingSpeedTest
{
    public static class TextGenerator
    {
        private static string[] _arrLines = File.ReadLines(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"words_alpha.txt")).ToArray();
        public static string[] GenerateLines(int lines, int wordsInLine)
        {
            string[] text = new string[lines];
            Random rn = new();
            for (int i = 0; i < text.Length; i++)
            {
                string line = "";
                for (int x = 0; x < wordsInLine; x++)
                {
                    line += _arrLines[rn.Next(_arrLines.Length)];
                    if (x != wordsInLine - 1)
                        line += " ";
                }
                text[i] = line;
            }
            return text;
        }
    }
}
