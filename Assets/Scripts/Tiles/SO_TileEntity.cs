using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Tile Entity", menuName = "ScriptableObjects/Tile Entity", order = 1)]
public class SO_TileEntity : ScriptableObject
{
    [Flags]
    public enum eBlockers
    {
        None,
        LineOfSight,
        Walking,
        Flying
    }

    [SerializeField]
    private eBlockers m_Blockers = eBlockers.Walking;
    public  eBlockers Blockers {  get { return m_Blockers; } }

}
