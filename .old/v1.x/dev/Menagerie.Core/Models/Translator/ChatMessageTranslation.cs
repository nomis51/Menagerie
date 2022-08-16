using System;

namespace Menagerie.Core.Models.Translator
{
    public class ChatMessageTranslation
    {
        public string OriginalMessage { get; set; }
        public string OriginalLang { get; set; }
        public string TranslatedMessage { get; set; }
        public string TranslationLang { get; set; }
        public string PlayerName { get; set; }
        public string MessageTag { get; set; }
        public DateTime Time { get; set; }
        public bool UserInitiated { get; set; } = false;
    }
}