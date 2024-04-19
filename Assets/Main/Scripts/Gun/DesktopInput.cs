using System;
using UnityEngine;


public class DesktopInput : MonoBehaviour, IInput
{
    public event Action<Vector3> ClickDown;
    public event Action<Vector3> ClickUp;
    public event Action<Vector3> ClickHold;
    public event Action ShowScoreBar;
    public event Action HideScoreBar;

    private bool isHold = false;
    private const int MouseKey = 0;


    private void Update()
    {
        ProcessClickDown();
        ProcessClickUp();
        ProcessClickHold();
        ProcessShowScoreBar();
        ProcessHideScoreBar();
    }

    public Vector3 GetPointerDelta()
    {
        return new Vector3(0, Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
    }
    
    private void ProcessClickDown()
    {
        if (Input.GetMouseButtonDown(MouseKey))
        {
            ClickDown?.Invoke(Input.mousePosition);
            isHold = true;
        }
    }

    private void ProcessClickUp()
    {
        if (Input.GetMouseButtonUp(MouseKey))
        {
            ClickUp?.Invoke(Input.mousePosition);
            isHold = false;
        }
    }

    private void ProcessClickHold()
    {
        if (isHold)
        {
            ClickHold?.Invoke(Input.mousePosition);
        }
    }

    private void ProcessShowScoreBar()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ShowScoreBar?.Invoke();
        }
    }

    private void ProcessHideScoreBar()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            HideScoreBar?.Invoke();
        }
    }
}