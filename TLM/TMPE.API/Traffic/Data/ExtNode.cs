namespace TrafficManager.API.Traffic.Data {
    using System;
    using System.Collections.Generic;
    using CSUtil.Commons;

    public struct ExtNode : IEquatable<ExtNode> {
        /// <summary>Node id in game.</summary>
        public readonly ushort NodeId;

        /// <summary>Connected segment ids, up to 8.</summary>
        public readonly HashSet<ushort> SegmentIds;

        /// <summary>Last removed segment id.</summary>
        public ISegmentEndId RemovedSegmentEndId;

        /// <inheritdoc />
        public override string ToString() {
            return string.Format(
                "ExtNode {0} {{ nodeId={1}\n\tsegmentIds={2}\n\tremovedSegmentEndId={3} }}",
                base.ToString(),
                NodeId,
                SegmentIds.CollectionToString(),
                RemovedSegmentEndId);
        }

        public ExtNode(ushort nodeId) {
            this.NodeId = nodeId;
            SegmentIds = new HashSet<ushort>();
            RemovedSegmentEndId = null;
        }

        public void Reset() {
            SegmentIds.Clear();
            RemovedSegmentEndId = null;
        }

        public bool Equals(ExtNode otherNode) {
            return NodeId == otherNode.NodeId;
        }

        public override bool Equals(object other) {
            return other is ExtNode node
                   && Equals(node);
        }

        public override int GetHashCode() {
            const int prime = 31;
            int result = 1;
            result = (prime * result) + NodeId.GetHashCode();
            return result;
        }
    }
}