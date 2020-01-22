namespace TrafficManager.API.Geometry {
    using JetBrains.Annotations;
    using TrafficManager.Geometry;

    // Not used
    [UsedImplicitly]
    public interface INodeGeometry {
        /// <summary>Gets the node id.</summary>
        ushort NodeId { get; }

        /// <summary>Gets a value indicating whether this node is a simple junction.</summary>
        bool SimpleJunction { get; }

        /// <summary>Gets the number of incoming segments.</summary>
        int NumIncomingSegments { get; }

        /// <summary>Gets the number of outgoing segments.</summary>
        int NumOutgoingSegments { get; }

        /// <summary>Gets all connected segment ends.</summary>
        ISegmentEndGeometry[] SegmentEndGeometries { get; }

        /// <summary>Gets the number of the connected segment ends.</summary>
        byte NumSegmentEnds { get; }

        /// <summary>Gets a value indicating whether the node is valid.</summary>
        bool Valid { get; }

        /// <summary>Recalculates the geometry.</summary>
        void Recalculate();
    }
}