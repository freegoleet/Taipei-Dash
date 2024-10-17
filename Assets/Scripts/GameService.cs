using UnityEngine;

//[ExecuteInEditMode]
public class GameService : MonoBehaviour
{
    // Singleton
    private static GameService m_Instance = null;
    public static GameService Instance { get { return m_Instance; } }

    // Managers
    [SerializeField]
    private GridManager m_GridManager = null;
    public GridManager GridManager { get { return m_GridManager; } }  

    private bool Initialized = false;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (m_Instance != null && m_Instance != this)
        {
            Destroy(this);
            return;
        }

        m_Instance = this;
    }

    private void Update()
    {
        if(Initialized == false)
        {
            return;
        }

        EntityManager.Tick(Time.deltaTime);
    }
}
