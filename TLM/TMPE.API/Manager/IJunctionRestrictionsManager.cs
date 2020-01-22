namespace TrafficManager.API.Manager {
    using CSUtil.Commons;

    public interface IJunctionRestrictionsManager {
        /// <summary>
        /// Determines if u-turn behavior may be controlled at the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <param name="node">node data.</param>
        /// <returns>Whether u-turns may be customized.</returns>
        bool IsUturnAllowedConfigurable(ushort segmentId, bool startNode, ref NetNode node);

        /// <summary>
        /// Determines if turn-on-red behavior is enabled for near turns and may be controlled at
        ///     the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <param name="node">node data.</param>
        /// <returns>Whether turn-on-red may be customized for near turns.</returns>
        bool IsNearTurnOnRedAllowedConfigurable(ushort segmentId, bool startNode, ref NetNode node);

        /// <summary>
        /// Determines if turn-on-red behavior is enabled for far turns and may be controlled at
        ///     the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <param name="node">node data.</param>
        /// <returns>Whether turn-on-red may be customized for far turns.</returns>
        bool IsFarTurnOnRedAllowedConfigurable(ushort segmentId, bool startNode, ref NetNode node);

        /// <summary>
        /// Determines if turn-on-red behavior is enabled for the given turn type and that it may
        ///     be controlled at the given segment end.
        /// </summary>
        /// <param name="near">Whether this is called for near turns.</param>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <param name="node">node data.</param>
        /// <returns>Whether turn-on-red may be customized.</returns>
        bool IsTurnOnRedAllowedConfigurable(bool near,
                                            ushort segmentId,
                                            bool startNode,
                                            ref NetNode node);

        /// <summary>
        /// Determines if lane changing behavior may be controlled at the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <param name="node">node data.</param>
        /// <returns>Whether lane changing may be customized.</returns>
        bool IsLaneChangingAllowedWhenGoingStraightConfigurable(
            ushort segmentId,
            bool startNode,
            ref NetNode node);

        /// <summary>
        /// Determines if entering blocked junctions may be controlled at the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <param name="node">node data.</param>
        /// <returns>Whether entering blocked junctions may be customized.</returns>
        bool IsEnteringBlockedJunctionAllowedConfigurable(ushort segmentId,
                                                          bool startNode,
                                                          ref NetNode node);

        /// <summary>
        /// Determines if pedestrian crossings may be controlled at the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <param name="node">node data.</param>
        /// <returns>Whether pedestrian crossings may be customized.</returns>
        bool IsPedestrianCrossingAllowedConfigurable(ushort segmentId,
                                                     bool startNode,
                                                     ref NetNode node);

        /// <summary>
        /// Determines the default setting for u-turns at the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <param name="node">node data.</param>
        /// <returns>Whether if u-turns are allowed by default.</returns>
        bool GetDefaultUturnAllowed(ushort segmentId, bool startNode, ref NetNode node);

        /// <summary>
        /// Determines the default setting for near turn-on-red at the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <param name="node">node data.</param>
        /// <returns>Whether turn-on-red is allowed for near turns by default.</returns>
        bool GetDefaultNearTurnOnRedAllowed(ushort segmentId, bool startNode, ref NetNode node);

        /// <summary>
        /// Determines the default setting for far turn-on-red at the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <param name="node">node data.</param>
        /// <returns>Whether turn-on-red is allowed for far turns by default.</returns>
        bool GetDefaultFarTurnOnRedAllowed(ushort segmentId, bool startNode, ref NetNode node);

        /// <summary>
        /// Determines the default turn-on-red setting for the given turn type at the given segment end.
        /// </summary>
        /// <param name="near">Whether called for near turns.</param>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <param name="node">node data.</param>
        /// <returns>Whether turn-on-red is allowed by default.</returns>
        bool GetDefaultTurnOnRedAllowed(bool near,
                                        ushort segmentId,
                                        bool startNode,
                                        ref NetNode node);

        /// <summary>
        /// Determines the default setting for straight lane changes at the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <param name="node">node data.</param>
        /// <returns>Whether straight lane changes are allowed by default.</returns>
        bool GetDefaultLaneChangingAllowedWhenGoingStraight(
            ushort segmentId,
            bool startNode,
            ref NetNode node);

        /// <summary>
        /// Determines the default setting for entering a blocked junction at the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <param name="node">node data.</param>
        /// <returns>Whether entering a blocked junction is allowed by default.</returns>
        bool GetDefaultEnteringBlockedJunctionAllowed(ushort segmentId,
                                                      bool startNode,
                                                      ref NetNode node);

        /// <summary>
        /// Determines the default setting for pedestrian crossings at the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <param name="node">node data.</param>
        /// <returns>Whether crossing the road is allowed by default.</returns>
        bool GetDefaultPedestrianCrossingAllowed(ushort segmentId,
                                                 bool startNode,
                                                 ref NetNode node);

        /// <summary>
        /// Determines whether u-turns are allowed at the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <returns>Whether u-turns are allowed.</returns>
        bool IsUturnAllowed(ushort segmentId, bool startNode);

        /// <summary>
        /// Determines whether turn-on-red is allowed for near turns at the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <returns>Whether turn-on-red is allowed for near turns.</returns>
        bool IsNearTurnOnRedAllowed(ushort segmentId, bool startNode);

        /// <summary>
        /// Determines whether turn-on-red is allowed for far turns at the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <returns>Whether turn-on-red is allowed for far turns.</returns>
        bool IsFarTurnOnRedAllowed(ushort segmentId, bool startNode);

        /// <summary>
        /// Determines whether turn-on-red is allowed for the given turn type at the given segment end.
        /// </summary>
        /// <param name="near">Whether called for near turns.</param>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <returns>Whether turn-on-red is allowed.</returns>
        bool IsTurnOnRedAllowed(bool near, ushort segmentId, bool startNode);

        /// <summary>
        /// Determines whether lane changing when going straight is allowed at the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <returns>Whether lane changing when going straight is allowed.</returns>
        bool IsLaneChangingAllowedWhenGoingStraight(ushort segmentId, bool startNode);

        /// <summary>
        /// Determines whether entering a blocked junction is allowed at the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <returns>Whether entering a blocked junction is allowed.</returns>
        bool IsEnteringBlockedJunctionAllowed(ushort segmentId, bool startNode);

        /// <summary>
        /// Determines whether crossing the road is allowed at the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <returns>Whether crossing the road is allowed.</returns>
        bool IsPedestrianCrossingAllowed(ushort segmentId, bool startNode);

        /// <summary>
        /// Retrieves the u-turn setting for the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <returns>ternary u-turn flag.</returns>
        TernaryBool GetUturnAllowed(ushort segmentId, bool startNode);

        /// <summary>
        /// Retrieves the turn-on-red setting for near turns and the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <returns>ternary turn-on-red flag for near turns.</returns>
        TernaryBool GetNearTurnOnRedAllowed(ushort segmentId, bool startNode);

        /// <summary>
        /// Retrieves the turn-on-red setting for far turns and the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <returns>ternary turn-on-red flag for far turns.</returns>
        TernaryBool GetFarTurnOnRedAllowed(ushort segmentId, bool startNode);

        /// <summary>
        /// Retrieves the turn-on-red setting for the given turn type and segment end.
        /// </summary>
        /// <param name="near">Whether is called for near turns.</param>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <returns>ternary turn-on-red flag.</returns>
        TernaryBool GetTurnOnRedAllowed(bool near, ushort segmentId, bool startNode);

        /// <summary>
        /// Retrieves the lane changing setting for the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <returns>ternary lane changing flag.</returns>
        TernaryBool GetLaneChangingAllowedWhenGoingStraight(ushort segmentId, bool startNode);

        /// <summary>
        /// Retrieves the "enter blocked junction" setting for the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <returns>ternary "enter blocked junction" flag.</returns>
        TernaryBool GetEnteringBlockedJunctionAllowed(ushort segmentId, bool startNode);

        /// <summary>
        /// Retrieves the pedestrian crossing setting for the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <returns>ternary pedestrian crossing flag.</returns>
        TernaryBool GetPedestrianCrossingAllowed(ushort segmentId, bool startNode);

        /// <summary>
        /// Switches the u-turn flag for the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <returns>Success flag.</returns>
        bool ToggleUturnAllowed(ushort segmentId, bool startNode);

        /// <summary>
        /// Switches the turn-on-red flag for near turns and given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <returns>Success flag.</returns>
        bool ToggleNearTurnOnRedAllowed(ushort segmentId, bool startNode);

        /// <summary>
        /// Switches the turn-on-red flag for far turns and given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <returns>Success flag.</returns>
        bool ToggleFarTurnOnRedAllowed(ushort segmentId, bool startNode);

        /// <summary>
        /// Switches the turn-on-red flag for the given turn type and segment end.
        /// </summary>
        /// <param name="near">Whether is called for near turns.</param>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <returns>Success flag.</returns>
        bool ToggleTurnOnRedAllowed(bool near, ushort segmentId, bool startNode);

        /// <summary>
        /// Switches the lane changing flag for the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <returns>Success flag.</returns>
        bool ToggleLaneChangingAllowedWhenGoingStraight(ushort segmentId, bool startNode);

        /// <summary>
        /// Switches the "enter blocked junction" flag for the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <returns>Success flag.</returns>
        bool ToggleEnteringBlockedJunctionAllowed(ushort segmentId, bool startNode);

        /// <summary>
        /// Switches the pedestrian crossing flag for the given segment end.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <returns>Success flag.</returns>
        bool TogglePedestrianCrossingAllowed(ushort segmentId, bool startNode);

        /// <summary>
        /// Sets the u-turn flag for the given segment end to the given value.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <param name="value">new value.</param>
        /// <returns>Success flag.</returns>
        bool SetUturnAllowed(ushort segmentId, bool startNode, bool value);

        /// <summary>
        /// Sets the turn-on-red flag for near turns at the given segment end to the given value.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <param name="value">new value.</param>
        /// <returns>Success flag.</returns>
        bool SetNearTurnOnRedAllowed(ushort segmentId, bool startNode, bool value);

        /// <summary>
        /// Sets the turn-on-red flag for far turns at the given segment end to the given value.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <param name="value">new value.</param>
        /// <returns>Success flag.</returns>
        bool SetFarTurnOnRedAllowed(ushort segmentId, bool startNode, bool value);

        /// <summary>
        /// Sets the turn-on-red flag for the given turn type and segment end to the given value.
        /// </summary>
        /// <param name="near">Whether is called for near turns.</param>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <param name="value">new value.</param>
        /// <returns>Success flag.</returns>
        bool SetTurnOnRedAllowed(bool near, ushort segmentId, bool startNode, bool value);

        /// <summary>
        /// Sets the lane changing flag for the given segment end to the given value.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <param name="value">new value.</param>
        /// <returns>Success flag.</returns>
        bool SetLaneChangingAllowedWhenGoingStraight(ushort segmentId, bool startNode, bool value);

        /// <summary>
        /// Sets the "enter blocked junction" flag for the given segment end to the given value.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <param name="value">new value.</param>
        /// <returns>Success flag.</returns>
        bool SetEnteringBlockedJunctionAllowed(ushort segmentId, bool startNode, bool value);

        /// <summary>
        /// Sets the pedestrian crossing flag for the given segment end to the given value.
        /// </summary>
        /// <param name="segmentId">segment id.</param>
        /// <param name="startNode">Whether is at starting node.</param>
        /// <param name="value">new value.</param>
        /// <returns>Success flag.</returns>
        bool SetPedestrianCrossingAllowed(ushort segmentId, bool startNode, bool value);

        /// <summary>
        /// Updates the default values for all junction restrictions and segments.
        /// </summary>
        void UpdateAllDefaults();
    }
}