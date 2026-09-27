using UnityEngine;
using UnityEngine.UI;


namespace SpRiseMChen.VividColorMix.ColorMixShaderMethod
{
    /// <summary>
    /// Use the slider to control lerp value of VividColorMix shader.
    /// </summary>
    public class ShaderSliderController : MonoBehaviour
    {

        /// <summary>
        /// slider which controls the lerp value of VividColorMix shader.
        /// </summary>
        public Slider slider;

        /// <summary>
        /// slider value text.
        /// </summary>
        public Text sliderTxt;

        /// <summary>
        /// material which contains VividColorMix shader.
        /// </summary>
        private Material colorMixMat;


        // Start is called before the first frame update
        void Start()
        {
            colorMixMat = this.GetComponent<Image>().material;
            OnSliderValueChanged();
        }


        /// <summary>
        /// Called when slider value changes.
        /// </summary>
        public void OnSliderValueChanged()
        {
            float value = slider.value;
            colorMixMat.SetFloat("_LerpT", value);
            sliderTxt.text = value.ToString();
        }

    }

}

