using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Colyseus;

using GameDevWare.Serialization;
using GameDevWare.Serialization.MessagePack;

public class ColyseusClient : MonoBehaviour {

  public string serverName = "localhost";
  public string port = "1111";
  public string username = "tempClientUserID";

  // Clients and rooms
  Client client;
  public RegistrarRoom registrar;
  public ChatRoom chat;
  public MatchmakingRoom matchmaking;

  // Use this for initialization
  public IEnumerator Initialize (Action<string> callback) {
    // Note that ws:// stands for websocket protocol
    String uri = "ws://" + serverName + ":" + port;
    client = new Client(uri, username);
    client.OnOpen += OnOpenHandler;
    client.OnClose += (object sender, EventArgs e) => {
      #if debug_server
        Debug.Log ("Network connection closed.");
      #endif
    };
    client.OnError += (object sender, Colyseus.ErrorEventArgs e) => Debug.Log (e);

    yield return StartCoroutine(client.Connect());

    // === Join game lobby & matchmaking (registrar) ===
    registrar = client.Join<RegistrarRoom>("registrar", new Dictionary<string, object>()
    {
      { "create", true },  // The existence and disposal of this room is to be managed by the server
      {"dispose", false}
    });

    registrar.OnReadyToConnect += (sender, e) => {
      StartCoroutine (registrar.Connect (this));
    };

    registrar.OnJoin += registrar.OnRoomJoined;   // Event raised by Room is handled in RegistrarRoom
    registrar.OnStateChange += registrar.OnStateChangeHandler;
    registrar.OnMessage += registrar.OnServerMessage;

    // --- Chat ---
    chat = client.Join<ChatRoom>("chat", new Dictionary<string, object>()
    {
      { "create", true },
      {"dispose", true}
    });

    chat.OnReadyToConnect += (sender, e) => {
      StartCoroutine (chat.Connect (this));
    };

    chat.OnJoin += chat.OnRoomJoined;
    chat.OnStateChange += chat.OnStateChangeHandler;

    //chat.Listen ("players/:id/:axis", this.OnPlayerMove);
    chat.Listen ("messages/:number", this.chat.OnMessageAdded);
    chat.Listen (this.chat.OnChangeFallback);

    chat.OnMessage += chat.OnServerMessage;


    callback("Network connection active on port "+port);
    int i = 0;

    while (true)
    {
      client.Recv();

      yield return 0;
    }
  }

  public void JoinMatchmakingRoom(){
    matchmaking = client.Join<MatchmakingRoom>("matchmaking", new Dictionary<string, object>()
    {
      { "create", true },  // The existence and disposal of this room is to be managed by the server
      {"dispose", true}
    });

    matchmaking.OnReadyToConnect += (sender, e) => {
      StartCoroutine (matchmaking.Connect (this));
    };

    matchmaking.OnJoin += matchmaking.OnRoomJoined;   // Event raised by Room is handled in RegistrarRoom
    matchmaking.OnStateChange += matchmaking.OnStateChangeHandler;
    matchmaking.OnMessage += matchmaking.OnServerMessage;
  }

  void OnOpenHandler (object sender, EventArgs e)
  {
    Debug.Log("Connected to server. Client id: " + client.id);
  }

  void OnApplicationQuit(){
    if (chat != null && client != null)
    {
      chat.Leave ();
    }

     // Make sure client will disconnect from the server
    if (registrar != null && client != null)
    {
      registrar.Leave ();
    }
  client.Close ();
  }
}