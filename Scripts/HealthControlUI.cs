using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class HealthControlUI : MonoBehaviour
{
    private Text _text;

    private void onEnable()
    {
        EventManager.PlayerHealthChanged += ChangeHealth;
    }

    private void onDisable()
    {
        EventManager.PlayerHealthChanged -= ChangeHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            _text = GetComponent<Text>();
            Debug.Log("Текст найден");
        }
        catch (ArgumentException e)
        {
            Debug.Log($"Ошибка: {e.Message}");
        }
        EventManager.PlayerHealthChanged += ChangeHealth;
    }

    void onDestroy()
    {
        EventManager.PlayerHealthChanged -= ChangeHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ChangeHealth(float currentHealth)
    {
        if (_text != null)
        {
            GetComponent<Text>().text = currentHealth.ToString();
        }
    }
}
