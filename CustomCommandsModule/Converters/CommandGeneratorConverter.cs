using CustomCommandsModule.Infrastructure;
using CustomCommandsModule.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CustomCommandsModule.Converters
{
    class CommandGeneratorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is string && values[1] is string)
            {
                try
                {
                    List<string> parameters = CommandBuilder.BuildParameters((string)values[0]);
                    return CommandBuilder.ApplyParameters((string)values[1], parameters, (string)values[0]);
                }
                catch
                {
                    return Strings.CustomCommands_Message_InvalidCommandSyntax;
                }
            }
            else
                return Binding.DoNothing;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
