using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class TrajectoryPredictor : MonoBehaviour
{
    [SerializeField] private Rigidbody _objectToThrow;
    [SerializeField] private Transform _hitMarkerPrefab;
    private Transform _startPoint;

    #region Members
    private LineRenderer _trajectoryLine;
    private Transform _hitMarker;
    [SerializeField, Range(10, 100), Tooltip("The maximum number of points the LineRenderer can have")] int _maxPoints = 50;
    [SerializeField, Range(0.01f, 0.5f), Tooltip("The time increment used to calculate the trajectory")] float _increment = 0.025f;
    [SerializeField, Range(1.05f, 2f), Tooltip("The raycast overlap between points in the trajectory, this is a multiplier of the length between points. 2 = twice as long")]
    float _rayOverlap = 1.1f;
    #endregion


    public void Initialize(Transform startPoint)
    {
        _hitMarker = Instantiate(_hitMarkerPrefab);
        _trajectoryLine = GetComponent<LineRenderer>();
        SetTrajectoryVisible(true);
        _startPoint = startPoint;
    }

    public void PredictTrajectory(float holdAmmount)
    {
        ProjectileProperties projectile = ProjectileData();
        Vector3 velocity = projectile.direction * (projectile.initialSpeed * holdAmmount / projectile.mass);
        Vector3 position = projectile.initialPosition;
        Vector3 nextPosition;
        float overlap;

        UpdateLineRender(_maxPoints, (0, position));

        for (int i = 1; i < _maxPoints; i++)
        {
            // Estimate velocity and update next predicted position
            velocity = CalculateNewVelocity(velocity, projectile.drag, _increment);
            nextPosition = position + velocity * _increment;

            // Overlap our rays by small margin to ensure we never miss a surface
            overlap = Vector3.Distance(position, nextPosition) * _rayOverlap;

            //When hitting a surface we want to show the surface marker and stop updating our line
            if (Physics.Raycast(position, velocity.normalized, out RaycastHit hit, overlap))
            {
                UpdateLineRender(i, (i - 1, hit.point));
                MoveHitMarker(hit);
                break;
            }

            //If nothing is hit, continue rendering the arc without a visual marker
            _hitMarker.gameObject.SetActive(false);
            position = nextPosition;
            UpdateLineRender(_maxPoints, (i, position)); //Unneccesary to set count here, but not harmful
        }
    }

    private void UpdateLineRender(int count, (int point, Vector3 pos) pointPos)
    {
        _trajectoryLine.positionCount = count;
        _trajectoryLine.SetPosition(pointPos.point, pointPos.pos);
    }

    private Vector3 CalculateNewVelocity(Vector3 velocity, float drag, float increment)
    {
        velocity += Physics.gravity * increment;
        velocity *= Mathf.Clamp01(1f - drag * increment);
        return velocity;
    }

    private void MoveHitMarker(RaycastHit hit)
    {
        _hitMarker.gameObject.SetActive(true);

        // Offset marker from surface
        float offset = 0.025f;
        _hitMarker.position = hit.point + hit.normal * offset;
        _hitMarker.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
    }

    public void SetTrajectoryVisible(bool visible)
    {
        _trajectoryLine.enabled = visible;
        //hitMarker.gameObject.SetActive(visible);
    }

    ProjectileProperties ProjectileData()
    {
        ProjectileProperties properties = new ProjectileProperties();

        properties.direction = _startPoint.transform.forward;
        properties.initialPosition = _startPoint.position;
        properties.initialSpeed = 40;
        properties.mass = _objectToThrow.mass;
        properties.drag = _objectToThrow.drag;

        return properties;
    }

    public void SetProjectoryActiveStatus(bool status)
    {
        _hitMarker.gameObject.SetActive(status);
        _trajectoryLine.gameObject.SetActive(status);
    }
}