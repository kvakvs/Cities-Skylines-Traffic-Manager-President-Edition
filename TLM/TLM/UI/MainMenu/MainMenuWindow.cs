// #define QUEUEDSTATS

namespace TrafficManager.UI.MainMenu {
    using ColossalFramework.UI;
    using CSUtil.Commons;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
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

        /// <summary>
        /// Panel floating below the main menu and shows keybinds and mouse shortcuts.
        /// Panel is hidden if it contains no controls.
        /// </summary>
        public UPanel KeybindsPanel { get; set; }

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

        /// <summary>Used to determine drag box height.</summary>
        public UILabel VersionLabel { get; private set; }

        public UILabel StatsLabel { get; private set; }

        public UIDragHandle DragHandle { get; private set; }

        IDisposable confDisposable;

        private bool isStarted_;

        private UITextureAtlas allButtonsAtlas_;

        /// <summary>Defines button placement on the main menu since last layout reset.</summary>
        private MainMenuLayout menuLayout_;

        public override void Start() {
            base.Start();

            U.UIUtil.MakeUniqueAndSetName(this.gameObject, "TMPE_MainMenuWindow");

            GlobalConfig conf = GlobalConfig.Instance;

            OnUpdate(conf);

            confDisposable = conf.Subscribe(this);
            SetupWindow();
        }

        /// <summary>
        /// Called from ModUI when need to create or re-create the MainMenu panel.
        /// </summary>
        /// <returns>The created panel.</returns>
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
            this.DragHandle = dragHandler.AddComponent<UIDragHandle>();
            this.DragHandle.enabled = !GlobalConfig.Instance.Main.MainMenuPosLocked;

            this.eventVisibilityChanged += OnVisibilityChanged;
        }

        /// <summary>Called from ModUI to setup children for the window.</summary>
        /// <param name="builder">The UI Builder.</param>
        public void SetupControls(UiBuilder<MainMenuWindow> builder) {
            UILabel versionLabel = null;
            using (var versionLabelB = builder.Label<U.Label.ULabel>(TrafficManagerMod.ModName)) {
                versionLabelB.ResizeFunction(r => r.Stack(UStackMode.Below));
                this.VersionLabel = versionLabel = versionLabelB.Control;
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
            U.Panel.UPanel leftPanel;

            using (var innerPanelB = builder.ChildPanel<U.Panel.UPanel>(setupFn: p => {
                p.name = "TMPE_MainMenu_InnerPanel";
            })) {
                innerPanelB.ResizeFunction(r => {
                    r.Stack(mode: UStackMode.Below, spacing: 0f, stackRef: versionLabel);
                    r.FitToChildren();
                });

                AddButtonsResult toolButtonsResult = SetupControls_ToolPanel(innerPanelB, atlasKeysSet);
                SetupControls_ExtraPanel(innerPanelB, atlasKeysSet, toolButtonsResult);
            }

            // Create atlas and give it to all buttons
            allButtonsAtlas_ = tmpSkin.CreateAtlas(
                loadingPath: "MainMenu.Tool",
                spriteWidth: 50,
                spriteHeight: 50,
                hintAtlasTextureSize: 512,
                atlasKeysSet);

            foreach (BaseMenuButton b in ToolButtonsList) {
                b.atlas = allButtonsAtlas_;
            }
            foreach (BaseMenuButton b in ExtraButtonsList) {
                b.atlas = allButtonsAtlas_;
            }

            //-------------------------------------------------------------------------
            // Foldable panel with keybinds, starts hidden below or above the main menu
            //-------------------------------------------------------------------------
            using (var keybindsB = builder.ChildPanel<U.Panel.UPanel>(
                p => {
                    p.name = "TMPE_MainMenu_KeybindsPanel";
                    p.isVisible = false;

                    // the GenericPanel sprite is silver, make it dark
                    p.atlas = TextureUtil.FindAtlas("Ingame");
                    p.backgroundSprite = "GenericPanel";
                    p.color = new Color32(64, 64, 64, 240);
                }))
            {
                keybindsB.SetPadding(Constants.UIPADDING);

                // The keybinds panel belongs to main menu but does not expand it to fit
                UResizerConfig.From(keybindsB.Control).ContributeToBoundingBox = false;
                this.KeybindsPanel = keybindsB.Control;
                keybindsB.ResizeFunction(
                    r => {
                        r.Stack(mode: UStackMode.Below,
                                spacing: Constants.UIPADDING * 2);
                        // As the control technically belongs inside the mainmenu, it will respect
                        // the 4px padding, we want to shift it slightly left to line up with the
                        // main menu panel.
                        r.MoveBy(new Vector2(-Constants.UIPADDING, 0f));
                        r.FitToChildren();
                    });
            }

            // Floating labels under TM:PE window
            SetupControls_DebugLabels(builder, this.KeybindsPanel);
        }

        private void SetupControls_KeybindsPanel(UiBuilder<UPanel> builder) {
        }

        private void SetupControls_DebugLabels(UiBuilder<MainMenuWindow> builder,
                                               UIComponent stackUnder) {
            // Pathfinder stats label (debug only)
            if (Options.showPathFindStats) {
                using (var statsLabelB = builder.Label<StatsLabel>(string.Empty)) {
                    // Allow the label to hang outside the parent box
                    UResizerConfig.From(statsLabelB.Control).ContributeToBoundingBox = false;

                    UIComponent under = stackUnder; // copy for the closure to work
                    statsLabelB.ResizeFunction(
                        r => {
                            // Extra 2x spacing because of form's inner padding
                            r.Stack(
                                mode: UStackMode.Below,
                                spacing: 2f * Constants.UIPADDING,
                                stackRef: under);
                        });
                    stackUnder = this.StatsLabel = statsLabelB.Control;
                }
            }

            // Hot Reload version label (debug only)
            if (TrafficManagerMod.Instance.InGameHotReload) {
                // Hot Reload version label (debug only)
                string text = $"HOT RELOAD {Assembly.GetExecutingAssembly().GetName().Version}";
                using (var hotReloadB = builder.Label<U.Label.ULabel>(text)) {
                    // Allow the label to hang outside the parent box
                    UResizerConfig.From(hotReloadB.Control).ContributeToBoundingBox = false;

                    hotReloadB.ResizeFunction(
                        r => {
                            // If pathFind stats label above was not visible, we need extra spacing
                            float extraSpacing = Options.showPathFindStats ? Constants.UIPADDING : 0f;
                            r.Stack(
                                mode: UStackMode.Below,
                                spacing: extraSpacing + Constants.UIPADDING,
                                stackRef: stackUnder);
                        });
                }
            }
        }

        private AddButtonsResult SetupControls_ToolPanel(UiBuilder<UPanel> innerPanelB,
                                                         HashSet<string> atlasKeysSet) {
            // This is tool buttons panel
            using (UiBuilder<UPanel> leftPanelB = innerPanelB.ChildPanel<U.Panel.UPanel>(
                setupFn: panel => {
                    panel.name = "TMPE_MainMenu_ToolPanel";
                }))
            {
                leftPanelB.ResizeFunction(r => {
                    r.Stack(mode: UStackMode.Below);
                    r.FitToChildren();
                });

                // Create 2 rows of button objects
                var toolButtonsResult = AddButtonsFromButtonDefinitions(
                    leftPanelB,
                    atlasKeysSet,
                    TOOL_BUTTON_DEFS,
                    minRowLength: 4,
                    maxRowLength: 5);
                ToolButtonsList = toolButtonsResult.Buttons;

                return toolButtonsResult;
            }
        }

        private void SetupControls_ExtraPanel(UiBuilder<UPanel> innerPanelB,
                                              HashSet<string> atlasKeysSet,
                                              AddButtonsResult toolButtonsResult) {
            // This is toggle despawn and clear traffic panel
            using (UiBuilder<UPanel> rightPanelB = innerPanelB.ChildPanel<U.Panel.UPanel>(
                setupFn: p => {
                    p.name = "TMPE_MainMenu_ExtraPanel";
                    // Silver background panel
                    p.atlas = TextureUtil.FindAtlas("Ingame");
                    p.backgroundSprite = "GenericPanel";
                }))
            {
                rightPanelB.ResizeFunction(r => {
                    // Step to the right by 4px
                    r.Stack(mode: UStackMode.ToTheRight,
                            spacing: Constants.UIPADDING);
                    r.FitToChildren();
                });

                // Create 1 or 2 rows of button objects
                // Row count depends on whether tool buttons panel was 2 or 1 rows
                var rowLength = toolButtonsResult.Layout.Rows == 2
                                    ? 1
                                    : EXTRA_BUTTON_DEFS.Length;
                var extraButtonsResult = AddButtonsFromButtonDefinitions(
                    rightPanelB,
                    atlasKeysSet,
                    EXTRA_BUTTON_DEFS,
                    minRowLength: rowLength,
                    maxRowLength: rowLength);
                ExtraButtonsList = extraButtonsResult.Buttons;
            }
        }

        /// <summary>Called by UResizer for every control to be 'resized'.</summary>
        public override void OnAfterResizerUpdate() {
            if (this.DragHandle != null) {
                // Resize to the window width but remember there's padding on the left
                this.DragHandle.size = new Vector2(x: this.width - (Constants.UIPADDING * 2f),
                                                   y: this.VersionLabel.height);
            }
        }

        private struct AddButtonsResult {
            public List<BaseMenuButton> Buttons;
            public MainMenuLayout Layout;
        }

        /// <summary>Create buttons and add them to the given panel UIBuilder.</summary>
        /// <param name="builder">UI builder to use.</param>
        /// <param name="atlasKeysSet">Atlas keys to update for button images.</param>
        /// <param name="buttonDefs">Button defs collection to create from it.</param>
        /// <param name="minRowLength">Shortest the row can be before breaking.</param>
        /// <param name="maxRowLength">Longest the row can be before breaking.</param>
        /// <returns>A list of created buttons.</returns>
        private AddButtonsResult AddButtonsFromButtonDefinitions(UiBuilder<UPanel> builder,
                                                                     HashSet<string> atlasKeysSet,
                                                                     MenuButtonDef[] buttonDefs,
                                                                     int minRowLength,
                                                                     int maxRowLength)
        {
            AddButtonsResult result;
            result.Buttons = new List<BaseMenuButton>();

            // Count the button objects and set their layout
            result.Layout = new MainMenuLayout();
            result.Layout.CountEnabledButtons(buttonDefs);
            int placedInARow = 0;

            foreach (MenuButtonDef buttonDef in buttonDefs) {
                if (!buttonDef.IsEnabledFunc()) {
                    // Skip buttons which are not enabled
                    continue;
                }

                // Create and populate the panel with buttons
                var buttonBuilder = builder.Button<BaseMenuButton>(buttonDef.ButtonType);

                // Count buttons in a row and break the line
                bool doRowBreak = result.Layout.IsRowBreak(placedInARow, minRowLength, maxRowLength);

                buttonBuilder.ResizeFunction(r => {
                    r.Stack(doRowBreak ? UStackMode.NewRowBelow : UStackMode.ToTheRight);
                    r.Width(UValue.FixedSize(40f));
                    r.Height(UValue.FixedSize(40f));
                });

                if (doRowBreak) {
                    placedInARow = 0;
                    result.Layout.Rows++;
                } else {
                    placedInARow++;
                }

                // Also ask each button what sprites they need
                buttonBuilder.Control.SetupButtonSkin(atlasKeysSet);

                string buttonName = buttonDef.ButtonType.ToString().Split('.').Last();
                buttonBuilder.Control.name = $"TMPE_MainMenuButton_{buttonName}";

                ButtonsDict.Add(buttonDef.Mode, buttonBuilder.Control);
                result.Buttons.Add(buttonBuilder.Control);
            }

            return result;
        }

        private void OnVisibilityChanged(UIComponent component, bool value) {
            VersionLabel.enabled = value;

            if (StatsLabel != null) {
                // might not exist
                StatsLabel.enabled = Options.showPathFindStats && value;
            }
            UResizer.UpdateControl(this);
        }

        public override void OnDestroy() {
            eventVisibilityChanged -= OnVisibilityChanged;
            confDisposable?.Dispose();
        }

        internal void SetPosLock(bool lck) {
            DragHandle.enabled = !lck;
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