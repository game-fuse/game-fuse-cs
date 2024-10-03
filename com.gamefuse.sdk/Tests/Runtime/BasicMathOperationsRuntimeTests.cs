using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using GameFuseCSharp;

namespace GameFuseCSharp.Tests.Runtime
{
    public class BasicMathOperationsRuntimeTests
    {
        private BasicMathOperations _mathOps;

        [SetUp]
        public void Setup()
        {
            _mathOps = new BasicMathOperations();
        }

        [UnityTest]
        public IEnumerator Add_TwoPositiveNumbers_ReturnsCorrectSum()
        {
            // Arrange
            float a = 5.5f;
            float b = 4.5f;
            float expected = 10.0f;

            // Act
            float result = _mathOps.Add(a, b);

            // Assert
            Assert.AreEqual(expected, result, "Add method should correctly add two positive numbers.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator Subtract_TwoNumbers_ReturnsCorrectDifference()
        {
            // Arrange
            float a = 10.0f;
            float b = 4.5f;
            float expected = 5.5f;

            // Act
            float result = _mathOps.Subtract(a, b);

            // Assert
            Assert.AreEqual(expected, result, "Subtract method should correctly subtract the second number from the first.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator Multiply_TwoNumbers_ReturnsCorrectProduct()
        {
            // Arrange
            float a = 3.0f;
            float b = 4.0f;
            float expected = 12.0f;

            // Act
            float result = _mathOps.Multiply(a, b);

            // Assert
            Assert.AreEqual(expected, result, "Multiply method should correctly multiply two numbers.");

            yield return null;
        }

        [UnityTest]
        public IEnumerator Divide_TwoNumbers_ReturnsCorrectQuotient()
        {
            // Arrange
            float a = 10.0f;
            float b = 2.0f;
            float expected = 5.0f;

            // Act
            float result = _mathOps.Divide(a, b);

            // Assert
            Assert.AreEqual(expected, result, "Divide method should correctly divide the first number by the second.");

            yield return null;
        }
    }
}
