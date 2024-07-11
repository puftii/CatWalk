using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HealthControlUI : MonoBehaviour
{
    private void onEnable()
    {
        PlayerCombat.damaged += ChangeHealth;
    }

    private void onDisable()
    {
        PlayerCombat.damaged -= ChangeHealth;
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ChangeHealth(float currentHealth)
    {
        Console.WriteLine("Ударился");
        GetComponent<TextMeshProUGUI>().text = currentHealth.ToString();
    }
}
