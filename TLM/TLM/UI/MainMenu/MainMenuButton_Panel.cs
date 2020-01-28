namespace TrafficManager.UI.MainMenu {
    /// <summary>
    /// Panel which is created inside MainMenuButton (on the game toolbar).
    /// </summary>
    public class MainMenuButton_Panel : GeneratedGroupPanel
    {
        public override ItemClass.Service service
        {
            get
            {
                return ItemClass.Service.None;
            }
        }

        public override string serviceName
        {
            get
            {
                return "FindIt";
            }
        }

        protected override bool IsServiceValid(PrefabInfo info)
        {
            return true;
        }
    }
}
