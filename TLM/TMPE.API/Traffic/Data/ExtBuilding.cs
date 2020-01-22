namespace TrafficManager.API.Traffic.Data {
    public struct ExtBuilding {
        /// <summary>Building id in game.</summary>
        public readonly ushort BuildingId;

        /// <summary>Current parking space demand (0-100).</summary>
        public byte ParkingSpaceDemand;

        /// <summary>Current incoming public transport demand (0-100).</summary>
        public byte IncomingPublicTransportDemand;

        /// <summary>Current outgoing public transport demand (0-100).</summary>
        public byte OutgoingPublicTransportDemand;

        /// <inheritdoc />
        public override string ToString() {
            return string.Format(
                "ExtBuilding {0} {{ buildingId = {1}\n\tparkingSpaceDemand = {2}\n" +
                "\tincomingPublicTransportDemand = {3}\n\toutgoingPublicTransportDemand = {4}" +
                " }}",
                base.ToString(),
                BuildingId,
                ParkingSpaceDemand,
                IncomingPublicTransportDemand,
                OutgoingPublicTransportDemand);
        }

        public ExtBuilding(ushort buildingId) {
            this.BuildingId = buildingId;
            ParkingSpaceDemand = 0;
            IncomingPublicTransportDemand = 0;
            OutgoingPublicTransportDemand = 0;
        }
    }
}