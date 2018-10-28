using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class DragonSpawner : MonoBehaviour {
	public GameObject[] dragon;
	public string dragonName;
	public RenderTexture activeTex;
	void Start()
	{
		SpawnDragon();
	}


	public void SpawnDragon()
	{
		
		GameObject oldDrag = GameObject.Find("dragon");
		if(oldDrag != null)
		{
			Destroy(oldDrag);
			Debug.Log("Destroyed current dragon");
		}

		Instantiate(dragon[Random.Range(0,dragon.Length)], Vector3.zero, Quaternion.identity);
	}

	public void CapturePNG()
  	{
    	//GameObject selection = GameObject.Find ("Zone Prefab(Clone)");
		RenderTexture.active = activeTex;
    	//Camera cam = GameObject.Find("Dragon Camera").camera;
        int width = activeTex.width;
        int height = activeTex.height;
        Texture2D tex = new Texture2D (width, height, TextureFormat.ARGB32, false);
        Rect sel = new Rect ();
        sel.width = width;
        sel.height = height;
        sel.x = 0;
        sel.y = 0;//selection.transform.position.y;

        tex.ReadPixels (sel, 0, 0);
 
        byte[] bytes = tex.EncodeToPNG ();
        File.WriteAllBytes ("Assets/Dragons/" + dragonName + ".png", bytes);
  	}
}
