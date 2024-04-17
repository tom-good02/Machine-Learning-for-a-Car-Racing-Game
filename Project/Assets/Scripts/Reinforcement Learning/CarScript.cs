using UnityEngine;

public class CarScript : MonoBehaviour 
{
    [SerializeField] private GameObject frontLeftMesh;
    [SerializeField] private GameObject frontRightMesh;
    [SerializeField] private GameObject rearLeftMesh;
    [SerializeField] private GameObject rearRightMesh;

    [SerializeField] private WheelCollider frontLeftCollider;
    [SerializeField] private WheelCollider frontRightCollider;
    [SerializeField] private WheelCollider rearLeftCollider;
    [SerializeField] private WheelCollider rearRightCollider;
    
    [SerializeField] private Rigidbody carRigidbody;
    
    private bool _left;
    private bool _right;
    private bool _forward;
    private bool _backward;

    private float _currentSpeed;
    private float _throttle;
    private float _turning;
    private float _currentTurnAngle;

    private int _currentCheckpointIndex = -1;
    private bool _penultimateCheckpointPassed;
    
    private float _startTime;
    private float _endTime;
    private float _lapTime = -1f;
    
    private int _maxSpeed = 140;
    private int _accelerationMultiplier = 3;
    private float _decelerationMultiplier = 0.2f;
    private float _turningMultiplier = 0.5f;
    private int _brakeForce = 600;
    private int _maxTurnAngle = 30;

    public void FixedUpdate()
    {
        _currentSpeed = carRigidbody.velocity.magnitude * 3.6f;
    }

    private void Forward() 
    {
        _throttle += Time.fixedDeltaTime * 1.5f;
        if(_throttle > 1f) 
        {
            _throttle = 1f;
        }
        
        if(transform.InverseTransformDirection(carRigidbody.velocity).z < -1f) 
        {
            Brake();
        } 
        else 
        {
            if(Mathf.Abs(Mathf.RoundToInt(_currentSpeed)) < _maxSpeed) 
            {
                if(_throttle < 0f) 
                {
                    _throttle = 0f;
                }
                var acceleration = (_accelerationMultiplier * 80f) * _throttle;

                frontLeftCollider.brakeTorque = 0f;
                frontRightCollider.brakeTorque = 0f;
                rearLeftCollider.brakeTorque = 0f;
                rearRightCollider.brakeTorque = 0f;
                frontLeftCollider.motorTorque = acceleration;
                frontRightCollider.motorTorque = acceleration;
                rearLeftCollider.motorTorque = acceleration;
                rearRightCollider.motorTorque = acceleration;
            } 
            else 
            {
                frontLeftCollider.motorTorque = 0f;
                frontRightCollider.motorTorque = 0f;
                rearLeftCollider.motorTorque = 0f;
                rearRightCollider.motorTorque = 0f;
            }
        }
    }

    private void Backward() 
    {
        _throttle -= Time.fixedDeltaTime * 1.5f;
        if(_throttle < -1f) 
            _throttle = -1f;

        if(transform.InverseTransformDirection(carRigidbody.velocity).z > 1f) 
        {
            Brake();
        } 
        else 
        {
            if(Mathf.Abs(Mathf.RoundToInt(_currentSpeed)) < _maxSpeed) 
            {
                if(_throttle > 0f) 
                {
                    _throttle = 0f;
                }
                var acceleration = (_accelerationMultiplier * 80f) * _throttle;

                frontLeftCollider.brakeTorque = 0f;
                frontRightCollider.brakeTorque = 0f;
                rearLeftCollider.brakeTorque = 0f;
                rearRightCollider.brakeTorque = 0f;
                frontLeftCollider.motorTorque = acceleration;
                frontRightCollider.motorTorque = acceleration;
                rearLeftCollider.motorTorque = acceleration;
                rearRightCollider.motorTorque = acceleration;
            } 
            else 
            {
                frontLeftCollider.motorTorque = 0f;
                frontRightCollider.motorTorque = 0f;
                rearLeftCollider.motorTorque = 0f;
                rearRightCollider.motorTorque = 0f;
            }
        }
    }

    private void Neutral() 
    {
        frontLeftCollider.motorTorque = 0f;
        frontRightCollider.motorTorque = 0f;
        rearLeftCollider.motorTorque = 0f;
        rearRightCollider.motorTorque = 0f;
        frontLeftCollider.brakeTorque = 0f;
        frontRightCollider.brakeTorque = 0f;
        rearLeftCollider.brakeTorque = 0f;
        rearRightCollider.brakeTorque = 0f;
    }

    private void TurnLeft() 
    {
        _turning -= Time.fixedDeltaTime * 6f * _turningMultiplier;
        if(_turning < -1f) 
            _turning = -1f;
        _currentTurnAngle = _maxTurnAngle * _turning;
        
        frontLeftCollider.steerAngle = _currentTurnAngle;
        frontRightCollider.steerAngle = _currentTurnAngle;
    }

    private void TurnRight() 
    {
        _turning += Time.fixedDeltaTime * 6f * _turningMultiplier;
        if (_turning > 1f) 
            _turning = 1f;
        _currentTurnAngle = _maxTurnAngle * _turning;

        frontLeftCollider.steerAngle = _currentTurnAngle;
        frontRightCollider.steerAngle = _currentTurnAngle;
    }

    private void TurnStraight() 
    {
        switch (_currentTurnAngle) 
        {
            case > 0f:
                _turning -= Time.fixedDeltaTime * 4f;
                if(_turning < 0f) 
                {
                    _turning = 0f;
                }
                break;
            case < 0f:
                _turning += Time.fixedDeltaTime * 4f;
                if(_turning > 0f) 
                {
                    _turning = 0f;
                }
                break;
        }
        
        if(_turning is > -0.05f and < 0.05f) 
        {
            _turning = 0f;
        }
        
        _currentTurnAngle = _maxTurnAngle * _turning;
        if(_currentTurnAngle is > -1f and < 1f)
        {
            _currentTurnAngle = 0f;
        }
        
        frontLeftCollider.steerAngle = _currentTurnAngle;
        frontRightCollider.steerAngle = _currentTurnAngle;
    }

    private void Brake() 
    {
        frontLeftCollider.brakeTorque = _brakeForce;
        frontRightCollider.brakeTorque = _brakeForce;
        rearLeftCollider.brakeTorque = _brakeForce;
        rearRightCollider.brakeTorque = _brakeForce;
    }

    private void Decelerate() 
    {
        // reset throttle
        switch (_throttle) 
        {
            case > 0f:
                _throttle -= Time.fixedDeltaTime * 1.5f;
                if(_throttle < 0f) 
                {
                    _throttle = 0f;
                }
                break;
            case < 0f:
                _throttle += Time.fixedDeltaTime * 1.5f;
                if(_throttle > 0f) 
                {
                    _throttle = 0f;
                }
                break;
        }
        
        if(_throttle is > -0.05f and < 0.05f) 
        {
            _throttle = 0f;
        }
        
        Neutral();

        // slow car
        if (carRigidbody.velocity.magnitude > 0.25f) 
        {
            carRigidbody.velocity *= 1f - _decelerationMultiplier * Time.fixedDeltaTime;
        } 
        else 
        {
            carRigidbody.velocity = Vector3.zero;
        }
    }
    
    protected void UpdateCarMovement() 
    {
        if(_left && !_right) 
        {
            TurnLeft();
        } 
        else if(_right && !_left) 
        {
            TurnRight();
        } else {
            TurnStraight();
        }

        if(_forward && !_backward) 
        {
            Forward();
        } 
        else if(_backward && !_forward) 
        {
            Backward();
        } 
        else 
        {
            Decelerate();
        }
    }

    protected void UpdateWheelMeshes() 
    {
        UpdateWheelMesh(frontLeftCollider, frontLeftMesh);
        UpdateWheelMesh(frontRightCollider, frontRightMesh);
        UpdateWheelMesh(rearLeftCollider, rearLeftMesh);
        UpdateWheelMesh(rearRightCollider, rearRightMesh);
    }

    private static void UpdateWheelMesh(WheelCollider wheelCollider, GameObject mesh)
    {
        wheelCollider.GetWorldPose(out var position, out var rotation);
        mesh.transform.position = position;
        mesh.transform.rotation = rotation;
    }

    protected void StartLapTime() 
    {
        _startTime = Time.fixedTime;
    }
    
    protected void EndLapTime() 
    {
        _endTime = Time.fixedTime;
        _lapTime = _endTime - _startTime;
    }
    
    public float GetLapTime() 
    {
        return _lapTime;
    }

    public float GetCurrentSpeed() 
    {
        return _currentSpeed;
    }

    public void SetLeft(bool value) 
    {
        _left = value;
    }

    public void SetRight(bool value) 
    {
        _right = value;
    }

    public void SetForward(bool value) 
    {
        _forward = value;
    }

    public void SetBackward(bool value) 
    {
        _backward = value;
    }

    public float GetThrottle() 
    {
        return _throttle;
    }

    public float GetTurning() 
    {
        return _turning;
    }

    public float GetCurrentTurnAngle()
    {
        return _currentTurnAngle;
    }
}

