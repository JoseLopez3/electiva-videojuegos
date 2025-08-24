using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Monster : Enemy
{
    // Constructor 
    public Monster(string name) : base(name)
    {
    }

    // Se sobreescriben los metodos de ataque y mejora
    public override void Attack()
    {
        Debug.Log($"------- Ataque -------");

        Debug.Log($"{Name} hace un ataque monstruoso :O");
    }

    public override void DefeatMessage()
    {
        Debug.Log($"------- Tras derrotar a un enemigo -------");

        Debug.Log($"{Name} Procede a comer galletas");
    }
}