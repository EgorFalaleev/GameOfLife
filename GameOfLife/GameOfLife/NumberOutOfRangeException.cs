using System;

namespace GameOfLife
{
    class NumberOutOfRangeException : Exception
    {
        public NumberOutOfRangeException(string message): base(message)
        {

        }
    }
}
