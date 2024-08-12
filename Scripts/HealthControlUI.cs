using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class HealthControlUI : MonoBehaviour
{
    private TMP_Text _text;

    private void OnEnable()
    {
        EventManager.PlayerHealthChanged += ChangeHealth;
    }

    private void OnDisable()
    {
        EventManager.PlayerHealthChanged -= ChangeHealth;
    }

    private void OnDestroy()
    {
        EventManager.PlayerHealthChanged -= ChangeHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            _text = GetComponent<TMP_Text>();
            PlayerCombat playerCombat = (PlayerCombat)FindFirstObjectByType(typeof(PlayerCombat));
            ChangeHealth(playerCombat.CurrentHealth);
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
            _text.text = currentHealth.ToString();
        }
    }
}
