using UnityEngine;

public class Mon {

    public UnityEngine.Random seed;
    public byte[] id;
    public string name;
    
    public HEXTokenType tokenType;
    public Attunement attunement;
    public Age age;
    public Race race;
    public Job job;
    public Soma soma;
    public Ego ego;
    public int[] glyph;
    

    public Mon(byte[] _id)  //string _name, HEXTokenType _tokenType, Age _age, Vector3 _attunement, Vector3 _race, Vector3 _job, Vector3 _soma, Vector3 _ego)
    {
        if(_id.Length == 32){id = _id;}
        else{Debug.Log("Invalid ID: Must be 32 bytes"); return;}
        
        int setAge = 0;
        int setAttunement = 0; 
        int setRace = 0; 
        int setJob = 0;
        soma = new Soma();
        ego = new Ego();

        for (int i = 0; i < 32 ; i++)   
        {
            UnityEngine.Random.InitState(id[i]);
            //setAge += UnityEngine.Random.;
            setAttunement += Random.Range(0,5);
            setRace += Random.Range(0,5);
            setJob += Random.Range(0,5);
            
            soma.Intelligence += Random.Range(0,24);
            soma.Strength += Random.Range(0,24);
            soma.Defense += Random.Range(0,24);
            soma.Dexterity += Random.Range(0,24);
            soma.Health += Random.Range(0,9999);
            soma.Mana += Random.Range(0,9999);

            ego.Movespeed += Random.Range(2,5);
            ego.Lifespan += Random.Range(24,100);
            ego.Power += Random.Range(1,24);
        }

        setAge = setAge % 6;
        setAttunement = setAttunement % 6;
        setRace = setRace % 6;
        setJob = setJob % 6;

        age = (Age)setAge;
        attunement = (Attunement)setAttunement;
        race = (Race)setRace;
        job = (Job)setJob;
        
        soma.Intelligence = soma.Intelligence % 24;
        soma.Strength = soma.Strength % 24;
        soma.Defense = soma.Defense % 24;
        soma.Dexterity = soma.Dexterity % 24;
        soma.Mana = soma.Mana % 9999;
        soma.Health = soma.Health % 9999;

        ego.Movespeed = (ego.Movespeed % 4) + 1;
        ego.Lifespan = (ego.Lifespan % 99) + 24;
        ego.Power = (ego.Power % 24) + 1;
        ego.Size = 1;
        ego.Level = 1;
        ego.Generation = 0;
    }
}