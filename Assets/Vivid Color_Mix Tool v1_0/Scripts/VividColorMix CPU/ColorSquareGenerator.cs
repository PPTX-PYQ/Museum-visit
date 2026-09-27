using System.Collections.Generic;
using SpRiseMChen.VividColorMix.ColorMixMethod;
using UnityEngine;
using UnityEngine.UI;

namespace SpRiseMChen.VividColorMix.ColorBandGenerator
{
    /// <summary>
    /// Color mix mode, traditional sRGB or VividColorMix.
    /// </summary>
    public enum MixType
    {
        sRGBMix = 0,
        vividColorMix = 1
    }


    /// <summary>
    /// Generate the color boxes, show the color mix gradient.
    /// </summary>
    [DisallowMultipleComponent]
    public class ColorSquareGenerator : MonoBehaviour
    {
        /// <summary>
        /// Instantiate this gameobject to get color gradient images.
        /// </summary>
        [SerializeField] private GameObject genobj;

        /// <summary>
        /// collector all auto-generated image objects.
        /// </summary>
        [SerializeField] private GameObject imagesCollector;

        /// <summary>
        /// get two mix colors from image1 and image2.
        /// </summary>
        [SerializeField] private Image img1, img2;

        /// <summary>
        /// color mix mode type.
        /// </summary>
        [SerializeField] private MixType mixType;

        /// <summary>
        /// gradient lerp steps.
        /// </summary>
        [Range(10, 100)] public int steps = 40;

        /// <summary>
        /// gradient color band's length.
        /// </summary>
        [SerializeField] float panelLen = 1200;

        /// <summary>
        /// gradient color band's height.
        /// </summary>
        [SerializeField] float imageHeight = 50;

        /// <summary>
        /// gradient band's start anchored position.
        /// </summary>
        public Vector2 anchoredStartPos = Vector2.zero;

        /// <summary>
        /// image list.
        /// </summary>
        private List<Image> imageList = new List<Image>();


        // Start is called before the first frame update
        void Start()
        {
            genobj.GetComponent<RectTransform>().sizeDelta = new Vector2(panelLen / steps, imageHeight);
            for (int i = 0; i <= steps; i++)
            {
                GameObject g = Instantiate(genobj);
                g.transform.SetParent(imagesCollector.transform);
                g.GetComponent<RectTransform>().anchoredPosition = new Vector2(anchoredStartPos.x + i * panelLen / steps, anchoredStartPos.y);
                imageList.Add(g.GetComponent<Image>());
            }
            //Debug.Log(img1.color.r);
        }


        private void FixedUpdate()
        {
            ColorMix();
        }


        /// <summary>
        /// Get the color mix gradient.
        /// </summary>
        public void ColorMix()
        {
            Color col1 = img1.color, col2 = img2.color;

            switch (mixType)
            {
                case MixType.sRGBMix:
                    for (int i = 0; i <= steps; ++i)
                    {
                        float t = (float)i / steps;
                        imageList[i].color = Color.Lerp(col1, col2, t);
                    }
                    break;
                case MixType.vividColorMix:
                    for (int i = 0; i <= steps; ++i)
                    {
                        float t = (float)i / steps;
                        imageList[i].color = VividColorMixCPU.Vivid_Color_Mix(col1, col2, t);
                    }
                    break;
                default:
                    break;
            }

        }

    }

}

