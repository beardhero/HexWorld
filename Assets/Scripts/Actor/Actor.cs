using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Actor : MonoBehaviour
{
  public GameObject player;
  [HideInInspector] public GameObject instance;
  [HideInInspector] public Transform instanceTrans;   // Usually initialized in the ActorSpawner
  [HideInInspector] public int onTile = 0;
  [HideInInspector] public List<int> moveRange;
  /* 
  public GameObject Spawn(HexTile loc, Vector3 facing, Vector3 normal)
  {
    GameObject output = (GameObject)MonoBehaviour.Instantiate (prefab, loc.hexagon.center, Quaternion.LookRotation(facing, normal));
    instance = output;
    instanceTrans = output.transform;
    onTile = loc;
    return output;
  }
  */
  void Start () 
  {
    player = GameObject.Find("Player");
    instanceTrans = player.transform;
    moveRange = new List<int>();

  }
  public IEnumerator Move(int destination) //A* pathfinding
  {
    if(!CombatManager.activeWorld.tiles[destination].passable)
    {
      Debug.Log("Destination not passable");
      yield break;
    }
    Debug.Log("Moving to " + destination);
    if(destination == onTile)
    {
      Debug.Log("Standing on destination");
      yield break; 
    }
    List<int> open = new List<int>(); //to check
    List<int> closed = new List<int>(); //checked
    List<int> path = new List<int>();
    List<int> potentialPath = new List<int>();
    Dictionary<int, int> costFromStart = new Dictionary<int, int>();
    Dictionary<int, int> movementCosts = new Dictionary<int, int>();

    //moverange test
    foreach(int i in moveRange)
    {
      CombatManager.activeWorld.tiles[i].MoveUnhighlight();
    }

    closed.Add(onTile); //add current position to closed
    costFromStart.Add(onTile,0);
    movementCosts.Add(onTile,0);
    foreach(int n in CombatManager.activeWorld.tiles[onTile].neighbors) //add neighbors to open
    {
      HexTile toOpen = CombatManager.activeWorld.tiles[n];
      if(toOpen.passable)
      {
        open.Add(toOpen.index);
        //Debug.Log("Opened " + toOpen.index);
        int mc = 1;
        int c = CombatManager.activeWorld.TileDistanceFromTo(n,destination);
        costFromStart.Add(n,mc);
        movementCosts.Add(n,mc+c);
      }
    }
    //Now that we are initialized, we recursively find the shortest path
    //bool searching = true;
    int next = open[0];
    int lowestCost = 999;
    while(open.Count > 0)
    {
      if(open.Contains(destination))
      {
        //closed.Add(destination);
        path.Add(destination);
        
        //backtrack to get path
        int bCost = 9999;
        int back = destination;
        for(int p = 0; p < closed.Count; p++)
        {
          
          foreach(int b in CombatManager.activeWorld.tiles[back].neighbors)
          {
            //if(b == onTile){break;}
            if(closed.Contains(b))
            {
              if(costFromStart[b] < bCost)
              {
                bCost = costFromStart[b];
                back = b;
              }
            }
          }
          if(back == onTile){break;}
          path.Add(back);
        }
        //path made now move it
        open.Clear();
        
        StartCoroutine(MoveOnPath(path));
        break;
      }
      //go to lowest moveCost in open list
      lowestCost = 9999; // movementCosts[next];
      foreach(int h in open)
      {
        if(movementCosts[h] < lowestCost)
        {
          next = h; //open[h];
          lowestCost = movementCosts[h];
        }
      }
      open.Remove(next);
      closed.Add(next);
      Debug.Log("Closed " + next + " Movecost " + movementCosts[next]);
      //on next tile, now check its neighbors
      HexTile nextTile = CombatManager.activeWorld.tiles[next];
      foreach(int n in nextTile.neighbors)
      {
        if(closed.Contains(n))
        {
          continue;
        }
        //get closed parents to calculate movecost
        int nCost = 0; //new movement cost
        int lCost = 999;
        int cParent = -1;
        foreach(int p in CombatManager.activeWorld.tiles[n].neighbors)
        {
          if(closed.Contains(p))
          {
            if(costFromStart[p] < lCost)
            {
              lCost = costFromStart[p];
              cParent = p;
            }
          }
        }
        if(cParent == -1)
        {Debug.LogError("discontinuous path");}
        else
        {nCost = lCost + 1 + CombatManager.activeWorld.TileDistanceFromTo(n,destination);}
         
        if(open.Contains(n))
        {
          //check if score from this path is lower than its current score
          if(movementCosts[n] > nCost)
          {
            movementCosts.Remove(n);
            movementCosts.Add(n,nCost);
          }
        }
        else
        {
          if(CombatManager.activeWorld.tiles[n].passable)
          {
            open.Add(n);
            costFromStart.Add(n,lCost + 1);
            movementCosts.Add(n,nCost);
          }
        }
      }
      yield return null;
    }
    //backtrack from destination to find path
    yield return null;
  }

  public IEnumerator MoveOnPath(List<int> path)
  {
    for(int i = path.Count-1; i >= 0; i--)
    {
      Debug.Log("MovingOnPath to " + path[i]);
      yield return StartCoroutine(MoveToTile(CombatManager.activeWorld.tiles[path[i]], 0.1f));
      
    }
    yield return null;
  }

  public IEnumerator MoveToTile(HexTile t, float time)
  {
    Vector3 startingPos = instanceTrans.position, 
      endingPos = t.hexagon.center;//+t.hexagon.normal;
    Quaternion startingRot = instanceTrans.rotation,
      endingRot = Quaternion.LookRotation(endingPos-startingPos, t.hexagon.normal);

    for (float i=0; i<time; i+=Time.deltaTime)
    {
      instanceTrans.position = Vector3.Lerp(startingPos, endingPos, i/time);
      instanceTrans.rotation = Quaternion.Slerp(startingRot, endingRot, i/time);
      yield return null;
    }
    onTile = t.index;
    moveRange = CombatManager.activeWorld.GetTilesInRadius(6,onTile);
    foreach(int i in moveRange)
    {
      CombatManager.activeWorld.tiles[i].MoveHighlight();
    }
    yield return null;
  }
}
