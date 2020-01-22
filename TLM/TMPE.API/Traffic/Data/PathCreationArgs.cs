namespace TrafficManager.API.Traffic.Data {
    using TrafficManager.API.Traffic.Enums;

    public struct PathCreationArgs {
        /// <summary>
        /// Extended path type.
        /// </summary>
        public Enums.ExtPathType ExtPathType;

        /// <summary>
        /// Extended vehicle type.
        /// </summary>
        public Enums.ExtVehicleType ExtVehicleType;

        /// <summary>
        /// (optional) vehicle id.
        /// </summary>
        public ushort VehicleId;

        /// <summary>
        /// Whether entity is already spawned.
        /// </summary>
        public bool Spawned;

        /// <summary>
        /// Current build index.
        /// </summary>
        public uint BuildIndex;

        /// <summary>
        /// Start position (first alternative).
        /// </summary>
        public PathUnit.Position StartPosA;

        /// <summary>
        /// Start position (second alternative, opposite road side).
        /// </summary>
        public PathUnit.Position StartPosB;

        /// <summary>
        /// End position (first alternative).
        /// </summary>
        public PathUnit.Position EndPosA;

        /// <summary>
        /// End position (second alternative, opposite road side).
        /// </summary>
        public PathUnit.Position EndPosB;

        /// <summary>
        /// (optional) position of the parked vehicle.
        /// </summary>
        public PathUnit.Position VehiclePosition;

        /// <summary>
        /// Allowed set of lane types.
        /// </summary>
        public NetInfo.LaneType LaneTypes;

        /// <summary>
        /// Allowed set of vehicle types.
        /// </summary>
        public VehicleInfo.VehicleType VehicleTypes;

        /// <summary>
        /// Maximum allowed path length.
        /// </summary>
        public float MaxLength;

        /// <summary>
        /// Whether the path is calculated for a heavy vehicle.
        /// </summary>
        public bool IsHeavyVehicle;

        /// <summary>
        /// Whether the path is calculated for a vehicle with a combustion engine.
        /// </summary>
        public bool HasCombustionEngine;

        /// <summary>
        /// Whether blocked segments should be ignored.
        /// </summary>
        public bool IgnoreBlocked;

        /// <summary>
        /// Whether flooded segments should be ignored.
        /// </summary>
        public bool IgnoreFlooded;

        /// <summary>
        /// Whether path costs should be ignored.
        /// </summary>
        public bool IgnoreCosts;

        /// <summary>
        /// Whether random parking should apply.
        /// </summary>
        public bool RandomParking;

        /// <summary>
        /// Whether the path should remain stable and not randomized.
        /// </summary>
        public bool StablePath;

        /// <summary>Whether this is a high priority path.</summary>
        public bool SkipQueue;
    }
}