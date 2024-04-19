using Mirror;
using System.Collections;
using UnityEngine;


public class BallLauncher : NetworkBehaviour, ILaunch
{
    [SerializeField] private Transform _shootPoint;
    [SerializeField] private LauncherConfig _config;

    private byte _ownerID;
    private Coroutine _currentCoroutine;
    private float cdTime => _config ? 1.0f / _config.AttackRate : 0.5f; //Cooldown time 

    public Transform StartPoint => _shootPoint;


    public void Initialize(byte ownerID)
    {
        _ownerID = ownerID;
    }

    public void Launch(float holdTime)
    {
        CmdSpawnBall(holdTime, _shootPoint.forward);
    }

    [Command]
    public void CmdSpawnBall(float holdTime, Vector3 dir)
    {
        if (_currentCoroutine != null) return;

        _currentCoroutine = StartCoroutine(Reload());
        
        float holdAmplification = Mathf.Clamp(holdTime, _config.MinHoldAmplification, _config.MaxHoldAmplification);
        Ball ball = PoolSystem.GetBallFromPool();

        RpcShowBall(ball.gameObject);
        ball.ResetBall(_shootPoint.position);
        ball.Push(dir, _config.ForceAmmount * holdAmplification, _ownerID);
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