namespace TrafficManager.API.Traffic.Data {
    using System;

    /// <summary>
    /// Holds left/right turn-on-red candidate segments.
    /// </summary>
    public struct TurnOnRedSegments {
        /// <summary>
        /// Left segment id (or 0 if no left turn-on-red candidate segment).
        /// </summary>
        public ushort LeftSegmentId;

        /// <summary>
        /// Right segment id (or 0 if no right turn-on-red candidate segment).
        /// </summary>
        public ushort RightSegmentId;

        public void Reset() {
            LeftSegmentId = 0;
            RightSegmentId = 0;
        }

        /// <inheritdoc />
        public override string ToString() {
            return string.Format(
                "[TurnOnRedSegments {0}\n\tleftSegmentId = {1}\n\trightSegmentId = {2}\nSegmentEnd]",
                base.ToString(),
                LeftSegmentId,
                RightSegmentId);
        }
    }
}