namespace TrafficManager.API.Traffic.Data {
    using TrafficManager.API.Traffic.Enums;

    public struct ExtCitizen {
        /// <summary>Citizen id in game.</summary>
        public readonly uint CitizenId;

        /// <summary>Mode of transport that is currently used to reach a destination.</summary>
        public ExtTransportMode TransportMode;

        /// <summary>Mode of transport that was previously used to reach a destination.</summary>
        public ExtTransportMode LastTransportMode;

        /// <summary>Previous building location.</summary>
        public readonly Citizen.Location LastLocation;

        public override string ToString() {
            return string.Format(
                "[ExtCitizen\n\tcitizenId = {0}\n\ttransportMode = {1}\n\tlastTransportMode = {2}\n" +
                "\tlastLocation = {3}\nExtCitizen]",
                CitizenId,
                TransportMode,
                TransportMode,
                LastLocation);
        }

        public ExtCitizen(uint citizenId) {
            this.CitizenId = citizenId;
            TransportMode = ExtTransportMode.None;
            LastTransportMode = ExtTransportMode.None;
            LastLocation = Citizen.Location.Moving;
        }
    }
}