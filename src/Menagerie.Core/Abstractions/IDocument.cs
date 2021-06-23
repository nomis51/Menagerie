using LiteDB;

namespace Menagerie.Core.Abstractions
{
    public interface IDocument
    {
        public ObjectId Id  { get; set; }
    }
}