// #define QUEUEDSTATS

namespace TrafficManager.UI.MainMenu {
    using ColossalFramework.UI;
    using CSUtil.Commons;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using JetBrains.Annotations;
    using TrafficManager.API.Util;
    using TrafficManager.State.Keybinds;
    using TrafficManager.State;
    using TrafficManager.U;
    using TrafficManager.U.Autosize;
    using TrafficManager.U.Panel;
    using UnityEngine;

    public class MainMenuWindow
        : U.Panel.BaseUWindowPanel,
          IObserver<GlobalConfig>
    {
        public const int DEFAULT_MENU_X = 85;
        public const int DEFAULT_MENU_Y = 60;
        private const string GAMEOBJECT_NAME = "TMPE_MainMenuPanel";

        /// <summary>Tool buttons occupy the left and bigger side of the main menu.</summary>
        private static readonly MenuButtonDef[] TOOL_BUTTON_DEFS
            = {
                  new MenuButtonDef {
                                        ButtonType = typeof(ToggleTrafficLightsButton),
                                        Mode = ToolMode.ToggleTrafficLight,
                                        IsEnabledFunc = () => true, // always ON
                                    },
                  new MenuButtonDef {
                                        ButtonType = typeof(TimedTrafficLightsButton),
                                        Mode = ToolMode.TimedLightsButton,
                                        IsEnabledFunc = TimedTrafficLightsButton.IsButtonEnabled,
                                    },
                  new MenuButtonDef {
                                        ButtonType = typeof(ManualTrafficLightsButton),
                                        Mode = ToolMode.ManualSwitch,
                                        IsEnabledFunc = ManualTrafficLightsButton.IsButtonEnabled,
                                    },
                  new MenuButtonDef {
                                        ButtonType = typeof(LaneConnectorButton),
                                        Mode = ToolMode.LaneConnector,
                                        IsEnabledFunc = LaneConnectorButton.IsButtonEnabled,
                                    },
                  new MenuButtonDef {
                                        ButtonType = typeof(LaneArrowsButton),
                                        Mode = ToolMode.LaneArrows,
                                        IsEnabledFunc = () => true, // always ON
                                    },
                  new MenuButtonDef {
                                        ButtonType = typeof(PrioritySignsButton),
                                        Mode = ToolMode.AddPrioritySigns,
                                        IsEnabledFunc = PrioritySignsButton.IsButtonEnabled,
                                    },
                  new MenuButtonDef {
                                        ButtonType = typeof(JunctionRestrictionsButton),
                                        Mode = ToolMode.JunctionRestrictions,
                                        IsEnabledFunc = JunctionRestrictionsButton.IsButtonEnabled,
                                    },
                  new MenuButtonDef {
                                        ButtonType = typeof(SpeedLimitsButton),
                                        Mode = ToolMode.SpeedLimits,
                                        IsEnabledFunc = SpeedLimitsButton.IsButtonEnabled,
                                    },
                  new MenuButtonDef {
                                        ButtonType = typeof(VehicleRestrictionsButton),
                                        Mode = ToolMode.VehicleRestrictions,
                                        IsEnabledFunc = VehicleRestrictionsButton.IsButtonEnabled,
                                    },
                  new MenuButtonDef {
                                        ButtonType = typeof(ParkingRestrictionsButton),
                                        Mode = ToolMode.ParkingRestrictions,
                                        IsEnabledFunc = ParkingRestrictionsButton.IsButtonEnabled,
                                    },
              };

        /// <summary>Extra buttons occupy the right side of the main menu.</summary>
        private static readonly MenuButtonDef[] EXTRA_BUTTON_DEFS
            = {
                  new MenuButtonDef {
                                        ButtonType = typeof(DespawnButton),
                                        Mode = ToolMode.DespawnButton,
                                        IsEnabledFunc = () => true,
                                    },
                  new MenuButtonDef {
                                        ButtonType = typeof(ClearTrafficButton),
                                        Mode = ToolMode.ClearTrafficButton,
                                        IsEnabledFunc = () => true,
                                    },
              };

        /// <summary>List of buttons stores created UIButtons in order. </summary>
        public List<BaseMenuButton> ToolButtonsList;
        public List<BaseMenuButton> ExtraButtonsList;

        /// <summary>Dict of buttons allows quick search by toolmode.</summary>
        private Dictionary<ToolMode, BaseMenuButton> ButtonsDict;

        public UILabel VersionLabel { get; private set; }

        public UILabel StatsLabel { get; private set; }

        public UIDragHandle Drag { get; private set; }

        IDisposable confDisposable;

        private bool isStarted_;

        private UITextureAtlas allButtonsAtlas_;

        /// <summary>Defines button placement on the main menu since last layout reset.</summary>
        private MainMenuLayout menuLayout_;

        public override void Start() {
            base.Start();

            U.UIUtil.MakeUniqueAndSetName(this.gameObject, GAMEOBJECT_NAME);

            GlobalConfig conf = GlobalConfig.Instance;

            OnUpdate(conf);

            confDisposable = conf.Subscribe(this);
            SetupWindow();
        }

        internal static MainMenuWindow CreateMainMenuWindow() {
            UIView parent = UIView.GetAView();
            MainMenuWindow window = (MainMenuWindow)parent.AddUIComponent(typeof(MainMenuWindow));

            window.gameObject.AddComponent<CustomKeyHandler>();

            using (var builder = new U.UiBuilder<MainMenuWindow>(window)) {
                builder.ResizeFunction(r => { r.FitToChildren(); });
                builder.SetPadding(Constants.UIPADDING);

                window.SetupControls(builder);

                // Resize everything correctly
                builder.Done();
            }

            return window;
        }

        /// <summary>Called from constructor to setup own properties and events.</summary>
        private void SetupWindow() {
            this.name = "TMPE_MainMenu_Window";
            this.isVisible = false;
            this.backgroundSprite = "GenericPanel";
            this.color = new Color32(64, 64, 64, 240);

            var dragHandler = new GameObject("TMPE_Menu_DragHandler");
            dragHandler.transform.parent = transform;
            dragHandler.transform.localPosition = Vector3.zero;
            this.Drag = dragHandler.AddComponent<UIDragHandle>();
            this.Drag.enabled = !GlobalConfig.Instance.Main.MainMenuPosLocked;

            this.eventVisibilityChanged += OnVisibilityChanged;
        }

        /// <summary>Called from ModUI to setup children for the window.</summary>
        /// <param name="builder">The UI Builder.</param>
        public void SetupControls(UiBuilder<MainMenuWindow> builder) {
            UILabel versionLabel = null;
            using (var versionLabelB = builder.Label<VersionLabel>(string.Empty)) {
                versionLabelB.ResizeFunction(r => r.StackVertical());
                this.VersionLabel = versionLabel = versionLabelB.Control;
            }

            using (var statsLabelB = builder.Label<StatsLabel>(string.Empty)) {
                statsLabelB.ResizeFunction(r => {
                    r.StackHorizontal(spacing: 2 * Constants.UIPADDING);
                });
                this.StatsLabel = statsLabelB.Control;
            }

            // Create and populate list of background atlas keys, used by all buttons
            // And also each button will have a chance to add their own atlas keys for loading.
            var tmpSkin = new U.Button.ButtonSkin {
                                                      Prefix = "MainMenuPanel",
                                                      BackgroundPrefix = "RoundButton",
                                                      ForegroundNormal = false,
                                                      BackgroundHovered = true,
                                                      BackgroundActive = true,
                                                  };
            // By default the atlas will include backgrounds: DefaultRound-bg-normal
            HashSet<string> atlasKeysSet = tmpSkin.CreateAtlasKeyset();

            // Main menu contains 2 panels side by side, one for tool buttons and another for
            // despawn & clear buttons.
            ButtonsDict = new Dictionary<ToolMode, BaseMenuButton>();

            // This is tool buttons panel
            using (UiBuilder<UPanel> leftPanelBuilder = builder.ChildPanel<U.Panel.UPanel>(
                setupFn: panel => {
                    panel.name = "TMPE_MainMenu_ToolPanel";
                }))
            {
                leftPanelBuilder.SetPadding(Constants.UIPADDING);

                leftPanelBuilder.ResizeFunction(r => {
                    r.StackVertical(spacing: 0f, stackUnder: versionLabel);
                    r.FitToChildren();
                });

                // Create actual button objects
                ToolButtonsList = AddButtonsFromButtonDefinitions(
                    leftPanelBuilder,
                    atlasKeysSet,
                    TOOL_BUTTON_DEFS,
                    maxBreakAt: 5);
            }

            // This is toggle despawn and clear traffic panel
            using (UiBuilder<UPanel> rightPanelBuilder = builder.ChildPanel<U.Panel.UPanel>(
                setupFn: panel => {
                    panel.name = "TMPE_MainMenu_ExtraPanel";
                }))
            {
                rightPanelBuilder.SetPadding(Constants.UIPADDING);

                rightPanelBuilder.ResizeFunction(r => {
                    r.StackHorizontal();
                    r.FitToChildren();
                });

                ExtraButtonsList = AddButtonsFromButtonDefinitions(
                    rightPanelBuilder,
                    atlasKeysSet,
                    EXTRA_BUTTON_DEFS,
                    maxBreakAt: 1);
            }

            // Create atlas and give it to all buttons
            allButtonsAtlas_ = tmpSkin.CreateAtlas(
                "MainMenu.Tool",
                50,
                50,
                512,
                atlasKeysSet);

            foreach (BaseMenuButton b in ToolButtonsList) {
                b.atlas = allButtonsAtlas_;
            }
            foreach (BaseMenuButton b in ExtraButtonsList) {
                b.atlas = allButtonsAtlas_;
            }
        }

        /// <summary>Create buttons and add them to the given panel UIBuilder.</summary>
        /// <param name="builder">UI builder to use.</param>
        /// <param name="atlasKeysSet">Atlas keys to update for button images.</param>
        /// <param name="buttonDefs">Button defs collection to create from it.</param>
        /// <returns>A list of created buttons.</returns>
        private List<BaseMenuButton> AddButtonsFromButtonDefinitions(
            UiBuilder<UPanel> builder,
            HashSet<string> atlasKeysSet,
            IEnumerable<MenuButtonDef> buttonDefs,
            int maxBreakAt)
        {
            var list = new List<BaseMenuButton>();

            // Count the button objects and set their layout
            MainMenuLayout layout = new MainMenuLayout();
            layout.CountEnabledButtons(buttonDefs);
            int placedInARow = 0;

            foreach (MenuButtonDef buttonDef in buttonDefs) {
                // Create and populate the panel with buttons
                var buttonBuilder = builder.Button<BaseMenuButton>(buttonDef.ButtonType);

                // Count buttons in a row and break the line
                placedInARow++;
                bool doRowBreak = layout.IsRowBreak(placedInARow, maxBreakAt);

                buttonBuilder.ResizeFunction(r => {
                    if (doRowBreak) {
                        r.StackVerticalNewRow(spacing: 0f);
                    } else {
                        r.StackHorizontal();
                    }
                    r.Width(UValue.FixedSize(40f));
                    r.Height(UValue.FixedSize(40f));
                });

                if (doRowBreak) {
                    placedInARow = 0;
                }

                // Also ask each button what sprites they need
                buttonBuilder.Control.SetupButtonSkin(atlasKeysSet);
                buttonBuilder.Control.name = $"TMPE_MainMenuButton_{buttonDef.ButtonType}";

                ButtonsDict.Add(buttonDef.Mode, buttonBuilder.Control);
                list.Add(buttonBuilder.Control);
            }

            return list;
        }

        private void OnVisibilityChanged(UIComponent component, bool value) {
            VersionLabel.enabled = value;
            StatsLabel.enabled = Options.showPathFindStats && value;
            UResizer.UpdateControl(this);
        }

        public override void OnDestroy() {
            eventVisibilityChanged -= OnVisibilityChanged;
            confDisposable?.Dispose();
        }

        internal void SetPosLock(bool lck) {
            Drag.enabled = !lck;
        }

        protected override void OnPositionChanged() {
            GlobalConfig config = GlobalConfig.Instance;

            bool posChanged = config.Main.MainMenuX != (int)absolutePosition.x ||
                               config.Main.MainMenuY != (int)absolutePosition.y;

            if (posChanged) {
                Log._Debug($"Menu position changed to {absolutePosition.x}|{absolutePosition.y}");

                config.Main.MainMenuX = (int)absolutePosition.x;
                config.Main.MainMenuY = (int)absolutePosition.y;

                GlobalConfig.WriteConfig();
            }

            base.OnPositionChanged();
        }

        private int lastUpdatePositionFrame_ = 0;

        public void OnUpdate(GlobalConfig config) {
            int nowFrame = Time.frameCount;
            int diff = nowFrame - this.lastUpdatePositionFrame_;

            // Do not call UpdatePosition more than once every 60 frames
            if (diff > 60) {
                UpdatePosition(new Vector2(config.Main.MainMenuX, config.Main.MainMenuY));
                lastUpdatePositionFrame_ = nowFrame;
            }

            // if (isStarted_) {
            //     this.Invalidate();
            // }
        }

        // public override void OnRescaleRequested() {
        //     // Update size
        //     //--------------
        //     menuLayout_ = RepositionToolButtons();
        //     this.width = ScaledSize.GetWidth(menuLayout_.MaxCols);
        //     this.height = ScaledSize.GetHeight(menuLayout_.Rows);
        //
        //     // Update drag size
        //     //-------------------
        //     this.Drag.width = this.width;
        //     this.Drag.height = ScaledSize.GetTitlebarHeight();
        // }

        /// <summary>Calculates button and panel sizes based on the screen resolution.</summary>
        // internal class ScaledSize {
        //     internal const int NUM_ROWS = 2;
        //     internal const int NUM_COLS = 6;
        //
        //     /// <summary>Calculate width of main menu panel, based on button width and spacings.</summary>
        //     /// <returns>Width of the panel.</returns>
        //     internal static float GetWidth(int cols) {
        //         // 6 buttons + spacings (each 1/8 of a button)
        //         float allSpacings = (cols + 1) * 0.125f;
        //         return GetButtonSize() * (cols + allSpacings);
        //     }
        //
        //     internal static float GetHeight() => GetHeight(NUM_ROWS);
        //
        //     internal static float GetHeight(int rows) {
        //         // Count height for `Rows` button rows + `Rows` spacings (1/8th) + titlebar
        //         return (GetButtonSize() * (rows + (rows * 0.125f)))
        //                + GetTitlebarHeight();
        //     }
        //
        //     /// <summary>Define size as smallest of 2.08% of width or 3.7% of height (40 px at 1080p).
        //     /// The button cannot be smaller than 40px.</summary>
        //     /// <returns>Button size for current screen resolution.</returns>
        //     internal static float GetButtonSize() {
        //         return 40f * UIScaler.GetScale();
        //     }
        //
        //     internal static float GetTitlebarHeight() {
        //         return GetButtonSize() * 0.66f;
        //     }
        // }

        // /// <summary>Reset sizes and positions for UI buttons.</summary>
        // /// <returns>Visible buttons count.</returns>
        // private MainMenuLayout RepositionToolButtons() {
        //     MainMenuLayout layout = new MainMenuLayout();
        //
        //     // Recreate tool buttons
        //     float y = ScaledSize.GetTitlebarHeight();
        //     float buttonSize = ScaledSize.GetButtonSize();
        //     float spacing = buttonSize / 8f;
        //
        //     layout.CountEnabledButtons(ButtonsList);
        //
        //     int placedInARow = 0;
        //     float x = spacing;
        //
        //     foreach (BaseMenuButton button in ButtonsList) {
        //         if (button.IsVisible()) {
        //             button.Show();
        //             button.relativePosition = new Vector3(x, y);
        //
        //             x += buttonSize + spacing;
        //
        //             placedInARow++;
        //             layout.MaxCols = Math.Max(layout.MaxCols, placedInARow);
        //
        //             if (layout.IsRowBreak(placedInARow, ScaledSize.NUM_COLS)) {
        //                 y += buttonSize + spacing;
        //                 x = spacing; // reset to the left side of the button area
        //                 placedInARow = 0;
        //                 layout.Rows++;
        //             }
        //         } else {
        //             button.Hide();
        //             // to avoid window upsizing to fit an invisible button
        //             button.relativePosition = Vector3.zero;
        //         }
        //
        //         button.width = buttonSize;
        //         button.height = buttonSize;
        //         button.Invalidate();
        //     } // foreach button
        //
        //     // Special case when new row was broken but no buttons placed on it, reduce Rows count
        //     // happens when button count is even
        //     if (x <= 2 * spacing) {
        //         layout.Rows--;
        //     }
        //
        //     return layout;
        // }

        /// <summary>Always invalidates the main menu, do not call too often!</summary>
        /// <param name="pos">Config main menu position.</param>
        public void UpdatePosition(Vector2 pos) {
            Rect rect = new Rect(
                pos.x,
                pos.y,
                ModUI.Instance.MainMenu.width,
                this.height);
            Vector2 resolution = UIView.GetAView().GetScreenResolution();
            VectorUtil.ClampRectToScreen(ref rect, resolution);
            Log.Info($"Setting main menu position to [{pos.x},{pos.y}]");
            absolutePosition = rect.position;
            Invalidate();
        }

        public void OnGUI() {
            // Return if modal window is active or the main menu is hidden
            if (!isVisible || UIView.HasModalInput() || UIView.HasInputFocus()) {
                return;
            }

            // Some safety checks to not trigger while full screen/modals are open
            // Check the key and then click the corresponding button
            if (KeybindSettingsBase.ToggleTrafficLightTool.IsPressed(Event.current)) {
                ClickToolButton(ToolMode.ToggleTrafficLight);
            } else if (KeybindSettingsBase.LaneArrowTool.IsPressed(Event.current)) {
                ClickToolButton(ToolMode.LaneArrows);
            } else if (KeybindSettingsBase.LaneConnectionsTool.IsPressed(Event.current)) {
                ClickToolButton(ToolMode.LaneConnector);
            } else if (KeybindSettingsBase.PrioritySignsTool.IsPressed(Event.current)) {
                ClickToolButton(ToolMode.AddPrioritySigns);
            } else if (KeybindSettingsBase.JunctionRestrictionsTool.IsPressed(Event.current)) {
                ClickToolButton(ToolMode.JunctionRestrictions);
            } else if (KeybindSettingsBase.SpeedLimitsTool.IsPressed(Event.current)) {
                ClickToolButton(ToolMode.JunctionRestrictions);
            }
        }

        /// <summary>For given button mode, send click.</summary>
        internal void ClickToolButton(ToolMode toolMode) {
            if (ButtonsDict.TryGetValue(toolMode, out var b)) {
                b.SimulateClick();
            }
        }

        public void UpdateButtons() {
            foreach (BaseMenuButton button in this.ToolButtonsList) {
                button.UpdateButtonImageAndTooltip();
            }
            foreach (BaseMenuButton button in this.ExtraButtonsList) {
                button.UpdateButtonImageAndTooltip();
            }
        }
    } // end class
}