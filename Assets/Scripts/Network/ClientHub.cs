using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ClientHub : NetworkBehaviour {

  GameObject myObj;
  Transform myTrans;

  [SyncVar] public string username;

  void Start()
  {

  }

  void OnSeedReceived(NetworkMessage msg)
  {
    Debug.LogError("seed received");
  }

  public void Initialize()
  {
    
  }

  // === Network Commands ===
  // Chat
  [Command]
  public void CmdSendChat(string username, string chat)
  {
    RpcReceiveChat(username + ": " + chat);
  }
  [ClientRpc]
  void RpcReceiveChat(string msg)
  {
    ChatUI.ReceiveChat(msg);
  }

  // Login/config
  [Command]
  public void CmdSetUsername(string user)
  {
    username = user;

    RpcUpdateUsername(username);
  }
  [ClientRpc]
  void RpcUpdateUsername(string user)
  {
    myObj.name = "A player named "+username;

    ChatUI.SystemMessage(username+" has joined the game.");
  }

  [Command]
  public void CmdAskForSeed()
  {

  }
  // === /Network Commands ===


  void OnChatMessage(NetworkMessage msg)
  {
    MyMsg message = msg.ReadMessage<MyMsg>();
    ChatUI.ReceiveChat(message.data);
  }

  public class MyMsg : MessageBase
  {
    public string data;

    public MyMsg(){}
    public MyMsg(string d)
    {
      data = d;
    }
  }
}

class MyMsgType : MsgType{
  public const int UpdateSeed = 6921;
}
