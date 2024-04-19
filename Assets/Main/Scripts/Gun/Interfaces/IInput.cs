using System;
using UnityEngine;


public interface IInput
{
    Vector3 GetPointerDelta();
    event Action<Vector3> ClickDown;
    event Action<Vector3> ClickUp;
    event Action<Vector3> ClickHold;

    event Action ShowScoreBar;
    event Action HideScoreBar;
}