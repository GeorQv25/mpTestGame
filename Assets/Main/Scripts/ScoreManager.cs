using Mirror;
using System.Collections.Generic;
using UnityEngine;


public class ScoreManager : NetworkBehaviour
{
    [SerializeField] private PlayerRecord _recordPrefab;
    [SerializeField] private Transform _rootObject;
    
    private readonly SyncDictionary<byte, GamePlayerInfo> _playersInfo = new SyncDictionary<byte, GamePlayerInfo>();

    private Dictionary<NetworkConnectionToClient, byte> _activeClients = new Dictionary<NetworkConnectionToClient, byte>();
    private Dictionary<byte, PlayerRecord> _playersRecord = new Dictionary<byte, PlayerRecord>();
    private List<IBallDetector> _ballDetectors = new List<IBallDetector>();


    public override void OnStartServer()
    {
        NetworkServer.OnDisconnectedEvent += OnClientDisconnected;
    }

    public override void OnStopServer()
    {
        NetworkServer.OnDisconnectedEvent -= OnClientDisconnected;
    }

    public void AddPlayer(byte playerID, GamePlayerInfo gamePlayerInfo, NetworkConnectionToClient conn)
    {
        _playersInfo.Add(playerID, gamePlayerInfo);
        _activeClients.Add(conn, playerID);
    }

    public void OnPlayerScored(byte shooterID, byte gateID)
    {
        if (shooterID == gateID) { return; }

        GamePlayerInfo temp = _playersInfo[shooterID];
        temp.score += 1;
        _playersInfo[shooterID] = temp;

        //Client might be disconnected, but his Gate should be active!
        if (_playersInfo.TryGetValue(gateID, out temp)) 
        {
            temp = _playersInfo[gateID];
            temp.score = Mathf.Clamp(temp.score - 1, 0, int.MaxValue);
            _playersInfo[gateID] = temp;
        }
    }

    public void AddBallDetector(IBallDetector ballDetector)
    {
        _ballDetectors.Add(ballDetector);
        ballDetector.OnBallDetected += OnPlayerScored;
    }

    private void OnDestroy()
    {
        foreach (IBallDetector ballDetector in _ballDetectors)
        {
            ballDetector.OnBallDetected -= OnPlayerScored;
        }
    }

    public override void OnStartClient()
    {
        _playersInfo.Callback += OnPlayersInfoChanged;

        foreach (KeyValuePair<byte, GamePlayerInfo> kvp in _playersInfo)
            OnPlayersInfoChanged(SyncDictionary<byte, GamePlayerInfo>.Operation.OP_ADD, kvp.Key, kvp.Value);
    }

    private void OnPlayersInfoChanged(SyncDictionary<byte, GamePlayerInfo>.Operation op, byte key, GamePlayerInfo value)
    {
        switch (op)
        {
            case SyncIDictionary<byte, GamePlayerInfo>.Operation.OP_ADD:
                AddPlayerRecord(key, value);
                break;
            case SyncIDictionary<byte, GamePlayerInfo>.Operation.OP_SET:
                UpdatePlayerRecord(key, value);
                break;
            case SyncIDictionary<byte, GamePlayerInfo>.Operation.OP_REMOVE:
                RemovePlayerRecord(key, value);
                break;
        }
    }

    private void AddPlayerRecord(byte playerID, GamePlayerInfo gamePlayerInfo)
    {
        PlayerRecord temp = Instantiate(_recordPrefab, _rootObject);
        _playersRecord.Add(playerID, temp);
        temp.Initialize(gamePlayerInfo);
    }

    private void UpdatePlayerRecord(byte playerID, GamePlayerInfo gamePlayerInfo)
    {
        _playersRecord[playerID].UpdateDisplayInfo(gamePlayerInfo);
    }

    private void RemovePlayerRecord(byte playerID, GamePlayerInfo gamePlayerInfo)
    {
        Destroy(_playersRecord[playerID].gameObject);
        _playersRecord.Remove(playerID);
    }

    private void OnClientDisconnected(NetworkConnectionToClient conn)
    {
        if(_activeClients.TryGetValue(conn, out byte leftPlayerID))
        {
            _playersInfo.Remove(leftPlayerID);
            _activeClients.Remove(conn);
        }
    }
}