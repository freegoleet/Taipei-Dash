using System;
using UnityEngine;

public class GameServices : MonoBehaviour
{
    // Singleton
    private static GameServices m_Instance = null;
    public static GameServices Instance { get { return m_Instance; } }
    
    // Managers
    [SerializeField]
    private GridManager m_GridManager = null;
    public GridManager GridManager { get { return m_GridManager; } }  
    public EntityManager EntityManager { get; private set; } = null;

    // Sub Services
    private TilemapService m_TilemapService = null;
    public TilemapService TilemapService { get { return m_TilemapService; } }

    // Utilities
    public TilemapUtilities TilemapUtilities { get; private set; } = null;
    
    private TileUtilities m_TileUtilities = null;
    public TileUtilities TileUtilities { get { return m_TileUtilities; } }

    // Editor
    private TilemapEditorUtilities m_MapEditor = null;
    public TilemapEditorUtilities MapEditor { get { return m_MapEditor; } } 

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

        Initialized = InitializeUtilities();
    }

    private void Update()
    {
        if(Initialized == false)
        {
            return;
        }

        EntityManager.Tick(Time.deltaTime);
    }

    private bool InitializeUtilities()
    {
       
        m_TilemapService = new TilemapService(m_GridManager);

        TilemapUtilities = new TilemapUtilities();

        m_MapEditor = new TilemapEditorUtilities();
        
        m_TileUtilities = new TileUtilities();

        EntityManager = new EntityManager();

        return true;
    }
}
