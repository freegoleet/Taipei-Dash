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