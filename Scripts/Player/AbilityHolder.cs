using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    public Ability ability;
    private PlayerCombat _player;
    float cooldownTime;
    float activeTime;

    enum AbilityState
    {
        ready,
        active,
        cooldown
    }

    AbilityState state = AbilityState.ready;
    public KeyCode key;


    private void Start()
    {
        _player = GetComponent<PlayerCombat>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case AbilityState.ready:
                if (Input.GetKeyDown(key) && ability.staminaCost <= _player.CurrentStamina)
                {
                    
                    ability.Activate(gameObject);
                    state = AbilityState.active;
                    _player.ChangeStamina(-ability.staminaCost);
                    activeTime = ability.activeTime;
                }
                break;
            case AbilityState.active:
                if (activeTime > 0)
                {
                    activeTime -= Time.deltaTime;
                }
                else
                {
                    ability.BeginCooldown(gameObject);
                    state = AbilityState.cooldown;
                    cooldownTime = ability.cooldownTime;

                }
                break;
            case AbilityState.cooldown:
                if (cooldownTime > 0)
                {
                    cooldownTime -= Time.deltaTime;
                }
                else
                {
                    state = AbilityState.ready;
                }
                break;
        }

    }
}
