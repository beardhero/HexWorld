using UnityEngine;
using System.Collections;

public class ChatUI : MonoBehaviour
{
  public static void SystemMessage(string msg)
  {
    Debug.Log(msg);
  }

  public static void ReceiveChat(string msg)
  {
    Debug.Log(msg);
  }
}
