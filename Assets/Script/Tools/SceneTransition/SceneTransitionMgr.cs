using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionMgr : Singleton<SceneTransitionMgr>
{

    public static event Action OnTransitionStarted;
    public static event Action OnTransitionCompleted;


    public Material Material
    {
        get
        {
            return m_material;
        }
    }

    public bool useUnscaledDeltaTime = false;

    private const int TransitionLayer = 31;

    private int m_dst = -1;

    private Camera m_transitionCamera;
    private Material m_material;
    private MeshFilter meshFilter;
    private Mesh mesh;

    private float deltaTime { get { return useUnscaledDeltaTime ? Time.unscaledDeltaTime : Time.deltaTime; } }


    private ISceneTransition transition;

    public void StartTransition(ISceneTransition transition, int dstScene)
    {
        this.transition = transition;

        m_dst = dstScene;

        Material.shader = this.transition.GetShader();
        m_transitionCamera.enabled = true;

        OnTransitionStarted?.Invoke();

        StartCoroutine(this.transition.OnScreenObscured(Instance));
    }

    public IEnumerator WaitForLevelToLoad()
    {
        if (m_dst >= 0)
        {
            while (SceneManager.GetActiveScene().buildIndex != m_dst)
            {
                yield return null;
            }
        }
    }

    public IEnumerator ProcessTransiton(float duration, bool reverse = false)
    {
        var start = reverse ? 1f : 0f;
        var end = reverse ? 0f : 1f;

        var time = 0f;

        while (time < duration)
        {
            time += deltaTime;
            Material.SetFloat("_Progress", Mathf.Lerp(start, end, Mathf.Pow(time / duration, 2f)));

            yield return null;
        }

        if (!reverse && m_dst >= 0)
        {
            SceneManager.LoadSceneAsync(m_dst);
        }

        if (reverse) ClearUp();
    }

    protected override void Awake()
    {
        base.Awake();

        Init();

        DontDestroyOnLoad(gameObject);
    }

    private void Init()
    {
        meshFilter = GetOrAddCompoent<MeshFilter>();
        meshFilter.mesh = GenerateQuadMesh();

        //m_material = new Material(Shader.Find("Hidden/SceneTransitions/Squares"));

        m_material = GetOrAddCompoent<MeshRenderer>().material;

        Material.color = Color.white;

        m_transitionCamera = GetOrAddCompoent<Camera>();
        m_transitionCamera.orthographic = true;
        m_transitionCamera.nearClipPlane = -1f;
        m_transitionCamera.farClipPlane = 1f;
        m_transitionCamera.depth = 100;
        m_transitionCamera.cullingMask = 1 << TransitionLayer;
        m_transitionCamera.clearFlags = CameraClearFlags.Nothing;
        m_transitionCamera.allowDynamicResolution = false;
        m_transitionCamera.allowHDR = false;
        m_transitionCamera.allowMSAA = false;
        m_transitionCamera.useOcclusionCulling = false;
        m_transitionCamera.enabled = false;

        Debug.Log("Init");
    }

    private Mesh GenerateQuadMesh()
    {
        var halfHeight = 5f;
        var halfWidth = halfHeight * ((float)Screen.width / Screen.height);
        var mesh = new Mesh();
        mesh.vertices = new Vector3[]
        {
            new Vector3(-halfWidth,-halfHeight,0),
            new Vector3(-halfWidth,halfHeight,0),
            new Vector3(halfWidth,-halfHeight,0),
            new Vector3(halfWidth,halfHeight,0),
        };
        mesh.uv = new Vector2[]
        {
            new Vector2( 0, 0 ),
            new Vector2( 0, 1 ),
            new Vector2( 1, 0 ),
            new Vector2( 1, 1 )
        };
        mesh.triangles = new int[] { 0, 1, 2, 3, 2, 1 };

        return mesh;
    }


    private void ClearUp()
    {
        if (Instance == null) return;

        OnTransitionCompleted?.Invoke();

        OnTransitionStarted = null;
        OnTransitionCompleted = null;

        m_dst = -1;

        m_transitionCamera.enabled = false;
    }

    private T GetOrAddCompoent<T>() where T : Component
    {
        var component = GetComponent<T>();
        if (!component)
        {
            component = gameObject.AddComponent<T>();
        }
        return component;
    }

}



