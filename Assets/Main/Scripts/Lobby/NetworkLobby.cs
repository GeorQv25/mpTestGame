using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Mirror;
using TMPro;


public class NetworkLobby : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private Button _startGameButton;
    [SerializeField] private Transform _rootObject;
    [SerializeField] private PlayerCard _playerCard;
    [SerializeField] private TMP_Text _hostNameText;
    
    [SyncVar(hook = nameof(UpdateHostName))] private string _hostName;
    private Dictionary<NetworkConnectionToClient, byte> _clientIDs = new Dictionary<NetworkConnectionToClient, byte>();
    private readonly SyncList<RoomPlayerInfo> _playersInfo = new SyncList<RoomPlayerInfo>();
    private byte _currentId = 0;


    private void UpdateHostName(string oldValue, string newValue)
    {
        _hostNameText.text = $"{newValue}'s Lobby";
    }

    public override void OnStartServer()
    {
        NetworkServer.OnConnectedEvent += OnClientConnected;
        NetworkServer.OnDisconnectedEvent += OnClientDisconect;
        _hostName = PlayerNameInput.DisplayName;
    }

    public override void OnStopServer()
    {
        NetworkServer.OnConnectedEvent -= OnClientConnected;
        NetworkServer.OnDisconnectedEvent -= OnClientDisconect;
    }

    private void OnClientConnected(NetworkConnectionToClient conn)
    {
        _clientIDs.Add(conn, _currentId++);
    }

    private void OnClientDisconect(NetworkConnectionToClient conn)
    {
        if (!TryGetRoomPlayerIndex(conn, out int index)) { return; }

        _playersInfo.Remove(_playersInfo[index]);
        _clientIDs.Remove(conn);
    }

    public void AddPlayer(NetworkConnectionToClient conn, PlayerNameMessage message)
    {
        _playersInfo.Add(new RoomPlayerInfo() { clientID = _clientIDs[conn], playerName = message.playerName, readyStatus = false, playerColor = GetRandomColor() });
    }

    public void Start()
    {
        if (isServer) { _startGameButton.gameObject.SetActive(true); }

        _playersInfo.Callback += OnPlayerUpdate;
        for (int index = 0; index < _playersInfo.Count; index++)
        {
            OnPlayerUpdate(SyncList<RoomPlayerInfo>.Operation.OP_ADD, index, new RoomPlayerInfo(), _playersInfo[index]);
        }
    }

    void OnPlayerUpdate(SyncList<RoomPlayerInfo>.Operation op, int index, RoomPlayerInfo oldItem, RoomPlayerInfo newItem)
    {
        switch (op)
        {
            case SyncList<RoomPlayerInfo>.Operation.OP_ADD:
                CreatePlayerCard(newItem);
                break;
            case SyncList<RoomPlayerInfo>.Operation.OP_REMOVEAT:
                RemovePlayerCard(index);
                break;
            case SyncList<RoomPlayerInfo>.Operation.OP_SET:
                UpdatePlayerCard(newItem, index);
                break;
        }
    }

    private void CreatePlayerCard(RoomPlayerInfo playerInfo)
    {
        PlayerCard card = Instantiate(_playerCard, _rootObject);
        card.SetCard(playerInfo.playerName, playerInfo.readyStatus, playerInfo.playerColor);
    }

    private void RemovePlayerCard(int index)
    {
        Destroy(_rootObject.GetChild(index).gameObject);
    }

    private void UpdatePlayerCard(RoomPlayerInfo playerInfo, int index)
    {
        _rootObject.GetChild(index).GetComponent<PlayerCard>().SetCard(playerInfo.playerName, playerInfo.readyStatus, playerInfo.playerColor);
    }

    [Command(requiresAuthority = false)]
    public void CmdSetReadyStatus(NetworkConnectionToClient conn = null)
    {
        if (!TryGetRoomPlayerIndex(conn, out int index)) { return; }

        RoomPlayerInfo temp = _playersInfo[index];
        temp.readyStatus = !temp.readyStatus;
        _playersInfo[index] = temp;

        CheckStartGame();
    }

    private void CheckStartGame()
    {
        bool isInteractable = true;
        
        foreach (RoomPlayerInfo playerInfo in _playersInfo)
        {
            if (!playerInfo.readyStatus)
            {
                isInteractable = false;
                break;
            }
        }

        _startGameButton.interactable = isInteractable;
    }

    public void UpdateReadyStatus()
    {
        CmdSetReadyStatus();
    }

    public void StartGame()
    {
        var manager = NetworkManager.singleton as CustomNetworkManager;
        manager.StartGame();
    }

    public RoomPlayerInfo GetData(NetworkConnectionToClient conn)
    {
        if (!TryGetRoomPlayerIndex(conn, out int index)) { return new RoomPlayerInfo(); }

        return _playersInfo[index];
    }

    private PlayerColor GetRandomColor()
    {
        List<PlayerColor> freeColors = new List<PlayerColor> { PlayerColor.Green, PlayerColor.Blue, PlayerColor.Yellow, PlayerColor.Red };

        for (int i = 0; i < _playersInfo.Count; i++)
        {
            freeColors.Remove(_playersInfo[i].playerColor);
        }
        
        if (freeColors.Count <= 0)
        {
            return PlayerColor.None;
        }

        return freeColors[Random.Range(0, freeColors.Count)];
    }

    [Command(requiresAuthority = false)]
    private void CmdChangePlayerColor(NetworkConnectionToClient conn = null)
    {
        PlayerColor randomColor = GetRandomColor();

        if (randomColor == PlayerColor.None) { return; }

        if (!TryGetRoomPlayerIndex(conn, out int index)) { return; }
    
        RoomPlayerInfo temp = _playersInfo[index];
        temp.playerColor = randomColor;
        _playersInfo[index] = temp;
    }

    public void ChangeColor()
    {
        CmdChangePlayerColor();
    }

    private bool TryGetRoomPlayerIndex(NetworkConnectionToClient conn, out int result)
    {
        result = -1;
        byte clientID = _clientIDs[conn];
        for (int i = 0; i < _playersInfo.Count; i++)
        {
            if (_playersInfo[i].clientID == clientID)
            {
                result = i;
                return true;
            }
        }
        return false;
    }
}