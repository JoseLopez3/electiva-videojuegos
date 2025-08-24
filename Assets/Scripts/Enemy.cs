using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy 
{
    // Propiedades
    public string Name { get; protected set; }
    public List<string> Skills { get; protected set; }
    public List<string> Weaknesses { get; protected set; }

    // Constructor
    public Enemy(string name)
    {
        this.Name = name;
        this.Skills = new List<string>();
        this.Weaknesses = new List<string>();
    }

    // Métodos
    public void AddSkill(string skill)
    {
        Skills.Add(skill);
    }

    public void AddWeakness(string weakness)
    {
        Weaknesses.Add(weakness);
    }

    public void ShowSkills()
    {
        Debug.Log($"----- Habilidades de {Name} -----");
        if (Skills.Count == 0)
        {
            Debug.Log("No tiene habilidades");
            return;
        }
        foreach (string skill in Skills)
        {
            Debug.Log($"- {skill}");
        }
    }

    public void ShowWeaknesses()
    {
        Debug.Log($"----- Debilidades de {Name}  -----");
        if (Weaknesses.Count == 0)
        {
            Debug.Log("No tiene debilidades");
            return;
        }
        foreach (string weakness in Weaknesses)
        {
            Debug.Log($"- {weakness}");
        }
    }

    public virtual void Attack()
    {
        Debug.Log($"{Name} realiza un ataque");
    }

    public virtual void DefeatMessage()
    {
        Debug.Log($"{Name} ha derrotado a alguien");
    }
}

