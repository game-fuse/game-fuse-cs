// BasicMathOperationsTests.cs
using NUnit.Framework;
using GameFuseCSharp;
using System;

namespace GameFuseCSharp.Tests
{
    /// <summary>
    /// Contains unit tests for the BasicMathOperations class.
    /// </summary>
    [TestFixture]
    public class BasicMathOperationsTests
    {
        private BasicMathOperations _mathOps;

        /// <summary>
        /// Set up before each test.
        /// </summary>
        [SetUp]
        public void Setup()
        {
            _mathOps = new BasicMathOperations();
        }

        /// <summary>
        /// Clean up after each test.
        /// </summary>
        [TearDown]
        public void Teardown()
        {
            _mathOps = null;
        }

        [Test]
        public void Add_TwoPositiveNumbers_ReturnsCorrectSum()
        {
            // Arrange
            float a = 5.5f;
            float b = 4.5f;
            float expected = 10.0f;

            // Act
            float result = _mathOps.Add(a, b);

            // Assert
            Assert.AreEqual(expected, result, "Add method should correctly add two positive numbers.");
        }

        [Test]
        public void Subtract_TwoNumbers_ReturnsCorrectDifference()
        {
            // Arrange
            float a = 10.0f;
            float b = 4.5f;
            float expected = 5.5f;

            // Act
            float result = _mathOps.Subtract(a, b);

            // Assert
            Assert.AreEqual(expected, result, "Subtract method should correctly subtract the second number from the first.");
        }

        [Test]
        public void Multiply_TwoNumbers_ReturnsCorrectProduct()
        {
            // Arrange
            float a = 3.0f;
            float b = 4.0f;
            float expected = 12.0f;

            // Act
            float result = _mathOps.Multiply(a, b);

            // Assert
            Assert.AreEqual(expected, result, "Multiply method should correctly multiply two numbers.");
        }

        [Test]
        public void Divide_TwoNumbers_ReturnsCorrectQuotient()
        {
            // Arrange
            float a = 10.0f;
            float b = 2.0f;
            float expected = 5.0f;

            // Act
            float result = _mathOps.Divide(a, b);

            // Assert
            Assert.AreEqual(expected, result, "Divide method should correctly divide the first number by the second.");
        }

        [Test]
        public void Divide_DivisorIsZero_ThrowsDivideByZeroException()
        {
            // Arrange
            float a = 10.0f;
            float b = 0.0f;

            // Act & Assert
            var ex = Assert.Throws<DivideByZeroException>(() => _mathOps.Divide(a, b));
            Assert.That(ex.Message, Is.EqualTo("Cannot divide by zero."));
        }

        [Test]
        public void Add_NegativeNumbers_ReturnsCorrectSum()
        {
            // Arrange
            float a = -5.0f;
            float b = -3.0f;
            float expected = -8.0f;

            // Act
            float result = _mathOps.Add(a, b);

            // Assert
            Assert.AreEqual(expected, result, "Add method should correctly add two negative numbers.");
        }

        [Test]
        public void Subtract_ResultIsNegative_ReturnsCorrectDifference()
        {
            // Arrange
            float a = 3.0f;
            float b = 5.0f;
            float expected = -2.0f;

            // Act
            float result = _mathOps.Subtract(a, b);

            // Assert
            Assert.AreEqual(expected, result, "Subtract method should correctly handle cases where the result is negative.");
        }

        [Test]
        public void Multiply_ByZero_ReturnsZero()
        {
            // Arrange
            float a = 0.0f;
            float b = 5.0f;
            float expected = 0.0f;

            // Act
            float result = _mathOps.Multiply(a, b);

            // Assert
            Assert.AreEqual(expected, result, "Multiply method should return zero when one of the operands is zero.");
        }

        [Test]
        public void Divide_NegativeNumbers_ReturnsCorrectQuotient()
        {
            // Arrange
            float a = -10.0f;
            float b = -2.0f;
            float expected = 5.0f;

            // Act
            float result = _mathOps.Divide(a, b);

            // Assert
            Assert.AreEqual(expected, result, "Divide method should correctly handle negative numbers.");
        }
    }
}

