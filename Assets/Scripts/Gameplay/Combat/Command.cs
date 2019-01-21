using UnityEngine;
using System.Collections;
//using System.Collections.Generic;

public abstract class Command
{
  public Unit unit;
  public float duration;
  public abstract void Execute();
}

public class MoveCommand : Command
{
  int direction, distance;
  public MoveCommand(Unit u, int dir, int dist, float dur)
  {
    unit = u;
    direction = dir;
    distance = dist;
    duration = dur;
  }
 
  public override void Execute()
  {
    HexTile t = CombatManager.activeWorld.tiles[unit.currentLocation].GetNeighbor(CombatManager.activeWorld.tiles,direction);
    for (int i=1; i<distance; i++)
    {
      t = CombatManager.activeWorld.tiles[unit.currentLocation].GetNeighbor(CombatManager.activeWorld.tiles,direction);
    }
    
    CombatManager.instance.StartCoroutine(unit.Move(t.index));
  }
}