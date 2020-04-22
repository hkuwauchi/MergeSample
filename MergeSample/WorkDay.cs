using System;
using System.Collections.Generic;
using System.Text;

namespace MergeSample
{
    public class WorkDay : IEquatable<WorkDay>
    {
        public DateTime Start { get; set; }
        public DateTime End { get; set; }

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
    }
}
