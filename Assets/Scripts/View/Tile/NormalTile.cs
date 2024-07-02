namespace View.Tile
{
  public class NormalTile : Tile
  {
    protected override void Start()
    {
      base.Start();
      
      gameObject.name = TileFeatureVo.Key + " Tile";
    }
  }
}