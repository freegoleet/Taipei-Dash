using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Traffic
{
    public class TileGameplay : Tile
    {
        [Header("Gameplay")]
        [SerializeField]
        private SpriteRenderer m_ImgCurb = null;
        [SerializeField]
        private SpriteRenderer m_ImgBackground = null;
        [SerializeField]
        private float m_Offset = 0;

        public List<Occupant> Occupants { get; set; } = new();

        public SpriteRenderer ImgCurb { get => m_ImgCurb; set => m_ImgCurb = value; }
        public SpriteRenderer ImgBackground { get => m_ImgBackground; set => m_ImgBackground = value; }
        public float Offset { get => m_Offset; }

        public void SetFacing(Directions direction) {
            switch (direction) {
                case Directions.Up:
                    ImgTile.transform.position.Set(0, 0, 0);
                    ImgCurb.transform.rotation = Quaternion.Euler(0, 0, 0);
                    break;
                case Directions.Right:
                    ImgTile.transform.position.Set(0, Offset, 0);
                    ImgCurb.transform.rotation = Quaternion.Euler(0, 0, 270);
                    break;
                case Directions.Down:
                    ImgTile.transform.position.Set(Offset, Offset, 0);
                    ImgCurb.transform.rotation = Quaternion.Euler(0, 0, 180);
                    break;
                case Directions.Left:
                    ImgTile.transform.position.Set(Offset, 0, 0);
                    ImgCurb.transform.rotation = Quaternion.Euler(0, 0, 90);
                    break;
            }
        }

        public void AddOccupant(Occupant occupant) {
            Occupants.Add(occupant);
        }
    }
}