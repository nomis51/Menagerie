using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Menagerie.Core.Exceptions;
using Menagerie.Core.Services.Parsing.Abstractions;

namespace Menagerie.Core.Services.Parsing
{
    public sealed class LogReaderService : ILogReaderService
    {
        #region Events

        public event ILogReaderService.NewLogEntryEvent NewLogEntry;

        #endregion

        #region Constants

        private readonly string[] _poeProcesses = new[]
        {
            "PathOfExile_x64",
            "PathOfExile_x64Steam"
        };

        #endregion

        #region Members

        private string _logFilePath;
        private long _endOfFile = 0;

        #endregion

        #region Constructors

        public LogReaderService(string logFilePath)
        {
            _logFilePath = logFilePath;
            SetEndOfFile();
            Watch();
        }

        public LogReaderService()
        {
            FindLogFilePath();
            SetEndOfFile();
            Watch();
        }

        #endregion

        private void FindLogFilePath()
        {
            var processes = Process.GetProcesses();

            foreach (var process in processes)
            {
                if (!_poeProcesses.Contains(process.ProcessName) || process.HasExited) continue;
                try
                {
                    if (process.MainModule?.FileName != null)
                        _logFilePath =
                            $"{(process.MainModule?.FileName)[..process.MainModule.FileName.LastIndexOf("\\", StringComparison.Ordinal)]}\\logs\\Client.txt";
                } catch (Exception)
                {
                    throw new CannotFindLogFileException(process.ProcessName);
                }
            }
        }

        private Task Watch()
        {
            return Task.Run(async () =>
            {
                while (true)
                {
                    var newLines = new List<string>();

                    do
                    {
                        try
                        {
                            await Task.Delay(500);

                            newLines = this.ReadNewLines();
                        } catch
                        {
                            // ignored
                        }
                    } while (!newLines.Any());

                    foreach (var line in newLines)
                    {
                        OnNewLogEntry(line);
                    }
                }
            });
        }

        private void SetEndOfFile()
        {
            var file = File.Open(_logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            _endOfFile = file.Length - 1;
            file.Close();
        }

        private List<string> ReadNewLines()
        {
            var lines = new List<string>();

            var currentPosition = _endOfFile;

            SetEndOfFile();

            if (currentPosition >= _endOfFile)
            {
                return lines;
            }

            var file = File.Open(_logFilePath, FileMode.Open, FileAccess.Read,
                FileShare.ReadWrite);
            file.Position = currentPosition;
            var reader = new StreamReader(file);

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                if (!string.IsNullOrEmpty(line))
                {
                    lines.Add(RemoveSpecialChars(line));
                }
            }

            reader.Close();
            file.Close();

            return lines;
        }

        private string GetNthLineFromEndOfFile(int lineNo)
        {
            if (string.IsNullOrEmpty(_logFilePath)) return null;

            if (_endOfFile < 1)
            {
                SetEndOfFile();
            }

            var file = File.Open(_logFilePath, FileMode.Open, FileAccess.Read,
                FileShare.ReadWrite);
            var currentPosition = _endOfFile - 1;
            var line = "";

            for (var i = 0; i < lineNo; ++i)
            {
                line = "";

                var currentCharValue = -1;
                var foundAChar = false;
                var isEol = false;

                do
                {
                    if (currentPosition < 1) break;
                    currentPosition -= 1;
                    file.Position = currentPosition;
                    currentCharValue = file.ReadByte();
                    var currentChar = (char) currentCharValue;
                    line += currentChar;
                    isEol = currentChar.Equals('\n');

                    if (!foundAChar && !isEol)
                    {
                        foundAChar = true;
                    }
                } while (currentCharValue != -1 && (!isEol || !foundAChar));
            }

            return new string(RemoveSpecialChars(line).Reverse().ToArray());
        }

        private string RemoveSpecialChars(string str)
        {
            return string.IsNullOrEmpty(str) ? str : Regex.Replace(str, "[\\r\\n]", "");
        }

        public string ReadLastLine()
        {
            return GetNthLineFromEndOfFile(1);
        }

        public string ReadLine(int lineNo)
        {
            return lineNo > 0
                ? RemoveSpecialChars(File.ReadLines(_logFilePath).Skip(lineNo - 1).Take(1).FirstOrDefault())
                : null;
        }

        public IEnumerable<string> ReadLines(int[] linesNo)
        {
            return linesNo.Select(ReadLine);
        }


        private void OnNewLogEntry(string line)
        {
            NewLogEntry?.Invoke(line);
        }
    }
}