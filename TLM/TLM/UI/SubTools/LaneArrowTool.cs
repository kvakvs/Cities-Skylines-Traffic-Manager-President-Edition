﻿using ColossalFramework;
using ColossalFramework.Math;
using ColossalFramework.UI;
using CSUtil.Commons;
using GenericGameBridge.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TrafficManager.Custom.AI;
using TrafficManager.Geometry;
using TrafficManager.Geometry.Impl;
using TrafficManager.Manager;
using TrafficManager.Manager.Impl;
using TrafficManager.State;
using TrafficManager.TrafficLight;
using TrafficManager.UI.CanvasGUI;
using TrafficManager.Util;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TrafficManager.UI.SubTools {
	public class LaneArrowTool : SubTool {
		enum LaneButtonState {
			On,
			Off,
			Disabled
		}

		private static WorldSpaceGUI wsGui = null;
		
		public LaneArrowTool(TrafficManagerTool mainTool)
			: base(mainTool) {
		}

		public override bool IsCursorInPanel() {
			return base.IsCursorInPanel() || wsGui?.RaycastMouse().Count > 0;
		}

		public override void OnPrimaryClickOverlay() {
			if (HoveredNodeId == 0 || HoveredSegmentId == 0) return;

			var netFlags = Singleton<NetManager>.instance.m_nodes.m_buffer[HoveredNodeId].m_flags;

			if ((netFlags & NetNode.Flags.Junction) == NetNode.Flags.None) return;

			if (Singleton<NetManager>.instance.m_segments.m_buffer[HoveredSegmentId].m_startNode != HoveredNodeId &&
				Singleton<NetManager>.instance.m_segments.m_buffer[HoveredSegmentId].m_endNode != HoveredNodeId)
				return;

			Deselect();
			SelectedSegmentId = HoveredSegmentId;
			SelectedNodeId = HoveredNodeId;
		}

		public override void OnSecondaryClickOverlay() {
			if (!IsCursorInPanel()) {
				Deselect();
			}
		}

		public override void OnToolGUI(Event e) {
			if (SelectedNodeId == 0 || SelectedSegmentId == 0) return;

			int numDirections;
			int numLanes = TrafficManagerTool.GetSegmentNumVehicleLanes(SelectedSegmentId, SelectedNodeId, out numDirections, LaneArrowManager.VEHICLE_TYPES);
			if (numLanes <= 0) {
				Deselect();
				return;
			}

			Vector3 nodePos = Singleton<NetManager>.instance.m_nodes.m_buffer[SelectedNodeId].m_position;

			Vector3 screenPos;
			bool visible = MainTool.WorldToScreenPoint(nodePos, out screenPos);

			if (!visible)
				return;

			var camPos = Singleton<SimulationManager>.instance.m_simulationView.m_position;
			var diff = nodePos - camPos;

			if (diff.magnitude > TrafficManagerTool.MaxOverlayDistance)
				return; // do not draw if too distant

			// Try click something on Canvas
			wsGui?.HandleInput();
		}

		private void Deselect() {
			SelectedSegmentId = 0;
			SelectedNodeId = 0;
			if (wsGui != null) {
				wsGui.DestroyCanvas();
				wsGui = null;
			}
		}

		public override void RenderOverlay(RenderManager.CameraInfo cameraInfo) {
			NetManager netManager = Singleton<NetManager>.instance;
			//Log._Debug($"LaneArrow Overlay: {HoveredNodeId} {HoveredSegmentId} {SelectedNodeId} {SelectedSegmentId}");
			bool cursorInSecondaryPanel = wsGui?.RaycastMouse().Count > 0;
			if (!cursorInSecondaryPanel && HoveredSegmentId != 0 && HoveredNodeId != 0 && (HoveredSegmentId != SelectedSegmentId || HoveredNodeId != SelectedNodeId)) {
				var nodeFlags = netManager.m_nodes.m_buffer[HoveredNodeId].m_flags;
				
				if ((netManager.m_segments.m_buffer[HoveredSegmentId].m_startNode == HoveredNodeId || netManager.m_segments.m_buffer[HoveredSegmentId].m_endNode == HoveredNodeId) && (nodeFlags & NetNode.Flags.Junction) != NetNode.Flags.None) {
					NetTool.RenderOverlay(cameraInfo, ref Singleton<NetManager>.instance.m_segments.m_buffer[HoveredSegmentId], MainTool.GetToolColor(false, false),
						MainTool.GetToolColor(false, false));
				}
			}

			if (SelectedSegmentId == 0) return;

			var netSegment = Singleton<NetManager>.instance.m_segments.m_buffer[SelectedSegmentId];
			NetTool.RenderOverlay(cameraInfo, ref netSegment, MainTool.GetToolColor(true, false), MainTool.GetToolColor(true, false));

			// Create UI on the ground
			if (wsGui == null) {
				CreateWorldSpaceGUI(netSegment);
			}
		}

		private const float LANE_GROUP_WIDTH = 11f;
		private const float LANE_BUTTON_GAP = 0.7f;
		private const float LANE_GROUP_GAP = LANE_BUTTON_GAP * 2f;
		private const float LANE_GROUP_HALFSIZE = (LANE_GROUP_WIDTH - LANE_BUTTON_GAP) / 2f;

		private void CreateWorldSpaceGUI(NetSegment netSegment) {
			var nodesBuffer = Singleton<NetManager>.instance.m_nodes.m_buffer;
			// var info = Singleton<NetManager>.instance.m_segments.m_buffer[SelectedSegmentId].Info;
			var segmentsBuffer = Singleton<NetManager>.instance.m_segments.m_buffer;
			var lanesBuffer = Singleton<NetManager>.instance.m_lanes.m_buffer;

			var laneList = Constants.ServiceFactory.NetService.GetSortedLanes(
				SelectedSegmentId,
				ref segmentsBuffer[SelectedSegmentId],
				segmentsBuffer[SelectedSegmentId].m_startNode == SelectedNodeId,
				LaneArrowManager.LANE_TYPES,
				LaneArrowManager.VEHICLE_TYPES,
				true);

			// Create the GUI form and center it according to the lane count

			// Forward is the direction of the selected segment, even if it's a curve
			var forwardVector = GetSegmentTangent(SelectedNodeId, netSegment);
			var rot = Quaternion.LookRotation(Vector3.down, forwardVector.normalized);

			// UI is floating 5 metres above the ground
			const float UI_FLOAT_HEIGHT = 5f;
			var adjustFloat = Vector3.up * UI_FLOAT_HEIGHT;

			// Adjust UI to the left
			wsGui = new WorldSpaceGUI(nodesBuffer[SelectedNodeId].m_position + adjustFloat, rot);

			// -------------------------------
			// Now iterate over eligible lanes
			// -------------------------------
			var geometry = SegmentGeometry.Get(SelectedSegmentId);
			if (geometry == null) {
				Log.Error($"LaneArrowTool._guiLaneChangeWindow: No geometry information " +
				          $"available for segment {SelectedSegmentId}");
				return;
			}

			var startNode = geometry.StartNodeId() == SelectedNodeId;
			var expectedGuiWidth = (laneList.Count * LANE_GROUP_WIDTH) +
			                       ((laneList.Count - 1) * LANE_GROUP_GAP);
			var offset = -expectedGuiWidth / 2f;

			for (var i = 0; i < laneList.Count; i++) {
				var laneId = laneList[i].laneId;
				var lane = lanesBuffer[laneId];
				var flags = (NetLane.Flags)lane.m_flags;

				if (!Flags.applyLaneArrowFlags(laneList[i].laneId)) {
					Flags.removeLaneArrowFlags(laneList[i].laneId);
				}

				// bool buttonClicked = false;

				// TODO: Here apply LaneButtonState.Disabled if the lane cannot turn there
				var forward = (flags & NetLane.Flags.Forward) != 0 ? LaneButtonState.On : LaneButtonState.Off;
				var left = (flags & NetLane.Flags.Left) != 0 ? LaneButtonState.On : LaneButtonState.Off;
				var right = (flags & NetLane.Flags.Right) != 0 ? LaneButtonState.On : LaneButtonState.Off;

				var bForward = GuiAddLaneControlForward(offset, forward);
				UnityAction clickForward = () => {
					OnClickForward(SelectedSegmentId, SelectedNodeId, laneId, startNode, bForward);
				};
				bForward.GetComponent<Button>().onClick.AddListener(clickForward);

				var bLeft = GuiAddLaneControlLeft(offset, left);
				UnityAction clickLeft = () => {
					OnClickLeft(SelectedSegmentId, SelectedNodeId, laneId, startNode, bLeft);
				};
				bLeft.GetComponent<Button>().onClick.AddListener(clickLeft);

				var bRight = GuiAddLaneControlRight(offset, right);
				UnityAction clickRight = () => {
					OnClickRight(SelectedSegmentId, SelectedNodeId, laneId, startNode, bRight);
				};
				bRight.GetComponent<Button>().onClick.AddListener(clickRight);

				offset += LANE_GROUP_WIDTH + LANE_GROUP_GAP;

				//	if (buttonClicked) {
				//		switch (res) {
				//			case Flags.LaneArrowChangeResult.Invalid:
				//			case Flags.LaneArrowChangeResult.Success:
				//			default:
				//				break;
				//			case Flags.LaneArrowChangeResult.HighwayArrows:
				//			MainTool.ShowTooltip(Translation.GetString("Lane_Arrow_Changer_Disabled_Highway"));
				//			break;
				//			case Flags.LaneArrowChangeResult.LaneConnection:
				//			MainTool.ShowTooltip(Translation.GetString("Lane_Arrow_Changer_Disabled_Connection"));
				//			break;
				//		}
				//	}
			}

		}

		private void UpdateButtonGraphics(ushort segmentId, ushort nodeId, uint laneId, NetLane.Flags direction, GameObject button) {
			var nodesBuffer = Singleton<NetManager>.instance.m_nodes.m_buffer;
			// var info = Singleton<NetManager>.instance.m_segments.m_buffer[SelectedSegmentId].Info;
			var segmentsBuffer = Singleton<NetManager>.instance.m_segments.m_buffer;
			var lanesBuffer = Singleton<NetManager>.instance.m_lanes.m_buffer;

			var laneList = Constants.ServiceFactory.NetService.GetSortedLanes(
				segmentId,
				ref segmentsBuffer[segmentId],
				segmentsBuffer[segmentId].m_startNode == nodeId,
				LaneArrowManager.LANE_TYPES,
				LaneArrowManager.VEHICLE_TYPES,
				true);

			var lane = lanesBuffer[laneId];
			var flags = (NetLane.Flags)lane.m_flags;
			var buttonState = (flags & direction) != 0 ? LaneButtonState.On : LaneButtonState.Off;
			wsGui.SetButtonImage(button, SelectButtonGraphics(direction, buttonState));
		}

		private void OnClickForward(ushort segmentId, ushort nodeId, uint laneId, bool startNode, GameObject button) {
			var res = Flags.LaneArrowChangeResult.Invalid;
			LaneArrowManager.Instance.ToggleLaneArrows(
				laneId,
				startNode,
				Flags.LaneArrows.Forward,
				out res);
			if (res == Flags.LaneArrowChangeResult.Invalid ||
			    res == Flags.LaneArrowChangeResult.Success) {
				UpdateButtonGraphics(segmentId, nodeId, laneId, NetLane.Flags.Forward, button);
			}
		}

		private void OnClickLeft(ushort segmentId, ushort nodeId, uint laneId, bool startNode, GameObject button) {
			var res = Flags.LaneArrowChangeResult.Invalid;
			LaneArrowManager.Instance.ToggleLaneArrows(
				laneId,
				startNode,
				Flags.LaneArrows.Left,
				out res);
			if (res == Flags.LaneArrowChangeResult.Invalid ||
			    res == Flags.LaneArrowChangeResult.Success) {
				UpdateButtonGraphics(segmentId, nodeId, laneId, NetLane.Flags.Left, button);
			}
		}

		private void OnClickRight(ushort segmentId, ushort nodeId, uint laneId, bool startNode, GameObject button) {
			var res = Flags.LaneArrowChangeResult.Invalid;
			LaneArrowManager.Instance.ToggleLaneArrows(
				laneId,
				startNode,
				Flags.LaneArrows.Right,
				out res);
			if (res == Flags.LaneArrowChangeResult.Invalid ||
			    res == Flags.LaneArrowChangeResult.Success) {
				UpdateButtonGraphics(segmentId, nodeId, laneId, NetLane.Flags.Right, button);
			}
		}

		private Texture2D SelectLaneButton(LaneButtonState state,
		                                   Texture2D on,
		                                   Texture2D off,
		                                   Texture2D disabled) {
			switch (state) {
				case LaneButtonState.On:
					return on;
				case LaneButtonState.Off:
					return off;
				case LaneButtonState.Disabled:
				default:
					return disabled;
			}
		}

		/// <summary>
		/// Based on NetLane Flags direction and button state (on, off, disabled),
		/// return the texture to display on button.
		/// </summary>
		/// <param name="direction">Left, Right, Forward, no bit combinations</param>
		/// <param name="state">Button state (on, off, disabled)</param>
		/// <returns>The texture</returns>
		private Texture2D SelectButtonGraphics(NetLane.Flags direction, LaneButtonState state) {
			switch (direction) {
				case NetLane.Flags.Forward:
					return SelectLaneButton(state,
					                 TextureResources.LaneArrowButtonForward,
					                 TextureResources.LaneArrowButtonForwardOff,
					                 TextureResources.LaneArrowButtonForwardDisabled);
				case NetLane.Flags.Left:
					return SelectLaneButton(state,
					                        TextureResources.LaneArrowButtonLeft,
					                        TextureResources.LaneArrowButtonLeftOff,
					                        TextureResources.LaneArrowButtonLeftDisabled);
				case NetLane.Flags.Right:
					return SelectLaneButton(state,
					                        TextureResources.LaneArrowButtonRight,
					                        TextureResources.LaneArrowButtonRightOff,
					                        TextureResources.LaneArrowButtonRightDisabled);
				default:
					Log.Error($"Trying to find texture for lane state {direction.ToString()}");
					return null;
			}
		}

		/// <summary>
		/// Creates Keep Going Straight lane control button.
		/// </summary>
		/// <param name="offset">Offset from the form left edge</param>
		/// <param name="left">The state of the button (on, off, disabled)</param>
		/// <returns>The button GameObj</returns>
		private GameObject GuiAddLaneControlForward(float offset,
		                                            LaneButtonState forward) {
			// Create wide button, forward lane
			var bForward = wsGui.AddButton(new Vector3(offset, 0f, 0f),
			                               new Vector2(LANE_GROUP_WIDTH, LANE_GROUP_HALFSIZE));
			wsGui.SetButtonImage(bForward, SelectButtonGraphics(NetLane.Flags.Forward, forward));
			return bForward;
		}

		/// <summary>
		/// Creates Turn Left lane control button.
		/// </summary>
		/// <param name="offset">Offset from the form left edge</param>
		/// <param name="left">The state of the button (on, off, disabled)</param>
		/// <returns>The button GameObj</returns>
		private GameObject GuiAddLaneControlLeft(float offset,
		                                         LaneButtonState left) {
			// Create left lane button under it
			var bLeft = wsGui.AddButton(new Vector3(offset, LANE_GROUP_HALFSIZE + LANE_BUTTON_GAP, 0f),
			                            new Vector2(LANE_GROUP_HALFSIZE,
			                                        LANE_GROUP_WIDTH - LANE_BUTTON_GAP));
			wsGui.SetButtonImage(bLeft, SelectButtonGraphics(NetLane.Flags.Left, left));
			return bLeft;
		}

		/// <summary>
		/// Creates Turn Right lane control button.
		/// </summary>
		/// <param name="offset">Offset from the form left edge</param>
		/// <param name="left">The state of the button (on, off, disabled)</param>
		/// <returns>The button GameObj</returns>
		private GameObject GuiAddLaneControlRight(float offset,
		                                          LaneButtonState right) {
			// Create right lane button to the right of it
			var bRight = wsGui.AddButton(new Vector3(offset + LANE_GROUP_HALFSIZE + LANE_BUTTON_GAP,
			                                         LANE_GROUP_HALFSIZE + LANE_BUTTON_GAP, 0f),
			                             new Vector2(LANE_GROUP_HALFSIZE, LANE_GROUP_WIDTH - LANE_BUTTON_GAP));
			wsGui.SetButtonImage(bRight, SelectButtonGraphics(NetLane.Flags.Right, right));
			return bRight;
		}

		/// <summary>
		/// For given segment and one of its end nodes, get the direction vector.
		/// </summary>
		/// <returns>Direction of the given end of the segment.</returns>
		private static Vector3 GetSegmentTangent(ushort nodeId, NetSegment segment) {
			var nodesBuffer = Singleton<NetManager>.instance.m_nodes.m_buffer;
			var otherNodeId = segment.GetOtherNode(nodeId);
			var nodePos = nodesBuffer[nodeId].m_position;
			var otherNodePos = nodesBuffer[otherNodeId].m_position;

			if (segment.IsStraight()) {
				return (nodePos - otherNodePos).normalized;
			}

			// Handle some curvature, take the last tangent
			var bezier = default(Bezier3);
			bezier.a = nodesBuffer[segment.m_startNode].m_position;
			bezier.d = nodesBuffer[segment.m_endNode].m_position;
			NetSegment.CalculateMiddlePoints(bezier.a, segment.m_startDirection,
			                                 bezier.d, segment.m_endDirection,
			                                 false, false,
			                                 out bezier.b, out bezier.c);
			var isStartNode = nodeId == segment.m_startNode;
			var tangent = bezier.Tangent(isStartNode ? 0f : 1f);
			return tangent;
		}
	}
}
