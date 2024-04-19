using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PoolSystem : NetworkBehaviour
{
    [SerializeField] private Ball _ballPrefab;

    private static Queue<Ball> _pool = new Queue<Ball>();
    private int _poolSize = 30;
        

    public override void OnStartServer()
    {
        StartCoroutine(FillPool());
    }

    private IEnumerator FillPool()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            Ball ball = Instantiate(_ballPrefab, transform.position, Quaternion.identity);
            NetworkServer.Spawn(ball.gameObject);
            _pool.Enqueue(ball);
            yield return null;
        }
    }

    public static Ball GetBallFromPool()
    {
        Ball temp = _pool.Dequeue();
        _pool.Enqueue(temp);
        return temp;
    }
}