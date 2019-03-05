using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Colyseus;
using GameDevWare.Serialization;

public class RegistrarRoom : Room {
	public void OnRoomJoined(object sender, System.EventArgs e){

	}

	public void OnStateChangeHandler (object sender, RoomUpdateEventArgs e)
	{
		if (e.isFirstState) {
		  IndexedDictionary<string, object> players = (IndexedDictionary<string, object>) e.state ["players"];
		  return;
		}

		if (e.state.ContainsKey("command")){
			switch(e.state["command"] as string){
				case "queued":
					Debug.Log("Queued for matchmaking");
				break;

				default:
				break;
			}
		}
		else{
			Debug.Log("-----Keys:");
			foreach (string key in e.state.Keys)
				Debug.Log(key);
			Debug.Log("-----values:");
			foreach (object v in e.state.Values)
				Debug.Log(v);
		}
	}
 	
 	// Continuous state change? Not sure how this is different from OnStateChangeHandler
	public void OnListen(DataChange change){
		Debug.Log("received OnListen message: ");
		Debug.Log(change);
	}

	public void OnServerMessage(object sender, Colyseus.MessageEventArgs e){
		IndexedDictionary<string, object> args = (IndexedDictionary<string, object>)e.message;
		if (args.ContainsKey("type"))
		{
			try{
				switch((string)args["type"]){
					case "status":
						#if debug_server
							Debug.Log("Server message: "+args["value"]);
						#endif
					break;

					case "command":
						switch((string)args["value"]){
							case "move to room":
								switch((string)args["value2"]){
									case "matchmaking":
										colyseusClient.JoinMatchmakingRoom();
									break;
								}
							break;
						}
					break;
				}
			}
			catch (System.Exception err){
				Debug.LogError(err);
			}
		}
		else
			Debug.LogError("Unexpected formatting of server message: "+e.message);
	}

	public void RequestJoinMatchmaking(){
		this.Send(new {command = "request join matchmaking"});
	}
}
