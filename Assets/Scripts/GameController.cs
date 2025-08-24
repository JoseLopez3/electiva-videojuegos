using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    void Start()
    {
        // Inicio de instancias de monstruo come galletas y marciano 
        Monster cookieMonster = new Monster("Monstruo come galletas");
        Alien kakarotto = new Alien("Goku");
        
        Debug.Log("Demostracion de clases de enemigos");

        // Se Agregan las  habilidades y debilidades para ambos
        cookieMonster.AddSkill("Comer galleta (Regeneración de vida)");
        cookieMonster.AddWeakness("Gluten");
        

        kakarotto.AddSkill("Genética Saiyan (Aumenta su limite de poder, tras cada batalla)");
        kakarotto.AddSkill("Detectar el Ki");
        kakarotto.AddSkill("Teletransportación");
        kakarotto.AddSkill("Resistencia al dolor");

        kakarotto.AddWeakness("Ingenuidad");
        kakarotto.AddWeakness("Limite de tiempo en transformaciones");

        
        Debug.Log($"\n -----{cookieMonster.Name.ToUpper()}-----");
        cookieMonster.ShowSkills();
        cookieMonster.ShowWeaknesses();
        cookieMonster.Attack();
        cookieMonster.DefeatMessage();

        Debug.Log($"\n -----{kakarotto.Name.ToUpper()}-----");
        kakarotto.ShowSkills();
        kakarotto.ShowWeaknesses();
        kakarotto.Attack();
        kakarotto.DefeatMessage();
        
    }
}