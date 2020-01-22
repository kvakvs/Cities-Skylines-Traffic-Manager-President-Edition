﻿namespace TrafficManager.API.Manager {
    using System;
    using TrafficManager.API.Traffic.Data;
    using UnityEngine;

    public interface IExtCitizenInstanceManager {
        // TODO define me!
        ExtCitizenInstance[] ExtInstances { get; }

        void ResetInstance(ushort instanceId);

        /// <summary>
        /// Handles a released citizen instance.
        /// </summary>
        /// <param name="instanceId">citizen instance id.</param>
        void OnReleaseInstance(ushort instanceId);

        /// <summary>
        /// Starts path-finding for the given citizen instance.
        /// </summary>
        /// <param name="instanceId">citizen instance id.</param>
        /// <param name="instanceData">citizen instance data.</param>
        /// <param name="extInstance">extended citizen instance data.</param>
        /// <param name="extCitizen">extended citizen data.</param>
        /// <param name="startPos">start position.</param>
        /// <param name="endPos">end position.</param>
        /// <param name="vehicleInfo">vehicle info.</param>
        /// <param name="enableTransport">allow public transport.</param>
        /// <param name="ignoreCost">ignore path-finding costs.</param>
        /// <returns>Success flag.</returns>
        bool StartPathFind(ushort instanceId,
                           ref CitizenInstance instanceData,
                           ref ExtCitizenInstance extInstance,
                           ref ExtCitizen extCitizen,
                           Vector3 startPos,
                           Vector3 endPos,
                           VehicleInfo vehicleInfo,
                           bool enableTransport,
                           bool ignoreCost);

        /// <summary>
        /// Finds a suitable end path position for citizen instances.
        /// </summary>
        /// <param name="instanceId">citizen instance id.</param>
        /// <param name="instanceData">citizen instance data.</param>
        /// <param name="pos">world position.</param>
        /// <param name="laneTypes">allowed lane types.</param>
        /// <param name="vehicleTypes">allowed vehicle types.</param>
        /// <param name="allowUnderground">allow position to be underground.</param>
        /// <param name="position">result receiver.</param>
        /// <returns>Success flag.</returns>
        bool FindPathPosition(ushort instanceId,
                              ref CitizenInstance instanceData,
                              Vector3 pos,
                              NetInfo.LaneType laneTypes,
                              VehicleInfo.VehicleType vehicleTypes,
                              bool allowUnderground,
                              out PathUnit.Position position);

        /// <summary>
        /// Generates the localized status for the given tourist.
        /// </summary>
        /// <param name="instanceId">citizen instance id.</param>
        /// <param name="data">citizen instance data.</param>
        /// <param name="mayAddCustomStatus">specifies whether the status may be customized.</param>
        /// <param name="target">instance id.</param>
        /// <returns>localized status.</returns>
        string GetTouristLocalizedStatus(ushort instanceId,
                                         ref CitizenInstance data,
                                         out bool mayAddCustomStatus,
                                         out InstanceID target);

        /// <summary>
        /// Generates the localized status for the given resident.
        /// </summary>
        /// <param name="instanceId">citizen instance id.</param>
        /// <param name="data">citizen instance data.</param>
        /// <param name="mayAddCustomStatus">specifies whether the status may be customized.</param>
        /// <param name="target">instance id.</param>
        /// <returns>localized status.</returns>
        string GetResidentLocalizedStatus(ushort instanceId,
                                          ref CitizenInstance data,
                                          out bool mayAddCustomStatus,
                                          out InstanceID target);

        /// <summary>
        /// Determines whether the given citizen instance is located at an outside connection based
        ///     on the given start position.
        /// </summary>
        /// <param name="instanceId">citizen instance id.</param>
        /// <param name="instanceData">citizen instance data.</param>
        /// <param name="extInstance">extended citizen instance data.</param>
        /// <param name="startPos">start position.</param>
        /// <returns>Whether the citizen instance is located at an outside connection.</returns>
        bool IsAtOutsideConnection(ushort instanceId,
                                   ref CitizenInstance instanceData,
                                   ref ExtCitizenInstance extInstance,
                                   Vector3 startPos);

        /// <summary>
        /// Checks whether the citizen instance with the given id is valid.
        /// </summary>
        /// <param name="instanceId">citzen instance id.</param>
        /// <returns>Whether it is valid.</returns>
        bool IsValid(ushort instanceId);

        /// <summary>Determines the citizen id for the given citizen instance id.</summary>
        /// <param name="instanceId">citizen instance id.</param>
        /// <returns>citizen id.</returns>
        uint GetCitizenId(ushort instanceId);

        /// <summary>Releases the return path (if present) for the given citizen instance.</summary>
        /// <param name="extInstance">ext. citizen instance.</param>
        void ReleaseReturnPath(ref ExtCitizenInstance extInstance);

        /// <summary>
        /// Checks the calculation state of the return path for the given citizen instance.
        /// Updates the citizen instance accordingly.
        /// </summary>
        /// <param name="extInstance">ext. citizen instance.</param>
        void UpdateReturnPathState(ref ExtCitizenInstance extInstance);

        /// <summary>
        /// Starts path-finding for the walking path from parking position at
        ///     <paramref name="parkPos"/> to target position at <paramref name="targetPos"/> for
        ///     the given citizen instance.
        /// </summary>
        /// <param name="extInstance">ext. citizen instance.</param>
        /// <param name="parkPos">parking position.</param>
        /// <param name="targetPos">target position.</param>
        /// <returns>Whether path-finding could be started.</returns>
        bool CalculateReturnPath(ref ExtCitizenInstance extInstance,
                                 Vector3 parkPos,
                                 Vector3 targetPos);

        /// <summary>Resets the given cititzen instance.</summary>
        /// <param name="extInstance">ext. citizen instance.</param>
        void Reset(ref ExtCitizenInstance extInstance);
    }
}