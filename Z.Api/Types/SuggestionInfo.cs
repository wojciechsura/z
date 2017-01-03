using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Z.Api.Types
{
    public sealed class SuggestionInfo
    {
        public SuggestionInfo(string text, string display, string comment, ImageSource image, byte match, object data = null)
        {
            if (match > 100)
                throw new ArgumentOutOfRangeException(nameof(match));

            this.Text = text;
            this.Display = display;
            this.Comment = comment;
            this.Image = image;
            this.Data = data;
            this.Match = match;
        }

        /// <summary>
        /// Suggestion title - will be displayed in suggestion list
        /// </summary>
        /// <remarks>Most of the time, should be similar or equal to Text, to avoid user disorientation</remarks>
        public string Display { get; private set; }
        /// <summary>
        /// Optional, may be null. Suggestion comment, displayed below title
        /// </summary>
        public string Comment { get; private set; }
        /// <summary>
        /// Text displayed in main window, when user chooses the suggestion in the list.
        /// This text replaces what user has entered earlier.
        /// </summary>
        public string Text { get; private set; }
        /// <summary>
        /// Optional, may be null. 16x16 icon to display next to suggestion.
        /// </summary>
        /// <remarks>In most of the cases, may be the same as module icon.</remarks>
        public ImageSource Image { get; private set; }
        /// <summary>
        /// Module can fill this field with any value, which can be later retrieved if user chooses to execute the suggestion.
        /// </summary>
        public object Data { get; private set; }
        /// <summary>
        /// How well suggestion matches entered text, in percents (0..100)
        /// </summary>
        public byte Match { get; private set; }
    }
}
