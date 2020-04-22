using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MergeSample.Tests
{
    [TestClass()]
    public class CombinationTests
    {
        [TestMethod()]
        [DataRow(false, 5, 2, 10)]
        [DataRow(true, 5, 2, 25)]
        public void EnumerateTest(bool withRepetition, int n, int k, int count)
        {
            var nums = Enumerable.Range(1, n).ToArray();
            var combinations = Combination.Enumerate(nums, k, withRepetition);

            int i = 0;
            foreach (var elem in combinations)
            {
                Console.WriteLine($"{++i:00} : ({string.Join(",", elem.Select(x => x.ToString()).ToArray())})");
            }
            i.Is(count);
        }
    }
}