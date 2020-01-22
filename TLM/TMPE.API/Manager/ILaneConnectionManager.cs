namespace TrafficManager.API.Manager {
    public interface ILaneConnectionManager {
        // TODO define me!

        /// <summary>
        /// Determines whether u-turn connections exist for the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is located at starting node.</param>
        /// <returns>Whether u-turn connections exist.</returns>
        bool HasUturnConnections(ushort segmentId, bool startNode);
    }
}