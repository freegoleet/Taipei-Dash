using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Traffic
{
    public class UI_Connections : MonoBehaviour
    {
        [SerializeField]
        private RawImage m_Up = null;
        [SerializeField]
        private RawImage m_Left = null;
        [SerializeField]
        private RawImage m_Right = null;
        [SerializeField]
        private RawImage m_Down = null;

        private Dictionary<Direction, RawImage> m_Images = null;

        public void Initialize() {
            m_Images = new Dictionary<Direction, RawImage>() {
            { Direction.Up,  m_Up },
            { Direction.Left,  m_Left },
            { Direction.Right, m_Right },
            { Direction.Down,  m_Down },
        };
        }

        public void ShowConnections(Direction[] directions) {
            for (int i = 0; i < directions.Length; i++) {
                if (directions[i] == Direction.None) {
                    m_Images[(Direction)i].color = Color.red;
                    continue;
                }
                m_Images[directions[i]].color = Color.green;
            }
        }
    }
}
