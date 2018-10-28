using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ZoneRenderer : MonoBehaviour  //turning this into card renderer
{
  public GameObject boardPrefab;
  //public Transform waterTrans;

	public List<GameObject> RenderZone(Zone zone, TileSet tileSet)
  {
    // Set water
    //waterTrans.position = new Vector3(waterTrans.position.x, zone.waterHeight, waterTrans.position.z);

    List<GameObject> output = new List<GameObject>();

    // Determine 
    int maxWidth = 55;
    int sectionsWide = 1;
    if (zone.width > maxWidth)
    {
      sectionsWide = (int)Mathf.Ceil(zone.width/(float)maxWidth);
    }

    for (int secX=0; secX<sectionsWide; secX++)
    {
      for (int secY=0; secY<sectionsWide; secY++)
      {
        // Copy a section of tiles from zone
        Tile[,] tileSection = new Tile[maxWidth,maxWidth];
        for (int tx=secX*maxWidth; tx<secX*maxWidth+maxWidth && tx<zone.width; tx++)
        {
          for (int ty=secY*maxWidth; ty<secY*maxWidth+maxWidth && ty<zone.width; ty++)
          {
            tileSection[tx-secX*maxWidth, ty-secY*maxWidth] = zone.tiles[tx,ty];
          }
        }

        float xOff = 0;
        if (secY%2==1)
          xOff += Hex.bisect;

        Vector3 zonePlacement = new Vector3(Hex.bisect*2 * maxWidth*secX+xOff, 0, Hex.sideAndAHalf * maxWidth*secY);
        GameObject render = (GameObject)Instantiate(boardPrefab, zonePlacement, Quaternion.identity);

        MeshFilter myFilter = render.GetComponent<MeshFilter>();
        MeshCollider myCollider = render.GetComponent<MeshCollider>();
        Renderer myRend = render.GetComponent<Renderer>();

        List<Vector3> vertices = new List<Vector3>();
        List<int> triangles = new List<int>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2>uvs = new List<Vector2>();
        
        Vector3 origin,
                    v1 = new Vector3(0,0,Hex.edge),
                    v2 = new Vector3(Hex.bisect, 0, Hex.halfSide),
                    v3 = new Vector3(Hex.bisect, 0, -Hex.halfSide),
                    v4 = new Vector3(0,0,-Hex.edge),
                    v5 = new Vector3(-Hex.bisect,0,-Hex.halfSide),
                    v6 = new Vector3(-Hex.bisect, 0, Hex.halfSide);

        float texHeight = tileSet.texture.height;
        float texWidth = tileSet.texture.width;
        //float root3 = Mathf.Sqrt(3);
        float uvTileWidth = 1f/42f;//tileSet.tileWidth/texWidth;
        float uvTileHeight = 1f/42f;//tileSet.tileWidth/texHeight;
        float side = uvTileWidth/2;
        float radius = Mathf.Sqrt((3*side*side)/4);
        /* OLD UVS
        Vector2 uv0 = new Vector2(side,side),
                uv1 = new Vector2(side, side+side),
                uv2 = new Vector2(side+radius, side+side/2),
                uv3 = new Vector2(side+radius, side/2),
                uv4 = new Vector2(side, 0),
                uv5 = new Vector2(side-radius, side/2),
                uv6 = new Vector2(side-radius, side+side/2);
        */
        Vector2 uv0 = new Vector2 (uvTileWidth/2.0f, uvTileHeight / 2.0f),
			  uv1 = new Vector2 (10/texWidth, 98/texHeight),
			  uv2 = new Vector2 (54/texWidth, 22/texHeight),
		  	uv3 = new Vector2 (141/texWidth, 22/texHeight),
			  uv4 = new Vector2 (185/texWidth, 98/texHeight),
			  uv5 = new Vector2 (141/texWidth, 173/texHeight),
			  uv6 = new Vector2 (54/texWidth, 173/texHeight);
        int counter = 0;
        for (int x=0; x<zone.width && x<tileSection.GetLength(0); x++)
        {
          for (int y=0;y<zone.width && y<tileSection.GetLength(1); y++)
          {
            if (tileSection[x,y] == null)
              continue;
            Tile tile = tileSection[x,y];
            TileType tileType = tile.type;

            if (tileType == TileType.None)
              continue;
            /*
                    1 = 0,side
                6         2=height, side/2
                origin(0)
                5         3=height, -side/2
                    4= 0,-side
            */

            //IntCoord uvCoord = tileSet.GetUVForType(tileType);  //not doing this in the editor rn
            Vector2 uvOffset = new Vector2(tile.uvx*uvTileWidth, tile.uvy*uvTileHeight);

            IntCoord sideUV = tileSet.GetSideUVForType(tileType);
            Vector2 sideUVOffset = new Vector2(sideUV.x*uvTileWidth, sideUV.y*uvTileHeight);

            float xOffset = x*Hex.doubleHeight;
            Vector3 tileHeight = Vector3.up*tileSection[x,y].height;

            if (y%2==1)
              xOffset += Hex.bisect;

            origin = new Vector3(xOffset, 0, y*Hex.sideAndAHalf);

            // Add the first hexagon (top)
            vertices.Add(origin + tileHeight);
            normals.Add(Vector3.up);
            uvs.Add(uv0+uvOffset);
        //1
            vertices.Add(origin+v1 + tileHeight);
            normals.Add(Vector3.up);
            uvs.Add(uv1+uvOffset);
        //2
            vertices.Add(origin+v2 + tileHeight);
            normals.Add(Vector3.up);
            uvs.Add(uv2+uvOffset);
        //3
            vertices.Add(origin+v3 + tileHeight);
            normals.Add(Vector3.up);
            uvs.Add(uv3+uvOffset);
        //4
            vertices.Add(origin+v4 + tileHeight);
            normals.Add(Vector3.up);
            uvs.Add(uv4+uvOffset);
        //5
            vertices.Add(origin+v5 + tileHeight);
            normals.Add(Vector3.up);
            uvs.Add(uv5+uvOffset);
        //6
            vertices.Add(origin+v6 + tileHeight);
            normals.Add(Vector3.up);
            uvs.Add(uv6+uvOffset);

            /*
                  .....
                 / 6|  /\
                / \ |1/  \
               / 5 \|/  2 \
               \  /4|3\  /
                \/..|..\/
            */

            // Hex 1 (top)
            // Triangle 1
            triangles.Add(counter);
            triangles.Add(counter+1);
            triangles.Add(counter+2);
            // Triangle 2
            triangles.Add(counter);
            triangles.Add(counter+2);
            triangles.Add(counter+3);
            // Triangle 3
            triangles.Add(counter);
            triangles.Add(counter+3);
            triangles.Add(counter+4);
            // Triangle 4
            triangles.Add(counter);
            triangles.Add(counter+4);
            triangles.Add(counter+5);
            // Triangle 5
            triangles.Add(counter);
            triangles.Add(counter+5);
            triangles.Add(counter+6);
            // Triangle 6
            triangles.Add(counter);
            triangles.Add(counter+6);
            triangles.Add(counter+1);

            counter += 7;

            // Don't render sides if it's water
            if (tileType != TileType.Water)
            {
              // Second hex (bottom)
              vertices.Add (origin);
              normals.Add (Vector3.up);
              uvs.Add (uv0+sideUVOffset);
              //8
              vertices.Add (origin+v1);
              normals.Add (Vector3.up);
              uvs.Add (uv1+sideUVOffset);
              //9
              vertices.Add (origin+v2);
              normals.Add (Vector3.up);
              uvs.Add (uv2+sideUVOffset);
              //10
              vertices.Add (origin+v3);
              normals.Add (Vector3.up);
              uvs.Add (uv3+sideUVOffset);
              //11
              vertices.Add (origin+v4);
              normals.Add (Vector3.up);
              uvs.Add (uv4+sideUVOffset);
              //12
              vertices.Add (origin+v5);
              normals.Add (Vector3.up);
              uvs.Add (uv5+sideUVOffset);
              //13
              vertices.Add (origin+v6);
              normals.Add (Vector3.up);
              uvs.Add (uv6+sideUVOffset);

              // Third hex for black border (top with black UVs)
              vertices.Add (origin + tileHeight);
              normals.Add (Vector3.up);
              uvs.Add (uv0+sideUVOffset);
              //15
              vertices.Add (origin+v1+ tileHeight);
              normals.Add (Vector3.up);
              uvs.Add (uv4+sideUVOffset);
              //16
              vertices.Add (origin+v2+ tileHeight);
              normals.Add (Vector3.up);
              uvs.Add (uv5+sideUVOffset);
              //17
              vertices.Add (origin+v3+ tileHeight);
              normals.Add (Vector3.up);
              uvs.Add (uv6+sideUVOffset);
              //18
              vertices.Add (origin+v4+ tileHeight);
              normals.Add (Vector3.up);
              uvs.Add (uv1+sideUVOffset);
              //19
              vertices.Add (origin+v5+ tileHeight);
              normals.Add (Vector3.up);
              uvs.Add (uv2+sideUVOffset);
              //20
              vertices.Add (origin+v6+ tileHeight);
              normals.Add (Vector3.up);
              uvs.Add (uv3+sideUVOffset);

              // The 6 Parallelograms (12 triangles) which complete the hexagon
              // Edge 1-2 on top 
              triangles.Add(counter + 1);
              triangles.Add(counter + 8);
              triangles.Add(counter + 13);
              triangles.Add (counter + 1);
              triangles.Add (counter + 13);
              triangles.Add (counter + 6);
              
              triangles.Add (counter + 2);
              triangles.Add (counter + 8);
              triangles.Add (counter + 1);
              triangles.Add (counter + 2);
              triangles.Add (counter + 9);
              triangles.Add (counter + 8);

              triangles.Add (counter + 3);
              triangles.Add (counter + 9);
              triangles.Add (counter + 2);
              triangles.Add (counter + 3);
              triangles.Add (counter + 10);
              triangles.Add (counter + 9);

              triangles.Add (counter + 4);
              triangles.Add (counter + 10);
              triangles.Add (counter + 3);
              triangles.Add (counter + 4);
              triangles.Add (counter + 11);
              triangles.Add (counter + 10);

              triangles.Add (counter + 5);
              triangles.Add (counter + 11);
              triangles.Add (counter + 4);
              triangles.Add (counter + 5);
              triangles.Add (counter + 12);
              triangles.Add (counter + 11);

              triangles.Add (counter + 6);
              triangles.Add (counter + 12);
              triangles.Add (counter + 5);
              triangles.Add (counter + 6);
              triangles.Add (counter + 13);
              triangles.Add (counter + 12);

              counter += 14;
            }
          }
        }


        Mesh m = new Mesh();
        m.vertices = vertices.ToArray();
        m.triangles = triangles.ToArray();
        m.normals = normals.ToArray();
        m.uv = uvs.ToArray();

        myCollider.sharedMesh = m;
        myFilter.sharedMesh = m;
        myRend.material.mainTexture = tileSet.texture;

        output.Add(render);
      }
    }

    return output;
  }
}