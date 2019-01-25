using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Colyseus;

using GameDevWare.Serialization;
using GameDevWare.Serialization.MessagePack;

public class ColyseusClient : MonoBehaviour {

  Client client;
  Room room;

  public string serverName = "localhost";
  public string port = "8080";
  public string roomName = "chat";

  // map of players
  Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();

  // Use this for initialization
  public IEnumerator Initialize (Action<string> callback) {
    // Note that ws:// stands for websocket protocol
    String uri = "ws://" + serverName + ":" + port;
    Debug.Log (uri);
    client = new Client(uri);
    client.OnOpen += OnOpenHandler;
    client.OnClose += (object sender, EventArgs e) => Debug.Log ("CONNECTION CLOSED");

    Debug.Log ("Let's connect the client!");
    yield return StartCoroutine(client.Connect());

    Debug.Log ("Let's join the room!");
    room = client.Join(roomName, new Dictionary<string, object>()
    {
      { "create", true }
    });
    room.OnReadyToConnect += (sender, e) => {
      Debug.Log("Ready to connect to room!");
      StartCoroutine (room.Connect ());
    };
    room.OnJoin += OnRoomJoined;
    room.OnStateChange += OnStateChangeHandler;

    room.Listen ("players/:id", this.OnPlayerChange);
    room.Listen ("players/:id/:axis", this.OnPlayerMove);
    room.Listen ("messages/:number", this.OnMessageAdded);
    room.Listen (this.OnChangeFallback);

    room.OnMessage += OnMessage;

    int i = 0;

    callback("Socket opened, listening");

    while (true)
    {
      client.Recv();

      i++;

      if (i % 50 == 0) {
        room.Send("some_command");
      }

      yield return 0;
    }
  }

  void OnOpenHandler (object sender, EventArgs e)
  {
    Debug.Log("Connected to server. Client id: " + client.id);
  }

  void OnRoomJoined (object sender, EventArgs e)
  {
    Debug.Log("Joined room successfully.");
  }

  void OnMessage (object sender, MessageEventArgs e)
  {
    var message = (IndexedDictionary<string, object>) e.message;
//    Debug.Log(data);
  }

  void OnStateChangeHandler (object sender, RoomUpdateEventArgs e)
  {
    // Setup room first state
    if (e.isFirstState) {
      IndexedDictionary<string, object> players = (IndexedDictionary<string, object>) e.state ["players"];
    }
  }

  void OnPlayerChange (DataChange change)
  {
    Debug.Log ("OnPlayerChange");
    Debug.Log (change.operation);
    Debug.Log (change.path["id"]);
//    Debug.Log (change.value);

    if (change.operation == "add") {
      IndexedDictionary<string, object> value = (IndexedDictionary<string, object>) change.value;

      GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);

      cube.transform.position = new Vector3 (Convert.ToSingle(value ["x"]), Convert.ToSingle(value ["y"]), 0);

      // add "player" to map of players by id.
      players.Add (change.path ["id"], cube);

    } else if (change.operation == "remove") {
      // remove player from scene
      GameObject cube;
      players.TryGetValue (change.path ["id"], out cube);
      Destroy (cube);

      players.Remove (change.path ["id"]);
    }
  }

  void OnPlayerMove (DataChange change)
  {
//    Debug.Log ("OnPlayerMove");
//    Debug.Log ("playerId: " + change.path["id"] + ", Axis: " + change.path["axis"]);
//    Debug.Log (change.value);

    GameObject cube;
    players.TryGetValue (change.path ["id"], out cube);

    cube.transform.Translate (new Vector3 (Convert.ToSingle(change.value), 0, 0));
  }

  void OnPlayerRemoved (DataChange change)
  {
//    Debug.Log ("OnPlayerRemoved");
//    Debug.Log (change.path);
//    Debug.Log (change.value);
  }

  void OnMessageAdded (DataChange change)
  {
//    Debug.Log ("OnMessageAdded");
//    Debug.Log (change.path["number"]);
//    Debug.Log (change.value);
  }

  void OnChangeFallback (PatchObject change)
  {
//    Debug.Log ("OnChangeFallback");
//    Debug.Log (change.operation);
//    Debug.Log (change.path);
//    Debug.Log (change.value);
  }

  void OnApplicationQuit()
  {
    // Make sure client will disconnect from the server
    if (room != null && client != null)
    {
      room.Leave ();
      client.Close ();
    }
  }
}