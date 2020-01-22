namespace TrafficManager.API.Geometry {
    using CSUtil.Commons;
    using TrafficManager.API.Traffic;
    using TrafficManager.API.Traffic.Enums;

    public interface ISegmentEndGeometry : ISegmentEndId {
        /// <summary>Gets the connected node id.</summary>
        ushort NodeId { get; }

        /// <summary>Gets the last known connected node id.</summary>
        ushort LastKnownNodeId { get; }

        /// <summary>Gets all connected segment ids.</summary>
        ushort[] ConnectedSegments { get; }

        /// <summary>Gets the number of connected segments.</summary>
        byte NumConnectedSegments { get; }

        /// <summary>Gets the number of incoming segments.</summary>
        byte NumIncomingSegments { get; }

        /// <summary>Gets the number of outgoing segments.</summary>
        byte NumOutgoingSegments { get; }

        /// <summary>Gets all left segment ids.</summary>
        ushort[] LeftSegments { get; }

        /// <summary>Gets the number of left segments.</summary>
        byte NumLeftSegments { get; }

        /// <summary>Gets all incoming left segment ids.</summary>
        ushort[] IncomingLeftSegments { get; }

        /// <summary>Gets the number of incoming left segments.</summary>
        byte NumIncomingLeftSegments { get; }

        /// <summary>Gets all outgoing left segment ids.</summary>
        ushort[] OutgoingLeftSegments { get; }

        /// <summary>Gets the number of outgoing left segments.</summary>
        byte NumOutgoingLeftSegments { get; }

        /// <summary>Gets all right segment ids.</summary>
        ushort[] RightSegments { get; }

        /// <summary>Gets the number of right segments.</summary>
        byte NumRightSegments { get; }

        /// <summary>Gets all incoming right segment ids.</summary>
        ushort[] IncomingRightSegments { get; }

        /// <summary>Gets holds the number of incoming right segments.</summary>
        byte NumIncomingRightSegments { get; }

        /// <summary>Gets all outgoing right segment ids.</summary>
        ushort[] OutgoingRightSegments { get; }

        /// <summary>Gets the number of outgoing right segments.</summary>
        byte NumOutgoingRightSegments { get; }

        /// <summary>Gets all straight segment ids.</summary>
        ushort[] StraightSegments { get; }

        /// <summary>Gets the number of straight segments.</summary>
        byte NumStraightSegments { get; }

        /// <summary>Gets all incoming straight segment ids.</summary>
        ushort[] IncomingStraightSegments { get; }

        /// <summary>Gets the number of incoming straight segments.</summary>
        byte NumIncomingStraightSegments { get; }

        /// <summary>Gets all outgoing straight segment ids.</summary>
        ushort[] OutgoingStraightSegments { get; }

        /// <summary>Gets the number of outgoing straight segments.</summary>
        byte NumOutgoingStraightSegments { get; }

        /// <summary>Gets a value indicating whether the segment end is only connected to highway segments.</summary>
        bool OnlyHighways { get; }

        /// <summary>Gets a value indicating whether the segment end is an outgoing one-way.</summary>
        bool OutgoingOneWay { get; }

        /// <summary>Gets a value indicating whether the segment end is an incoming one-way.</summary>
        bool IncomingOneWay { get; }

        /// <summary>Gets a value indicating whether the segment end is incoming.</summary>
        bool Incoming { get; }

        /// <summary>Gets a value indicating whether the segment end is outgoing.</summary>
        bool Outgoing { get; }

        /// <summary>Gets all incoming segment ids.</summary>
        ushort[] IncomingSegments { get; }

        /// <summary>Gets all outgoing segment ids.</summary>
        ushort[] OutgoingSegments { get; }

        /// <summary>Gets whether the segment end is valid.</summary>
        bool Valid { get; }

        /// <summary>Recalculates the segment end.</summary>
        /// <param name="calcMode">propagation mode.</param>
        void Recalculate(GeometryCalculationMode calcMode);

        /// <summary>Determines wheter the given segment is right to this segment end.</summary>
        /// <param name="toSegmentId">segment id.</param>
        bool IsRightSegment(ushort toSegmentId);

        /// <summary>Determines wheter the given segment is left to this segment end.</summary>
        /// <param name="toSegmentId">segment id.</param>
        bool IsLeftSegment(ushort toSegmentId);

        /// <summary>Determines wheter the given segment is straight to this segment end.</summary>
        /// <param name="toSegmentId">segment id.</param>
        bool IsStraightSegment(ushort toSegmentId);

        /// <summary>Calculates the direction of the given segment relative to this segment end.</summary>
        /// <param name="otherSegmentId">segment id.</param>
        ArrowDirection GetDirection(ushort otherSegmentId);
    }
}