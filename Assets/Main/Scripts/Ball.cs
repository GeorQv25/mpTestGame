using UnityEngine;


//Base Ball class 
public class Ball : MonoBehaviour
{
    [SerializeField] private Rigidbody _rb;
    private byte _ownerID;
    public bool CanScore { get; private set; } = true;

    public byte GetOwnerID()
    {
        return _ownerID;
    }
    
    public void MarkAsScored()
    {
        //Make sure ball score only once after being launched
        CanScore = false;
    }

    public virtual void Push(Vector3 direction, float pushForce, byte ownerID)
    {
        _ownerID = ownerID;
        _rb.AddForce(direction.normalized * pushForce, ForceMode.Impulse);
    }

    public virtual void ResetBall(Vector3 startPos)
    {
        CanScore = true;
        transform.position = startPos;
        _rb.velocity = Vector3.zero;
        //I know it's hard to believe but this the best way to stop ball movement on server before lauch
        _rb.constraints = RigidbodyConstraints.FreezeAll;
        _rb.constraints = RigidbodyConstraints.None;
        //
        transform.rotation = Quaternion.identity;
    }
}