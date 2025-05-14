using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFunctionTask : MonoBehaviour
{
    // Start is called before the first frame update
    public int playerHealth = 100;
    public int playerArmor = 100;
    public int playerDamage = 20;
    void Start()
    {
        int effectiveDamage = GetEffectiveDamage(playerArmor);
        playerHealth -= effectiveDamage;

        Debug.Log("Damage diterima: " + effectiveDamage);
        Debug.Log("Status: " + GetPlayerStatus(playerHealth));
        Debug.Log("Masih hidup? " + IsPlayerAlive(playerHealth));
    }

    string GetPlayerStatus(int health)
    {
        if (health > 50)
            return "Hidup";
        else if (health > 0)
            return "Lemah";
        else
            return "Mati";
    }

    int GetEffectiveDamage(int armor)
    {
        int damageReduction = armor / 10; // Armor 100 = -10 damage
        int result = playerDamage - damageReduction;
        return result > 0 ? result : 0;   // damage minimal 0
    }

    bool IsPlayerAlive(int health)
    {
        return health > 0;
    }
    
}
