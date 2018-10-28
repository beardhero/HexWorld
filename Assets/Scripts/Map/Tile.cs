using UnityEngine;
using System;
using System.Collections;
using Random = UnityEngine.Random;

[Serializable]
public enum TileType {
  None, Gray, 
  Water, Fire, Earth, Air, Dark, Light,
  Metal, Ice, Vapor, Crystal, Arbor, Astral, 
  Luna, Sol, Void, Divine
};

[Serializable]
public class Tile
{
  public float height;

  public bool border;
  public bool posBorderCheck= false;

  public int uvx, uvy;

  public TileType type = TileType.Water;

  public Tile(){}
  public Tile(int _uvx, int _uvy)
  {
    uvx = _uvx;
    uvy = _uvy;
  }
  public Tile(float startingHeight)
  {
    height = startingHeight;
    type = TileType.Water;
  }
  
  public Tile(float x, float y, int width, float lacunarity, float probability, float height_in = -1)
  {
	type = TileType.None;
    if (height_in == -1)
      height = 0;
    else
      height = height_in;
  }

  public virtual void OnUnitEnter(){}
}

public class Tile_Grass : Tile
{ 
  public override void OnUnitEnter()
  {
    Debug.Log("The grass rustles as a unit enters.");
    // Some custom tile logic here
  }
}