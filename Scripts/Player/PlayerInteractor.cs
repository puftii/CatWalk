using StarterAssets;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.Windows;
#endif


public class PlayerInteractor : MonoBehaviour
{
    public float InteractRadius = 1f;
    private StarterAssetsInputs _input;
#if ENABLE_INPUT_SYSTEM
    private PlayerInput _playerInput;
    
#endif
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
        _playerInput = GetComponent<PlayerInput>();
#else

			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if(_input.interact == true && !GameManager.GamePaused)
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, InteractRadius);
            foreach (Collider hit in hits) 
            {
                if (hit.TryGetComponent(out IInteractable interactable)) 
                { 
                    interactable.Interact();
                }
            }
            _input.interact = false;
        }
    }
}
