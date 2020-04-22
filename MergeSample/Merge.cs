using System;
using System.Collections.Generic;
using System.Linq;

namespace MergeSample
{
    /// <summary>
    /// マージする
    /// </summary>
    public static class Merge
    {
        /// <summary>
        /// マージを実行する
        /// </summary>
        /// <param name="standard"></param>
        /// <param name="comparison"></param>
        /// <returns></returns>
        public static (bool success, WorkDay newDay) Do(WorkDay standard, WorkDay comparison)
        {
            //完全に重ならない
            if (standard.End < comparison.Start || comparison.End < standard.Start)
            {
                return (false, null);
            }

            //何れかが重なるる
            var ns = standard.Start < comparison.Start ? standard.Start : comparison.Start;
            var ne = comparison.End < standard.End ? standard.End : comparison.End;
#if DEBUG
            Console.WriteLine((ns, ne));
#endif
            return (true, new WorkDay() { Start = ns, End = ne });
        }

        /// <summary>
        /// リストに重なりがないか確認する
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static (bool success, WorkDay newDay, WorkDay outDay) Check(HashSet<WorkDay> target)
        {
            var combinations = Combination.Enumerate(target, 2, false);

            foreach (var elem in combinations.Select((v, i) => new { v, i }))
            {
                var (success, newDay) = Do(elem.v[0], elem.v[1]);
#if DEBUG
                Console.WriteLine($"{elem.i + 1:00} : {success} : ({string.Join(",", elem.v.Select(x => (x.Start, x.End)).ToArray())})");
#endif
                if (success) return (false, newDay, elem.v[0].TimeSpan < elem.v[1].TimeSpan ? elem.v[0] : elem.v[1]);
            }
            return (true, null, null);
        }

        /// <summary>
        /// リストの重なりを修復する
        /// </summary>
        /// <param name="target"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static (bool success, int count, HashSet<WorkDay> results) SelfHealing(HashSet<WorkDay> target, int limit = 10)
        {
            var temp = new HashSet<WorkDay>(target);

            int count = 0;

            (bool success, WorkDay newDay, WorkDay outDay) check = Check(temp);

            while (!check.success)
            {
                if (!temp.Remove(check.outDay))
                {
                    return (false, count, null);
                }

                if (!temp.Contains(check.newDay) && !temp.Add(check.newDay))
                {
                    return (false, count, null);
                }

                check = Check(temp);

                count++;

                if (limit < count)
                {
                    return (false, count, null);
                }
            }

            return (true, count, new HashSet<WorkDay>(temp.OrderBy(c => c)));
        }

        public static (bool success, HashSet<WorkDay> results) DoList(HashSet<WorkDay> standard, WorkDay comparison)
        {
            if (standard == null)
            {
                return (false, null);
            }

            var temp = new HashSet<WorkDay>(standard);

            if (comparison == null)
            {
                return (true, temp);
            }

            if (!temp.Add(comparison))
            {
                //既に同じ値が存在している
                return (true, temp);
            }

            var (success, _, results) = SelfHealing(temp);

            return (success, results);
        }
    }
}
