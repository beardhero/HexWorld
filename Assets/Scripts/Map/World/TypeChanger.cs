using UnityEngine;

using System.Collections;

public class TypeChanger : MonoBehaviour {

  public Ray ray;
  public RaycastHit hit;
  public TileType type; //Changes to set type
	
	// Update is called once per frame
	void Update () {
    //Whatcha gonna do boss?
    //Type Changer
    if (Input.GetKeyDown(KeyCode.Mouse1))
    {
      ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (Physics.Raycast(ray, out hit, 100.0f))
      {
        foreach (HexTile ht in GetComponentInParent<WorldManager>().activeWorld.tiles)
        {
          ht.type = type;

        }
      }
    }
  }
}
