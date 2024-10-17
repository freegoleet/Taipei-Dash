using UnityEngine;

[CreateAssetMenu(fileName = "Terrain Type", menuName = "ScriptableObjects/Terrain Type", order = 2)]
public class SOTerrainType : ScriptableObject
{
    [SerializeField]
    private string m_TerrainName = null;
    [SerializeField]
    private int m_TraversalCost = 1;
    [SerializeField]
    private bool m_Combustible = false;

    public string TerrainName { get { return m_TerrainName; } }
    public int TraversalCost { get { return m_TraversalCost; } }
    public bool Combustible { get { return m_Combustible; } }
}
