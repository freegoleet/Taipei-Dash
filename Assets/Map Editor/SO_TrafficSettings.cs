using UnityEngine;

namespace Traffic
{
    [CreateAssetMenu(fileName = "Traffic Settings", menuName = "ScriptableObjects/Traffic Settings", order = 1)]
    public class SO_TrafficSettings : ScriptableObject {
        [SerializeField]
        private int m_PointerSearchDistance = 0;

        public int PointerSearchDistance { get => m_PointerSearchDistance; }
    }

}
