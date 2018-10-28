using UnityEngine;
using System.Collections;

[System.Serializable]
public class Actor
{
  public GameObject prefab;
  [HideInInspector] public GameObject instance;
  [HideInInspector] public Transform instanceTrans;   // Usually initialized in the ActorSpawner

  public GameObject Spawn(Vector3 loc, Vector3 facing, Vector3 normal)
  {
    GameObject output = (GameObject)MonoBehaviour.Instantiate (prefab, loc+normal, Quaternion.LookRotation(facing, normal));
    instance = output;
    instanceTrans = output.transform;
    
    return output;
  }

  public IEnumerator MoveToTile(HexTile t, float time)
  {
    Vector3 startingPos = instanceTrans.position, 
      endingPos = t.hexagon.center+t.hexagon.normal;
    Quaternion startingRot = instanceTrans.rotation,
      endingRot = Quaternion.LookRotation(endingPos-startingPos, t.hexagon.normal);

    for (float i=0; i<time; i+=Time.deltaTime)
    {
      instanceTrans.position = Vector3.Lerp(startingPos, endingPos, i/time);
      instanceTrans.rotation = Quaternion.Slerp(startingRot, endingRot, i/time);
      yield return null;
    }
  }
}
