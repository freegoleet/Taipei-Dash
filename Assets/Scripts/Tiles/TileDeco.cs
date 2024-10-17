using UnityEngine;

namespace Traffic
{
    public class TileDeco : Tile
    {
        [Header("Deco Tile")]
        [SerializeField]
        private SpriteRenderer m_ImgShadow = null;

        public int Layer { get; set; } = -1;
        public SpriteRenderer Shadow { get => m_ImgShadow; set => m_ImgShadow = value; }

        public override void Initialize(SO_Tile data, Vector2Int gridPos, bool cursor = false) {
            base.Initialize(data, gridPos, cursor);
            if(cursor == false) {
                name = "Layer " + Layer + ". x: " + Col + ". y: " + Row;
            }
        }

        public void SetLayer(int layer) {
            Layer = layer;
        }

        public void SetFacing(Direction direction) {
            switch (direction) {
                case Direction.Up:
                    ImgBackground.transform.position.Set(0, 0, 0);
                    break;
                case Direction.Right:
                    ImgBackground.transform.position.Set(0, 4, 0);
                    break;
                case Direction.Down:
                    ImgBackground.transform.position.Set(4, 4, 0);
                    break;
                case Direction.Left:
                    ImgBackground.transform.position.Set(4, 0, 0);
                    break;
            }
        }
    }
}
