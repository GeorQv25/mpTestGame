using Mirror;
using System.Collections;
using UnityEngine;


public class BallLauncher : NetworkBehaviour, ILaunch
{
    [SerializeField] private Ball ballReference;
    [SerializeField] private Transform shootPoint;
    
    private float _minHoldAmplification = 0.2f;
    private float _maxHoldAmplification = 3f;
    private float _forceAmmount = 40f;
    private float _attackRate = 5f; //Attacks per second

    private byte _ownerID;
    private Coroutine _currentCoroutine;
    private float cdTime => 1.0f / _attackRate; //Cooldown time 

    public Transform StartPoint => shootPoint;


    public void Initialize(byte ownerID)
    {
        _ownerID = ownerID;
    }

    public void Launch(float holdTime)
    {
        CmdSpawnBall(holdTime, shootPoint.forward);
    }

    [Command]
    public void CmdSpawnBall(float holdTime, Vector3 dir)
    {
        if (_currentCoroutine != null) return;

        _currentCoroutine = StartCoroutine(Reload());
        
        float holdAmplification = Mathf.Clamp(holdTime, _minHoldAmplification, _maxHoldAmplification);
        Ball ball = PoolSystem.GetBallFromPool();

        RpcShowBall(ball.gameObject);
        ball.ResetBall(shootPoint.position);
        ball.Push(dir, _forceAmmount * holdAmplification, _ownerID);
    }

    private IEnumerator Reload()
    {
        yield return new WaitForSeconds(cdTime);
        _currentCoroutine = null;
    }

    [ClientRpc]
    private void RpcShowBall(GameObject ballObject)
    {
        //Rpc call cannot take an Ball parameters, but still get component runs only on clients
        ballObject.transform.GetChild(0).gameObject.SetActive(true);
    }
}