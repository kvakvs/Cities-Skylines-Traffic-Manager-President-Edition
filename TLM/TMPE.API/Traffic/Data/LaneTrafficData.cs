namespace TrafficManager.API.Traffic.Data {
    using System;

    public struct LaneTrafficData {
        /// <summary>
        /// Number of seen vehicles since last speed measurement.
        /// </summary>
        public ushort TrafficBuffer;

        /// <summary>
        /// Number of seen vehicles before last speed measurement.
        /// </summary>
        public ushort LastTrafficBuffer;

        /// <summary>
        /// All-time max. traffic buffer.
        /// </summary>
        public ushort MaxTrafficBuffer;

        /// <summary>
        /// Accumulated speeds since last traffic measurement.
        /// </summary>
        public uint AccumulatedSpeeds;

        /// <summary>
        /// Current lane mean speed, per ten thousands.
        /// </summary>
        public ushort MeanSpeed;

        /// <inheritdoc />
        public override string ToString() {
            return string.Format(
                "LaneTrafficData {{ trafficBuffer = {0}\n\tlastTrafficBuffer = {1}\n" +
                "\tmaxTrafficBuffer = {2}\n\ttrafficBuffer = {3}\n\taccumulatedSpeeds = {4}\n" +
                "\tmeanSpeed = {5} }}",
                TrafficBuffer,
                LastTrafficBuffer,
                MaxTrafficBuffer,
                TrafficBuffer,
                AccumulatedSpeeds,
                MeanSpeed);
        }
    }
}