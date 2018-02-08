﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy
{
    public int Id;
    public string Name;
    public int Health;
    public int Stemina;
    public int Defence;
    public int Strength;
    public int Concentration;
    public int Speed;
    public string Path;

    public Enemy(int id, string name, int health, int stemina, int defence, int strength, int concentration, int speed, string path)
    {
        Id = id;
        Name = name;
        Health = health;
        Stemina = stemina;
        Defence = defence;
        Strength = strength;
        Concentration = concentration;
        Speed = speed;
        Path = path;
    }
}

public class EnemyData : MonoBehaviour {

    static List<Enemy> list = new List<Enemy>();

    public static void Read()
    {
        List<Dictionary<string, object>> data = CSVParser.Read("EnemyData");

        foreach (Dictionary<string, object> e in data)
        {
            Enemy p = new Enemy(int.Parse(e["KEY"].ToString()),
                e["NAME"].ToString(),
                int.Parse(e["HEALTH"].ToString()),
                int.Parse(e["STEMINA"].ToString()),
                int.Parse(e["DEFENCE"].ToString()),
                int.Parse(e["STRENGTH"].ToString()),
                int.Parse(e["CONCENTRATION"].ToString()),
                int.Parse(e["SPEED"].ToString()),
                e["FILEPATH"].ToString());

            list.Add(p);
        }
    }

    public static Enemy FindEnemyInfoByID(int id)
    {
        foreach (Enemy e in list)
        {
            if (id == e.Id)
                return e;
        }

        Enemy c = new Enemy(0, "not found", 0, 0, 0, 0, 0, 0, "not found");

        return c;
    }
}
