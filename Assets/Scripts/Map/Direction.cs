using UnityEngine;

public static class Direction
{ 
  public const int
  Y     = 0, // 0, 1
  NegXY = 1,
  X     = 2,      // 1, 0
  NegY  = 3,   // 0, -1
  XY    = 4,   // -1, 0
  NegX  = 5, 
  Count = 6;

  public static string ToString(int dir)
  {
    switch (dir)
    {
      case Y:     return "+Y";
      case NegXY: return "-XY";
      case X:     return "+X";
      case NegY:  return "-Y";
      case XY:    return "XY";
      case NegX:  return "-X";
      default:    return "Not a direction";
    }
  }

  public static Color ToColor (int dir)
  {
    switch (dir)
    {
      case Y:     return Color.yellow;
      case NegXY: return Color.blue;
      case X:     return Color.red;
      case NegY:  return Color.green;
      case XY:    return Color.cyan;
      case NegX:  return Color.magenta;
      default:    return Color.gray;
    }
  }
}

