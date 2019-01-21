using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Unit : Actor
{
  //public string name;
  //public Actor actor;
  public Stats stats;
  [HideInInspector] public int currentLocation;

  public void SpawnUnit(int loc)
  {
    if (loc == -1 || loc >= CombatManager.activeWorld.tiles.Count)
    {
     Debug.LogError("Invalid spawn location: "+loc);
    }
    currentLocation = loc;
    HexTile tile = CombatManager.activeWorld.tiles[loc];
    Vector3 facing = CombatManager.activeWorld.tiles[tile.neighbors[0]].hexagon.center - tile.hexagon.center;
    //Spawn(tile, facing, tile.hexagon.normal);
  }
}



[System.Serializable]
public class Stats
{
  public int maxHealth, health, moveSpeed;
}