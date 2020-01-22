namespace TrafficManager.API.Traffic.Data {
    using System;

    public struct ExtSegment : IEquatable<ExtSegment> {
        /// <summary>Segment id in game.</summary>
        public readonly ushort SegmentId;

        /// <summary>Whether segment is valid (exists).</summary>
        public bool IsValid;

        /// <summary>Whether segment is one-way road.</summary>
        public bool IsOneWay;

        /// <summary>Whether segment is a highway road (special rules, no buildings).</summary>
        public bool IsHighway;

        /// <summary>Whether the segment has a bus lane in it.</summary>
        public bool HasBusLane;

        /// <inheritdoc />
        public override string ToString() {
            return string.Format(
                "ExtSegment {0} {{ segmentId={1}\n\tvalid={2}\n\toneWay={3}\n\thighway={4}\n" +
                "\tbuslane={5} }}",
                base.ToString(),
                SegmentId,
                IsValid,
                IsOneWay,
                IsHighway,
                HasBusLane);
        }

        public ExtSegment(ushort segmentId) {
            this.SegmentId = segmentId;
            IsValid = false;
            IsOneWay = false;
            IsHighway = false;
            HasBusLane = false;
        }

        public void Reset() {
            IsOneWay = false;
            IsHighway = false;
            HasBusLane = false;
        }

        public bool Equals(ExtSegment otherSeg) {
            return SegmentId == otherSeg.SegmentId;
        }

        public override bool Equals(object other) {
            return other is ExtSegment segment
                   && Equals(segment);
        }

        public override int GetHashCode() {
            const int prime = 31;
            int result = 1;
            result = (prime * result) + SegmentId.GetHashCode();
            return result;
        }
    }
}