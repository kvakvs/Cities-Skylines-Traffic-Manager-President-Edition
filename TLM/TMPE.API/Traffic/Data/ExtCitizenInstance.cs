namespace TrafficManager.API.Traffic.Data {
    using System;
    using TrafficManager.API.Traffic.Enums;

    public struct ExtCitizenInstance {
        /// <summary>Citizen instance id.</summary>
        public readonly ushort InstanceId;

        /// <summary>
        /// Citizen path mode (used for Parking AI).
        /// </summary>
        public ExtPathMode PathMode;

        /// <summary>
        /// Number of times a formerly found parking space is already occupied after reaching its position.
        /// </summary>
        public int FailedParkingAttempts;

        /// <summary>
        /// Segment id / Building id where a parking space has been found.
        /// </summary>
        public ushort ParkingSpaceLocationId;

        /// <summary>
        /// Type of object (segment/building) where a parking space has been found.
        /// </summary>
        public ExtParkingSpaceLocation ParkingSpaceLocation;

        /// <summary>
        /// Path position that is used as a start position when parking fails.
        /// </summary>
        public PathUnit.Position? ParkingPathStartPosition;

        /// <summary>
        /// Walking path from (alternative) parking spot to target (only used to check if there is
        /// a valid walking path, not actually used at the moment).
        /// </summary>
        public uint ReturnPathId;

        /// <summary>State of the return path.</summary>
        public ExtPathState ReturnPathState;

        /// <summary>Last known distance to the citizen's parked car.</summary>
        public float LastDistanceToParkedCar;

        /// <summary>
        /// Specifies whether the last path-finding started at an outside connection.
        /// </summary>
        public bool AtOutsideConnection;

        /// <inheritdoc />
        public override string ToString() {
            return string.Format(
                "ExtCitizenInstance {{ instanceId = {0}\n\tpathMode = {1}\n" +
                "\tfailedParkingAttempts = {2}\n\tparkingSpaceLocationId = {3}\n" +
                "\tparkingSpaceLocation = {4}\n\tparkingPathStartPosition = {5}\n" +
                "\treturnPathId = {6}\n\treturnPathState = {7}\n\tlastDistanceToParkedCar = {8}\n" +
                "\tatOutsideConnection = {9} }}",
                InstanceId,
                PathMode,
                FailedParkingAttempts,
                ParkingSpaceLocationId,
                ParkingSpaceLocation,
                ParkingPathStartPosition,
                ReturnPathId,
                ReturnPathState,
                LastDistanceToParkedCar,
                AtOutsideConnection);
        }

        public ExtCitizenInstance(ushort instanceId) {
            this.InstanceId = instanceId;
            PathMode = ExtPathMode.None;
            FailedParkingAttempts = 0;
            ParkingSpaceLocationId = 0;
            ParkingSpaceLocation = ExtParkingSpaceLocation.None;
            ParkingPathStartPosition = null;
            ReturnPathId = 0;
            ReturnPathState = ExtPathState.None;
            LastDistanceToParkedCar = 0;
            AtOutsideConnection = false;
        }

        /// <summary>
        /// Determines the path type through evaluating the current path mode.
        /// </summary>
        /// <returns>The path type.</returns>
        public ExtPathType GetPathType() {
            switch (PathMode) {
                case ExtPathMode.CalculatingCarPathToAltParkPos:
                case ExtPathMode.CalculatingCarPathToKnownParkPos:
                case ExtPathMode.CalculatingCarPathToTarget:
                case ExtPathMode.DrivingToAltParkPos:
                case ExtPathMode.DrivingToKnownParkPos:
                case ExtPathMode.DrivingToTarget:
                case ExtPathMode.RequiresCarPath:
                case ExtPathMode.RequiresMixedCarPathToTarget:
                case ExtPathMode.ParkingFailed: {
                    return ExtPathType.DrivingOnly;
                }

                case ExtPathMode.CalculatingWalkingPathToParkedCar:
                case ExtPathMode.CalculatingWalkingPathToTarget:
                case ExtPathMode.RequiresWalkingPathToParkedCar:
                case ExtPathMode.RequiresWalkingPathToTarget:
                case ExtPathMode.ApproachingParkedCar:
                case ExtPathMode.WalkingToParkedCar:
                case ExtPathMode.WalkingToTarget: {
                    return ExtPathType.WalkingOnly;
                }

                default: {
                    return ExtPathType.None;
                }
            }
        }

        /// <summary>Converts an ExtPathState to a ExtSoftPathState.</summary>
        /// <param name="state">Input path.</param>
        /// <returns>New path.</returns>
        public static ExtSoftPathState ConvertPathStateToSoftPathState(ExtPathState state) {
            return (ExtSoftPathState)((int)state);
        }
    }
}