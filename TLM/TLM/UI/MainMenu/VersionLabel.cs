namespace TrafficManager.UI.MainMenu {
    using System.Reflection;

    public class VersionLabel : U.Label.ULabel {
        public override void Start() {
            // TODO use current size profile
            // size = new Vector2(
            //     ModUI.Instance.MainMenu.width,
            //     MainMenuWindow.ScaledSize.GetTitlebarHeight());
            this.text = TrafficManagerMod.ModName;

            if (TrafficManagerMod.Instance.InGameHotReload) {
                // make it easier to Identify Hot reload.
                this.text += " HOT RELOAD " +
                             Assembly.GetExecutingAssembly().GetName().Version;
            }

            // relativePosition = new Vector3(5f, 5f);
            // textAlignment = UIHorizontalAlignment.Left;
        }
    }
}