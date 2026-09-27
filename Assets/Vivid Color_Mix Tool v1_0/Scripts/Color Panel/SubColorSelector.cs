using UnityEngine;
using UnityEngine.UI;

namespace SpRiseMChen.VividColorMix.ColorPicker
{
    /// <summary>
    /// Controls the pointer on the subcolor panel to determine the final color selected.
    /// </summary>
    [DisallowMultipleComponent]
    public class SubColorSelector : MonoBehaviour
    {
        /// <summary>
        /// gameobject that contains BasicColorSlider component.
        /// </summary>
        public GameObject basicColorSlider;

        /// <summary>
        /// get the BasicColorSlider component from basicColorSlider.
        /// </summary>
        private BasicColorSlider bcs;

        /// <summary>
        /// selector icon.
        /// </summary>
        public Image selector;

        /// <summary>
        /// show the final current color on the image.
        /// </summary>
        public Image outputColorImage;

        /// <summary>
        /// show the final current color on the image text.
        /// </summary> 
        public Text outputColorText;

        /// <summary>
        /// Subcolor panel background image texture's RectTransform info.
        /// </summary>
        private RectTransform BackImageRectTrans;

        /// <summary>
        /// Subcolor panel background image texture's width.
        /// </summary>
        private float subColorImgWidth;

        /// <summary>
        /// Subcolor panel background image texture's height.
        /// </summary>
        private float subColorImgHeight;

        /// <summary>
        /// The position of the selector determines the interpolation parameter in color gradient.
        /// </summary>
        private RectTransform selectorRectTrans;

        /// <summary>
        /// the final output color.
        /// </summary>
        private Color outputColor;


        // Start is called before the first frame update
        void Start()
        {
            selectorRectTrans = selector.GetComponent<RectTransform>();

            BackImageRectTrans = this.GetComponent<RectTransform>();
            subColorImgHeight = BackImageRectTrans.rect.height;
            subColorImgWidth = BackImageRectTrans.rect.width;

            bcs = basicColorSlider.GetComponent<BasicColorSlider>();

            RefreshColor();
            outputColorImage.color = outputColor;
            outputColorText.text = "(" + (int)(outputColor.r * 255 + 0.5f) + ", " + (int)(outputColor.g * 255 + 0.5f) + ", " + (int)(outputColor.b * 255 + 0.5f) + ")";
        }


        /// <summary>
        /// Refresh the final output color when color selector moves or color slider value changes.
        /// </summary>
        public void RefreshColor()
        {
            float posx = selectorRectTrans.anchoredPosition.x;
            float posy = selectorRectTrans.anchoredPosition.y;

            Color col = bcs.GetColor();
            Color c = col * (posx / subColorImgWidth) + Color.white * (1 - posx / subColorImgWidth);
            outputColor = c * (posy / subColorImgHeight) + Color.black * (1 - posy / subColorImgHeight);

            outputColorImage.color = outputColor;
            outputColorText.text = "(" + (int)(outputColor.r * 255 + 0.5f) + ", " + (int)(outputColor.g * 255 + 0.5f) + ", " + (int)(outputColor.b * 255 + 0.5f) + ")";
        }


        /// <summary>
        /// This method is called when the mouse first click on the selector of the child color panel.
        /// This method is called in the EventTrigger in gameobjct 'SubColorImg'
        /// </summary>
        public void SelectColor_Click()
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(BackImageRectTrans, Input.mousePosition) == false)
                return;

            Vector2 screenPoint = Input.mousePosition;
            selector.transform.position = screenPoint;

            float posx = selectorRectTrans.anchoredPosition.x;
            float posy = selectorRectTrans.anchoredPosition.y;

            Color col = bcs.GetColor();
            Color c = col * (posx / subColorImgWidth) + Color.white * (1 - posx / subColorImgWidth);
            outputColor = c * (posy / subColorImgHeight) + Color.black * (1 - posy / subColorImgHeight);

            outputColorImage.color = outputColor;
            outputColorText.text = "(" + (int)(outputColor.r * 255 + 0.5f) + ", " + (int)(outputColor.g * 255 + 0.5f) + ", " + (int)(outputColor.b * 255 + 0.5f) + ")";
        }


        /// <summary>
        /// This method is called when the mouse is dragging the selector of the child color panel.
        /// This method is called in the EventTrigger in gameobjct 'SubColorImg'
        /// </summary>
        public void SelectColor_Drag()
        {
            Vector2 screenPoint = Input.mousePosition;
            selector.transform.position = screenPoint;

            float xrange = Mathf.Max(0, Mathf.Min(subColorImgWidth, selectorRectTrans.anchoredPosition.x));
            float yrange = Mathf.Max(0, Mathf.Min(subColorImgHeight, selectorRectTrans.anchoredPosition.y));
            selectorRectTrans.anchoredPosition = new Vector2(xrange, yrange);

            float posx = selectorRectTrans.anchoredPosition.x;
            float posy = selectorRectTrans.anchoredPosition.y;

            Color col = bcs.GetColor();
            Color c = col * (posx / subColorImgWidth) + Color.white * (1 - posx / subColorImgWidth);
            outputColor = c * (posy / subColorImgHeight) + Color.black * (1 - posy / subColorImgHeight);

            outputColorImage.color = outputColor;
            outputColorText.text = "(" + (int)(outputColor.r * 255 + 0.5f) + ", " + (int)(outputColor.g * 255 + 0.5f) + ", " + (int)(outputColor.b * 255 + 0.5f) + ")";
        }

    }
}

