using UnityEngine;
using UnityEngine.UI;

namespace SpRiseMChen.VividColorMix.ColorPicker
{
    /// <summary>
    /// Change the color slider value and get the color you want.
    /// </summary>
    [DisallowMultipleComponent]
    public class BasicColorSlider : MonoBehaviour
    {        
        /// <summary>
        /// slider's color gradient texture.
        /// </summary>
        private Texture2D backgroundTex;

        /// <summary>
        /// the slider.
        /// </summary>
        private Slider colorSlider;

        /// <summary>
        /// background texture's height in pixel.
        /// </summary>
        private int sliderBgPixelHeight;

        /// <summary>
        /// current slider color.
        /// </summary>
        private Color currentColor;

        /// <summary>
        /// Sub-color panel image.
        /// </summary>
        public Image subColorImg;

        /// <summary>
        /// use this to get slider's color gradient texture.
        /// </summary>
        public GameObject BasicColorBackground;

        /// <summary>
        /// the gameobject contains the SubColorSelector script(component).
        /// </summary>
        public GameObject selectorGO;

        /// <summary>
        /// initialize color.
        /// </summary>
        public Color InitColor = Color.yellow;

        /// <summary>
        /// class SubColorSelector object.
        /// </summary>
        private SubColorSelector subSelector;


        private void Awake()
        {
            // Initialization
            subSelector = selectorGO.GetComponent<SubColorSelector>();
            colorSlider = GetComponent<Slider>();

            backgroundTex = (Texture2D)BasicColorBackground.GetComponent<Image>().mainTexture;
            sliderBgPixelHeight = backgroundTex.height;

            currentColor = InitColor;
            SetSubColorPanel(currentColor);
        }


        /// <summary>
        /// Let other class Get the slider's current color value.
        /// </summary>
        /// <returns>the slider's current color value</returns>
        public Color GetColor()
        {
            return currentColor;
        }


        /// <summary>
        /// Get the color corresponding to the current value of the slider.
        /// Call this mathod when slider value changes.
        /// </summary>
        public void OnSliderValueChanged()
        {
            float f = 1 - colorSlider.value;
            Color col = backgroundTex.GetPixel(0, (int)(f * sliderBgPixelHeight));
            currentColor = col;

            SetSubColorPanel(col);
            subSelector.RefreshColor();
        }


        /// <summary>
        /// Set the color gradient for the sub-color palette panel.
        /// </summary>
        /// <param name="col">slider value color</param>
        private void SetSubColorPanel(Color col)
        {
            subColorImg.material.SetColor("_Color", col);
        }

    }

}

