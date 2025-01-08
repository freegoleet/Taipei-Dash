using UnityEngine;

[CreateAssetMenu(fileName = "Car Type", menuName = "ScriptableObjects/Entities/Car Type", order = 1)]
public class SO_CarType : ScriptableObject
{
    [SerializeField]
    private Sprite m_CarBody = null;
    [SerializeField]
    private Sprite m_Windows = null;

    public Sprite CarBody { get => m_CarBody; }
    public Sprite Windows { get => m_Windows; }
}
