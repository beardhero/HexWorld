using UnityEngine;
using System.Collections;

public class MapViewCamera : MonoBehaviour
{
  Camera cam;
  Transform myTrans;
  public float panSpeed = 1, zoomSpeed = 10;

  void Awake()
  {
    cam = GetComponent<Camera>();
    myTrans = transform;
  }

  void Update()
  {
    float scroll = Input.GetAxis("Mouse ScrollWheel");

    if (scroll != 0)
    {
      cam.orthographicSize -= scroll*zoomSpeed;
    }

    if (Input.anyKey)
    {
      myTrans.Translate(-Input.GetAxis("Mouse X")*panSpeed, -Input.GetAxis("Mouse Y")*panSpeed, 0);
    }
  }
	
}
