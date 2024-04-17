public class CarControllerSettings
{
    
    private int _maxSpeed;
    private int _accelerationMultiplier;
    private float _decelerationMultiplier;
    private float _turningMultiplier;
    private int _brakeForce;
    private int _maxTurnAngle;
    
    public CarControllerSettings(int maxSpeed, int accelerationMultiplier, float decelerationMultiplier, float turningMultiplier, int brakeForce, int maxTurnAngle)
    {
        _maxSpeed = maxSpeed;
        _accelerationMultiplier = accelerationMultiplier;
        _decelerationMultiplier = decelerationMultiplier;
        _turningMultiplier = turningMultiplier;
        _brakeForce = brakeForce;
        _maxTurnAngle = maxTurnAngle;
    }
    
    public int GetMaxSpeed() 
    {
        return _maxSpeed;
    }
    
    public void SetMaxSpeed(int speed) 
    {
        _maxSpeed = speed;
    }
    
    public int GetAccelerationMultiplier() 
    {
        return _accelerationMultiplier;
    }
    
    public void SetAccelerationMultiplier(int multiplier) 
    {
        _accelerationMultiplier = multiplier;
    }
    
    public float GetDecelerationMultiplier() 
    {
        return _decelerationMultiplier;
    }
    
    public void SetDecelerationMultiplier(float multiplier) 
    {
        _decelerationMultiplier = multiplier;
    }
    
    public float GetTurningMultiplier() 
    {
        return _turningMultiplier;
    }
    
    public void SetTurningMultiplier(float multiplier) 
    {
        _turningMultiplier = multiplier;
    }
    
    public int GetBrakeForce() 
    {
        return _brakeForce;
    }
    
    public void SetBrakeForce(int force) 
    {
        _brakeForce = force;
    }
    
    public int GetMaxTurnAngle() 
    {
        return _maxTurnAngle;
    }
    
    public void SetMaxTurnAngle(int angle) 
    {
        _maxTurnAngle = angle;
    }
}
