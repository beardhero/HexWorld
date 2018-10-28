using UnityEngine;
using System.Collections;

[System.Serializable]
public class Hexagon
{
  public int index;
  public SerializableVector3 center, normal, v1, v2, v3, v4, v5, v6;
  public int uv0i, uv1i, uv2i, uv3i, uv4i, uv5i, uv6i;
  public int[] neighbors;
  public bool isPentagon;
  private float _scale;
	public float scale { get{return _scale;}
	  set{
			v1 /= v1.magnitude;
			v1 *= value;
			v2 /= v2.magnitude;
			v2 *= value;
			v3 /= v3.magnitude;
			v3 *= value;
			v4 /= v4.magnitude;
			v4 *= value;
			v5 /= v5.magnitude;
			v5 *= value;
			v6 /= v6.magnitude;
			v6 *= value;
			center = (v1 + v2 + v3 + v4 + v5 + v6) / 6f;
			_scale = center.magnitude;
		}
  }

  public Hexagon(){}
  public Hexagon(int i, Vector3 c, Vector3[] verts, SerializableVector3 origin)
  {
    index = i;
    neighbors = new int[]{-1,-1,-1,-1,-1,-1};
    center = c;
    v1 = verts[0];
    v2 = verts[1];
    v3 = verts[2];
    v4 = verts[3];
    v5 = verts[4];
    v6 = verts[5];
    _scale = center.magnitude;
    normal = ((Vector3)(center - origin)).normalized;
    foreach (Vector3 v in PolySphere.icoCoords)
    {
      if (center == v)
      {
        isPentagon = true;
      }
    }   
  }
  public void Scale(float _scale) //This will multiply all vectors in the hexagon by the value, if you want to set the scale directly set the property
  {
    v1 *= _scale;
    v2 *= _scale;
    v3 *= _scale;
    v4 *= _scale;
    v5 *= _scale;
    v6 *= _scale;
    center *= _scale; //(v1 + v2 + v3 + v4 + v5 + v6) / 6f;
    scale = center.magnitude;
  }
}
