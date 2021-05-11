using UnityEngine;
class DebugManager : MonoBehaviour
{
    public static DebugManager instance;
    public int fontSize;
    public bool show;
    float delay, fps, ms;
    public string additionalInformation;
    GUIStyle style = new GUIStyle();
    void Awake() { if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); } else Destroy(gameObject); }
    void Start() { style.alignment = TextAnchor.UpperRight; }
    void OnGUI()
    {
        if (show)
        {
            style.fontSize = fontSize;
            delay -= Time.unscaledDeltaTime;
            if (delay < 0)
            {
                fps = 1 / Time.unscaledDeltaTime;
                ms = Time.unscaledDeltaTime * 1000;
                delay = .2f;
            }
            GUI.Label(new Rect(0, 0, Screen.width, Screen.height), Application.version + string.Format("\n{0:0.0} fps\n{1:0.0} ms", fps, ms) + additionalInformation, style);
        }
    }
}