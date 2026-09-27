using System;
using UnityEngine;


namespace SpRiseMChen.VividColorMix.ColorMixMethod
{
    /// <summary>
    /// CPU/C# version of Vivid-Color-Mix.
    /// Call function Vivid_Color_Mix(srgb1, srgb2, t) to get your color mixture result.
    /// </summary>
    public static class VividColorMixCPU
    {
        // Spectral curve parameters & Color Match Function parameters.
        private static readonly float[] StrR = { 0.031470668f, 0.0268726f, 0.009516875f, 0.0050994f, 0.051073f, 0.654535f, 0.97104996f, 0.98513f, 0.98558f };
        private static readonly float[] StrG = { 0.0095358f, 0.01154674f, 0.21091549f, 0.9520866f, 0.89332336f, 0.322625f, 0.049603604f, 0.028878666f, 0.028120285f };
        private static readonly float[] StrB = { 0.9790333f, 0.97517794f, 0.76804f, 0.06681483f, 0.013839334f, 0.0136985f, 0.014988f, 0.015551f, 0.0155922845f };
        private static readonly float[] StrX = { 0.0014046701f, 0.11071678f, 0.0674384f, 0.095958486f, 0.20877825f, 0.17659217f, 0.2634815f, 0.02278559f, 0.0030079398f };
        private static readonly float[] StrY = { 3.906E-05f, 0.00789945f, 0.053244937f, 0.43517935f, 0.25978968f, 0.117268085f, 0.11711134f, 0.00838078f, 0.00108706f };
        private static readonly float[] StrZ = { 0.00665496f, 0.5582487f, 0.46310344f, 0.05920551f, 0.00071002997f, 0.00016044f, 5.0119997E-05f, 0f, 0f };


        /// <summary>
        /// Input srgb color, and get its spectral reflectance data.
        /// </summary>
        /// <param name="srgb">input srgb color, notice that value range [0,1]</param>
        /// <returns>Spectral reflectance of the input color</returns>
        private static float[] sRGB_To_Reflectance(Color srgb)
        {
            float[] lRGB = Util_sRGB_To_LinearRGB(srgb.r, srgb.g, srgb.b);

            float[] reflectance = new float[9];
            for (int i = 0; i < 9; ++i)
            {
                reflectance[i] = lRGB[0] * StrR[i] + lRGB[1] * StrG[i] + lRGB[2] * StrB[i];
            }
            return reflectance;
        }


        /// <summary>
        /// Main algorithm of Vivid Color-Mix Tool.
        /// </summary>
        /// <param name="srgb1">first color to be mixed</param>
        /// <param name="srgb2">second color to be mixed</param>
        /// <param name="t">lerp t</param>
        /// <returns>the mixture result color</returns>
        public static Color Vivid_Color_Mix(Color srgb1, Color srgb2, float t)
        {
            // Prevents the blackbody spectrum at sRGB(0,0,0), resulting in a blend transition to all black(blackbody)
            // Untiy's Color can automatically clamp Color value in range [0, 1], so it can't break the ceil value 1.
            srgb1 += new Color(0.004f, 0.004f, 0.004f);
            srgb2 += new Color(0.004f, 0.004f, 0.004f);


            float[] R1 = sRGB_To_Reflectance(srgb1);
            float[] R2 = sRGB_To_Reflectance(srgb2);


            float dis1, dis2;
            dis1 = srgb1.r * srgb1.r + srgb1.g * srgb1.g + srgb1.b * srgb1.b + 1e-2f;
            dis2 = srgb2.r * srgb2.r + srgb2.g * srgb2.g + srgb2.b * srgb2.b + 1e-2f;


            float rate = dis1 / dis2;
            float con_t = t * t / (rate * (1 - t) * (1 - t) + t * t);


            float X = 0, Y = 0, Z = 0;
            for (int i = 0; i < 9; ++i)
            {
                float KS = (1 - con_t) * ((1 - R1[i]) * (1 - R1[i]) / (2 * R1[i])) + con_t * ((1 - R2[i]) * (1 - R2[i]) / (2 * R2[i]));
                float KM = 1 + KS - MathF.Sqrt(KS * KS + 2 * KS);

                X += KM * StrX[i];
                Y += KM * StrY[i];
                Z += KM * StrZ[i];
            }

            float R = ( 3.2406f * X - 1.5372f * Y - 0.4986f * Z);
            float G = (-0.9689f * X + 1.8758f * Y + 0.0415f * Z);
            float B = ( 0.0557f * X - 0.2040f * Y + 1.0570f * Z);

            return Util_LinearRGB_To_sRGB(R, G, B);
        }



        /// <summary>
        /// Transform linear RGB into sRGB and output it.
        /// </summary>
        /// <param name="lr">linear R</param>
        /// <param name="lg">linear G</param>
        /// <param name="lb">linear B</param>
        /// <returns>color value in RGB</returns>
        private static Color Util_LinearRGB_To_sRGB(float lr, float lg, float lb)
        {
            float R = Util_Gamma_LinearToSRGB(Util_Clamp01(lr));
            float G = Util_Gamma_LinearToSRGB(Util_Clamp01(lg));
            float B = Util_Gamma_LinearToSRGB(Util_Clamp01(lb));

            return new Color(R, G, B);
        }


        /// <summary>
        /// Transform sRGB into linear RGB and output it.
        /// </summary>
        /// <param name="sr">sRGB R</param>
        /// <param name="sg">sRGB G</param>
        /// <param name="sb">sRGB B</param>
        /// <returns>color value in linear RGB</returns>
        private static float[] Util_sRGB_To_LinearRGB(float sr, float sg, float sb)
        {
            float LR = Util_Gamma_SRGBToLinear(sr);
            float LG = Util_Gamma_SRGBToLinear(sg);
            float LB = Util_Gamma_SRGBToLinear(sb);

            return new float[] { LR, LG, LB };
        }


        /// <summary>
        /// Gamma correction.
        /// </summary>
        /// <param name="x">sRGB component r, g and b</param>
        /// <returns>linear RGB component r, g and b</returns>
        private static float Util_Gamma_SRGBToLinear(float x)
        {
            return (x >= 0.04045f) ? (float)Math.Pow((x + 0.055f) / 1.055f, 2.4f) : x / 12.92f;
        }


        /// <summary>
        /// Invert Gamma correction.
        /// </summary>
        /// <param name="x">linear RGB component r, g and b</param>
        /// <returns>sRGB component r, g and b</returns>
        private static float Util_Gamma_LinearToSRGB(float x)
        {
            return (x >= 0.0031308f) ? 1.055f * ((float)Math.Pow(x, 1.0f / 2.4f)) - 0.055f : 12.92f * x;
        }


        /// <summary>
        /// Clamp linear RGB components in range [0, 1]
        /// </summary>
        /// <param name="x">r, g and b component of linear RGB</param>
        /// <returns>the clamped linear RGB component value</returns>
        private static float Util_Clamp01(float x)
        {
            return x < 0.0f ? 0.0f : x > 1.0f ? 1.0f : x;
        }


        /// <summary>
        /// Clamp sRGB components in range [0, 255]
        /// </summary>
        /// <param name="x">r, g and b component of sRGB</param>
        /// <returns>the clamped sRGB component value</returns>
        private static int Util_Clamp0255(int x)
        {
            return x < 0 ? 0 : x > 255 ? 255 : x;
        }

    }

}
