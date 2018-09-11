using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
 
[Serializable]
[PostProcess(typeof(MotionVectorsDebugRenderer), PostProcessEvent.AfterStack, "Motion Vectors Debug", false)]
public sealed class MotionVectorsDebug : PostProcessEffectSettings
{
    [Range(0f, 1f), Tooltip("")]
    public FloatParameter opacity = new FloatParameter { value = 1.0f };
    [Range(0f, 1f), Tooltip("")]
    public FloatParameter amplitude = new FloatParameter { value = 1.0f };
    [Range(0f, 1f), Tooltip("")]
    public FloatParameter scale = new FloatParameter { value = 1.0f };
}
 
public sealed class MotionVectorsDebugRenderer : PostProcessEffectRenderer<MotionVectorsDebug>
{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/MotionVectorsDebug"));
        sheet.properties.SetFloat("_Opacity", settings.opacity);
        sheet.properties.SetFloat("_Amplitude", settings.amplitude);
        sheet.properties.SetFloat("_Scale", settings.scale);
        context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
    }
}