using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static event Action<float> PlayerHealthChanged;
    public static event Action PlayerDied;

    public static void OnPlayerHealthChanged(float currentHealth)
    {
        PlayerHealthChanged?.Invoke(currentHealth);
    }

    public static void OnPlayerDied() 
    { 
        PlayerDied?.Invoke();
    }
}
