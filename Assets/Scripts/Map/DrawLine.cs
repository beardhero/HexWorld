using UnityEngine;
using System.Collections;

public class DrawLine : MonoBehaviour {

  private LineRenderer lineRend;
  private float counter;
  private float dist;

  public Transform origin;
  public Transform destination;

  public float lineDrawSpeed = 7f;
  public DrawLine()
  {

  }
  public DrawLine(Transform from, Transform to)
  {
    origin = from;
    destination = to;
    lineRend = GetComponent<LineRenderer>();
    lineRend.SetPosition(0, from.position);
    lineRend.SetWidth(.5f, .5f);
    dist = Vector3.Distance(from.position, to.position);
    StartCoroutine(Draw());
  }
  public IEnumerator Draw()
  {
    float counter = 0;
    while (counter < dist)
    {
      counter += .1f / lineDrawSpeed;
      float lerp = Mathf.Lerp(0, dist, counter);
      Vector3 pointOnLine = lerp * Vector3.Normalize(destination.position - origin.position) + origin.position;

      lineRend.SetPosition(1, pointOnLine);
    }
    yield return null;
  }
}
