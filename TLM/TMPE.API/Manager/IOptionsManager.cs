namespace TrafficManager.API.Manager {
    /// <summary>Manages mod options.</summary>
    public interface IOptionsManager : ICustomDataManager<byte[]> {
        /// <summary>
        /// Determines if modifications to segments may be published in the current state.
        /// </summary>
        /// <returns>Whether changes may be published.</returns>
        bool MayPublishSegmentChanges();
    }
}