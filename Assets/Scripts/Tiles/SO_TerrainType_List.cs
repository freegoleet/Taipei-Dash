using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Terrain Type List", menuName = "ScriptableObjects/Terrain Type List", order = 2)]
public class SOTerrainType_List : ScriptableObject
{
    [SerializeField]
    private List<SOTerrainType> m_TerrainTypes = new List<SOTerrainType>();
    public List<SOTerrainType> TerrainTypes { get { return m_TerrainTypes; } }
}
