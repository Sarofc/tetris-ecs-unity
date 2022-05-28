//#define DEBUG_BATCH_RENDERER

using System.Collections.Generic;
using UnityEngine;

namespace Tetris
{
    [DisallowMultipleComponent]
    public sealed class BatchRenderer : MonoBehaviour
    {
        [SerializeField]
        private Material m_Material;

        [SerializeField]
        private LayerMask m_Layer;

        [SerializeField]
        private Camera m_RenderingCamera;

        private Mesh m_Mesh;
        private MaterialPropertyBlock m_Mpb;

        private readonly int m_MainTexSt = Shader.PropertyToID("_MainTex_ST");
        private readonly int m_Color = Shader.PropertyToID("_Color");

        public List<Matrix4x4[]> TransformMatrixBatches { get; set; }
        public List<Vector4[]> SpriteOffsetBatches { get; set; }
        public List<Vector4[]> ColorBatches { get; set; }

        public void SetLayer(LayerMask layer)
        {
            m_Layer = layer;
        }

        public void SetRenderingCamera(Camera camera)
        {
            m_RenderingCamera = camera;
        }

        public void Init()
        {
            //Debug.LogError("support gpu instance: " + ((m_Material.enableInstancing && SystemInfo.supportsInstancing) ? "yes" : "no"));

            var rect = new Rect(0, 0, 1, 1);
            var size = rect.size;
            var pivot = rect.center;

            m_Mesh = GenerateQuad(size, pivot);
            m_Mpb = new MaterialPropertyBlock();
        }

        public void BatchAndRender()
        {
            if (TransformMatrixBatches.Count <= 0) return;

            for (int i = 0; i < TransformMatrixBatches.Count; i++)
            {
                var matricesBatches = TransformMatrixBatches[i];

                if (SpriteOffsetBatches != null && SpriteOffsetBatches.Count > 0)
                {
                    var offsetBatches = SpriteOffsetBatches[i];
                    m_Mpb.SetVectorArray(m_MainTexSt, offsetBatches);
                }

                if (ColorBatches != null && ColorBatches.Count > 0)
                {
                    var colorBatches = ColorBatches[i];
                    m_Mpb.SetVectorArray(m_Color, colorBatches);
                }

                Graphics.DrawMeshInstanced(
                    m_Mesh,
                    0,
                    m_Material,
                    matricesBatches,
                    matricesBatches.Length,
                    m_Mpb,
                    UnityEngine.Rendering.ShadowCastingMode.Off,
                    false,
                    m_Layer,
                    m_RenderingCamera,
                    UnityEngine.Rendering.LightProbeUsage.Off
                );
            }
        }

        public void Destroy()
        {
            if (m_Mesh != null)
            {
                GameObject.Destroy(m_Mesh);
            }
        }

        private Mesh GenerateQuad(Vector2 size, Vector2 pivot)
        {
            if (true)
            {
                // xy
                Vector3[] vertices =
                {
                    new Vector3(size.x - pivot.x, size.y - pivot.y, 0),
                    new Vector3(size.x - pivot.x, 0 - pivot.y, 0),
                    new Vector3(0 - pivot.x, 0 - pivot.y, 0),
                    new Vector3(0 - pivot.x, size.y - pivot.y, 0)
                };

                Vector2[] uv =
                {
                    new Vector2(1, 1),
                    new Vector2(1, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 1)
                };

                int[] triangles =
                {
                    2, 3, 0,
                    0, 1, 2,
                };

                return new Mesh
                {
                    vertices = vertices,
                    uv = uv,
                    triangles = triangles
                };
            }
            else
            {
                // xz
                Vector3[] vertices =
                {
                    new Vector3(size.x - pivot.x, 0, size.y - pivot.y),
                    new Vector3(size.x - pivot.x, 0, 0 - pivot.y),
                    new Vector3(0 - pivot.x, 0, 0 - pivot.y),
                    new Vector3(0 - pivot.x, 0, size.y - pivot.y)
                };

                Vector2[] uv =
                {
                    new Vector2(1, 1),
                    new Vector2(1, 0),
                    new Vector2(0, 0),
                    new Vector2(0, 1)
                };

                int[] triangles =
                {
                    2, 3, 0,
                    0, 1, 2,
                };

                return new Mesh
                {
                    vertices = vertices,
                    uv = uv,
                    triangles = triangles
                };
            }
        }

        //private void Start()
        //{
        //    Initialize();
        //}

        //private void Update()
        //{
        //    BatchAndRender();
        //}

        //private void OnDestroy()
        //{
        //    Dispose();
        //}

#if UNITY_EDITOR
#if DEBUG_BATCH_RENDERER
        private void OnDrawGizmos()
        {
            if (TransfromMatrixBatches == null) return;

            Gizmos.color = Color.green;
            for (int i = 0; i < TransfromMatrixBatches.Count; i++)
            {
                var matricesBatches = TransfromMatrixBatches[i];

                foreach (var matrix in matricesBatches)
                {
                    var pos = matrix.ExtractPosition();

                    var scale = matrix.ExtractScale();

                    var rot = matrix.ExtractRotation();

                    Gizmos.DrawWireCube(pos, scale);
                }
            }
        }
#endif
#endif
    }
}