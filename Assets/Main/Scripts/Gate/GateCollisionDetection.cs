using Mirror;
using UnityEngine;


public class GateCollisionDetection : NetworkBehaviour, IBallDetector
{
    public event System.Action<byte, byte> OnBallDetected;
    public event System.Action<Vector3> OnObstacleDetected;

    [SerializeField] private GameObject _particlePrefab;

    private byte _ownerID;


    [Server]
    public void Initialize(byte ownerID)
    {
        _ownerID = ownerID;
    }

    [ServerCallback]
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.TryGetComponent(out Ball ball) && ball.CanScore)
        {
            ball.MarkAsScored();
            OnBallDetected?.Invoke(ball.GetOwnerID(), _ownerID);
            OnObstacleDetected?.Invoke(collision.relativeVelocity);
            RpcHideBall(collision.contacts[0].point, collision.gameObject);
            return;
        }

        OnObstacleDetected?.Invoke(collision.contacts[0].normal);
    }

    [ClientRpc]
    public void RpcHideBall(Vector3 position, GameObject ballObject)
    {
        //Particles are automatically destroyed at the end of their lifecycle
        Instantiate(_particlePrefab, position, Quaternion.identity);
        
        //Rpc call cannot take an Ball parameters, but still get component runs only on clients
        ballObject.transform.GetChild(0).gameObject.SetActive(false);
    }
}