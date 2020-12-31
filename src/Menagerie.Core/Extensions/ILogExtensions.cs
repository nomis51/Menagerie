using log4net;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Menagerie.Core.Extensions {
    public static class ILogExtentions {
        public static void Trace(this ILog log, string message, Exception exception) {
                log.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                log4net.Core.Level.Trace, $"{message}", exception);
        }

        public static void Trace(this ILog log, string message) {
            log.Trace(message, null);
        }

        public static void Verbose(this ILog log, string message, Exception exception) {
            log.Logger.Log(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType,
                log4net.Core.Level.Verbose, $"{message}", exception);
        }

        public static void Verbose(this ILog log, string message) {
            log.Verbose(message, null);
        }

    }
}
