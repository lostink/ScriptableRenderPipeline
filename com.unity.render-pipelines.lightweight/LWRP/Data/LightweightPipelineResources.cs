namespace UnityEngine.Experimental.Rendering.LightweightPipeline
{
    public class LightweightPipelineResources : ScriptableObject
    {
        [SerializeField] Shader m_BlitShader = null;
        [SerializeField] Shader m_CopyDepthShader = null;
        [SerializeField] Shader m_ScreenSpaceShadowShader = null;
        [SerializeField] Shader m_SamplingShader = null;

        public Shader blitShader
        {
            get { return m_BlitShader; }
        }

        public Shader copyDepthShader
        {
            get { return m_CopyDepthShader; }
        }

        public Shader screenSpaceShadowShader
        {
            get { return m_ScreenSpaceShadowShader; }
        }

        public Shader samplingShader
        {
            get { return m_SamplingShader; }
        }
    }
}
