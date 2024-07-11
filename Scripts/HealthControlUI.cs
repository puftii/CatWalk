using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HealthControlUI : MonoBehaviour
{
    private void onEnable()
    {

    }

    private void onDisable()
    {
        EventManager.PlayerHealthChanged -= ChangeHealth;
    }


    // Start is called before the first frame update
    void Start()
    {
        EventManager.PlayerHealthChanged += ChangeHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ChangeHealth(float currentHealth)
    {
        Debug.Log("Ударился");
        GetComponent<TextMeshProUGUI>().text = currentHealth.ToString();
    }
}
