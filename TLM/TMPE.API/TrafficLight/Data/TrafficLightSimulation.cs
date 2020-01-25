namespace TrafficManager.API.TrafficLight.Data {
    using CSUtil.Commons;
    using System;
    using TrafficManager.API.Traffic.Enums;
    using TrafficManager.API.TrafficLight;

    public struct TrafficLightSimulation {
        /// <summary>Timed traffic light by node id.</summary>
        public ITimedTrafficLights timedLight;

        public ushort nodeId;
        public TrafficLightSimulationType simType;

        /// <inheritdoc />
        public override string ToString() {
            return string.Format(
                "TrafficLightSimulation {{ nodeId = {0}\n\ttype = {1}\n\ttimedLight = {2} }}",
                nodeId,
                simType,
                timedLight);
        }

        public TrafficLightSimulation(ushort nodeId) {
            // Log._Debug($"TrafficLightSimulation: Constructor called @ node {nodeId}");
            this.nodeId = nodeId;
            timedLight = null;
            simType = TrafficLightSimulationType.None;
        }

        public bool IsTimedLight() {
            return simType == TrafficLightSimulationType.Timed && timedLight != null;
        }

        public bool IsManualLight() {
            return simType == TrafficLightSimulationType.Manual;
        }

        public bool IsTimedLightRunning() {
            return IsTimedLight() && timedLight.IsStarted();
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
                timedLight.SimulationStep();
            }
        }

        public void Update() {
            Log._Trace($"TrafficLightSimulation.Update(): called for node {nodeId}");

            if (IsTimedLight()) {
                timedLight.OnGeometryUpdate();
                timedLight.Housekeeping();
            }
        }

        public void Housekeeping() {
            // TODO improve & remove
            timedLight?.Housekeeping(); // removes unused step lights
        }
    }
}