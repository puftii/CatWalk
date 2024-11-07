using System;
using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    private GameObject _player;

    private Slider _staminaBar;

    private void OnEnable()
    {
        EventManager.PlayerStaminaChanged += ChangeStamina;
    }

    private void OnDisable()
    {
        EventManager.PlayerStaminaChanged -= ChangeStamina;
    }

    private void OnDestroy()
    {
        EventManager.PlayerStaminaChanged -= ChangeStamina;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        try
        {
            _staminaBar = GetComponent<Slider>();
            _player = GameObject.Find("PlayerArmature");
        }
        catch (ArgumentException e)
        {
            Debug.Log($"Ошибка: {e.Message}");
        }
        ValuesChange();
    }

    // Update is called once per frame
    void Update()
    {
        //ValuesChange();
    }

    void ValuesChange()
    {
        if (_staminaBar != null)
        {
            _staminaBar.maxValue = _player.GetComponent<PlayerCombat>().MaxStamina;
            _staminaBar.value = _player.GetComponent<PlayerCombat>().CurrentStamina;
        }
    }

    void ChangeStamina(float currentStamina)
    {
        if (_staminaBar != null)
        {
            _staminaBar.value = currentStamina;
        }
    }
}
