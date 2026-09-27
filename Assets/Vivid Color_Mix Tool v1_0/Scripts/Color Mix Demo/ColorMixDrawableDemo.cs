using UnityEngine;
using UnityEngine.UI;

namespace SpRiseMChen.VividColorMix.ColorMixDemo
{
    /// <summary>
    /// Drawable Demo, to let users draw and check the differences between RGB-Mix and Vivid-Color-Mix.
    /// </summary>
    public class ColorMixDrawableDemo : MonoBehaviour
    {
        /// <summary>
        /// canvas pixel resolution, x for width, y for height.
        /// </summary>
        public Vector2Int canvasResolution;

        /// <summary>
        /// canvas image material.
        /// </summary>
        public Material canvasMat;


        /// <summary>
        /// final result canvas render texture.
        /// </summary>
        private RenderTexture canvasTex;

        /// <summary>
        /// render texture that records the strokes which are already been drawn.
        /// </summary>
        private RenderTexture alreadyDrawTex;

        /// <summary>
        /// render texture that records the current new drawn stroke.
        /// </summary>
        private RenderTexture curDrawingTex;


        /// <summary>
        /// this demo uses compute shader to draw on canvas's texture. 
        /// </summary>
        public ComputeShader canvasComputeShader;

        /// <summary>
        /// image used to show the current color-picker's color;
        /// </summary>
        public Image currentColorIMG;

        /// <summary>
        /// slider used to control color's alpha value, which is used as the lerp value in Vivid-Color-Mix function.
        /// </summary>
        public Slider alphaSlider;

        /// <summary>
        /// text to show alpha value.
        /// </summary>
        public Text alphaSliderTxt;

        /// <summary>
        /// Toggle: whether to draw sample pattern when refresh the canvas.
        /// </summary>
        public Toggle drawExampleWhenRefresh;

        /// <summary>
        /// canvas width resolution.
        /// </summary>
        public int CanvasResolutionX { get { return canvasResolution_X; } }

        /// <summary>
        /// canvas height resolution.
        /// </summary>
        public int CanvasResolutionY { get { return canvasResolution_Y; } }

        /// <summary>
        /// canvas width resolution.
        /// </summary>
        private int canvasResolution_X;

        /// <summary>
        /// canvas height resolution.
        /// </summary>
        private int canvasResolution_Y;

        /// <summary>
        /// canvas RectTransform component.
        /// </summary>
        private RectTransform canvasRectTrans;

        // compute shader kernels
        private int kernel_DrawCanvas, kernel_InitCanvas;
        private int kernel_RefreshCurDrawingTex, kernel_DrawToCurTex;
        private int kernel_DrawCircle, kernel_DrawRect;


        // Start is called before the first frame update
        void Start()
        {
            CanvasInit(canvasResolution.x, canvasResolution.y);

            alphaSlider.value = 0.5f;

            // DrawSampleCanvas();

            Time.fixedDeltaTime = 0.05f;
        }


        // Update is called once per frame
        void Update()
        {
            var x = Input.mousePosition;
            if (IfScreenPositionInCanvas(x) == false) return;

            // set brush color.
            Color col = currentColorIMG.color;
            col.a = alphaSlider.value;
            canvasComputeShader.SetVector("brushColor", col);

            // Press left mouse to draw stroke.
            if (Input.GetMouseButton(0))
            {
                var v = ScreenPositionToCanvasPosition(x);

                canvasComputeShader.SetFloat("mousePos_x", v.x);
                canvasComputeShader.SetFloat("mousePos_y", v.y);
                canvasComputeShader.Dispatch(kernel_DrawToCurTex, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);

            }
            else if (Input.GetMouseButtonUp(0))
            {
                canvasComputeShader.Dispatch(kernel_DrawCanvas, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);
                canvasComputeShader.Dispatch(kernel_RefreshCurDrawingTex, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);
            }

        }


        /// <summary>
        /// Reset canvas and use Vivid-Color-Mix.
        /// </summary>
        public void Btn_ResetCanvas_VividColorMix()
        {
            canvasComputeShader.Dispatch(kernel_InitCanvas, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);
            canvasComputeShader.SetBool("useVividColorMix", true);

            // if (drawExampleWhenRefresh.isOn)
            //     DrawSampleCanvas();
        }

        /// <summary>
        /// Reset canvas and use RGB-Color-Mix.
        /// </summary>
        public void Btn_ResetCanvas_sRGBMix()
        {
            canvasComputeShader.Dispatch(kernel_InitCanvas, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);
            canvasComputeShader.SetBool("useVividColorMix", false);

            // if (drawExampleWhenRefresh.isOn)
            //     DrawSampleCanvas();
        }

        /// <summary>
        /// Called when alpha slider value changes.
        /// </summary>
        public void OnAlphaSliderChanged()
        {
            alphaSliderTxt.text = "Alpha: " + alphaSlider.value;
        }


        /// <summary>
        /// Draw sample pattern to canvas.
        /// </summary>
        private void DrawSampleCanvas()
        {
            // Draw Color Rectangle.
            canvasComputeShader.SetVector("brushColor", new Color(0, 0, 1, 0.7f));
            canvasComputeShader.SetVector("rect_topLeftbottomRight", new Vector4(170, 460, 800, 170));
            canvasComputeShader.Dispatch(kernel_DrawRect, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);
            canvasComputeShader.Dispatch(kernel_DrawCanvas, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);
            canvasComputeShader.Dispatch(kernel_RefreshCurDrawingTex, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);

            // Draw Color Circle.
            canvasComputeShader.SetVector("brushColor", new Color(1, 1, 0, 0.6f));
            canvasComputeShader.SetVector("circle_center_SQRradius", new Vector4(450, 430, 170 * 170, 0));
            canvasComputeShader.Dispatch(kernel_DrawCircle, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);
            canvasComputeShader.Dispatch(kernel_DrawCanvas, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);
            canvasComputeShader.Dispatch(kernel_RefreshCurDrawingTex, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);

            // Draw Color Circle.
            canvasComputeShader.SetVector("brushColor", new Color(1, 0.05f, 0, 0.4f));
            canvasComputeShader.SetVector("circle_center_SQRradius", new Vector4(490, 200, 160 * 160, 0));
            canvasComputeShader.Dispatch(kernel_DrawCircle, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);
            canvasComputeShader.Dispatch(kernel_DrawCanvas, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);
            canvasComputeShader.Dispatch(kernel_RefreshCurDrawingTex, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);

            // Draw Color Rectangle.
            canvasComputeShader.SetVector("brushColor", new Color(0.1f, 0.8f, 0.1f, 0.5f));
            canvasComputeShader.SetVector("rect_topLeftbottomRight", new Vector4(110, 610, 220, 215));
            canvasComputeShader.Dispatch(kernel_DrawRect, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);
            canvasComputeShader.Dispatch(kernel_DrawCanvas, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);
            canvasComputeShader.Dispatch(kernel_RefreshCurDrawingTex, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);

            // Draw Color Circle.
            canvasComputeShader.SetVector("brushColor", new Color(1, 0.1f, 1, 0.65f));
            canvasComputeShader.SetVector("circle_center_SQRradius", new Vector4(270, 530, 120 * 120, 0));
            canvasComputeShader.Dispatch(kernel_DrawCircle, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);
            canvasComputeShader.Dispatch(kernel_DrawCanvas, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);
            canvasComputeShader.Dispatch(kernel_RefreshCurDrawingTex, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);

            // Draw Color Rectangle.
            canvasComputeShader.SetVector("brushColor", new Color(0.05f, 0.8f, 0.8f, 0.4f));
            canvasComputeShader.SetVector("rect_topLeftbottomRight", new Vector4(270, 672, 700, 530));
            canvasComputeShader.Dispatch(kernel_DrawRect, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);
            canvasComputeShader.Dispatch(kernel_DrawCanvas, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);
            canvasComputeShader.Dispatch(kernel_RefreshCurDrawingTex, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);

            // Draw Color Circle.
            canvasComputeShader.SetVector("brushColor", new Color(1, 0.3f, 0, 0.4f));
            canvasComputeShader.SetVector("circle_center_SQRradius", new Vector4(710, 500, 160 * 160, 0));
            canvasComputeShader.Dispatch(kernel_DrawCircle, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);
            canvasComputeShader.Dispatch(kernel_DrawCanvas, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);
            canvasComputeShader.Dispatch(kernel_RefreshCurDrawingTex, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);
        }


        /// <summary>
        /// Initialize the canvas properties.
        /// </summary>
        /// <param name="resolutionX"> canvas width </param>
        /// <param name="resolutionY"> canvas height </param>
        public void CanvasInit(int resolutionX, int resolutionY)
        {
            // set canvas resolution.
            canvasResolution_X = resolutionX;
            canvasResolution_Y = resolutionY;


            // create render textures
            canvasTex = new RenderTexture(resolutionX, resolutionY, 0, RenderTextureFormat.ARGBInt);
            canvasTex.enableRandomWrite = true;
            canvasTex.Create();

            curDrawingTex = new RenderTexture(resolutionX, resolutionY, 0, RenderTextureFormat.ARGBInt);
            curDrawingTex.enableRandomWrite = true;
            curDrawingTex.Create();

            alreadyDrawTex = new RenderTexture(resolutionX, resolutionY, 0, RenderTextureFormat.ARGBInt);
            alreadyDrawTex.enableRandomWrite = true;
            alreadyDrawTex.Create();


            // set canvas image resolution size.
            canvasRectTrans = GetComponent<RectTransform>();
            canvasRectTrans.sizeDelta = new Vector2(resolutionX, resolutionY);

            // set texture to unlit shader
            canvasMat.SetTexture("_MainTex", canvasTex);

            // set resolution to compute shader.
            canvasComputeShader.SetInt("resolutionX", resolutionX);
            canvasComputeShader.SetInt("resolutionY", resolutionY);



            // kernel_InitCanvas
            kernel_InitCanvas = canvasComputeShader.FindKernel("InitCanvas");
            canvasComputeShader.SetTexture(kernel_InitCanvas, "ResultCanvas", canvasTex);
            canvasComputeShader.SetTexture(kernel_InitCanvas, "AlreadyDrawTex", alreadyDrawTex);


            // kernel_DrawCanvas
            kernel_DrawCanvas = canvasComputeShader.FindKernel("DrawToCanvas");
            canvasComputeShader.SetTexture(kernel_DrawCanvas, "ResultCanvas", canvasTex);
            canvasComputeShader.SetTexture(kernel_DrawCanvas, "CurDrawingTex", curDrawingTex);
            canvasComputeShader.SetTexture(kernel_DrawCanvas, "AlreadyDrawTex", alreadyDrawTex);


            // kernel_RefreshCurDrawingTex
            kernel_RefreshCurDrawingTex = canvasComputeShader.FindKernel("RefreshCurDrawingTex");
            canvasComputeShader.SetTexture(kernel_RefreshCurDrawingTex, "CurDrawingTex", curDrawingTex);

            // kernel_DrawToCurTex
            kernel_DrawToCurTex = canvasComputeShader.FindKernel("DrawToCurTex");
            canvasComputeShader.SetTexture(kernel_DrawToCurTex, "CurDrawingTex", curDrawingTex);
            canvasComputeShader.SetTexture(kernel_DrawToCurTex, "ResultCanvas", canvasTex);
            canvasComputeShader.SetTexture(kernel_DrawToCurTex, "AlreadyDrawTex", alreadyDrawTex);

            // Dispatch kernel functions for canvas initialization.
            canvasComputeShader.Dispatch(kernel_InitCanvas, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);
            canvasComputeShader.Dispatch(kernel_RefreshCurDrawingTex, canvasResolution_X / 16 + 1, canvasResolution_Y / 16 + 1, 1);


            // Draw circle.
            kernel_DrawCircle = canvasComputeShader.FindKernel("DrawCircle");
            canvasComputeShader.SetTexture(kernel_DrawCircle, "CurDrawingTex", curDrawingTex);
            canvasComputeShader.SetTexture(kernel_DrawCircle, "ResultCanvas", canvasTex);
            canvasComputeShader.SetTexture(kernel_DrawCircle, "AlreadyDrawTex", alreadyDrawTex);

            // Draw rectangle.
            kernel_DrawRect = canvasComputeShader.FindKernel("DrawRect");
            canvasComputeShader.SetTexture(kernel_DrawRect, "CurDrawingTex", curDrawingTex);
            canvasComputeShader.SetTexture(kernel_DrawRect, "ResultCanvas", canvasTex);
            canvasComputeShader.SetTexture(kernel_DrawRect, "AlreadyDrawTex", alreadyDrawTex);

            // Use Vivid-Color-Mix by default.
            canvasComputeShader.SetBool("useVividColorMix", true);


            Debug.Log("Rect Width " + this.GetComponent<RectTransform>().rect.width);
            Debug.Log("Rect height " + this.GetComponent<RectTransform>().rect.height);
            Debug.Log("anchored pos " + canvasRectTrans.anchoredPosition);
        }


        /// <summary>
        /// Convert screen position to canvas local position.
        /// If go beyond the canvas scope, the return result is automatically limited to the canvas scope.
        /// </summary>
        /// <param name="screenPosition"> Vector2 screen position </param>
        /// <returns> Vector2 canvas position </returns>
        public Vector2 ScreenPositionToCanvasPosition(Vector2 screenPosition)
        {
            float scaleRate = canvasRectTrans.lossyScale.x;

            Vector2 v = screenPosition - new Vector2(Screen.width / 2, Screen.height / 2);

            Vector2 pos = (v - canvasRectTrans.anchoredPosition) / scaleRate + new Vector2(canvasResolution_X / 2, canvasResolution_Y / 2);

            pos = new Vector2(Mathf.Min(canvasResolution_X, Mathf.Max(0, pos.x)), Mathf.Min(canvasResolution_Y, Mathf.Max(0, pos.y)));

            return pos;
        }


        /// <summary>
        /// Converts the current canvas pixel position to screen position.
        /// </summary>
        /// <param name="canvasPosition"> Vector2 canvas position </param>
        /// <returns> Vector2 screen position </returns>
        public Vector2 CanvasPositionToScreenPosition(Vector2 canvasPosition)
        {
            float scaleRate = canvasRectTrans.lossyScale.x;

            Vector2 pos = (canvasPosition - new Vector2(canvasResolution_X / 2, canvasResolution_Y / 2)) * scaleRate + canvasRectTrans.anchoredPosition;

            Vector2 v = pos + new Vector2(Screen.width / 2, Screen.height / 2);

            return v;
        }


        /// <summary>
        /// Detects whether the current screen drop position is in the boundaries of the canvas.
        /// </summary>
        /// <param name="screenPosition"> Vector2 screen position </param>
        /// <returns> whether the screen position is in the boundaries of the canvas. </returns>
        public bool IfScreenPositionInCanvas(Vector2 screenPosition)
        {
            float scaleRate = canvasRectTrans.lossyScale.x;

            Vector2 v = screenPosition - new Vector2(Screen.width / 2, Screen.height / 2);

            Vector2 pos = (v - canvasRectTrans.anchoredPosition) / scaleRate + new Vector2(canvasResolution_X / 2, canvasResolution_Y / 2);

            if (pos.x < 0 || pos.x >= canvasResolution_X || pos.y < 0 || pos.y > canvasResolution_Y)
                return false;

            return true;
        }


        /// <summary>
        /// OnDestroy
        /// </summary>
        private void OnDestroy()
        {
            canvasTex.Release();
        }

    }
}

