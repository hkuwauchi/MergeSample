using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MergeSample
{
    public class Merge
    {
        public static WorkDay Mearge(WorkDay standard, WorkDay comparison)
        {
            //完全に重ならない
            //S0:        |------|
            //C1:                |------|
            //C2:|------|
            if (standard.End < comparison.Start || comparison.End < standard.Start)
            {
                return null;
            }

            //いづれかが重なる
            //結合
            //S0:       |------|
            //C1:              |------|
            //C2:|------|
            //完全に包摂
            //S0:|------|
            //C1:|------|
            //C2:|----|
            //C3:  |----|
            //C4: |----|
            //両方の拡張（入れ替え）
            //S0: |------|
            //C1:|--------|
            //開始の拡張
            //S0: |------|
            //C1:|------|
            //C2:|-------|
            //C3:|--------|
            //終了の拡張
            //S0: |------|
            //C1:  |------|
            //C2: |-------|
            //C3:|--------|

            var ns = standard.Start < comparison.Start ? standard.Start : comparison.Start;
            var ne = comparison.End < standard.End ? standard.End : comparison.End;
            return new WorkDay() { Start = ns, End = ne };
        }

        public static List<WorkDay> Do(List<WorkDay> list)
        {
            var input = new Stack<WorkDay>();

            foreach (var wd in list.Reverse<WorkDay>())
            {
                input.Push(wd);
            }

            var results = new Stack<WorkDay>();

            while (input.Count > 0)
            {
                var wd = input.Pop();

                Console.WriteLine((wd.Start, wd.End));

                if (results.Count == 0)
                {
                    results.Push(wd);
                    continue;
                }

                var p = results.Pop();

            }

            Console.WriteLine(input.Count);

            return list;
        }
    }
}
