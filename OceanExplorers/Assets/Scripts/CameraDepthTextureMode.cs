using UnityEngine; 
public class CameraDepthTextureMode : MonoBehaviour
{
    [SerializeField] DepthTextureMode depthTextureMode;
    [SerializeField]  Color FogColour;
    [SerializeField] [Range(0,0.01f)] float FogMinDencity = 0.003f;
    [SerializeField] [Range(0,0.1f)] float FogMaxDencity = 0.015f;
    [SerializeField] [Range(0,30)] float FogTransition = 30;

    private void OnValidate()
    {
        SetCameraDepthTextureMode();
        RenderSettings.fogColor = FogColour;
    }

    private void Awake()
    {
        SetCameraDepthTextureMode();
    }

    private void SetCameraDepthTextureMode()
    {
        GetComponent<Camera>().depthTextureMode = depthTextureMode;
    }
    private void Start() { 
            RenderSettings.fogColor = FogColour;
    }
    private void Update() {
        if (gameObject.transform.position.y < 10) {
            float Dencity = -((gameObject.transform.position.y - 10) / FogTransition); 
            RenderSettings.fogDensity = Mathf.Lerp(FogMinDencity, FogMaxDencity, Dencity);
            RenderSettings.fog = enabled;
        } else
            RenderSettings.fog = !enabled;
    } 
}