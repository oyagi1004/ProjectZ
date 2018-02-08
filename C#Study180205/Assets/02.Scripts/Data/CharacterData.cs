using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character
{
    public int Id;
    public string Name;
    public int Health;
    public int Stemina;
    public int Defence;
    public int Strength;
    public int Concentration;
    public int Speed;

    public Character(int id, string name, int health, int stemina, int defence, int strength, int concentration, int speed)
    {
        Id = id;
        Name = name;
        Health = health;
        Stemina = stemina;
        Defence = defence;
        Strength = strength;
        Concentration = concentration;
        Speed = speed;
    }
}

public class CharacterData : MonoBehaviour {
    static List<Character> list = new List<Character>();

    public static void Read()
    {
        List<Dictionary<string, object>> data = CSVParser.Read("testData");

        foreach (Dictionary<string, object> e in data)
        {
            Character p = new Character(int.Parse(e["KEY"].ToString()),
                e["NAME"].ToString(),
                int.Parse(e["HEALTH"].ToString()), 
                int.Parse(e["STEMINA"].ToString()),
                int.Parse(e["DEFENCE"].ToString()),
                int.Parse(e["STRENGTH"].ToString()),
                int.Parse(e["CONCENTRATION"].ToString()),
                int.Parse(e["SPEED"].ToString()));

            list.Add(p);
        }
    }

    public static Character FindCharacterInfoByID(int id)
    {
        foreach(Character e in list)
        {
            if (id == e.Id)
                return e;
        }

        Character c = new Character(0, "not found", 0, 0, 0, 0, 0, 0);

        return c;
    }
}
