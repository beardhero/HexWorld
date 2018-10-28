//------------------------------//
//  Tutorial_6.cs               //
//  Written by Alucard Jay      //
//  2014/6/29                   //
//------------------------------//

/* http://libnoise.sourceforge.net/tutorials/tutorial6.html


using UnityEngine;
using System.Collections;

using LibNoise.Unity;
using LibNoise.Unity.Generator;
using LibNoise.Unity.Operator;


public class SphereMap : MonoBehaviour
{
  public int mapSizeX = 256; // for heightmaps, this would be 2^n +1
  public int mapSizeY = 256; // for heightmaps, this would be 2^n +1

  public float sampleSizeX = 4.0f; // perlin sample size
  public float sampleSizeY = 4.0f; // perlin sample size

  public float sampleOffsetX = 6.0f; // to tile, add size to the offset. eg, next tile across would be 6.0f
  public float sampleOffsetY = 1.0f; // to tile, add size to the offset. eg, next tile up would be 5.0f

  void Start()
  {
    Generate();
  }


  void Update()
  {
    if (Input.GetMouseButtonDown(0))
      Generate();
  }


  //  Other Functions
  //    ----------------------------------------------------------------------------


  public float baseflatFrequency = 2.0f;

  public float flatScale = 0.125f;
  public float flatBias = -0.75f;

  public float terraintypeFrequency = 0.5f;
  public float terraintypePersistence = 0.25f;

  public float terrainSelectorEdgeFalloff = 0.125f;

  public float finalterrainFrequency = 4.0f;
  public float finalterrainPower = 0.125f;


  void Generate()
  {
    // - Mountain Terrain -

    //module::RidgedMulti mountainTerrain;

    Billow mountainTerrain = new Billow();



    // - Base Flat Terrain -

    //module::Billow baseFlatTerrain;
    //baseFlatTerrain.SetFrequency (2.0);

    Billow baseFlatTerrain = new Billow();

    baseFlatTerrain.Frequency = baseflatFrequency;



    // - Flat Terrain -

    //module::ScaleBias flatTerrain;
    //flatTerrain.SetSourceModule( 0, baseFlatTerrain );
    //flatTerrain.SetScale (0.125);
    //flatTerrain.SetBias (-0.75);

    ScaleBias flatTerrain = new ScaleBias(flatScale, flatBias, baseFlatTerrain); // scale, bias, input



    // - Terrain Type -

    //module::Perlin terrainType;
    //terrainType.SetFrequency (0.5);
    //terrainType.SetPersistence (0.25);

    Perlin terrainType = new Perlin();

    terrainType.Frequency = terraintypeFrequency;
    terrainType.Persistence = terraintypePersistence;



    // - Terrain Selector -

    //module::Select terrainSelector;
    //terrainSelector.SetSourceModule (0, flatTerrain);
    //terrainSelector.SetSourceModule (1, mountainTerrain);

    Select terrainSelector = new Select(flatTerrain, mountainTerrain, terrainType); // input A, input B, Controller


    // terrainSelector.SetBounds (0.0, 1000.0);
    terrainSelector.SetBounds(0.0, 1000.0);

    //terrainSelector.SetEdgeFalloff (0.125);
    terrainSelector.FallOff = terrainSelectorEdgeFalloff;



    // - Final Terrain -

    //module::Turbulence finalTerrain;
    //finalTerrain.SetSourceModule (0, terrainSelector);

    Turbulence finalTerrain = new Turbulence(terrainSelector);

    //finalTerrain.SetFrequency (4.0);
    finalTerrain.Frequency = finalterrainFrequency;

    //finalTerrain.SetPower (0.125);
    finalTerrain.Power = finalterrainPower;




    // ------------------------------------------------------------------------------------------

    // - Compiled Terrain -

    ModuleBase myModule;

    myModule = finalTerrain;



    // ------------------------------------------------------------------------------------------

    // - Generate -

    // this part generates the heightmap to a texture, 
    // and sets the renderer material texture of a cube to the generated texture


    Noise2D heightMap;

    heightMap = new Noise2D(mapSizeX, mapSizeY, myModule);

    heightMap.GeneratePlanar(
        sampleOffsetX,
        sampleOffsetX + sampleSizeX,
        sampleOffsetY,
        sampleOffsetY + sampleSizeY
        );

    texture = heightMap.GetTexture(GradientPresets.Grayscale);

    cubeRenderer.material.mainTexture = texture;
  }
}
*/