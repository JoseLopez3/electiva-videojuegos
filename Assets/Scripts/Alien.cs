using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : Enemy
{
    // Constructor
    public Alien(string name) : base(name)
    {
    }

    // Sobrescribe el metodo de ataque y derrota
    public override void Attack()
    {
        Debug.Log($"------- Ataque -------");

        Debug.Log($"{Name} procede a hacer una Genkidama");
    }

    public override void DefeatMessage()
    {
        Debug.Log($"------- Tras derrotar a un enemigo -------");

        Debug.Log($"{Name} procede a decir que tiene hambre");
    }
}