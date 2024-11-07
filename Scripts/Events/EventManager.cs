using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{

    //PlayerEvents
    public static event Action<float> PlayerHealthChanged;
    public static event Action<float> PlayerMaxHealthChanged;
    public static event Action PlayerDied;
    public static event Action<float> PlayerStaminaChanged;
    public static event Action<float> PlayerMaxStaminaChanged;
    public static event Action<bool> PlayerWeaponHolstered;

    //DemonEvents
    public static event Action DemonDied;
    

    //Player Methods
    public static void OnPlayerHealthChanged(float currentHealth)
    {
        PlayerHealthChanged?.Invoke(currentHealth);
    }

    public static void OnPlayerMaxHealthChanged(float currentHealth)
    {
        PlayerMaxHealthChanged?.Invoke(currentHealth);
    }

    public static void OnPlayerDied() 
    { 
        PlayerDied?.Invoke();
    }

    public static void OnPlayerStaminaChanged(float currentStamina)
    {
        PlayerStaminaChanged?.Invoke(currentStamina);
    }

    public static void OnPlayerMaxStaminaChanged(float maxStamina)
    {
        PlayerMaxStaminaChanged?.Invoke(maxStamina);
    }

    public static void OnPlayerWeaponHolstered(bool weaponHolstered)
    {
        PlayerWeaponHolstered?.Invoke(weaponHolstered);
    }


    //Demon Methods
    public static void OnDemonDied()
    {
        DemonDied?.Invoke();
    }
}
