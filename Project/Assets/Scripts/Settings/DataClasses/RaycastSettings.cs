public class RaycastSettings
{
    private int _numOfRaycasts;
    private bool _drawRaycasts;
    
    public RaycastSettings(int numOfRaycasts, bool drawRaycasts) 
    {
        _numOfRaycasts = numOfRaycasts;
        _drawRaycasts = drawRaycasts;
    }
    
    public int GetNumOfRaycasts() 
    {
        return _numOfRaycasts;
    }
    
    public void SetNumOfRaycasts(int num) 
    {
        _numOfRaycasts = num;
    }

    public bool GetDrawRaycasts() 
    {
        return _drawRaycasts;
    }
    
    public void SetDrawRaycasts(bool draw) 
    {
        _drawRaycasts = draw;
    }
}
