using UnityEngine;

public class Raycasts : MonoBehaviour
{
    [Header("Reinforcement Learning")]
    [SerializeField] private bool reinforcementLearning;
    [Tooltip("Only used when reinforcement learning is enabled due to RL not using user settings")]
    [SerializeField] private int numOfRaycasts = 5;
    [Tooltip("Only used when reinforcement learning is enabled due to RL not using user settings")]
    [SerializeField] private bool drawRaycasts;
    
    private RaycastSettings _raycastSettings;
    private LineRenderer[] _lineRenderers;
    private RaycastHit[] _hits;
    private int _numOfRaycasts;
    private bool _drawRaycasts;

    public RaycastHit[] GetHits() 
    {
        return _hits;
    }

    public int GetNumOfRaycasts()
    {
        return _numOfRaycasts;
    }

    public void Awake()
    {
        if (!reinforcementLearning)
            _raycastSettings = GameObject.Find("Settings").GetComponent<Settings>().GetRaycastSettings();
        
        _numOfRaycasts = reinforcementLearning ? numOfRaycasts : _raycastSettings.GetNumOfRaycasts();
        _drawRaycasts = reinforcementLearning ? drawRaycasts : _raycastSettings.GetDrawRaycasts();
        
        _hits = new RaycastHit[_numOfRaycasts];
        _lineRenderers = new LineRenderer[_numOfRaycasts];
        
        SetUpLineRenderers();
    }

    private void SetUpLineRenderers() 
    {
        for (var i = 0; i < _numOfRaycasts; i++)
        {   
            var lineRenderer = new GameObject("RaycastLineRenderer").AddComponent<LineRenderer>();
            lineRenderer.transform.parent = transform;
            
            lineRenderer.positionCount = 2;
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.startColor = Color.red;
            lineRenderer.endColor = Color.red;
            
            lineRenderer.enabled = _drawRaycasts;
            
            _lineRenderers[i] = lineRenderer;
        }
    }

    private void DrawLineRenderers(bool collision, RaycastHit hit, Vector3 direction, int i) 
    {
        var lineRenderer = _lineRenderers[i];

        lineRenderer.SetPosition(0, transform.position + new Vector3(0, 1.25f, 0));
        
        if(collision) 
            lineRenderer.SetPosition(1, hit.point);
        else 
            lineRenderer.SetPosition(1, transform.position + new Vector3(0, 1.25f, 0) + direction * 1000);
    }

    public void FixedUpdate() 
    {
        for (var i = 0; i < _numOfRaycasts; i++)
        {
            var angle = (180f / (_numOfRaycasts - 1)) * i - 90;
            var direction = Quaternion.AngleAxis(angle, transform.up) * transform.forward;
            var layerMask = LayerMask.GetMask("Wall");

            var collision = Physics.Raycast(transform.position + new Vector3(0, 1.25f, 0), direction, out var hit,
                Mathf.Infinity, layerMask, QueryTriggerInteraction.Collide);

            _hits[i] = hit;
            if (_drawRaycasts || (reinforcementLearning && drawRaycasts))
                DrawLineRenderers(collision, hit, direction, i);
        }
    }

    public void Update()
    {
        if (!reinforcementLearning && Input.GetKeyDown(KeyCode.Alpha3))
        {
            _drawRaycasts = !_drawRaycasts;
            _raycastSettings.SetDrawRaycasts(_drawRaycasts);
            for (var i = 0; i < _numOfRaycasts; i++)
                _lineRenderers[i].enabled = _drawRaycasts;
        }
    }
}
