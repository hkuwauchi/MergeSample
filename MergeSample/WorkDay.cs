using System;
using System.Diagnostics.CodeAnalysis;

namespace MergeSample
{
    public class WorkDay : IEquatable<WorkDay>, IComparable<WorkDay>
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public TimeSpan TimeSpan { get { return End - Start; } }

        public int CompareTo([AllowNull] WorkDay other)
        {
            if (other == null) return -1;

            if (Start < other.Start)
            {
                return -1;
            }

            if (Start == other.Start && TimeSpan == other.TimeSpan) return 0;

            if (Start == other.Start && TimeSpan < other.TimeSpan)
            {
                return -1;
            }

            return 1;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as WorkDay);
        }

        public bool Equals(WorkDay other)
        {
            return other != null &&
                   Start == other.Start &&
                   End == other.End;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Start, End);
        }

        public static bool operator ==(WorkDay left, WorkDay right)
        {
            if (left is null)
            {
                return right is null;
            }

            return left.Equals(right);
        }

        public static bool operator !=(WorkDay left, WorkDay right)
        {
            return !(left == right);
        }

        public static bool operator <(WorkDay left, WorkDay right)
        {
            return left is null ? right is object : left.CompareTo(right) < 0;
        }

        public static bool operator <=(WorkDay left, WorkDay right)
        {
            return left is null || left.CompareTo(right) <= 0;
        }

        public static bool operator >(WorkDay left, WorkDay right)
        {
            return left is object && left.CompareTo(right) > 0;
        }

        public static bool operator >=(WorkDay left, WorkDay right)
        {
            return left is null ? right is null : left.CompareTo(right) >= 0;
        }
    }
}
