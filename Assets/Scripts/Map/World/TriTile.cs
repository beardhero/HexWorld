
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class TriTile
{
  public int index;
  public int plate = -1;
  public bool rendered;
  //public float height;
  string terrainType;
  public SerializableVector3 center, v1, v2, v3;
  public TileType type;
  public List<int> neighbors;
  public bool boundary;
  private float _height;
  public float height
  {
    get { return _height; }
    set
    {
      _height = value;
      v1 /= v1.magnitude;
      v1 *= value;
      v2 /= v2.magnitude;
      v2 *= value;
      v3 /= center.magnitude;
      v3 *= value;
      center = (v1 + v2 + v3) / 3;
    }
  }



  //Tectonics
  public float pressure, shear, scale, temp, humidity;

  private float _elevation;
  public float elevation
  {
    get { return _elevation; }
    set { _elevation = value; }
  }
  private float _heat;

  public float heat
  {
    get { return _heat; }
    set { _heat = value; }
  }
  private float _precipitation;

  public float precipitation
  {
    get { return _precipitation; }
    set { _precipitation = value; }
  }
  public TriTile() { }

  public TriTile(Triangle t)
  {
    index = t.index;
    //triangle = t;
    center = t.center;
    v1 = t.v1;
    v2 = t.v2;
    v3 = t.v3;
  }
  public TriTile(Triangle tri, int p, List<int> neighbs, bool b, float hi, TileType t)
  {
    type = t;
    index = tri.index;
    center = tri.center;
    v1 = tri.v1;
    v2 = tri.v2;
    v3 = tri.v3;
    plate = p;
    boundary = b;
    neighbors = new List<int>(neighbs);
    height = hi;
  }
  public void ChangeType(TileType t)
  {
    type = t;
    //@TODO: other stuff
  }
  /*
  public int GetNeighborID(int dir)
  {
    return triangle.neighbors[dir];
  }
   
  public HexTile GetNeighbor(List<HexTile> tiles, int dir)
  {
    return tiles[triangle];
  }
  */

  public virtual void OnUnitEnter() { }
}

public class TriTile_Grass : HexTile
{
  public override void OnUnitEnter()
  {
    Debug.Log("The grass rustles as a unit enters.");
    // Some custom tile logic here
  }
}