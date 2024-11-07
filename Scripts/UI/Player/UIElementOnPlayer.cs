using UnityEngine;

public class UIElementOnPlayer : MonoBehaviour
{

    public Vector3 PlayerPositionOffset;
    public Vector3 PositionOffset;
    private Vector3 _position;
    private GameObject _player;
    private GameObject _mainCamera;
    void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.Find("PlayerArmature");
    }

    // Update is called once per frame
    void Update()
    {
        _position = _mainCamera.GetComponent<Camera>().WorldToScreenPoint(_player.transform.position + PlayerPositionOffset);
        _position += PositionOffset;
        this.transform.position = _position;
        //transform.LookAt(_mainCamera.transform.position);
    }


    
}
