using System.Text.RegularExpressions;
using Menagerie.Core.Services.Abstractions;

namespace Menagerie.Core.Services;

public class ClientFileService : IClientFileService
{
    #region Members

    private string _logFilePath = string.Empty;
    private long _endOfFilePosition;

    #endregion

    #region Public methods

    public void Initialize()
    {
    }

    public void SetClientFilePath(string filePath)
    {
        _logFilePath = filePath;
        SetEndOfFile();
        WatchFile();
    }

    #endregion

    #region Private methods

    private void WatchFile()
    {
        var thread = new Thread(() =>
        {
            while (true)
            {
                var newLines = new List<string>();

                do
                {
                    try
                    {
                        Thread.Sleep(500);

                        newLines = ReadNewLines();
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                } while (!newLines.Any());

                foreach (var line in newLines)
                {
                    AppService.Instance.NewClientFileLine(line);
                }
            }
        })
        {
            IsBackground = true
        };
        thread.Start();
    }

    private void SetEndOfFile()
    {
        var file = File.Open(_logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        _endOfFilePosition = file.Length - 1;
        file.Close();
    }

    private List<string> ReadNewLines()
    {
        var lines = new List<string>();

        var currentPosition = _endOfFilePosition;

        SetEndOfFile();

        if (currentPosition >= _endOfFilePosition)
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

    private string RemoveSpecialChars(string str)
    {
        return string.IsNullOrEmpty(str) ? str : Regex.Replace(str, "[\\r\\n]", "");
    }

    #endregion
}