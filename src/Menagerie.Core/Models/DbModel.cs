using System;
using System.Collections.Generic;
using System.Text;
using LiteDB;
using Menagerie.Core.Abstractions;

namespace Menagerie.Core.Models
{
    public abstract class DbModel : IDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }
    }
}