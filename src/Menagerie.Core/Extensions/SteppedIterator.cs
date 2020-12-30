using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Extensions {
    public static class SteppedIterator {
        public static IEnumerable<int> GetIterator(int startIndex, int endIndex, int stepSize) {
            for (int i = startIndex; i < endIndex; i = i + stepSize) {
                yield return i;
            }
        }
    }
}
