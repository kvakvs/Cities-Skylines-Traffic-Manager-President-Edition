namespace TrafficManager.API.Traffic.Data {
    using System;
    using CSUtil.Commons;

    /// <summary>Segment flags hold both segment end flags.</summary>
    public struct SegmentFlags {
        public SegmentEndFlags StartNodeFlags;
        public SegmentEndFlags EndNodeFlags;

        public bool IsUturnAllowed(bool startNode) {
            return startNode
                       ? StartNodeFlags.IsUturnAllowed()
                       : EndNodeFlags.IsUturnAllowed();
        }

        public bool IsNearTurnOnRedAllowed(bool startNode) {
            return startNode
                       ? StartNodeFlags.IsNearTurnOnRedAllowed()
                       : EndNodeFlags.IsNearTurnOnRedAllowed();
        }

        public bool IsFarTurnOnRedAllowed(bool startNode) {
            return startNode
                       ? StartNodeFlags.IsFarTurnOnRedAllowed()
                       : EndNodeFlags.IsFarTurnOnRedAllowed();
        }

        public bool IsLaneChangingAllowedWhenGoingStraight(bool startNode) {
            return startNode
                       ? StartNodeFlags.IsLaneChangingAllowedWhenGoingStraight()
                       : EndNodeFlags.IsLaneChangingAllowedWhenGoingStraight();
        }

        public bool IsEnteringBlockedJunctionAllowed(bool startNode) {
            return startNode
                       ? StartNodeFlags.IsEnteringBlockedJunctionAllowed()
                       : EndNodeFlags.IsEnteringBlockedJunctionAllowed();
        }

        public bool IsPedestrianCrossingAllowed(bool startNode) {
            return startNode
                       ? StartNodeFlags.IsPedestrianCrossingAllowed()
                       : EndNodeFlags.IsPedestrianCrossingAllowed();
        }

        public TernaryBool GetUturnAllowed(bool startNode) {
            return startNode ? StartNodeFlags.UturnAllowed : EndNodeFlags.UturnAllowed;
        }

        public TernaryBool GetNearTurnOnRedAllowed(bool startNode) {
            return startNode
                       ? StartNodeFlags.NearTurnOnRedAllowed
                       : EndNodeFlags.NearTurnOnRedAllowed;
        }

        public TernaryBool GetFarTurnOnRedAllowed(bool startNode) {
            return startNode
                       ? StartNodeFlags.FarTurnOnRedAllowed
                       : EndNodeFlags.FarTurnOnRedAllowed;
        }

        public TernaryBool GetLaneChangingAllowedWhenGoingStraight(bool startNode) {
            return startNode
                       ? StartNodeFlags.StraightLaneChangingAllowed
                       : EndNodeFlags.StraightLaneChangingAllowed;
        }

        public TernaryBool GetEnteringBlockedJunctionAllowed(bool startNode) {
            return startNode
                       ? StartNodeFlags.EnterWhenBlockedAllowed
                       : EndNodeFlags.EnterWhenBlockedAllowed;
        }

        public TernaryBool GetPedestrianCrossingAllowed(bool startNode) {
            return startNode
                       ? StartNodeFlags.PedestrianCrossingAllowed
                       : EndNodeFlags.PedestrianCrossingAllowed;
        }

        public void SetUturnAllowed(bool startNode, bool value) {
            if (startNode) {
                StartNodeFlags.SetUturnAllowed(value);
            } else {
                EndNodeFlags.SetUturnAllowed(value);
            }
        }

        public void SetNearTurnOnRedAllowed(bool startNode, bool value) {
            if (startNode) {
                StartNodeFlags.SetNearTurnOnRedAllowed(value);
            } else {
                EndNodeFlags.SetNearTurnOnRedAllowed(value);
            }
        }

        public void SetFarTurnOnRedAllowed(bool startNode, bool value) {
            if (startNode) {
                StartNodeFlags.SetFarTurnOnRedAllowed(value);
            } else {
                EndNodeFlags.SetFarTurnOnRedAllowed(value);
            }
        }

        public void SetLaneChangingAllowedWhenGoingStraight(bool startNode, bool value) {
            if (startNode) {
                StartNodeFlags.SetLaneChangingAllowedWhenGoingStraight(value);
            } else {
                EndNodeFlags.SetLaneChangingAllowedWhenGoingStraight(value);
            }
        }

        public void SetEnteringBlockedJunctionAllowed(bool startNode, bool value) {
            if (startNode) {
                StartNodeFlags.SetEnteringBlockedJunctionAllowed(value);
            } else {
                EndNodeFlags.SetEnteringBlockedJunctionAllowed(value);
            }
        }

        public void SetPedestrianCrossingAllowed(bool startNode, bool value) {
            if (startNode) {
                StartNodeFlags.SetPedestrianCrossingAllowed(value);
            } else {
                EndNodeFlags.SetPedestrianCrossingAllowed(value);
            }
        }

        public bool IsDefault() {
            return StartNodeFlags.IsDefault() && EndNodeFlags.IsDefault();
        }

        public void Reset(bool? startNode = null, bool resetDefaults = true) {
            if (startNode == null || (bool)startNode) {
                StartNodeFlags.Reset(resetDefaults);
            }

            if (startNode == null || !(bool)startNode) {
                EndNodeFlags.Reset(resetDefaults);
            }
        }

        public override string ToString() {
            return string.Format(
                "[SegmentFlags\n\tstartNodeFlags = {0}\n\tendNodeFlags = {1}\nSegmentFlags]",
                StartNodeFlags,
                EndNodeFlags);
        }
    }
}