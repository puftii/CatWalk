using UnityEngine;

public class FormHolder : MonoBehaviour
{
    public Forms Form;
    private PlayerCombat _player;
    float cooldownTime;
    float activeTime;

    enum FormState
    {
        ready,
        active,
        cooldown
    }

    FormState state = FormState.ready;
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
            case FormState.ready:
                if (Input.GetKeyDown(key) && Form.staminaCost <= _player.CurrentStamina)
                {

                    Form.Activate(gameObject);
                    state = FormState.active;
                    _player.ChangeStamina(-Form.staminaCost);
                    activeTime = Form.activeTime;
                }
                break;
            case FormState.active:
                if (activeTime > 0)
                {
                    activeTime -= Time.deltaTime;
                }
                else
                {
                    Form.BeginCooldown(gameObject);
                    state = FormState.cooldown;
                    cooldownTime = Form.cooldownTime;

                }
                break;
            case FormState.cooldown:
                if (cooldownTime > 0)
                {
                    cooldownTime -= Time.deltaTime;
                }
                else
                {
                    state = FormState.ready;
                }
                break;
        }

    }
}
