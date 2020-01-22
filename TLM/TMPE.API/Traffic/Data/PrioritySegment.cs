namespace TrafficManager.API.Traffic.Data {
    using JetBrains.Annotations;
    using TrafficManager.API.Traffic.Enums;

    /// <summary>
    /// A priority segment specifies the priority signs that are present at each end of a certain segment.
    /// </summary>
    public struct PrioritySegment {
        /// <summary>
        /// Priority sign at start node (default: None).
        /// </summary>
        public PriorityType StartType;

        /// <summary>
        /// Priority sign at end node (default: None).
        /// </summary>
        public PriorityType EndType;

        /// <inheritdoc />
        public override string ToString() {
            return string.Format(
                "[PrioritySegment\n\tstartType = {0}\n\tendType = {1}\nPrioritySegment]",
                StartType,
                EndType);
        }

        [UsedImplicitly]
        public PrioritySegment(PriorityType startType, PriorityType endType) {
            this.StartType = startType;
            this.EndType = endType;
        }

        public void Reset() {
            StartType = PriorityType.None;
            EndType = PriorityType.None;
        }

        public bool IsDefault() {
            return !HasPrioritySignAtNode(true) && !HasPrioritySignAtNode(false);
        }

        public bool HasPrioritySignAtNode(bool startNode) {
            return startNode
                       ? StartType != PriorityType.None
                       : EndType != PriorityType.None;
        }
    }
}