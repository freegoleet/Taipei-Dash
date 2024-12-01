using UnityEngine;

[CreateAssetMenu(fileName = "Car", menuName = "ScriptableObjects/Entities/Car", order = 1)]
public class SO_EntityCar : SO_Entity
{
    [SerializeField]
    private SpriteRenderer m_SpriteRenderer = null;

}
