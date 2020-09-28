using System;

namespace GameOfLife
{
    /// <summary>
    /// Исключение, которое возникает при вводе числа вне границ допустимого диапазона
    /// </summary>
    class NumberOutOfRangeException : Exception
    {
        public NumberOutOfRangeException(string message): base(message)
        {

        }
    }
}
