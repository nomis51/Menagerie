using Toucan.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Toucan {
    public sealed class BoolToVisibilityConverter : BoolConverter<Visibility> {
        public BoolToVisibilityConverter() : base(Visibility.Visible, Visibility.Hidden) { }
    }
}
