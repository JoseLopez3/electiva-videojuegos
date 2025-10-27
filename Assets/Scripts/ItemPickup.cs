using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ConsumableItem item; // Referencia al ScriptableObject que representa este item

   private void Start()
{
    if (item == null)
    {
        Debug.LogError("ERROR: ¡No hay ningún ConsumableItem asignado en el Inspector de " + gameObject.name + "!");
        return; 
    }

    if (item.icon == null)
    {
        Debug.LogWarning("ADVERTENCIA: El ConsumableItem '" + item.name + "' no tiene un Sprite asignado en su campo 'Icon'.");
    }
    else
    {
        Debug.Log("ÉXITO: Asignando el sprite '" + item.icon.name + "' al objeto " + gameObject.name);
    }

    // Asigna el sprite del item al objeto en la escena para que se vea correcto
    GetComponent<SpriteRenderer>().sprite = item.icon;
}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            
            GameEvents.ItemPickedUp(item); // Anuncia que se intentó recoger un item
            Destroy(gameObject); // se asume que siempre se recoge.
        }
    }
}
