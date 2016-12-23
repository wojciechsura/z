using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommandsModule.Infrastructure
{
    public enum CommandTokenType
    {
        ClosingBrace = 0,
        OpeningBrace = 1,
        Parameter = 2,
        RegularText = 3,
        UrlParameter = 4,
        Unknown = 5
    };

    public class CommandToken : BaseToken<CommandTokenType>
    {
    }

    public class CommandParser : BaseParser<CommandTokenType, CommandToken>
    {
        private readonly int[] IsAcceptState = new int[] { -1, 3, -1, -1, -1, -1, 1, 0, 2, -1, 4 };

        private readonly int[] TargetState = new int[] { -1, 0, -1, -1, -1, -1, 0, 0, 0, -1, 0 };

        protected override int[] GetIsAcceptState() => IsAcceptState;

        protected override int[] GetTargetState() => TargetState;

        protected override CommandTokenType GetTokenType(int currentAccept) => (CommandTokenType)currentAccept;

        protected override CommandTokenType GetUnknownTokenType() => CommandTokenType.Unknown;

        protected override int Transition(int state, char ch)
        {
            if (state == 0)
            {
                if (ch >= 0 && ch < 123)
                {
                    return 1;
                }
                else if (ch == 123)
                {
                    return 2;
                }
                else if (ch == 124)
                {
                    return 1;
                }
                else if (ch == 125)
                {
                    return 3;
                }
                else if (ch >= 126)
                {
                    return 1;
                }
            }
            else if (state == 1)
            {
                if (ch >= 0 && ch < 123)
                {
                    return 1;
                }
                else if (ch == 124)
                {
                    return 1;
                }
                else if (ch >= 126)
                {
                    return 1;
                }
            }
            else if (state == 2)
            {
                if (ch >= 48 && ch < 58)
                {
                    return 4;
                }
                else if (ch == 85)
                {
                    return 5;
                }
                else if (ch == 117)
                {
                    return 5;
                }
                else if (ch == 123)
                {
                    return 6;
                }
            }
            else if (state == 3)
            {
                if (ch == 125)
                {
                    return 7;
                }
            }
            else if (state == 4)
            {
                if (ch >= 48 && ch < 58)
                {
                    return 4;
                }
                else if (ch == 125)
                {
                    return 8;
                }
            }
            else if (state == 5)
            {
                if (ch >= 48 && ch < 58)
                {
                    return 9;
                }
            }
            else if (state == 9)
            {
                if (ch >= 48 && ch < 58)
                {
                    return 9;
                }
                else if (ch == 125)
                {
                    return 10;
                }
            }

            return -1;
        }
    }
}
