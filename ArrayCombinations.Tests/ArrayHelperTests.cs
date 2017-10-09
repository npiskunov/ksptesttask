using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArrayCombinations.Tests
{
    [TestFixture]
    public class ArrayHelperTests
    {
        [Test]
        [TestCase(new[] { int.MaxValue, 0, 1, int.MaxValue - 1, int.MaxValue }, int.MaxValue, ExpectedResult = 2)]
        [TestCase(new[] { 1, 2, 1, 1, 0 }, 2, ExpectedResult = 2)]
        [TestCase(new[] { 1, 2, 3, 4, 5, 6, 7, 9, 10 }, 6, ExpectedResult = 2)]
        [TestCase(new[] { 1 }, 6, ExpectedResult = 0)]
        [TestCase(new[] { 0,0,0,0,0,0,0,0,0,0,0 }, 0, ExpectedResult = 5)]
        [TestCase(new[] { 1, 1 }, 2, ExpectedResult = 1)]
        [TestCase(new[] { -1, 1, -2, 2, -3, 3, int.MaxValue*-1, int.MaxValue}, 0, ExpectedResult = 4)]
        public int ShouldSort_Success(int[] array, long threshold)
        {
            var sorted = ArrayHelper.ExtractPairs(array, threshold);
            Assert.That(sorted.Count(), Is.LessThanOrEqualTo(array.Length / 2));
            return sorted.Count();
        }

        [Test]
        public void ShouldThrow_InvalidInput()
        {
            Assert.Throws<ArgumentNullException>(() => ArrayHelper.ExtractPairs(null, 0));
        }
    }
}
