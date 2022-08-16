using System;

namespace Menagerie.Core.Exceptions
{
    public class CannotFindLogFileException : Exception
    {
        public CannotFindLogFileException(string processName) : base($"Cannot find log file for {processName}")
        {
        }
    }
}