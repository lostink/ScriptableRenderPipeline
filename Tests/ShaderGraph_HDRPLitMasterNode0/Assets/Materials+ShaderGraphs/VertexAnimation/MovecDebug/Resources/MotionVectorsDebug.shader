Shader "Hidden/MotionVectorsDebug"
{
    HLSLINCLUDE

        #include "PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        TEXTURE2D_SAMPLER2D(_CameraMotionVectorsTexture, sampler_CameraMotionVectorsTexture);

        float _Opacity;
        float _Amplitude;
        float4 _Scale;

        float4 FragMovecsOpacity(VaryingsDefault i) : SV_Target
        {
            float4 src = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);
            return float4(src.rgb * _Opacity, src.a);
        }

        // Convert a motion vector into RGBA color.
        float4 VectorToColor(float2 mv)
        {
            float phi = atan2(mv.x, mv.y);
            float hue = (phi / 3.14 + 1.0) * 0.5;

            float r = abs(hue * 6.0 - 3.0) - 1.0;
            float g = 2.0 - abs(hue * 6.0 - 2.0);
            float b = 2.0 - abs(hue * 6.0 - 4.0);
            float a = length(mv);

            return saturate(float4(r, g, b, a));
        }

        float4 FragMovecsImaging(VaryingsDefault i) : SV_Target
        {
            float4 src = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

            float2 mv = SAMPLE_TEXTURE2D(_CameraMotionVectorsTexture, sampler_CameraMotionVectorsTexture, i.texcoord).rg * _Amplitude;

        #if UNITY_UV_STARTS_AT_TOP
            mv.y *= -1.0;
        #endif

            float4 mc = VectorToColor(mv);

            float3 rgb = src.rgb;

        #if !UNITY_COLORSPACE_GAMMA
            rgb = LinearToGammaSpace(rgb);
        #endif

            rgb = lerp(rgb, mc.rgb, mc.a * _Opacity);

        #if !UNITY_COLORSPACE_GAMMA
            rgb = GammaToLinearSpace(rgb);
        #endif

            return float4(rgb, src.a);
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment FragMovecsImaging

            ENDHLSL
        }
    }
}