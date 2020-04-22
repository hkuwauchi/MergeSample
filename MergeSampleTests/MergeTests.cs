using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MergeSample.Tests
{
    [TestClass()]
    public class MergeTests
    {
        readonly WorkDay Standard = new WorkDay() { Start = DateTime.Parse("2020/4/10 8:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") };

        readonly Dictionary<string, (WorkDay comparision, (bool success, WorkDay newDay) expected)> TestData = new Dictionary<string, (WorkDay comparision, (bool success, WorkDay newDay) expected)>()
        {
            //完全に重ならない
            //S   :        |------|
            //C1-1:                |------|
            //C1-2:|------|
            { "C1-1", (new WorkDay() { Start = DateTime.Parse("2020/4/10 21:00:00"), End = DateTime.Parse("2020/4/10 22:00:00") },(false,null)) },
            { "C1-2", (new WorkDay() { Start = DateTime.Parse("2020/4/10 06:00:00"), End = DateTime.Parse("2020/4/10 07:00:00") },(false,null)) },
            //何れかが重なるる
            //結合
            //S   :       |------|
            //C2-1:              |------|
            //C2-2:|------|
            { "C2-1", (new WorkDay() { Start = DateTime.Parse("2020/4/10 20:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") },(true, new WorkDay() { Start = DateTime.Parse("2020/4/10 08:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") })) },
            { "C2-2", (new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 08:00:00") },(true, new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") })) },
            //完全に包摂
            //S   :|------|
            //C3-1:|------|
            //C3-2:|----|
            //C3-3:  |----|
            //C3-4: |----|
            { "C3-1", (new WorkDay() { Start = DateTime.Parse("2020/4/10 08:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") }, (true,new WorkDay() { Start = DateTime.Parse("2020/4/10 08:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") })) },
            { "C3-2", (new WorkDay() { Start = DateTime.Parse("2020/4/10 08:00:00"), End = DateTime.Parse("2020/4/10 19:00:00") }, (true,new WorkDay() { Start = DateTime.Parse("2020/4/10 08:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") })) },
            { "C3-3", (new WorkDay() { Start = DateTime.Parse("2020/4/10 09:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") }, (true,new WorkDay() { Start = DateTime.Parse("2020/4/10 08:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") })) },
            { "C3-4", (new WorkDay() { Start = DateTime.Parse("2020/4/10 09:00:00"), End = DateTime.Parse("2020/4/10 19:00:00") }, (true,new WorkDay() { Start = DateTime.Parse("2020/4/10 08:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") })) },
            //両方の拡張（入れ替え）
            //S   : |------|
            //C4-1:|--------|
            { "C4-1", (new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") }, (true,new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") })) },
            //開始の拡張
            //S   : |------|
            //C5-1:|------|
            //C5-2:|-------|
            //C5-3:|--------|
            { "C5-1", (new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 19:00:00") }, (true,new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") })) },
            { "C5-2", (new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") }, (true,new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") })) },
            { "C5-3", (new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") }, (true,new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") })) },
            //終了の拡張
            //S   : |------|
            //C6-1:  |------|
            //C6-2: |-------|
            //C6-3:|--------|
            { "C6-1", (new WorkDay() { Start = DateTime.Parse("2020/4/10 09:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") }, (true,new WorkDay() { Start = DateTime.Parse("2020/4/10 08:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") })) },
            { "C6-2", (new WorkDay() { Start = DateTime.Parse("2020/4/10 08:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") }, (true,new WorkDay() { Start = DateTime.Parse("2020/4/10 08:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") })) },
            { "C6-3", (new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") }, (true,new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") })) },
        };

        [TestMethod()]
        [DataRow("C1-1")]
        [DataRow("C1-2")]
        [DataRow("C2-1")]
        [DataRow("C2-2")]
        [DataRow("C3-1")]
        [DataRow("C3-2")]
        [DataRow("C3-3")]
        [DataRow("C3-4")]
        [DataRow("C4-1")]
        [DataRow("C5-1")]
        [DataRow("C5-2")]
        [DataRow("C5-3")]
        [DataRow("C6-1")]
        [DataRow("C6-2")]
        [DataRow("C6-3")]
        public void DoTest(string key)
        {
            var result = Merge.Do(Standard, TestData[key].comparision);
            if (result.success)
            {
                Console.WriteLine((result.newDay.Start, result.newDay.End));
            }
            result.Is(TestData[key].expected);
        }

        readonly Dictionary<string, ((bool success, WorkDay newDay, WorkDay outDay) expected, HashSet<WorkDay> list)> Combination = new Dictionary<string, ((bool success, WorkDay newDay, WorkDay outDay) expected, HashSet<WorkDay> list)>()
        {
            {"C1-1", ((true,null,null),new HashSet<WorkDay>()
                {
                    new WorkDay() { Start = DateTime.Parse("2020/4/10 8:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/11 8:00:00"), End = DateTime.Parse("2020/4/11 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/12 8:00:00"), End = DateTime.Parse("2020/4/12 20:00:00") },
                })},
            {"C1-2", ((false,
                new WorkDay() { Start = DateTime.Parse("2020/4/11 8:00:00"), End = DateTime.Parse("2020/4/12 20:00:00") },
                new WorkDay() { Start = DateTime.Parse("2020/4/11 8:00:00"), End = DateTime.Parse("2020/4/11 20:00:00") }),
                new HashSet<WorkDay>()
                {
                    new WorkDay() { Start = DateTime.Parse("2020/4/10 8:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/11 8:00:00"), End = DateTime.Parse("2020/4/11 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/11 8:00:00"), End = DateTime.Parse("2020/4/12 20:00:00") },
                })},
        };

        [TestMethod()]
        [DataRow("C1-1")]
        [DataRow("C1-2")]
        public void CheckTest(string key)
        {
            var result = Merge.Check(Combination[key].list);
            result.Is(Combination[key].expected);
        }

        readonly Dictionary<string, ((bool success, int count, HashSet<WorkDay> list) expected, HashSet<WorkDay> list)> SelfHealingTestData = new Dictionary<string, ((bool success, int count, HashSet<WorkDay> list) expected, HashSet<WorkDay> list)>()
        {
            {"C1-1", ((true,0,
                new HashSet<WorkDay>()
                {
                    new WorkDay() { Start = DateTime.Parse("2020/4/10 8:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/11 8:00:00"), End = DateTime.Parse("2020/4/11 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/12 8:00:00"), End = DateTime.Parse("2020/4/12 20:00:00") },
                }),
                new HashSet<WorkDay>()
                {
                    new WorkDay() { Start = DateTime.Parse("2020/4/10 8:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/11 8:00:00"), End = DateTime.Parse("2020/4/11 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/12 8:00:00"), End = DateTime.Parse("2020/4/12 20:00:00") },
                })},
            {"C1-2", ((true,1,
                new HashSet<WorkDay>()
                {
                    new WorkDay() { Start = DateTime.Parse("2020/4/10 8:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/11 8:00:00"), End = DateTime.Parse("2020/4/12 20:00:00") },
                }),
                new HashSet<WorkDay>()
                {
                    new WorkDay() { Start = DateTime.Parse("2020/4/10 8:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/11 8:00:00"), End = DateTime.Parse("2020/4/11 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/11 8:00:00"), End = DateTime.Parse("2020/4/12 20:00:00") },
                })},
        };

        [TestMethod()]
        //[DataRow("C1-1")]
        [DataRow("C1-2")]
        public void SelfHealingTest(string key)
        {
            var (success, count, results) = Merge.SelfHealing(SelfHealingTestData[key].list);
            success.Is(SelfHealingTestData[key].expected.success);
            count.Is(SelfHealingTestData[key].expected.count);
            results.Is(SelfHealingTestData[key].expected.list);
        }

        readonly Dictionary<string, ((bool success, HashSet<WorkDay> results) expected, WorkDay comparison, HashSet<WorkDay> standard)> DoListTestData = new Dictionary<string, ((bool success, HashSet<WorkDay> results) expected, WorkDay comparison, HashSet<WorkDay> standard)>()
        {
            {"C1-1", ((true,
                new HashSet<WorkDay>()
                {
                    new WorkDay() { Start = DateTime.Parse("2020/4/10 8:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/11 8:00:00"), End = DateTime.Parse("2020/4/11 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/12 8:00:00"), End = DateTime.Parse("2020/4/12 20:00:00") },
                }),
                new WorkDay() { Start = DateTime.Parse("2020/4/11 8:00:00"), End = DateTime.Parse("2020/4/11 20:00:00") },
                new HashSet<WorkDay>()
                {
                    new WorkDay() { Start = DateTime.Parse("2020/4/10 8:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/11 8:00:00"), End = DateTime.Parse("2020/4/11 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/12 8:00:00"), End = DateTime.Parse("2020/4/12 20:00:00") },
                })},
            {"C1-2", ((true,
                new HashSet<WorkDay>()
                {
                    new WorkDay() { Start = DateTime.Parse("2020/4/10 8:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/11 7:00:00"), End = DateTime.Parse("2020/4/11 21:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/12 8:00:00"), End = DateTime.Parse("2020/4/12 20:00:00") },
                }),
                new WorkDay() { Start = DateTime.Parse("2020/4/11 7:00:00"), End = DateTime.Parse("2020/4/11 21:00:00") },
                new HashSet<WorkDay>()
                {
                    new WorkDay() { Start = DateTime.Parse("2020/4/10 8:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/11 8:00:00"), End = DateTime.Parse("2020/4/11 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/12 8:00:00"), End = DateTime.Parse("2020/4/12 20:00:00") },
                })},
            {"C1-3", ((true,
                new HashSet<WorkDay>()
                {
                    new WorkDay() { Start = DateTime.Parse("2020/4/10 8:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/11 7:00:00"), End = DateTime.Parse("2020/4/11 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/12 8:00:00"), End = DateTime.Parse("2020/4/12 20:00:00") },
                }),
                new WorkDay() { Start = DateTime.Parse("2020/4/11 7:00:00"), End = DateTime.Parse("2020/4/11 19:00:00") },
                new HashSet<WorkDay>()
                {
                    new WorkDay() { Start = DateTime.Parse("2020/4/10 8:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/11 8:00:00"), End = DateTime.Parse("2020/4/11 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/12 8:00:00"), End = DateTime.Parse("2020/4/12 20:00:00") },
                })},
            {"C1-4", ((true,
                new HashSet<WorkDay>()
                {
                    new WorkDay() { Start = DateTime.Parse("2020/4/10 8:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/11 8:00:00"), End = DateTime.Parse("2020/4/11 21:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/12 8:00:00"), End = DateTime.Parse("2020/4/12 20:00:00") },
                }),
                new WorkDay() { Start = DateTime.Parse("2020/4/11 10:00:00"), End = DateTime.Parse("2020/4/11 21:00:00") },
                new HashSet<WorkDay>()
                {
                    new WorkDay() { Start = DateTime.Parse("2020/4/10 8:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/11 8:00:00"), End = DateTime.Parse("2020/4/11 20:00:00") },
                    new WorkDay() { Start = DateTime.Parse("2020/4/12 8:00:00"), End = DateTime.Parse("2020/4/12 20:00:00") },
                })},
        };

        [TestMethod()]
        [DataRow("C1-1")]
        [DataRow("C1-2")]
        [DataRow("C1-3")]
        [DataRow("C1-4")]
        public void DoListTest(string key)
        {
            var (success, results) = Merge.DoList(DoListTestData[key].standard, DoListTestData[key].comparison);
            success.Is(DoListTestData[key].expected.success);
            results.Is(DoListTestData[key].expected.results);
        }

    }
}