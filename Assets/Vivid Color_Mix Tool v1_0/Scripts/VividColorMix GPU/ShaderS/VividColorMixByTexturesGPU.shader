Shader "Unlit/SpRiseMChen/VividColorMixByTexturesGPU"
{
    Properties
    {
        _MainTex ("Texture 1", 2D) = "white" {}
        _MainTex2 ("Texture 2", 2D) = "white" {}
        _LerpT ("Lerp t", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv1 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv1 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex, _MainTex2;
            float4 _MainTex_ST, _MainTex2_ST;
            float _LerpT;


            // ---------     Vivid Color-Mix Algorithm in Texture with GPU     ------------
            // ------ The algorithm is consistent with the script "VividColorMixCPU" ------

            static fixed GammaCorrection(fixed srgb01_value)
            {
                if(srgb01_value < 0.04045)
                    return srgb01_value / 12.92;
                else  
                    return pow(abs(srgb01_value + 0.055) / 1.055, 2.4);
            }
            
            
            static fixed Invert_GammaCorrection(fixed lrgb_value)
            {
                 if(lrgb_value < 0.0031308)
                    return lrgb_value * 12.92;
                 else 
                    return 1.055 * pow(abs(lrgb_value), 1.0 / 2.4) - 0.055;
            }
            
            
            static fixed3 sRGB01_To_lRGB(fixed3 srgb01)
            {
                fixed r = saturate(GammaCorrection(saturate(srgb01.r)));
                fixed g = saturate(GammaCorrection(saturate(srgb01.g)));
                fixed b = saturate(GammaCorrection(saturate(srgb01.b)));
            
                return fixed3(r,g,b);
            }
            
            
            static fixed3 sRGB0255_To_lRGB(int3 srgb0255)
            {
                fixed r = GammaCorrection((fixed)srgb0255.r / 255.0);
                fixed g = GammaCorrection((fixed)srgb0255.g / 255.0);
                fixed b = GammaCorrection((fixed)srgb0255.b / 255.0);
            
                return fixed3(r,g,b);
            }
            
            
            static fixed3 lRGB_To_sRGB01(fixed3 lrgb)
            {
                fixed r = saturate(Invert_GammaCorrection(saturate(lrgb.r)));
                fixed g = saturate(Invert_GammaCorrection(saturate(lrgb.g)));
                fixed b = saturate(Invert_GammaCorrection(saturate(lrgb.b)));
            
                return fixed3(r,g,b);
            }
            
            
            static int3 lRGB_To_sRGB0255(fixed3 lrgb)
            {
                int r = saturate(Invert_GammaCorrection(lrgb.r)) * 255.0 + 0.5;
                int g = saturate(Invert_GammaCorrection(lrgb.g)) * 255.0 + 0.5;
                int b = saturate(Invert_GammaCorrection(lrgb.b)) * 255.0 + 0.5;
            
                return int3(r,g,b);
            }

            fixed4 VividColorMix(fixed4 srgb1, fixed4 srgb2, float t)
            {
                fixed3 lrgb1 = sRGB01_To_lRGB(srgb1.rgb + fixed3(0.0039, 0.0039, 0.0039));
                fixed3 lrgb2 = sRGB01_To_lRGB(srgb2.rgb + fixed3(0.0039, 0.0039, 0.0039));

                float ref1[9], ref2[9];
                ref1[0] = lrgb1.r * 0.031470668 + lrgb1.g * 0.0095358   + lrgb1.b * 0.9790333;
                ref2[0] = lrgb2.r * 0.031470668 + lrgb2.g * 0.0095358   + lrgb2.b * 0.9790333;
                ref1[1] = lrgb1.r * 0.0268726   + lrgb1.g * 0.01154674  + lrgb1.b * 0.97517794;
                ref2[1] = lrgb2.r * 0.0268726   + lrgb2.g * 0.01154674  + lrgb2.b * 0.97517794;
                ref1[2] = lrgb1.r * 0.009516875 + lrgb1.g * 0.21091549  + lrgb1.b * 0.76804;
                ref2[2] = lrgb2.r * 0.009516875 + lrgb2.g * 0.21091549  + lrgb2.b * 0.76804;
                ref1[3] = lrgb1.r * 0.0050994   + lrgb1.g * 0.9520866   + lrgb1.b * 0.06681483;
                ref2[3] = lrgb2.r * 0.0050994   + lrgb2.g * 0.9520866   + lrgb2.b * 0.06681483;
                ref1[4] = lrgb1.r * 0.051073    + lrgb1.g * 0.89332336  + lrgb1.b * 0.013839334;
                ref2[4] = lrgb2.r * 0.051073    + lrgb2.g * 0.89332336  + lrgb2.b * 0.013839334;
                ref1[5] = lrgb1.r * 0.654535    + lrgb1.g * 0.322625    + lrgb1.b * 0.0136985;
                ref2[5] = lrgb2.r * 0.654535    + lrgb2.g * 0.322625    + lrgb2.b * 0.0136985;
                ref1[6] = lrgb1.r * 0.97104996  + lrgb1.g * 0.049603604 + lrgb1.b * 0.014988;
                ref2[6] = lrgb2.r * 0.97104996  + lrgb2.g * 0.049603604 + lrgb2.b * 0.014988;
                ref1[7] = lrgb1.r * 0.98513     + lrgb1.g * 0.028878666 + lrgb1.b * 0.015551;
                ref2[7] = lrgb2.r * 0.98513     + lrgb2.g * 0.028878666 + lrgb2.b * 0.015551;
                ref1[8] = lrgb1.r * 0.98558     + lrgb1.g * 0.028120285 + lrgb1.b * 0.0155922845;
                ref2[8] = lrgb2.r * 0.98558     + lrgb2.g * 0.028120285 + lrgb2.b * 0.0155922845;
                

                float dis1 = lrgb1.r * lrgb1.r + lrgb1.g * lrgb1.g + lrgb1.b * lrgb1.b + 0.01;
                float dis2 = lrgb2.r * lrgb2.r + lrgb2.g * lrgb2.g + lrgb2.b * lrgb2.b + 0.01;
                float rate = dis1 / dis2;
                float lerp_t = t * t / (rate * (1 - t) * (1 - t) + t * t);


                float Rm = 0, Rprime = 0, X = 0, Y = 0, Z = 0;

                Rm = (1-lerp_t) * (1-ref1[0]) * (1-ref1[0]) / (2 * ref1[0]) + lerp_t * (1-ref2[0]) * (1-ref2[0]) / (2 * ref2[0]);
                Rprime = 1 + Rm - sqrt(Rm * Rm + 2 * Rm);
                X += Rprime * 0.0014046701;
                Y += Rprime * 0.00003906;
                Z += Rprime * 0.00665496;
                Rm = (1-lerp_t) * (1-ref1[1]) * (1-ref1[1]) / (2 * ref1[1]) + lerp_t * (1-ref2[1]) * (1-ref2[1]) / (2 * ref2[1]);
                Rprime = 1 + Rm - sqrt(Rm * Rm + 2 * Rm);
                X += Rprime * 0.11071678;
                Y += Rprime * 0.00789945;
                Z += Rprime * 0.5582487;
                Rm = (1-lerp_t) * (1-ref1[2]) * (1-ref1[2]) / (2 * ref1[2]) + lerp_t * (1-ref2[2]) * (1-ref2[2]) / (2 * ref2[2]);
                Rprime = 1 + Rm - sqrt(Rm * Rm + 2 * Rm);
                X += Rprime * 0.0674384;
                Y += Rprime * 0.053244937;
                Z += Rprime * 0.46310344;
                Rm = (1-lerp_t) * (1-ref1[3]) * (1-ref1[3]) / (2 * ref1[3]) + lerp_t * (1-ref2[3]) * (1-ref2[3]) / (2 * ref2[3]);
                Rprime = 1 + Rm - sqrt(Rm * Rm + 2 * Rm);
                X += Rprime * 0.095958486;
                Y += Rprime * 0.43517935;
                Z += Rprime * 0.05920551;
                Rm = (1-lerp_t) * (1-ref1[4]) * (1-ref1[4]) / (2 * ref1[4]) + lerp_t * (1-ref2[4]) * (1-ref2[4]) / (2 * ref2[4]);
                Rprime = 1 + Rm - sqrt(Rm * Rm + 2 * Rm);
                X += Rprime * 0.20877825;
                Y += Rprime * 0.25978968;
                Z += Rprime * 0.00071003;
                Rm = (1-lerp_t) * (1-ref1[5]) * (1-ref1[5]) / (2 * ref1[5]) + lerp_t * (1-ref2[5]) * (1-ref2[5]) / (2 * ref2[5]);
                Rprime = 1 + Rm - sqrt(Rm * Rm + 2 * Rm);
                X += Rprime * 0.17659217;
                Y += Rprime * 0.117268085;
                Z += Rprime * 0.00016044;
                Rm = (1-lerp_t) * (1-ref1[6]) * (1-ref1[6]) / (2 * ref1[6]) + lerp_t * (1-ref2[6]) * (1-ref2[6]) / (2 * ref2[6]);
                Rprime = 1 + Rm - sqrt(Rm * Rm + 2 * Rm);
                X += Rprime * 0.2634815;
                Y += Rprime * 0.11711134;
                Z += Rprime * 0.00005012;
                Rm = (1-lerp_t) * (1-ref1[7]) * (1-ref1[7]) / (2 * ref1[7]) + lerp_t * (1-ref2[7]) * (1-ref2[7]) / (2 * ref2[7]);
                Rprime = 1 + Rm - sqrt(Rm * Rm + 2 * Rm);
                X += Rprime * 0.02278559;
                Y += Rprime * 0.00838078;
                Rm = (1-lerp_t) * (1-ref1[8]) * (1-ref1[8]) / (2 * ref1[8]) + lerp_t * (1-ref2[8]) * (1-ref2[8]) / (2 * ref2[8]);
                Rprime = 1 + Rm - sqrt(Rm * Rm + 2 * Rm);
                X += Rprime * 0.0030079398;
                Y += Rprime * 0.00108706;


                float R = ( 3.2406 * X - 1.5372 * Y - 0.4986 * Z);
                float G = (-0.9689 * X + 1.8758 * Y + 0.0415 * Z);
                float B = ( 0.0557 * X - 0.2040 * Y + 1.0570 * Z);

                fixed3 col = lRGB_To_sRGB01(float3(R, G, B));
                return fixed4(col, 1);
            }

            // ------------------------ algorithm end ----------------------------


            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv1 = TRANSFORM_TEX(v.uv1, _MainTex);
                o.uv2 = TRANSFORM_TEX(v.uv2, _MainTex2);
                return o;
            }


            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col1 = tex2D(_MainTex, i.uv1);
                fixed4 col2 = tex2D(_MainTex2, i.uv2);

                fixed4 col = VividColorMix(col1, col2, _LerpT);

                return col;
            }

            ENDCG
        }

    }

}

