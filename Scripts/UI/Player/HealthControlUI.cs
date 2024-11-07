using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Unity.VisualScripting;

public class HealthControlUI : MonoBehaviour
{
    public float HealthSizePerTick = 5;
    private PlayerCombat _player;
    private TMP_Text _text;
    private Slider _slider;
    private RectTransform _rectTransform;

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
        _rectTransform = GetComponent<RectTransform>();
        _slider = GetComponent<Slider>();
        try
        {
            _text = GetComponent<TMP_Text>();
            _slider = GetComponent<Slider>();
            _player = GameObject.Find("PlayerArmature").GetComponent<PlayerCombat>();
            Debug.Log("Slider gotov");
            if(_slider != null)
            {
                ValuesChange();
            }
        }
        catch (ArgumentException e)
        {
            Debug.Log($"Ошибка: {e.Message}");
        }
        /*_text = GetComponent<TMP_Text>();
        PlayerCombat playerCombat = (PlayerCombat)FindFirstObjectByType(typeof(PlayerCombat));
        ChangeHealth(playerCombat.CurrentHealth);
        try
        {
            _text = GetComponent<TMP_Text>();
            playerCombat = (PlayerCombat)FindFirstObjectByType(typeof(PlayerCombat));
            ChangeHealth(playerCombat.CurrentHealth);
        }
        catch (ArgumentException e)
        {
            Debug.Log($"Ошибка: {e.Message}");
        }*/
        EventManager.PlayerHealthChanged += ChangeHealth;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void ChangeHealth(float currentHealth)
    {
        if (_text != null)
        {
            float health = Mathf.Round(currentHealth);
            _text.text = health.ToString();
        }
        if(_slider != null)
        {
            ValuesChange();
        }
    }

    void ValuesChange()
    {
        if (_slider != null)
        {
            _slider.maxValue = _player.MaxHealth;
            _slider.value = _player.CurrentHealth;
            MaxHealthChange(_player.MaxHealth);
        }
    }


    void MaxHealthChange(float MaxHealth)
    {
        _rectTransform.sizeDelta = new Vector2(MaxHealth * HealthSizePerTick, _rectTransform.sizeDelta.y);
    }
}
