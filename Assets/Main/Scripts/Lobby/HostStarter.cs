using UnityEngine;


public class HostStarter : MonoBehaviour
{
    [SerializeField] private CustomNetworkManager _networkManager;

    [Header("UI")]
    [SerializeField] private GameObject _landingPagePanel;


    public void HostLobby()
    {
        _networkManager.StartHost();
        _landingPagePanel.SetActive(false);
    }
}