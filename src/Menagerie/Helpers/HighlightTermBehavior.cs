using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Menagerie {
    public static class HighlightTermBehavior {
        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached(
            "Text",
            typeof(string),
            typeof(HighlightTermBehavior),
            new FrameworkPropertyMetadata("", OnTextChanged));

        public static string GetText(FrameworkElement frameworkElement) => (string)frameworkElement.GetValue(TextProperty);
        public static void SetText(FrameworkElement frameworkElement, string value) => frameworkElement.SetValue(TextProperty, value);


        public static readonly DependencyProperty TermToBeHighlightedProperty = DependencyProperty.RegisterAttached(
            "TermToBeHighlighted",
            typeof(string),
            typeof(HighlightTermBehavior),
            new FrameworkPropertyMetadata("", OnTextChanged));

        public static string GetTermToBeHighlighted(FrameworkElement frameworkElement) {
            return (string)frameworkElement.GetValue(TermToBeHighlightedProperty);
        }

        public static void SetTermToBeHighlighted(FrameworkElement frameworkElement, string value) {
            frameworkElement.SetValue(TermToBeHighlightedProperty, value);
        }


        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (d is TextBlock textBlock)
                SetTextBlockTextAndHighlightTerm(textBlock, GetText(textBlock), GetTermToBeHighlighted(textBlock));
        }

        private static void SetTextBlockTextAndHighlightTerm(TextBlock textBlock, string text, string termToBeHighlighted) {
            textBlock.Text = string.Empty;

            if (TextIsEmpty(text))
                return;

            if (TextIsNotContainingTermToBeHighlighted(text, termToBeHighlighted)) {
                AddPartToTextBlock(textBlock, text);
                return;
            }

            var textParts = SplitTextIntoTermAndNotTermParts(text, termToBeHighlighted);

            foreach (var textPart in textParts)
                AddPartToTextBlockAndHighlightIfNecessary(textBlock, termToBeHighlighted, textPart);
        }

        private static bool TextIsEmpty(string text) {
            return text.Length == 0;
        }

        private static bool TextIsNotContainingTermToBeHighlighted(string text, string termToBeHighlighted) {
            return !text.Contains(termToBeHighlighted);
        }

        private static void AddPartToTextBlockAndHighlightIfNecessary(TextBlock textBlock, string termToBeHighlighted, string textPart) {
            if (textPart == termToBeHighlighted)
                AddHighlightedPartToTextBlock(textBlock, textPart);
            else
                AddPartToTextBlock(textBlock, textPart);
        }

        private static void AddPartToTextBlock(TextBlock textBlock, string part) {
            textBlock.Inlines.Add(new Run { Text = part, FontWeight = FontWeights.Light });
        }

        private static void AddHighlightedPartToTextBlock(TextBlock textBlock, string part) {
            textBlock.Inlines.Add(new Run { Text = part, FontWeight = FontWeights.Bold, Foreground = Brushes.Yellow });
        }


        public static List<string> SplitTextIntoTermAndNotTermParts(string text, string term) {
            if (string.IsNullOrEmpty(text)) {
                return new List<string>() { string.Empty };
            }

            return Regex.Split(text, $@"({Regex.Escape(term)})")
                        .Where(p => p != string.Empty)
                        .ToList();
        }
    }
}
