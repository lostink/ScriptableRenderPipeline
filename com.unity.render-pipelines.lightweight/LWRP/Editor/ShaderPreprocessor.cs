//#define LOG_VARIANTS
//#define LOG_ONLY_LWRP_VARIANTS

using System.Collections.Generic;
using UnityEditor.Build;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.LightweightPipeline;
using UnityEngine.Rendering;

namespace UnityEditor.Experimental.Rendering.LightweightPipeline
{
    public static class LitShaderKeywords
    {
        public static readonly ShaderKeyword RealtimeDirectionalShadows = new ShaderKeyword(ShaderKeywordStrings.RealtimeDirectionalShadows);
        public static readonly ShaderKeyword RealtimePunctualLightsVertex = new ShaderKeyword(ShaderKeywordStrings.RealtimePunctualLightsVertex);
        public static readonly ShaderKeyword RealtimePunctualLights = new ShaderKeyword(ShaderKeywordStrings.RealtimePunctualLights);
        public static readonly ShaderKeyword RealtimePunctualLightShadows = new ShaderKeyword(ShaderKeywordStrings.RealtimePunctualLightShadows);
        public static readonly ShaderKeyword CascadeShadows = new ShaderKeyword(ShaderKeywordStrings.CascadeShadows);
        public static readonly ShaderKeyword SoftShadows = new ShaderKeyword(ShaderKeywordStrings.SoftShadows);
        public static readonly ShaderKeyword MixedLightingSubtractive = new ShaderKeyword(ShaderKeywordStrings.MixedLightingSubtractive);

        public static readonly ShaderKeyword Lightmap = new ShaderKeyword("LIGHTMAP_ON");
        public static readonly ShaderKeyword DirectionalLightmap = new ShaderKeyword("DIRLIGHTMAP_COMBINED");
    }

    internal class ShaderPreprocessor : IPreprocessShaders
    {
#if LOG_VARIANTS
        int m_TotalVariantsInputCount;
        int m_TotalVariantsOutputCount;
#endif

        // Multiple callback may be implemented.
        // The first one executed is the one where callbackOrder is returning the smallest number.
        public int callbackOrder { get { return 0; } }

        bool StripUnusedShader(ShaderFeatures features, Shader shader)
        {
            if (shader.name.Contains("Debug"))
                return true;

            if (shader.name.Contains("HDRenderPipeline"))
                return true;

            if (!CoreUtils.HasFlag(features, ShaderFeatures.RealtimeDirectionalLightShadows) &&
                shader.name.Contains("ScreenSpaceShadows"))
                return true;

            return false;
        }

        bool StripUnusedPass(ShaderFeatures features, ShaderSnippetData snippetData)
        {
            if (snippetData.passType == PassType.Meta)
                return true;

            if (snippetData.passType == PassType.ShadowCaster)
                if (!CoreUtils.HasFlag(features, ShaderFeatures.RealtimeDirectionalLightShadows) && !CoreUtils.HasFlag(features, ShaderFeatures.RealtimePunctualLightShadows))
                    return true;

            return false;
        }

        bool StripUnusedFeatures(ShaderFeatures features, ShaderCompilerData compilerData)
        {
            if (compilerData.shaderKeywordSet.IsEnabled(LitShaderKeywords.RealtimeDirectionalShadows) &&
                !CoreUtils.HasFlag(features, ShaderFeatures.RealtimeDirectionalLightShadows))
                return true;

            if (compilerData.shaderKeywordSet.IsEnabled(LitShaderKeywords.RealtimePunctualLightsVertex) &&
                !CoreUtils.HasFlag(features, ShaderFeatures.RealtimePunctualLightsVertex))
                return true;

            if (compilerData.shaderKeywordSet.IsEnabled(LitShaderKeywords.RealtimePunctualLights) &&
                !CoreUtils.HasFlag(features, ShaderFeatures.RealtimePunctualLights))
                return true;

            if (compilerData.shaderKeywordSet.IsEnabled(LitShaderKeywords.RealtimePunctualLightShadows) &&
                !CoreUtils.HasFlag(features, ShaderFeatures.RealtimePunctualLightShadows))
                return true;

            if (compilerData.shaderKeywordSet.IsEnabled(LitShaderKeywords.SoftShadows) &&
                !CoreUtils.HasFlag(features, ShaderFeatures.SoftShadows))
                return true;

            if (compilerData.shaderKeywordSet.IsEnabled(LitShaderKeywords.MixedLightingSubtractive) &&
                !CoreUtils.HasFlag(features, ShaderFeatures.MixedLighting))
                return true;

            return false;
        }

        bool StripUnsupportedVariants(ShaderCompilerData compilerData)
        {
            // Dynamic GI is not supported so we can strip variants that have directional lightmap
            // enabled but not baked lightmap.
            if (compilerData.shaderKeywordSet.IsEnabled(LitShaderKeywords.DirectionalLightmap) &&
                !compilerData.shaderKeywordSet.IsEnabled(LitShaderKeywords.Lightmap))
                return true;

            if (compilerData.shaderCompilerPlatform == ShaderCompilerPlatform.GLES20)
            {
                if (compilerData.shaderKeywordSet.IsEnabled(LitShaderKeywords.CascadeShadows))
                    return true;
            }

            return false;
        }

        bool StripInvalidVariants(ShaderCompilerData compilerData)
        {
            bool isDirectionalShadows = compilerData.shaderKeywordSet.IsEnabled(LitShaderKeywords.RealtimeDirectionalShadows);
            bool isPunctualShadows = compilerData.shaderKeywordSet.IsEnabled(LitShaderKeywords.RealtimePunctualLightShadows);
            bool isShadowVariant = isDirectionalShadows || isPunctualShadows;

            if (!isDirectionalShadows && compilerData.shaderKeywordSet.IsEnabled(LitShaderKeywords.CascadeShadows))
                return true;

            if (!isShadowVariant && compilerData.shaderKeywordSet.IsEnabled(LitShaderKeywords.SoftShadows))
                return true;

            if (isPunctualShadows && !compilerData.shaderKeywordSet.IsEnabled(LitShaderKeywords.RealtimePunctualLights)) 
                return true;

            return false;
        }

        bool StripUnused(ShaderFeatures features, Shader shader, ShaderSnippetData snippetData, ShaderCompilerData compilerData)
        {
            if (StripUnusedShader(features, shader))
                return true;

            if (StripUnusedPass(features, snippetData))
                return true;

            if (StripUnusedFeatures(features, compilerData))
                return true;

            if (StripUnsupportedVariants(compilerData))
                return true;

            if (StripInvalidVariants(compilerData))
                return true;

            return false;
        }

#if LOG_VARIANTS
        void LogVariants(Shader shader, ShaderSnippetData snippetData, int prevVariantsCount, int currVariantsCount)
        {
#if LOG_ONLY_LWRP_VARIANTS
            if (shader.name.Contains("LightweightPipeline"))
#endif
            {
                float percentageCurrent = (float)currVariantsCount / (float)prevVariantsCount * 100f;
                float percentageTotal = (float)m_TotalVariantsOutputCount / (float)m_TotalVariantsInputCount * 100f;

                string result = string.Format("STRIPPING: {0} ({1} pass) ({2}) -" +
                        " Remaining shader variants = {3}/{4} = {5}% - Total = {6}/{7} = {8}%",
                        shader.name, snippetData.passName, snippetData.shaderType.ToString(), currVariantsCount,
                        prevVariantsCount, percentageCurrent, m_TotalVariantsOutputCount, m_TotalVariantsInputCount,
                        percentageTotal);
                Debug.Log(result);
            }
        }

#endif

        public void OnProcessShader(Shader shader, ShaderSnippetData snippetData, IList<ShaderCompilerData> compilerDataList)
        {
            LightweightRenderPipeline lw = RenderPipelineManager.currentPipeline as LightweightRenderPipeline;
            if (lw == null)
                return;

            ShaderFeatures features = LightweightRenderPipeline.supportedShaderFeatures;

            if (compilerDataList == null)
                return;

            int prevVariantCount = compilerDataList.Count;

            for (int i = 0; i < compilerDataList.Count; ++i)
            {
                if (StripUnused(features, shader, snippetData, compilerDataList[i]))
                {
                    compilerDataList.RemoveAt(i);
                    --i;
                }
            }

#if LOG_VARIANTS
            m_TotalVariantsInputCount += prevVariantCount;
            m_TotalVariantsOutputCount += compilerDataList.Count;

            LogVariants(shader, snippetData, prevVariantCount, compilerDataList.Count);
#endif
        }
    }
}
