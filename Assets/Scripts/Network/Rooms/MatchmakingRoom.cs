using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Colyseus;
using GameDevWare.Serialization;

public class MatchmakingRoom : Room {
	public void OnRoomJoined(object sender, System.EventArgs e){

	}

	public void OnStateChangeHandler (object sender, RoomUpdateEventArgs e)
	{
		
	}

	public void OnServerMessage(object sender, Colyseus.MessageEventArgs e){
		IndexedDictionary<string, object> args = (IndexedDictionary<string, object>)e.message;
		if (args.ContainsKey("type"))
		{

		}
		else
			Debug.LogError("Unexpected formatting of server message: "+e.message);
	}
}
