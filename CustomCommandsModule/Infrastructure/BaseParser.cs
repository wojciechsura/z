using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomCommandsModule.Infrastructure
{
    public class BaseToken<T>
    {
        public T TokenType { get; set; }
        public string TokenStr { get; set; }
    }

    public abstract class BaseParser<TokenTypeEnum, TokenClass> where TokenClass : BaseToken<TokenTypeEnum>, new()
    {
        private int startState;

        protected abstract int[] GetIsAcceptState();

        protected abstract int Transition(int state, char ch);

        protected abstract int[] GetTargetState();

        protected abstract TokenTypeEnum GetTokenType(int currentAccept);

        protected abstract TokenTypeEnum GetUnknownTokenType();

        public TokenClass Process(string input, ref int start)
        {
            int CurrentState, CurrentAccept, NextState, NextAccept;
            int StringStart;
            TokenClass Result;
            bool Finish;

            CurrentState = startState;
            CurrentAccept = GetIsAcceptState()[startState];
            StringStart = 0;
            Finish = false;

            Result = null;

            do
            {
                if (start < input.Length)
                {
                    NextState = Transition(CurrentState, input[start]);
                    if (NextState != -1)
                        NextAccept = GetIsAcceptState()[NextState];
                    else
                        NextAccept = -1;
                }
                else
                {
                    NextState = -1;
                    NextAccept = -1;
                }

                if (NextState == -1 || (CurrentAccept != -1 && NextAccept == -1))
                {
                    Finish = true;

                    if (CurrentAccept != -1)
                        startState = GetTargetState()[CurrentState];
                    switch (CurrentAccept)
                    {
                        case -1:
                            {
                                Result = new TokenClass();

                                Result.TokenType = GetUnknownTokenType();
                                if (StringStart == 0)
                                    Result.TokenStr = "";
                                else
                                    Result.TokenStr = input.Substring(StringStart, start - StringStart);
                                break;
                            }
                        default:
                            {
                                Result = new TokenClass();

                                Result.TokenType = GetTokenType(CurrentAccept);
                                if (StringStart == start)
                                    Result.TokenStr = "";
                                else
                                    Result.TokenStr = input.Substring(StringStart, start - StringStart);
                                break;
                            }
                    }
                }
                else
                {
                    CurrentState = NextState;
                    CurrentAccept = NextAccept;
                    start++;
                }
            }
            while (!Finish);

            return Result;
        }

        public BaseParser()
        {
            startState = 0;
        }

        public void Reset()
        {
            startState = 0;
        }
    }
}
