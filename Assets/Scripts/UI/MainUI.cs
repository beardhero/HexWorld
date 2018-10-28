using UnityEngine;
using System.Collections;

public class MainUI : MonoBehaviour
{
  public void Initialize()
  {

  }

  // Called from within GameManager.OnGUI(){}
  public void OnMainGUI()   
  {

  }

  public static void SystemMessage(string msg)
  {
    Debug.Log("SYSTEM: "+msg);
  }
}
