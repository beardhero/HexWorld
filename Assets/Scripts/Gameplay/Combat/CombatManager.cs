using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour
{
  public int roundTimer = 6;
  public Army lightArmy, darkArmy;
  public Commander player1, player2;
  PlayerStatus[] playerStatus;
  public static World activeWorld;
  public static CombatManager instance;
  public int glyphSpawnRadius = 6;
  public void Initialize(World w)
  {
    instance = this;
    activeWorld = w;

    playerStatus = new PlayerStatus[2];
    playerStatus[0] = new PlayerStatus(0);
    playerStatus[1] = new PlayerStatus(1);

    player1 = new AICommander(playerStatus[0].id, lightArmy);
    player2 = new AICommander(playerStatus[1].id, darkArmy);
  }

  public void InitializeGlyph(Rune rune)
  {
    Object[] enemies = Resources.LoadAll("Enemies");
    List<int> spawnTiles = activeWorld.GetTilesInRadius(glyphSpawnRadius,rune.hexTile); 
    
    foreach(byte b in rune.ID)
    {
      UnityEngine.Random.InitState((int)b);
      //int rEnemy = UnityEngine.Random.Range(0,enemies.Length);
      //int rTile = UnityEngine.Random.Range(0,spawnTiles.Count); 
    }
  }
  public void InitializeInvasion(World w)
  {
    activeWorld = w;
  }

  public void BeginDuel()
  {
    player1.BeginCombat(0);
    player2.BeginCombat(7);
    NewRound();
  }

  void NewRound()
  {
    // Reset state
    for (int i=0; i<playerStatus.Length; i++)
      playerStatus[i].submittedCommands = false;

    // Ask for input
    player1.OnWaitingForCommands();
    player2.OnWaitingForCommands();
  }

  void ProcessRound()
  {
    int breaker = 0, cmdsRemaining=1;
    while (cmdsRemaining != 0 && breaker <100)
    {
      breaker++;

      for (int i=0;i<playerStatus.Length;i++)
      {
        if (playerStatus[i].pendingCommands.Count > 0)
          ProcessCommand(playerStatus[i].pendingCommands.Dequeue());
      }
    }
    
  }

  void ProcessCommand(Command c)
  {
    c.Execute();
  }

  // === Callbacks ===
  public void ReceiveCommands(int id, Queue<Command> cmds)
  {
    playerStatus[id].pendingCommands = cmds;
    playerStatus[id].submittedCommands = true;

    // Check to see if all have submitted

    bool all = true;
    for (int i=0;i<playerStatus.Length;i++)
    {
      if (!playerStatus[i].submittedCommands)
        all = false;
    }
    if (all)
      ProcessRound();
  }

  class PlayerStatus
  {
    public int id;
    public bool submittedCommands;

    public Queue<Command> pendingCommands;

    public PlayerStatus(int i)
    {
      id = i;
    }
  }
}