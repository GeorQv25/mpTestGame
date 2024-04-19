using Mirror;
using UnityEngine;


[RequireComponent(typeof(IInput), typeof(ILaunch))]
public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Transform _cameraSpot;

    [SyncVar] private GameObject _scoreManager;
    private GameObject _scoreManagerPanel;
    private GunRotator _gunRotator;
    private IInput _playerInput;
    private ILaunch _launcher;
    private TimeCounter _timeCounter;
    

    public void Initialize(IInput input, TimeCounter timeCounter, GunRotator gunRotator, ILaunch launcher)
    {
        Camera.main.GetComponent<CameraMovement>().CameraInit(_cameraSpot);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _playerInput = input;
        _timeCounter = timeCounter;
        _launcher = launcher;
        _timeCounter.TimerEnd += Shoot;
        _gunRotator = gunRotator;
        _playerInput.ShowScoreBar += OnShowScoreBar;
        _playerInput.HideScoreBar += OnHideScoreBar;
        _scoreManagerPanel = _scoreManager.transform.GetChild(0).gameObject;
    }

    public void SetScoreManager(GameObject scoreManager)
    {
        //Cannot take child object, since SYNC var requires network identity on object
        _scoreManager = scoreManager;
    }

    public override void OnStopLocalPlayer()
    {
        _playerInput.ShowScoreBar -= OnShowScoreBar;
        _playerInput.HideScoreBar -= OnHideScoreBar;
        _timeCounter.TimerEnd -= Shoot;
    }

    private void Update()
    {
        if (!isLocalPlayer || !_gunRotator) { return; }

        _gunRotator.Rotate(_playerInput.GetPointerDelta());
    }

    private void Shoot(float holdTime)
    {
        if (holdTime > 0 && _launcher != null)
        {
            _launcher.Launch(holdTime);
        }
    }

    private void OnShowScoreBar()
    {
        if (_scoreManagerPanel == null) { return; }
        _scoreManagerPanel.SetActive(true);
    }

    private void OnHideScoreBar()
    {
        if (_scoreManagerPanel == null) { return; }
        _scoreManagerPanel.SetActive(false);
    }
}