namespace TrafficManager.API.Traffic.Data {
    using System;
    using CSUtil.Commons;

    /// <summary>Segment end flags store junction restrictions.</summary>
    public struct SegmentEndFlags {
        public TernaryBool UturnAllowed;
        public TernaryBool NearTurnOnRedAllowed;
        public TernaryBool FarTurnOnRedAllowed;
        public TernaryBool StraightLaneChangingAllowed;
        public TernaryBool EnterWhenBlockedAllowed;
        public TernaryBool PedestrianCrossingAllowed;

        public bool DefaultUturnAllowed;
        public bool DefaultNearTurnOnRedAllowed;
        public bool DefaultFarTurnOnRedAllowed;
        public bool DefaultStraightLaneChangingAllowed;
        public bool DefaultEnterWhenBlockedAllowed;
        public bool DefaultPedestrianCrossingAllowed;

        public bool IsUturnAllowed() {
            return UturnAllowed == TernaryBool.Undefined
                       ? DefaultUturnAllowed
                       : TernaryBoolUtil.ToBool(UturnAllowed);
        }

        public bool IsNearTurnOnRedAllowed() {
            return NearTurnOnRedAllowed == TernaryBool.Undefined
                       ? DefaultNearTurnOnRedAllowed
                       : TernaryBoolUtil.ToBool(NearTurnOnRedAllowed);
        }

        public bool IsFarTurnOnRedAllowed() {
            return FarTurnOnRedAllowed == TernaryBool.Undefined
                       ? DefaultFarTurnOnRedAllowed
                       : TernaryBoolUtil.ToBool(FarTurnOnRedAllowed);
        }

        public bool IsLaneChangingAllowedWhenGoingStraight() {
            return StraightLaneChangingAllowed == TernaryBool.Undefined
                       ? DefaultStraightLaneChangingAllowed
                       : TernaryBoolUtil.ToBool(StraightLaneChangingAllowed);
        }

        public bool IsEnteringBlockedJunctionAllowed() {
            return EnterWhenBlockedAllowed == TernaryBool.Undefined
                       ? DefaultEnterWhenBlockedAllowed
                       : TernaryBoolUtil.ToBool(EnterWhenBlockedAllowed);
        }

        public bool IsPedestrianCrossingAllowed() {
            return PedestrianCrossingAllowed == TernaryBool.Undefined
                       ? DefaultPedestrianCrossingAllowed
                       : TernaryBoolUtil.ToBool(PedestrianCrossingAllowed);
        }

        public void SetUturnAllowed(bool value) {
            UturnAllowed = TernaryBoolUtil.ToTernaryBool(value);
        }

        public void SetNearTurnOnRedAllowed(bool value) {
            NearTurnOnRedAllowed = TernaryBoolUtil.ToTernaryBool(value);
        }

        public void SetFarTurnOnRedAllowed(bool value) {
            FarTurnOnRedAllowed = TernaryBoolUtil.ToTernaryBool(value);
        }

        public void SetLaneChangingAllowedWhenGoingStraight(bool value) {
            StraightLaneChangingAllowed = TernaryBoolUtil.ToTernaryBool(value);
        }

        public void SetEnteringBlockedJunctionAllowed(bool value) {
            EnterWhenBlockedAllowed = TernaryBoolUtil.ToTernaryBool(value);
        }

        public void SetPedestrianCrossingAllowed(bool value) {
            PedestrianCrossingAllowed = TernaryBoolUtil.ToTernaryBool(value);
        }

        public bool IsDefault() {
            bool uturnIsDefault = UturnAllowed == TernaryBool.Undefined ||
                                  TernaryBoolUtil.ToBool(UturnAllowed) == DefaultUturnAllowed;
            bool nearTurnOnRedIsDefault = NearTurnOnRedAllowed == TernaryBool.Undefined ||
                                          TernaryBoolUtil.ToBool(NearTurnOnRedAllowed) ==
                                          DefaultNearTurnOnRedAllowed;
            bool farTurnOnRedIsDefault = FarTurnOnRedAllowed == TernaryBool.Undefined ||
                                         TernaryBoolUtil.ToBool(FarTurnOnRedAllowed) ==
                                         DefaultFarTurnOnRedAllowed;
            bool straightChangeIsDefault = StraightLaneChangingAllowed == TernaryBool.Undefined ||
                                           TernaryBoolUtil.ToBool(StraightLaneChangingAllowed) ==
                                           DefaultStraightLaneChangingAllowed;
            bool enterWhenBlockedIsDefault = EnterWhenBlockedAllowed == TernaryBool.Undefined ||
                                             TernaryBoolUtil.ToBool(EnterWhenBlockedAllowed) ==
                                             DefaultEnterWhenBlockedAllowed;
            bool pedCrossingIsDefault = PedestrianCrossingAllowed == TernaryBool.Undefined ||
                                        TernaryBoolUtil.ToBool(PedestrianCrossingAllowed) ==
                                        DefaultPedestrianCrossingAllowed;

            return uturnIsDefault && nearTurnOnRedIsDefault && farTurnOnRedIsDefault &&
                   straightChangeIsDefault && enterWhenBlockedIsDefault && pedCrossingIsDefault;
        }

        public void Reset(bool resetDefaults = true) {
            UturnAllowed = TernaryBool.Undefined;
            NearTurnOnRedAllowed = TernaryBool.Undefined;
            FarTurnOnRedAllowed = TernaryBool.Undefined;
            StraightLaneChangingAllowed = TernaryBool.Undefined;
            EnterWhenBlockedAllowed = TernaryBool.Undefined;
            PedestrianCrossingAllowed = TernaryBool.Undefined;

            if (resetDefaults) {
                DefaultUturnAllowed = false;
                DefaultNearTurnOnRedAllowed = false;
                DefaultFarTurnOnRedAllowed = false;
                DefaultStraightLaneChangingAllowed = false;
                DefaultEnterWhenBlockedAllowed = false;
                DefaultPedestrianCrossingAllowed = false;
            }
        }

        public override string ToString() {
            return string.Format(
                "[SegmentEndFlags\n\tuturnAllowed = {0}\n\tnearTurnOnRedAllowed = {1}\n" +
                "\tfarTurnOnRedAllowed = {2}\n\tstraightLaneChangingAllowed = {3}\n\t" +
                "enterWhenBlockedAllowed = {4}\n\tpedestrianCrossingAllowed = {5}\n" +
                "SegmentEndFlags]",
                UturnAllowed,
                NearTurnOnRedAllowed,
                FarTurnOnRedAllowed,
                StraightLaneChangingAllowed,
                EnterWhenBlockedAllowed,
                PedestrianCrossingAllowed);
        }
    }
}