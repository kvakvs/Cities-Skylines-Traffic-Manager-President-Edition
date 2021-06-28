﻿namespace TrafficManager.UI.SubTools.SpeedLimits {
    using ColossalFramework.UI;
    using TrafficManager.State;
    using TrafficManager.U;
    using TrafficManager.U.Autosize;
    using UnityEngine;

    internal partial class ToolWindow {
        /// <summary>
        /// Wrapper panel around mode toggle buttons.
        /// <see cref="SegmentLaneModeToggleButton"/>, <see cref="EditDefaultsModeButton"/>,
        /// <see cref="MphToggleButton"/>.
        /// </summary>
        internal class ModeButtonsPanel : UPanel {
            /// <summary>UI button which toggles per-segment or per-lane speed limits.</summary>
            public UButton SegmentLaneModeToggleButton;

            public UButton EditDefaultsModeButton;

            public MphToggleButton ToggleMphButton;

            public void SetupControls(ToolWindow window, UBuilder builder) {
                this.name = GAMEOBJECT_NAME + "_ModesPanel";

                void ButtonpanelResizeFn(UResizer r) {
                    r.Stack(
                        mode: UStackMode.NewRowBelow,
                        spacing: UConst.UIPADDING,
                        stackRef: window.windowTitleLabel_);
                    r.FitToChildren();
                }

                this.ResizeFunction(ButtonpanelResizeFn);

                Vector2 buttonSize = new Vector2(50f, 50f);
                UITextureAtlas uiAtlas = window.GetUiAtlas();

                //----------------
                // Edit Segments/Lanes mode button
                //----------------
                this.SegmentLaneModeToggleButton = builder.Button<UButton, UIComponent>(
                    parent: this,
                    text: string.Empty,
                    tooltip: "Edit segments. Click to edit lanes.",
                    size: buttonSize,
                    stack: UStackMode.Below);
                this.SegmentLaneModeToggleButton.atlas = uiAtlas;

                // Note the atlas is loaded before this skin is created in window.GetUiAtlas()
                this.SegmentLaneModeToggleButton.Skin =
                    ButtonSkin.CreateSimple(
                                  foregroundPrefix: "EditSegments",
                                  backgroundPrefix: UConst.MAINMENU_ROUND_BUTTON_BG)
                              .CanActivate(background: false)
                              .CanHover(foreground: false);
                this.SegmentLaneModeToggleButton.ApplyButtonSkin();

                // the onclick handler is set by SpeedLimitsTool outside of this module

                //----------------
                // Edit Defaults mode button
                //----------------
                this.EditDefaultsModeButton = builder.Button<UButton, UIComponent>(
                    parent: this,
                    text: string.Empty,
                    tooltip: "Default speed limits per road type",
                    size: buttonSize,
                    stack: UStackMode.Below);
                this.EditDefaultsModeButton.atlas = uiAtlas;

                // Note the atlas is loaded before this skin is created in window.GetUiAtlas()
                this.EditDefaultsModeButton.Skin = ButtonSkin
                                                   .CreateSimple(
                                                       foregroundPrefix: "EditDefaults",
                                                       backgroundPrefix: UConst.MAINMENU_ROUND_BUTTON_BG)
                                                   .CanActivate(background: false)
                                                   .CanHover(foreground: false);
                this.EditDefaultsModeButton.ApplyButtonSkin();

                // the onclick handler is set by SpeedLimitsTool outside of this module

                //----------------
                // MPH/Kmph switch
                //----------------
                bool displayMph = GlobalConfig.Instance.Main.DisplaySpeedLimitsMph;
                this.ToggleMphButton = builder.Button<MphToggleButton, UIComponent>(
                    parent: this,
                    text: string.Empty,
                    tooltip: displayMph
                                 ? Translation.SpeedLimits.Get("Miles per hour")
                                 : Translation.SpeedLimits.Get("Kilometers per hour"),
                    size: buttonSize,
                    stack: UStackMode.Below);
                this.ToggleMphButton.atlas = uiAtlas;

                // Note the atlas is loaded before this skin is created in window.GetUiAtlas()
                this.ToggleMphButton.Skin = ButtonSkin.CreateSimple(
                                                     foregroundPrefix: "MphToggle",
                                                     backgroundPrefix: UConst.MAINMENU_ROUND_BUTTON_BG)
                                                 .CanActivate(background: false)
                                                 .CanHover(foreground: false);
                this.ToggleMphButton.ApplyButtonSkin();

                // the onclick handler is set by SpeedLimitsTool outside of this module
            }
        }
    }
}