using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MergeSample
{
    public class EquipmentState : IEquatable<EquipmentState>, IComparable<EquipmentState>
    {
        public string equipment_cd { get; set; }
        public string equipment_name { get; set; }
        public string equipment_ja { get; set; }
        public int sort { get; set; }
        public DateTime validsta_dt { get; set; }
        public DateTime validend_dt { get; set; }
        public int efficiency_rate { get; set; }
        public int StopClass { get; set; }

        public TimeSpan TimeSpan { get { return validend_dt - validsta_dt; } }

        public string ToStringEx() =>
            string.Join("\t", typeof(EquipmentState).GetProperties()
                .Select(c => c.GetValue(this, null) == null ? string.Empty : c.GetValue(this, null).ToString()));

        public bool Equals(EquipmentState obj) =>
            obj != null &&
            obj.ToStringEx() == ToStringEx();

        public override int GetHashCode() => ToStringEx().GetHashCode();

        public override bool Equals(object obj)
        {
            return obj is EquipmentState other && Equals(other);
        }

        public int CompareTo([AllowNull] EquipmentState other)
        {
            if (other == null) return -1;

            var cd = string.CompareOrdinal(equipment_cd, other.equipment_cd);
            if (cd != 0)
            {
                return cd;
            }

            var name = string.CompareOrdinal(equipment_name, other.equipment_name);
            if (name != 0)
            {
                return name;
            }

            var ja = string.CompareOrdinal(equipment_ja, other.equipment_ja);
            if (ja != 0)
            {
                return ja;
            }

            if (sort != other.sort)
            {
                return sort < other.sort ? -1 : 1;
            }

            if (efficiency_rate != other.efficiency_rate)
            {
                return efficiency_rate < other.efficiency_rate ? -1 : 1;
            }

            if (StopClass != other.StopClass)
            {
                return StopClass < other.StopClass ? -1 : 1;
            }

            if (validsta_dt < other.validsta_dt)
            {
                return -1;
            }

            if (validsta_dt == other.validsta_dt && TimeSpan == other.TimeSpan) return 0;

            if (validsta_dt == other.validsta_dt && TimeSpan < other.TimeSpan)
            {
                return -1;
            }

            return 1;
        }
    }
}
