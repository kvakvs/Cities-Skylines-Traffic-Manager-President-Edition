namespace TrafficManager.U.Controls {
    using System;
    using CSUtil.Commons;
    using JetBrains.Annotations;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    public class UButton
        : Button,
          IPointerExitHandler,
          IPointerEnterHandler,
          IPointerDownHandler,
          IPointerUpHandler
    {
        /// <summary>
        /// Either contains an image in this button, or colors the button with a solid color
        /// </summary>
        private Image imgComponent_;

        private static RectTransform rectTransform_;
        private static LayoutElement layoutElement_;

        public static UButton Construct([NotNull] GameObject parent,
                                        string buttonName,
                                        string text) {
            var btnObject = new GameObject(buttonName);

            var buttonComponent = btnObject.AddComponent<UButton>();

            // Set the colors
            ColorBlock c = buttonComponent.colors;
            c.normalColor = Constants.NORMAL_BUTTON_BACKGROUND;
            c.highlightedColor = Constants.HOVERED_BUTTON_BACKGROUND;
            c.pressedColor = Constants.PRESSED_BUTTON_BACKGROUND;
            buttonComponent.colors = c;

//            buttonComponent.onClick.AddListener(() => Log.Info("Clicked canvas button"));

            layoutElement_ = btnObject.AddComponent<LayoutElement>();
            rectTransform_ = btnObject.GetComponent<RectTransform>();
//            var rectTransform = btnObject.AddComponent<RectTransform>();
//            rectTransform.SetPositionAndRotation(position, Quaternion.identity);
//            rectTransform.sizeDelta = size;

            // Nested text for the button
            if (!string.IsNullOrEmpty(text)) {
                GameObject textObj = UText.Create(btnObject, "Label", text);
                textObj.GetComponent<UText>()
                       .Alignment(TextAnchor.MiddleCenter);
            }

            btnObject.AddComponent<UConstrained>();
            
            // let button contents stack vertically if there is a sprite and a label for example, or fill entire button
            btnObject.AddComponent<VerticalLayoutGroup>(); 
            
            btnObject.transform.SetParent(parent.transform, false);
            
            return buttonComponent;
        }

        protected override void Start() {
            this.imgComponent_ = this.gameObject.AddComponent<Image>();
            this.imgComponent_.color = this.colors.normalColor;
        }

        /// <summary>
        /// Sets the position for the button, if it wasn't managed by a layout group
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public UButton Position(Vector2 position) {
            rectTransform_.SetPositionAndRotation(position, Quaternion.identity);
            return this;
        }

        /// <summary>
        /// Sets size for the button if it wasn't managed by a layout group
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public UButton Size(Vector2 size) {
            rectTransform_.sizeDelta = size;
            return this;
        }

        public void OnPointerEnter(PointerEventData eventData) {
            Log.Info("Entered CanvasButton");
            this.imgComponent_.color = this.colors.highlightedColor;
        }

        public void OnPointerExit(PointerEventData eventData) {
            Log.Info("Exited CanvasButton");
            this.imgComponent_.color = this.colors.normalColor;
        }

        public override void OnPointerDown(PointerEventData eventData) {
            // set normal color lbut darker
            this.imgComponent_.color = Color.Lerp(this.colors.normalColor, Color.black, 0.5f);
        }

        public override void OnPointerUp(PointerEventData eventData) {
            // restore mouse hover color only if still hovering
            if (eventData.hovered.Contains(this.gameObject)) {
                this.imgComponent_.color = this.colors.highlightedColor;
            }
        }
    }
}