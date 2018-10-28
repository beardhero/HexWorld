using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Unit : Actor
{
  public string name;
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
    Spawn(tile.hexagon.center, facing, tile.hexagon.normal);
  }

  public void Move(int dest) //Hex Tile destination 
  {
    //A* pathing
    //Determine value of neighbors of onTile
    //Move to best tile, put into path, repeat 1
    //If backtracking is the only option, restart and try a different initial tile
    List<int> path = new List<int>();

  }
}



[System.Serializable]
public class Stats
{
  public int maxHealth, health, moveSpeed;
}