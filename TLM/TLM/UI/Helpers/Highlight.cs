﻿namespace TrafficManager.UI {
    using ColossalFramework;
    using ColossalFramework.Math;
    using TrafficManager.UI.SubTools.TimedTrafficLights;
    using TrafficManager.Util;
    using UnityEngine;

    /// <summary>
    /// Provides static functions for drawing overlay textures.
    /// Must be called from GUI callbacks only, will not work from other code.
    /// </summary>
    public static class Highlight {
        /// <summary>
        /// Create this to describe a grid for rendering multiple icons.
        /// Icons are positioned in the XZ plane in the world around the GridOrigin, but rendered
        /// normally in screen space with their sides axis aligned.
        /// </summary>
        public class Grid {
            /// <summary>Grid starts here.</summary>
            public Vector3 GridOrigin;

            /// <summary>Grid cell width.</summary>
            public float CellWidth;

            /// <summary>Grid cell height.</summary>
            public float CellHeight;

            /// <summary>Grid basis vector for X axis.</summary>
            public Vector3 Xu;

            /// <summary>Grid basis vector for Y axis.</summary>
            public Vector3 Yu;

            public Grid(Vector3 gridOrigin,
                        float cellWidth,
                        float cellHeight,
                        Vector3 xu,
                        Vector3 yu) {
                GridOrigin = gridOrigin;
                CellWidth = cellWidth;
                CellHeight = cellHeight;
                Xu = xu;
                Yu = yu;
            }

            /// <summary>Grid position in game coordinates for row and column.</summary>
            /// <param name="col">Column.</param>
            /// <param name="row">Row.</param>
            /// <returns>World position.</returns>
            public Vector3 GetPositionForRowCol(float col, float row) {
                return this.GridOrigin
                       + (this.CellWidth * col * this.Xu)
                       + (this.CellHeight * row * this.Yu);
            }

            /// <summary>
            /// Position a texture rectangle in a "grid cell" of a regular grid with center in the
            /// GridOrigin, and basis xu,yu. The draw box is not rotated together with the grid basis
            /// and is aligned with screen axes.
            /// </summary>
            /// <param name="texture">Draw this.</param>
            /// <param name="camPos">Visible from here.</param>
            /// <param name="x">Column in grid.</param>
            /// <param name="y">Row in grid.</param>
            /// <param name="size">Square draw size (axis aligned).</param>
            /// <param name="screenRect">Output visible screen rect.</param>
            public void DrawStaticSquareOverlayGridTexture(Texture2D texture,
                                                           Vector3 camPos,
                                                           float x,
                                                           float y,
                                                           float size,
                                                           out Rect screenRect) {
                DrawGenericOverlayGridTexture(
                    texture: texture,
                    camPos: camPos,
                    x: x,
                    y: y,
                    width: size,
                    height: size,
                    canHover: false,
                    screenRect: out screenRect);
            }

            /// <summary>
            /// Position a texture rectangle in a "grid cell" of a regular grid with center in the
            /// GridOrigin, and basis xu,yu. The draw box is not rotated together with the grid basis
            /// and is aligned with screen axes.
            /// </summary>
            /// <param name="texture">Draw this.</param>
            /// <param name="camPos">Visible from here.</param>
            /// <param name="x">X position in grid.</param>
            /// <param name="y">Y position in grid.</param>
            /// <param name="width">Draw box size x.</param>
            /// <param name="height">Draw box size y.</param>
            /// <param name="canHover">Whether the icon is interacting with the mouse.</param>
            /// <param name="screenRect">Output visible screen rect.</param>
            /// <returns>Whether mouse hovers the icon.</returns>
            public bool DrawGenericOverlayGridTexture(Texture2D texture,
                                                      Vector3 camPos,
                                                      float x,
                                                      float y,
                                                      float width,
                                                      float height,
                                                      bool canHover,
                                                      out Rect screenRect) {
                Vector3 worldPos = this.GetPositionForRowCol(x, y);

                return Highlight.DrawGenericOverlayTexture(
                    texture,
                    camPos,
                    worldPos,
                    width,
                    height,
                    canHover,
                    out screenRect);
            }
        }

        public static void DrawNodeCircle(RenderManager.CameraInfo cameraInfo,
                                          ushort nodeId,
                                          bool warning = false,
                                          bool alpha = false) {
            DrawNodeCircle(
                cameraInfo: cameraInfo,
                nodeId: nodeId,
                color: ModUI.GetTrafficManagerTool(createIfRequired: false)
                            .GetToolColor(warning: warning, error: false),
                alpha: alpha);
            // TODO: Potentially we do not need to refer to a TrafficManagerTool object
        }

        /// <summary>
        /// Gets the coordinates of the given node.
        /// </summary>
        private static Vector3 GetNodePos(ushort nodeId) {
            NetNode[] nodeBuffer = Singleton<NetManager>.instance.m_nodes.m_buffer;
            Vector3 pos = nodeBuffer[nodeId].m_position;
            float terrainY = Singleton<TerrainManager>.instance.SampleDetailHeightSmooth(pos);
            if (terrainY > pos.y) {
                pos.y = terrainY;
            }

            return pos;
        }

        /// <returns>the average half width of all connected segments</returns>
        private static float CalculateNodeRadius(ushort nodeId) {
            float sumHalfWidth = 0;
            int count = 0;
            ref NetNode node = ref nodeId.ToNode();
            for (int i = 0; i < 8; ++i) {
                ushort segmentId = node.GetSegment(i);
                if (segmentId != 0) {
                    sumHalfWidth += segmentId.ToSegment().Info.m_halfWidth;
                    count++;
                }
            }

            return sumHalfWidth / count;
        }

        public static bool IsUndergroundMode =>
            InfoManager.instance.CurrentMode == InfoManager.InfoMode.Underground;

        public static bool IsNodeVisible(ushort node) {
            return node.IsUndergroundNode() == IsUndergroundMode;
        }

        //--- Use DrawNodeCircle with color instead of warning, and call tool.GetToolColor to get the color
        // public static void DrawNodeCircle(RenderManager.CameraInfo cameraInfo,
        //                                   ushort nodeId,
        //                                   bool warning = false,
        //                                   bool alpha = false,
        //                                   bool overrideRenderLimits = false) {
        //     DrawNodeCircle(
        //         cameraInfo: cameraInfo,
        //         nodeId: nodeId,
        //         color: tool.GetToolColor(warning: warning, error: false),
        //         alpha: alpha,
        //         overrideRenderLimits: overrideRenderLimits);
        // }

        public static void DrawNodeCircle(RenderManager.CameraInfo cameraInfo,
                                          ushort nodeId,
                                          Color color,
                                          bool alpha = false,
                                          bool overrideRenderLimits = false) {
            float r = CalculateNodeRadius(nodeId);
            Vector3 pos = Singleton<NetManager>.instance.m_nodes.m_buffer[nodeId].m_position;
            bool renderLimits = TerrainManager.instance.SampleDetailHeightSmooth(pos) > pos.y;
            DrawOverlayCircle(
                cameraInfo,
                color,
                pos,
                width: r * 2,
                alpha,
                renderLimits: renderLimits || overrideRenderLimits);
        }

        private static void DrawOverlayCircle(RenderManager.CameraInfo cameraInfo,
                                              Color color,
                                              Vector3 position,
                                              float width,
                                              bool alpha,
                                              bool renderLimits = false) {
            float overdrawHeight = renderLimits ? 0f : 5f;
            Singleton<ToolManager>.instance.m_drawCallData.m_overlayCalls++;
            Singleton<RenderManager>.instance.OverlayEffect.DrawCircle(
                cameraInfo,
                color,
                position,
                size: width,
                minY: position.y - overdrawHeight,
                maxY: position.y + overdrawHeight,
                renderLimits,
                alpha);
        }

        /// <summary>
        /// Draws a half sausage at segment end.
        /// </summary>
        /// <param name="segmentId"></param>
        /// <param name="cut">The lenght of the highlight [0~1] </param>
        /// <param name="bStartNode">Determines the direction of the half sausage.</param>
        public static void DrawCutSegmentEnd(RenderManager.CameraInfo cameraInfo,
                                      ushort segmentId,
                                      float cut,
                                      bool bStartNode,
                                      Color color,
                                      bool alpha = false) {
            if( segmentId == 0) {
                return;
            }
            ref NetSegment segment = ref Singleton<NetManager>.instance.m_segments.m_buffer[segmentId];
            float width = segment.Info.m_halfWidth;

            NetNode[] nodeBuffer = Singleton<NetManager>.instance.m_nodes.m_buffer;
            bool IsMiddle(ushort nodeId) => (nodeBuffer[nodeId].m_flags & NetNode.Flags.Middle) != 0;

            Bezier3 bezier;
            bezier.a = GetNodePos(segment.m_startNode);
            bezier.d = GetNodePos(segment.m_endNode);

            NetSegment.CalculateMiddlePoints(
                startPos: bezier.a,
                startDir: segment.m_startDirection,
                endPos: bezier.d,
                endDir: segment.m_endDirection,
                smoothStart: IsMiddle(segment.m_startNode),
                smoothEnd: IsMiddle(segment.m_endNode),
                middlePos1: out bezier.b,
                middlePos2: out bezier.c);

            if (bStartNode) {
                bezier = bezier.Cut(0, cut);
            } else {
                bezier = bezier.Cut(1 - cut, 1);
            }

            Singleton<ToolManager>.instance.m_drawCallData.m_overlayCalls++;
            Singleton<RenderManager>.instance.OverlayEffect.DrawBezier(
                cameraInfo: cameraInfo,
                color: color,
                bezier: bezier,
                size: width * 2f,
                cutStart: bStartNode ? 0 : width,
                cutEnd: bStartNode ? width : 0,
                minY: -1f,
                maxY: 1280f,
                renderLimits: false,
                alphaBlend: alpha);
        }

        /// <summary>
        /// similar to NetTool.RenderOverlay()
        /// but with additional control over alphaBlend.
        /// </summary>
        internal static void DrawSegmentOverlay(
            RenderManager.CameraInfo cameraInfo,
            ushort segmentId,
            Color color,
            bool alphaBlend) {
            if (segmentId == 0) {
                return;
            }

            ref NetSegment segment =
                ref Singleton<NetManager>.instance.m_segments.m_buffer[segmentId];
            float width = segment.Info.m_halfWidth;

            NetNode[] nodeBuffer = Singleton<NetManager>.instance.m_nodes.m_buffer;

            bool IsMiddle(ushort nodeId) =>
                (nodeBuffer[nodeId].m_flags & NetNode.Flags.Middle) != 0;

            Bezier3 bezier;
            bezier.a = GetNodePos(segment.m_startNode);
            bezier.d = GetNodePos(segment.m_endNode);

            NetSegment.CalculateMiddlePoints(
                startPos: bezier.a,
                startDir: segment.m_startDirection,
                endPos: bezier.d,
                endDir: segment.m_endDirection,
                smoothStart: IsMiddle(segment.m_startNode),
                smoothEnd: IsMiddle(segment.m_endNode),
                middlePos1: out bezier.b,
                middlePos2: out bezier.c);

            Singleton<ToolManager>.instance.m_drawCallData.m_overlayCalls++;
            Singleton<RenderManager>.instance.OverlayEffect.DrawBezier(
                cameraInfo,
                color,
                bezier,
                size: width * 2f,
                cutStart: 0,
                cutEnd: 0,
                minY: -1f,
                maxY: 1280f,
                renderLimits: false,
                alphaBlend);
        }

        private static void DrawOverlayCircle(RenderManager.CameraInfo cameraInfo,
                                              Color color,
                                              Vector3 position,
                                              float width,
                                              bool alpha) {
            Singleton<ToolManager>.instance.m_drawCallData.m_overlayCalls++;
            Singleton<RenderManager>.instance.OverlayEffect.DrawCircle(
                cameraInfo,
                color,
                position,
                size: width,
                minY: position.y - 100f,
                maxY: position.y + 100f,
                renderLimits: false,
                alpha);
        }

        public static bool DrawHoverableSquareOverlayTexture(Texture2D texture,
                                                             Vector3 camPos,
                                                             Vector3 worldPos,
                                                             float size) {
            return DrawGenericOverlayTexture(
                texture,
                camPos,
                worldPos,
                width: size,
                height: size,
                canHover: true,
                screenRect: out Rect _);
        }

        public static bool DrawGenericSquareOverlayTexture(Texture2D texture,
                                                           Vector3 camPos,
                                                           Vector3 worldPos,
                                                           float size,
                                                           bool canHover) {
            return DrawGenericOverlayTexture(
                texture,
                camPos,
                worldPos,
                width: size,
                height: size,
                canHover,
                screenRect: out Rect _);
        }

        public static bool DrawGenericOverlayTexture(Texture2D texture,
                                                     Vector3 camPos,
                                                     Vector3 worldPos,
                                                     float width,
                                                     float height,
                                                     bool canHover,
                                                     out Rect screenRect) {
            // Is point in screen?
            if (!GeometryUtil.WorldToScreenPoint(worldPos, out Vector3 screenPos)) {
                screenRect = default;
                return false;
            }

            // UI Scale should not affect the overlays (no multiplication by U.UIScaler.GetScale())
            float visibleScale = 1.0f / (worldPos - camPos).magnitude * 100f;
            width *= visibleScale;
            height *= visibleScale;

            screenRect = new Rect(
                x: screenPos.x - (width / 2f),
                y: screenPos.y - (height / 2f),
                width: width,
                height: height);

            Color guiColor = GUI.color;
            bool hovered = false;

            if (canHover) {
                hovered = TrafficManagerTool.IsMouseOver(screenRect);
            }

            guiColor.a = TrafficManagerTool.GetHandleAlpha(hovered);

            GUI.color = guiColor;
            GUI.DrawTexture(screenRect, texture);

            return hovered;
        }
    }
}