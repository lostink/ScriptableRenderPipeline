namespace UnityEngine.Experimental.Rendering.LightweightPipeline
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Camera))]
    [ImageEffectAllowedInSceneView]
    public class LightweightAdditionalCameraData : MonoBehaviour
    {
        [Tooltip("If enabled shadows will render for this camera.")]
        [SerializeField] public bool m_RenderShadows = true;

        [Tooltip("If enabled depth texture will render for this camera bound as _CameraDepthTexture.")]
        [SerializeField] public bool m_RequiresDepthTexture = false;

        [Tooltip("If enabled opaque color texture will render for this camera and bound as _CameraOpaqueTexture.")]
        [SerializeField] public bool m_RequiresColorTexture = false;

        [HideInInspector]
        [SerializeField]
        float m_Version = 1;

        public float version
        {
            get { return m_Version; }
        }

        public bool renderShadows
        {
            get { return m_RenderShadows; }
        }

        public bool requiresDepthTexture
        {
            get { return m_RequiresDepthTexture; }
        }

        public bool requiresColorTexture
        {
            get { return m_RequiresColorTexture; }
        }
    }
}
