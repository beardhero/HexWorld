using UnityEngine;
using System.Collections;

public class RoundManager : MonoBehaviour
{
  public Unit unitToSpawn;

  LayerMask  layermask;

  public void Initialize()
  {
    layermask = 1<<8;

    SpawnUnits();
  }

  void SpawnUnits()
  {

  }

  public void UpdateRound()
  {
    
  }

  void MoveUnit()
  {
    
  }

  public void OnTapInput(Vector2 tap)
  {
    RaycastHit hit;

    if (Physics.Raycast(GameManager.cam.ScreenPointToRay(tap), out hit, 500, layermask))
    {
      IntCoord hexCoordSelected = new IntCoord(Hex.TileAt(hit.point));

      if (hexCoordSelected.x == -1)
        return;

      //actorSpawner.SpawnUnit(null, unitToSpawn, hexCoordSelected);

      Debug.Log("Ray at "+hexCoordSelected.x+","+hexCoordSelected.y);
    }
    else
    {
      Debug.Log("No ray hit on Tap");
    }
  }
}
