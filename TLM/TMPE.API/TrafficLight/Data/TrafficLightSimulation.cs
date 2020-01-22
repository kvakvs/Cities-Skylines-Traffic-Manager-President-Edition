namespace TrafficManager.API.TrafficLight.Data {
    using CSUtil.Commons;
    using System;
    using TrafficManager.API.Traffic.Enums;
    using TrafficManager.API.TrafficLight;

    public struct TrafficLightSimulation {
        /// <summary>Timed traffic light by node id.</summary>
        public ITimedTrafficLights TimedLight;

        public ushort NodeId;
        public TrafficLightSimulationType Type;

        /// <inheritdoc />
        public override string ToString() {
            return string.Format(
                "TrafficLightSimulation {{ nodeId = {0}\n\ttype = {1}\n\ttimedLight = {2} }}",
                NodeId,
                Type,
                TimedLight);
        }

        public TrafficLightSimulation(ushort nodeId) {
            // Log._Debug($"TrafficLightSimulation: Constructor called @ node {nodeId}");
            this.NodeId = nodeId;
            TimedLight = null;
            Type = TrafficLightSimulationType.None;
        }

        public bool IsTimedLight() {
            return Type == TrafficLightSimulationType.Timed && TimedLight != null;
        }

        public bool IsManualLight() {
            return Type == TrafficLightSimulationType.Manual;
        }

        public bool IsTimedLightRunning() {
            return IsTimedLight() && TimedLight.IsStarted();
        }

        public bool IsSimulationRunning() {
            return IsManualLight() || IsTimedLightRunning();
        }

        public bool HasSimulation() {
            return IsManualLight() || IsTimedLight();
        }

        public void SimulationStep() {
            if (!HasSimulation()) {
                return;
            }

            if (IsTimedLightRunning()) {
                TimedLight.SimulationStep();
            }
        }

        public void Update() {
            Log._Trace($"TrafficLightSimulation.Update(): called for node {NodeId}");

            if (IsTimedLight()) {
                TimedLight.OnGeometryUpdate();
                TimedLight.Housekeeping();
            }
        }

        public void Housekeeping() {
            // TODO improve & remove
            TimedLight?.Housekeeping(); // removes unused step lights
        }
    }
}