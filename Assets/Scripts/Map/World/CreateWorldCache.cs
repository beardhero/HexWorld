using UnityEngine;
using System.Collections;

public class CreateWorldCache : MonoBehaviour {

  public float scale;
  public int subdivisions;

	public void BuildCache  (World world) 
  {
    world.PrepForCache(scale, subdivisions);

    try
    {
      BinaryHandler.WriteData<World>(world, World.cachePath);
      Debug.Log ("World cache concluded.");
    }
    catch(System.Exception e)
    {
      Debug.LogError ("World cache fail: "+e);
    }
	}
	
}
