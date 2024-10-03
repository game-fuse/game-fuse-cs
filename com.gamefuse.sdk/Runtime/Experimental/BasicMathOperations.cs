// BasicMathOperations.cs
using System;

namespace GameFuseCSharp
{
    /// <summary>
    /// Provides basic mathematical operations.
    /// </summary>
    public class BasicMathOperations
    {
        /// <summary>
        /// Adds two floating-point numbers.
        /// </summary>
        /// <param name="a">First float.</param>
        /// <param name="b">Second float.</param>
        /// <returns>Sum of a and b.</returns>
        public float Add(float a, float b)
        {
            return a + b;
        }

        /// <summary>
        /// Subtracts the second floating-point number from the first.
        /// </summary>
        /// <param name="a">First float.</param>
        /// <param name="b">Second float.</param>
        /// <returns>Result of a minus b.</returns>
        public float Subtract(float a, float b)
        {
            return a - b;
        }

        /// <summary>
        /// Multiplies two floating-point numbers.
        /// </summary>
        /// <param name="a">First float.</param>
        /// <param name="b">Second float.</param>
        /// <returns>Product of a and b.</returns>
        public float Multiply(float a, float b)
        {
            return a * b;
        }

        /// <summary>
        /// Divides the first floating-point number by the second.
        /// </summary>
        /// <param name="a">Dividend float.</param>
        /// <param name="b">Divisor float.</param>
        /// <returns>Result of a divided by b.</returns>
        /// <exception cref="DivideByZeroException">Thrown when divisor is zero.</exception>
        public float Divide(float a, float b)
        {
            if (b == 0f)
            {
                throw new DivideByZeroException("Cannot divide by zero.");
            }
            return a / b;
        }
    }
}

