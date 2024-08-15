using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Traffic
{
    public class TileDeco : Tile
    {
        [Header("Deco Tile")]
        [SerializeField]
        private SpriteRenderer m_ImgShadow = null;

        public int Layer { get; set; } = -1;
        public SpriteRenderer Shadow { get => m_ImgShadow; set => m_ImgShadow = value; }

        public override void Initialize<T>(T data, int col = -1, int row = -1) {
            base.Initialize(data, col, row);
            name = "Layer " + Layer + ". x: " + col + ". y: " + row;
        }

        public void SetLayer(int layer) {
            Layer = layer;
        }

        public void SetFacing(Directions direction) {
            switch (direction) {
                case Directions.Up:
                    ImgTile.transform.position.Set(0, 0, 0);
                    break;
                case Directions.Right:
                    ImgTile.transform.position.Set(0, 4, 0);
                    break;
                case Directions.Down:
                    ImgTile.transform.position.Set(4, 4, 0);
                    break;
                case Directions.Left:
                    ImgTile.transform.position.Set(4, 0, 0);
                    break;
            }
        }
    }
}
