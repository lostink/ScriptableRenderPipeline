namespace UnityEngine.Experimental.Rendering.LightweightPipeline
{
    public class LightweightPipelineEditorResources : ScriptableObject
    {
        [SerializeField] Material m_DefaultMaterial = null;
        [SerializeField] Material m_DefaultParticleMaterial = null;
        [SerializeField] Material m_DefaultTerrainMaterial = null;

        public Material defaultMaterial
        {
            get { return m_DefaultMaterial; }
        }

        public Material defaultParticleMaterial
        {
            get { return m_DefaultParticleMaterial; }
        }

        public Material defaultTerrainMaterial
        {
            get { return m_DefaultTerrainMaterial; }
        }
    }
}
