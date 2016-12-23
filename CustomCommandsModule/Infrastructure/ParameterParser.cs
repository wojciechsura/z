using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommandsModule.Infrastructure
{
    /*
    quotedParameter="([^"]|"")*"
    regularParameter=[^ "]+
    whitespace= +
    */

    public enum ParameterTokenType
    {
        QuotedParameter = 0,
        RegularParameter = 1,
        Whitespace = 2,
        Unknown = 3
    };

    public class ParameterToken : BaseToken<ParameterTokenType>
    {

    }

    public class ParameterParser : BaseParser<ParameterTokenType, ParameterToken>
    {
        private readonly int[] IsAcceptState = new int[] { -1, 1, 2, -1, 0 };

        private readonly int[] TargetState = new int[] { -1, 0, 0, -1, 0 };

        protected override int[] GetIsAcceptState() => IsAcceptState;

        protected override int[] GetTargetState() => TargetState;

        protected override ParameterTokenType GetTokenType(int currentAccept) => (ParameterTokenType)currentAccept;

        protected override ParameterTokenType GetUnknownTokenType() => ParameterTokenType.Unknown;

        protected override int Transition(int state, char ch)
        {
            if (state == 0)
            {
                if (ch >= 0 && ch < 32)
                {
                    return 1;
                }
                else if (ch == 32)
                {
                    return 2;
                }
                else if (ch == 33)
                {
                    return 1;
                }
                else if (ch == 34)
                {
                    return 3;
                }
                else if (ch >= 35)
                {
                    return 1;
                }
            }
            else if (state == 1)
            {
                if (ch >= 0 && ch < 32)
                {
                    return 1;
                }
                else if (ch == 33)
                {
                    return 1;
                }
                else if (ch >= 35)
                {
                    return 1;
                }
            }
            else if (state == 2)
            {
                if (ch == 32)
                {
                    return 2;
                }
            }
            else if (state == 3)
            {
                if (ch >= 0 && ch < 34)
                {
                    return 3;
                }
                else if (ch == 34)
                {
                    return 4;
                }
                else if (ch >= 35)
                {
                    return 3;
                }
            }
            else if (state == 4)
            {
                if (ch == 34)
                {
                    return 3;
                }
            }

            return -1;
        }
    }
}
