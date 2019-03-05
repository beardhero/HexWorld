using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Colyseus;
using System;
using GameDevWare.Serialization;

public class ChatRoom : Room{

  public void OnMessageAdded (DataChange change)
  {
//    Debug.Log ("OnMessageAdded");
//    Debug.Log (change.path["number"]);
//    Debug.Log (change.value);
  }

  public void OnChangeFallback (PatchObject change)
  {
//    Debug.Log ("OnChangeFallback");
//    Debug.Log (change.operation);
//    Debug.Log (change.path);
//    Debug.Log (change.value);
  }

  public void OnRoomJoined(object sender, System.EventArgs e){

  }

  public void OnServerMessage(object sender, Colyseus.MessageEventArgs e){
  }

  public void OnStateChangeHandler (object sender, RoomUpdateEventArgs e){
  	
  }
}
