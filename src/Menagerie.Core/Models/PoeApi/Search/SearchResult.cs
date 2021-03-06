﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Menagerie.Core.Models
{
    public abstract class SearchResult
    {
        public string Id { get; set; }
        public List<string> Result { get; set; }
        public int Total { get; set; }
        public bool Inexact { get; set; }
        public SearchResultError Error { get; set; }
        public string League { get; set; }
    }
}