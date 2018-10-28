using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DragonSpawner))]
public class DragonEditor : Editor {

	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();
		DragonSpawner spawner = (DragonSpawner)target;
		if(GUILayout.Button("Spawn Dragon"))
		{
			spawner.SpawnDragon();
		}
		
		if(GUILayout.Button("Capture PNG"))
		{
			spawner.CapturePNG();
		}
	}
	
}
