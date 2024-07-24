using UnityEngine;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;

public class FPSCounter : MonoBehaviour
{
    public float updateInterval = 0.5F;
    
    private double lastInterval;
    private int frames = 0;
    private double fps;
    public GUIStyle textStyle;
    public int pixel;

    private Rect rect = new Rect(10, 10, 100, 20);
    private StringBuilder sb = new StringBuilder(10);
    
    public  int             lockedFrameRate = 0;
    public  bool            isFpsShow = true;

    // Use this for initialization
    void Start()
    {
        if (this.lockedFrameRate != 0)
        {
            Application.targetFrameRate = this.lockedFrameRate;

#if UNITY_WEBGL
            Application.targetFrameRate = 0;
#endif
        }

        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFpsShow)
        {
            ++frames;
            float timeNow = Time.realtimeSinceStartup;
            if (timeNow > lastInterval + updateInterval)
            {
                fps = frames / (timeNow - lastInterval);
                frames = 0;
                lastInterval = timeNow;

                sb.Length = 0;
                sb.Append("FPS: ").AppendFormat("{0:f2}", fps);
                sb.AppendLine();
                sb.Append("RES: ").Append(Screen.width).Append("x").Append(Screen.height);
            }
        }
    }

    void OnGUI()
    {
        if (isFpsShow)
        {
            GUI.color = Color.black;
            rect.x -= pixel;
            GUI.Label(rect, sb.ToString(), textStyle);
            rect.x += pixel * 2;
            GUI.Label(rect, sb.ToString(), textStyle);
            rect.x -= pixel;
            rect.y -= pixel;
            GUI.Label(rect, sb.ToString(), textStyle);
            rect.y += pixel * 2;
            GUI.Label(rect, sb.ToString(), textStyle);
            rect.y -= pixel;
            GUI.color = Color.white;
            GUI.Label(rect, sb.ToString(), textStyle);
        }
    }
}