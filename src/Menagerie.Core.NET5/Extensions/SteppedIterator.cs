using System.Collections.Generic;

namespace Menagerie.Core.Extensions {
    public static class SteppedIterator {
        public static IEnumerable<int> GetIterator(int startIndex, int endIndex, int stepSize) {
            for (var i = startIndex; i < endIndex; i += stepSize) {
                yield return i;
            }
        }
    }
}
