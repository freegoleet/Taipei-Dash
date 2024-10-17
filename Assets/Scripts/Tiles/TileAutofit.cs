using UnityEngine;

namespace Traffic
{
    public class TileAutofit : TileGameplay
    {
        public AutofitType AutofitType { get; private set; }
        public SO_TileAutoFit AutofitData { get; private set; }

        public override void Initialize(SO_Tile data, Vector2Int gridPos, bool cursor = false) {
            base.Initialize(data, gridPos, cursor);
            if (Data is SO_TileAutoFit afData) {
                AutofitData = afData;
            }
        }

        public void FitTile() {
            int neighborcount = NeighborSystem.NeighborCount();

            if (neighborcount == 4) {
                SetFacing(Direction.Up);
                SetAutofitType(AutofitType.Middle);
                return;
            }

            if (neighborcount == 3) {
                var direction = NeighborSystem.GetFirstUnfittableDirection();
                SetFacing(direction);
                SetAutofitType(AutofitType.Side);
                return;
            }

            if (neighborcount == 2) {
                Direction direction = NeighborSystem.GetCornerDirection();
                if(direction == Direction.None) {
                    SetFacing(NeighborSystem.GetFirstUnfittableDirection());
                    SetAutofitType(AutofitType.Bridge);
                    return;
                }
                SetFacing(NeighborSystem.GetCornerDirection());
                SetAutofitType(AutofitType.Corner);
                return;
            }

            if (neighborcount == 1) {
                SetFacing(NeighborSystem.GetFirstFittableDirection());
                SetAutofitType(AutofitType.DeadEnd);
                return;
            }
        }

        public virtual void SetAutofitType(AutofitType autofitType) {
            AutofitType = autofitType;
            SetSprites(Data.Background, AutofitData.TileMiddle);
        }

        public override void SetFacing(Direction facing) {
            base.SetFacing(facing);
        }

        public virtual void SetSprites(Sprite bg, Sprite feature) {
            bool showBg = bg != null;
            if (showBg == true) {
                ImgBackground.sprite = bg;
            }
            ImgBackground.gameObject.SetActive(showBg);

            bool showFeature = feature != null;
            if (showFeature == true) {
                ImgFeature.sprite = feature;
            }
            ImgFeature.gameObject.SetActive(showFeature);
        }
    }
}