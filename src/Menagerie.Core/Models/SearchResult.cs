using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models {
    public class SearchResult {
        public string Id { get; set; }
        public List<string> Result { get; set; }
        public int Total { get; set; }
        public bool Inexact { get; set; }
        public SearchResultError Error { get; set; }
    }

    public class SearchResultError {
        public int Code { get; set; }
        public string Message { get; set; }
    }
}
