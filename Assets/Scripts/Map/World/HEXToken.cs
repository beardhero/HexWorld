using UnityEngine;
//using System;
//using System.Collections;

public enum HEXTokenType{MON, NOM, MOVE, LOOT, HERO, WORLD};
public enum Age{Egg, Child, Adult, Parent, Elder, Soul};
public enum Race{Mind, Mech, Root, Hive, Nether, Aether};
public enum Attunement{Water, Fire, Earth, Air, Light, Dark};
public enum Job{Control, Burst, Melee, Ranged, Tank, Healer};
public struct Soma{public float Intelligence, Strength, Defense, Dexterity, Mana, Health;}
public struct Ego{public int Level, Power, Size, Movespeed, Lifespan, Generation;};
public enum Status{Soak, Burn, Root, Chill, Confuse, Blind};
//public enum NameSyll{tra, arb, reb, syn, vin, rin, lin, tar, fil, gar, nar, var, gub, lub, veb, leb, vub, ad, ta, xo, va};

//public enum Ego{Luck, Cost, Size, };

public class HEXToken
{
    public UnityEngine.Random seed;
    public byte[] id;
    
    public HEXToken(){}

    public HEXToken(byte[] _id, HEXTokenType tt)
    {
        id = _id;
    }     
}

