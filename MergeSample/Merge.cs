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
        public static (bool success, EquipmentState newDay) Do(EquipmentState standard, EquipmentState comparison)
        {
            //完全に重ならない
            if (standard.validend_dt < comparison.validsta_dt || comparison.validend_dt < standard.validsta_dt)
            {
                return (false, null);
            }

            //何れかが重なるる
            var ns = standard.validsta_dt < comparison.validsta_dt ? standard.validsta_dt : comparison.validsta_dt;
            var ne = comparison.validend_dt < standard.validend_dt ? standard.validend_dt : comparison.validend_dt;
#if DEBUG
            Console.WriteLine((ns, ne));
#endif
            return (true, new EquipmentState() { validsta_dt = ns, validend_dt = ne });
        }

        /// <summary>
        /// リストに重なりがないか確認する
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static (bool success, EquipmentState newDay, EquipmentState outDay) Check(HashSet<EquipmentState> target)
        {
            var combinations = Combination.Enumerate(target, 2, false);

            foreach (var elem in combinations.Select((v, i) => new { v, i }))
            {
                var (success, newDay) = Do(elem.v[0], elem.v[1]);
#if DEBUG
                Console.WriteLine($"{elem.i + 1:00} : {success} : ({string.Join(",", elem.v.Select(x => (x.validsta_dt, x.validend_dt)).ToArray())})");
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
        public static (bool success, int count, HashSet<EquipmentState> results) SelfHealing(HashSet<EquipmentState> target, int limit = 10)
        {
            var temp = new HashSet<EquipmentState>(target);

            int count = 0;

            (bool success, EquipmentState newDay, EquipmentState outDay) check = Check(temp);

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

            return (true, count, new HashSet<EquipmentState>(temp.OrderBy(c => c)));
        }

        public static (bool success, HashSet<EquipmentState> results) DoList(HashSet<EquipmentState> standard, EquipmentState comparison)
        {
            if (standard == null)
            {
                return (false, null);
            }

            var temp = new HashSet<EquipmentState>(standard);

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
