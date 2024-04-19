using UnityEngine;


public class GatePusher : MonoBehaviour
{
    [SerializeField] private Rigidbody _gateRb;
    [SerializeField] private GateCollisionDetection _gateCollisionDetection;

    private float _pushForce = 10f;
    private float _initialRandomForce = 3f;


    private void OnEnable()
    {
        _gateCollisionDetection.OnObstacleDetected += Push;    
    }

    private void OnDisable()
    {
        _gateCollisionDetection.OnObstacleDetected -= Push;    
    }

    public void Start()
    {
        Push(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)).normalized * _initialRandomForce);
    }

    private void Push(Vector3 direction)
    {
        _gateRb.AddForce(direction * _pushForce, ForceMode.VelocityChange);
    }
}