public class AICarSettings
{
    private float _deathTimerLength;
    private float _globalDeathTimer;
    private float _threshold;
    
    public AICarSettings(float deathTimerLength, float globalDeathTimer, float threshold) 
    {
        _deathTimerLength = deathTimerLength;
        _globalDeathTimer = globalDeathTimer;
        _threshold = threshold;
    }

    public float GetDeathTimerLength() 
    {
        return _deathTimerLength;
    }
    
    public void SetDeathTimerLength(float length) 
    {
        _deathTimerLength = length;
    }
    
    public float GetGlobalDeathTimer() 
    {
        return _globalDeathTimer;
    }
    
    public void SetGlobalDeathTimer(float length) 
    {
        _globalDeathTimer = length;
    }
    
    public float GetThreshold() 
    {
        return _threshold;
    }
    
    public void SetThreshold(float value) 
    {
        _threshold = value;
    }
}
