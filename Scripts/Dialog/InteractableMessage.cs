using UnityEngine;

public class InteractableMessage : MonoBehaviour, IInteractable
{
    public float InteractRange = 1.0f;
    private DialogManager _dialogManager;

    public void Interact()
    {
        if(_dialogManager != null)
        {
            _dialogManager.StartDialog();
        }
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _dialogManager = GetComponent<DialogManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
