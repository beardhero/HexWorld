using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Commander
{
  public int participantID;
  public int nexusLoc;
  public Army army;

  public Commander(){}
  public Commander(int id, Army a)
  {
    participantID = id;
    army = a;
  }

  public void BeginCombat(int nl)
  {
    nexusLoc = nl;
    DeployUnits();
  }

  public abstract void DeployUnits();
  public abstract void OnWaitingForCommands();
  public void SubmitCommands(Queue<Command> cmds)
  {
    GameManager.combatManager.ReceiveCommands(participantID, cmds);
  }
}


[System.Serializable]
public class Army
{
  public List<Unit> units;
}