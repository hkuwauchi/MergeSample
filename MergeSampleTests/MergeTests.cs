using Microsoft.VisualStudio.TestTools.UnitTesting;
using MergeSample;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MergeSample.Tests
{
    [TestClass()]
    public class MergeTests
    {
        [TestMethod()]
        public void DoTest()
        {
            var input = new List<WorkDay>()
            {
                new WorkDay(){Start = DateTime.Parse("2020/4/10 8:00:00"),End = DateTime.Parse("2020/4/11 20:00:00")},
                new WorkDay(){Start = DateTime.Parse("2020/4/10 9:00:00"),End = DateTime.Parse("2020/4/10 21:00:00")},
                new WorkDay(){Start = DateTime.Parse("2020/4/10 10:00:00"),End = DateTime.Parse("2020/4/10 22:00:00")},
            };
            var result = Merge.Do(input);
            //Console.WriteLine(string.Join(Environment.NewLine, result.Select(c => (c.Start, c.End))));
            result.Count.Is(3);
        }

        readonly WorkDay Standard = new WorkDay() { Start = DateTime.Parse("2020/4/10 8:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") };

        readonly Dictionary<string, (WorkDay comparision, WorkDay expected)> TestData = new Dictionary<string, (WorkDay comparision, WorkDay expected)>()
        {
            //完全に重ならない
            //S   :        |------|
            //C1-1:                |------|
            //C1-2:|------|
            { "C1-1", (new WorkDay() { Start = DateTime.Parse("2020/4/10 21:00:00"), End = DateTime.Parse("2020/4/10 22:00:00") },null) },
            { "C1-2", (new WorkDay() { Start = DateTime.Parse("2020/4/10 06:00:00"), End = DateTime.Parse("2020/4/10 07:00:00") },null) },
            //いづれかが重なる
            //結合
            //S   :       |------|
            //C2-1:              |------|
            //C2-2:|------|
            { "C2-1", (new WorkDay() { Start = DateTime.Parse("2020/4/10 20:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") }, new WorkDay() { Start = DateTime.Parse("2020/4/10 08:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") }) },
            { "C2-2", (new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 08:00:00") }, new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") }) },
            //完全に包摂
            //S   :|------|
            //C3-1:|------|
            //C3-2:|----|
            //C3-3:  |----|
            //C3-4: |----|
            { "C3-1", (new WorkDay() { Start = DateTime.Parse("2020/4/10 08:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") }, new WorkDay() { Start = DateTime.Parse("2020/4/10 08:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") }) },
            { "C3-2", (new WorkDay() { Start = DateTime.Parse("2020/4/10 08:00:00"), End = DateTime.Parse("2020/4/10 19:00:00") }, new WorkDay() { Start = DateTime.Parse("2020/4/10 08:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") }) },
            { "C3-3", (new WorkDay() { Start = DateTime.Parse("2020/4/10 09:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") }, new WorkDay() { Start = DateTime.Parse("2020/4/10 08:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") }) },
            { "C3-4", (new WorkDay() { Start = DateTime.Parse("2020/4/10 09:00:00"), End = DateTime.Parse("2020/4/10 19:00:00") }, new WorkDay() { Start = DateTime.Parse("2020/4/10 08:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") }) },
            //両方の拡張（入れ替え）
            //S   : |------|
            //C4-1:|--------|
            { "C4-1", (new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") }, new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") }) },
            //開始の拡張
            //S   : |------|
            //C5-1:|------|
            //C5-2:|-------|
            //C5-3:|--------|
            { "C5-1", (new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 19:00:00") }, new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") }) },
            { "C5-2", (new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") }, new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 20:00:00") }) },
            { "C5-3", (new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") }, new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") }) },
            //終了の拡張
            //S   : |------|
            //C6-1:  |------|
            //C6-2: |-------|
            //C6-3:|--------|
            { "C6-1", (new WorkDay() { Start = DateTime.Parse("2020/4/10 09:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") }, new WorkDay() { Start = DateTime.Parse("2020/4/10 08:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") }) },
            { "C6-2", (new WorkDay() { Start = DateTime.Parse("2020/4/10 08:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") }, new WorkDay() { Start = DateTime.Parse("2020/4/10 08:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") }) },
            { "C6-3", (new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") }, new WorkDay() { Start = DateTime.Parse("2020/4/10 07:00:00"), End = DateTime.Parse("2020/4/10 21:00:00") }) },
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
        public void MeargeTest(string key)
        {
            var result = Merge.Mearge(Standard, TestData[key].comparision);
            if(result != null)
            {
                Console.WriteLine((result?.Start, result?.End));
            }
            result.Is(TestData[key].expected);
        }
    }
}