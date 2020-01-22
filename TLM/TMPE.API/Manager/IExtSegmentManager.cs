namespace TrafficManager.API.Manager {
    using TrafficManager.API.Traffic.Data;

    public interface IExtSegmentManager {
        /// <summary>Extended segment data.</summary>
        ExtSegment[] ExtSegments { get; }

        /// <summary>
        /// Checks if the segment with the given id is valid.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <returns>Whether the segment is valid.</returns>
        bool IsValid(ushort segmentId);

        /// <summary>Performs recalcution of the segment with the given id.</summary>
        /// <param name="segmentId">segment id.</param>
        void Recalculate(ushort segmentId);

        /// <summary>Calculates if the given segment is a one-way road.</summary>
        /// <param name="segmentId">segment to check.</param>
        /// <returns>Whether the managed segment is a one-way road.</returns>
        bool CalculateIsOneWay(ushort segmentId);

        /// <summary>
        /// Calculates if the given segment has a buslane.
        /// </summary>
        /// <param name="segmentId">segment to check.</param>
        /// <returns>Whether the given segment has a buslane.</returns>
        bool CalculateHasBusLane(ushort segmentId);

        /// <summary>Calculates if the given segment is a highway.</summary>
        /// <param name="segmentId">segment to check.</param>
        /// <returns>Whether the segment is a highway type road.</returns>
        bool CalculateIsHighway(ushort segmentId);
    }
}