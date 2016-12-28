using CustomCommandsModule.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommandsModule.Infrastructure
{
    internal static class CommandBuilder
    {
        internal static CommandParams SplitCommand(string expression)
        {
            expression = expression.TrimStart();

            int pos = expression.IndexOf(' ');
            if (pos == -1)
                return new CommandParams { Command = expression, Params = null };
            else
                return new CommandParams { Command = expression.Substring(0, pos), Params = pos + 1 > expression.Length ? string.Empty : expression.Substring(pos + 1) };
        }

        internal static List<string> BuildParameters(string parameters)
        {
            List<string> result = new List<string>();
            ParameterParser parser = new ParameterParser();

            int i = 0;
            while (i < parameters.Length)
            {
                ParameterToken token = parser.Process(parameters, ref i);

                if (token.TokenType == ParameterTokenType.QuotedParameter)
                {
                    StringBuilder builder = new StringBuilder();
                    string baseParam = token.TokenStr.Substring(1, token.TokenStr.Length - 2);

                    int j = 0;
                    while (j < baseParam.Length)
                    {
                        if (baseParam[j] == '"')
                        {
                            j++;
                            if (j >= baseParam.Length || baseParam[j] != '"')
                                throw new InvalidOperationException("Parser algorithm broken!");

                            builder.Append('"');
                            j++;
                        }
                        else
                        {
                            builder.Append(baseParam[j]);
                            j++;
                        }
                    }

                    result.Add(builder.ToString());
                }
                else if (token.TokenType == ParameterTokenType.RegularParameter)
                {
                    result.Add(token.TokenStr);
                }
                else if (token.TokenType == ParameterTokenType.Unknown)
                    throw new InvalidOperationException("Broken parameters!");
            }

            return result;
        }

        internal static string ApplyParameters(string text, List<string> parameters, string parameterString)
        {
            CommandParser parser = new CommandParser();
            int i = 0;
            StringBuilder builder = new StringBuilder();

            while (i < text.Length)
            {
                CommandToken token = parser.Process(text, ref i);
                switch (token.TokenType)
                {
                    case CommandTokenType.RegularText:
                        {
                            builder.Append(token.TokenStr);
                            break;
                        }
                    case CommandTokenType.OpeningBrace:
                        {
                            builder.Append('{');
                            break;
                        }
                    case CommandTokenType.ClosingBrace:
                        {
                            builder.Append('}');
                            break;
                        }
                    case CommandTokenType.Parameter:
                        {
                            int parameterId = int.Parse(token.TokenStr.Substring(1, token.TokenStr.Length - 2));
                            if (parameterId < parameters.Count)
                                builder.Append(parameters[parameterId]);
                            break;
                        }
                    case CommandTokenType.UrlParameter:
                        {
                            int parameterId = int.Parse(token.TokenStr.Substring(2, token.TokenStr.Length - 3));
                            if (parameterId < parameters.Count)
                                builder.Append(WebUtility.UrlEncode(parameters[parameterId]));
                            break;
                        }
                    case CommandTokenType.All:
                        {
                            builder.Append(parameterString);
                            break;
                        }
                    case CommandTokenType.UrlAll:
                        {
                            builder.Append(WebUtility.UrlEncode(parameterString));
                            break;
                        }
                    case CommandTokenType.Unknown:
                        throw new InvalidOperationException("Invalid command!");
                }
            }

            return builder.ToString();
        }
    }
}
