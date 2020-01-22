namespace TrafficManager.API.Traffic.Data {
    using System;

    public struct ExtSegmentEnd : IEquatable<ExtSegmentEnd> {
        /// <summary>Segment id in game.</summary>
        public readonly ushort SegmentId;

        /// <summary>Whether this is a start node.</summary>
        public readonly bool IsStartNode;

        /// <summary>Node id.</summary>
        public readonly ushort NodeId;

        /// <summary>Whether vehicles can leave the node via this segment end.</summary>
        public bool IsOutgoing;

        /// <summary>Whether vehicles can enter the node via this segment end.</summary>
        public bool IsIncoming;

        /// <summary>First registered vehicle id on this segment end.</summary>
        public ushort FirstVehicleId;

        /// <inheritdoc />
        public override string ToString() {
            return string.Format(
                "ExtSegmentEnd {0} {{ segmentId={1}\n\tstartNode={2}\n\tnodeId={3}\n" +
                "\toutgoing={4}\n\tincoming={5}\n\tfirstVehicleId={6} }}",
                base.ToString(),
                SegmentId,
                IsStartNode,
                NodeId,
                IsOutgoing,
                IsIncoming,
                FirstVehicleId);
        }

        public ExtSegmentEnd(ushort segmentId, bool isStartNode) {
            this.SegmentId = segmentId;
            this.IsStartNode = isStartNode;
            NodeId = 0;
            IsOutgoing = false;
            IsIncoming = false;
            FirstVehicleId = 0;
        }

        public bool Equals(ExtSegmentEnd otherSegEnd) {
            return SegmentId == otherSegEnd.SegmentId && IsStartNode == otherSegEnd.IsStartNode;
        }

        public override bool Equals(object other) {
            return other is ExtSegmentEnd end
                   && Equals(end);
        }

        public override int GetHashCode() {
            const int prime = 31;
            int result = 1;
            result = (prime * result) + SegmentId.GetHashCode();
            result = (prime * result) + IsStartNode.GetHashCode();
            return result;
        }
    }
}