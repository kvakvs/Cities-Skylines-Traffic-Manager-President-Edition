namespace TrafficManager.UI.SubTools.SpeedLimits {
    using System.Collections.Generic;
    using API.Traffic.Data;
    using State;
    using UnityEngine;
    using Util;

    /// <summary>
    /// Defines styles available for road signs
    /// </summary>
    public enum MphSignStyle {
        SquareUS = 0,
        RoundUK = 1,
        RoundGerman = 2,
    }

    public enum SpeedUnit {
        CurrentlyConfigured, // Currently selected in the options menu
        Kmph,
        Mph
    }

    /// <summary>
    /// Contains constants and implements algorithms for building the speed limit palette and
    /// selecting speeds from the palette.
    /// </summary>
    public class SpeedLimit {
        public const int
            BREAK_PALETTE_COLUMN_KMPH = 8; // palette shows N in a row, then break and another row

        public const int
            BREAK_PALETTE_COLUMN_MPH = 10; // palette shows M in a row, then break and another row

        private const ushort LOWER_KMPH = 10;
        public const ushort UPPER_KMPH = 140;
        private const ushort KMPH_STEP = 10;

        private const ushort LOWER_MPH = 5;
        public const ushort UPPER_MPH = 90;
        private const ushort MPH_STEP = 5;

        /// <summary>
        /// Produces list of speed limits to offer user in the palette
        /// </summary>
        /// <param name="unit">What kind of speed limit list is required</param>
        /// <returns>List from smallest to largest speed with the given unit. Zero (no limit) is not added to the list.
        /// The values are in-game speeds as float.</returns>
        public static List<float> EnumerateSpeedLimits(SpeedUnit unit) {
            var result = new List<float>();
            switch (unit) {
                case SpeedUnit.Kmph:
                    for (var km = LOWER_KMPH; km <= UPPER_KMPH; km += KMPH_STEP) {
                        result.Add(km / Constants.SPEED_TO_KMPH);
                    }

                    break;
                case SpeedUnit.Mph:
                    for (var mi = LOWER_MPH; mi <= UPPER_MPH; mi += MPH_STEP) {
                        result.Add(mi / Constants.SPEED_TO_MPH);
                    }

                    break;
                case SpeedUnit.CurrentlyConfigured:
                    // Automatically choose from the config
                    return GlobalConfig.Instance.Main.DisplaySpeedLimitsMph
                               ? EnumerateSpeedLimits(SpeedUnit.Mph)
                               : EnumerateSpeedLimits(SpeedUnit.Kmph);
            }

            return result;
        }

        public static string ToMphPreciseString(SpeedValue speed) {
            if (FloatUtil.IsZero(speed.GameUnits)) {
                return Translation.GetString("Speed_limit_unlimited");
            }

            return speed.ToMphPrecise() + " MPH";
        }

        public static string ToKmphPreciseString(SpeedValue speed) {
            if (FloatUtil.IsZero(speed.GameUnits)) {
                return Translation.GetString("Speed_limit_unlimited");
            }

            return speed.ToKmphPrecise() + " km/h";
        }

        /// <summary>
        /// Based on the MPH/KMPH settings round the current speed to the nearest STEP and
        /// then decrease by STEP.
        /// </summary>
        /// <param name="speed">Ingame speed</param>
        /// <returns>Ingame speed decreased by the increment for MPH or KMPH</returns>
        public static SpeedValue GetPrevious(SpeedValue speed) {
            if (speed.GameUnits < 0f) {
                return new SpeedValue(-1f);
            }

            if (GlobalConfig.Instance.Main.DisplaySpeedLimitsMph) {
                MphValue rounded = speed.ToMphRounded(MPH_STEP);
                if (rounded.Mph == LOWER_MPH) {
                    return new SpeedValue(0);
                }

                if (rounded.Mph == 0) {
                    return SpeedValue.FromMph(UPPER_MPH);
                }

                return SpeedValue.FromMph(rounded.Mph > LOWER_MPH
                                              ? (ushort)(rounded.Mph - MPH_STEP)
                                              : LOWER_MPH);
            } else {
                KmphValue rounded = speed.ToKmphRounded(KMPH_STEP);
                if (rounded.Kmph == LOWER_KMPH) {
                    return new SpeedValue(0);
                }

                if (rounded.Kmph == 0) {
                    return SpeedValue.FromKmph(UPPER_KMPH);
                }

                return SpeedValue.FromKmph(rounded.Kmph > LOWER_KMPH
                                               ? (ushort)(rounded.Kmph - KMPH_STEP)
                                               : LOWER_KMPH);
            }
        }

        /// <summary>
        /// Based on the MPH/KMPH settings round the current speed to the nearest STEP and
        /// then increase by STEP.
        /// </summary>
        /// <param name="speed">Ingame speed</param>
        /// <returns>Ingame speed increased by the increment for MPH or KMPH</returns>
        public static SpeedValue GetNext(SpeedValue speed) {
            if (speed.GameUnits < 0f) {
                return new SpeedValue(-1f);
            }

            if (GlobalConfig.Instance.Main.DisplaySpeedLimitsMph) {
                MphValue rounded = speed.ToMphRounded(MPH_STEP);
                rounded.Mph += MPH_STEP;

                if (rounded.Mph > UPPER_MPH) {
                    rounded.Mph = 0;
                }

                return SpeedValue.FromMph(rounded);
            } else {
                KmphValue rounded = speed.ToKmphRounded(KMPH_STEP);
                rounded.Kmph += KMPH_STEP;

                if (rounded.Kmph > UPPER_KMPH) {
                    rounded.Kmph = 0;
                }

                return SpeedValue.FromKmph(rounded);
            }
        }

        /// <summary>
        /// For US signs and MPH enabled, scale textures vertically by 1.25f.
        /// Other signs are round.
        /// </summary>
        /// <returns>Multiplier for horizontal sign size</returns>
        public static float GetVerticalTextureScale() {
            return (GlobalConfig.Instance.Main.DisplaySpeedLimitsMph &&
                    GlobalConfig.Instance.Main.MphRoadSignStyle == MphSignStyle.SquareUS)
                       ? 1.25f
                       : 1.0f;
        }
    }
}