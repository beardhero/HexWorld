using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AICommander : Commander
{
  public AICommander(int id, Army a) : base(id, a){}

  public override void DeployUnits()
  {
    army.units[0].SpawnUnit(nexusLoc);
  }

  public override void OnWaitingForCommands()
  {
    Queue<Command> commands = new Queue<Command>();
    commands.Enqueue(new MoveCommand(army.units[0], Direction.X, 2, 2.5f));
  }
}
