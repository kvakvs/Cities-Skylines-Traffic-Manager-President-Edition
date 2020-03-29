namespace TrafficManager.U.Panel {
    using ColossalFramework.UI;
    using TrafficManager.U.Autosize;

    /// <summary>Base panel for smart sizeable panels.</summary>
    public class UPanel : UIPanel, ISmartSizableControl {
        private UResizerConfig resizerConfig_;

        public UResizerConfig GetResizerConfig() {
            if (resizerConfig_ == null) {
                resizerConfig_ = new UResizerConfig(null, 0f);
            }
            return resizerConfig_;
        }
    }
}