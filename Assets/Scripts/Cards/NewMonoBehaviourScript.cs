using System.Runtime.InteropServices;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    
    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int X, int Y);
    
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;
    }
    
    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out POINT lpPoint);
   

    private float dir;
    private float speed = 3;
    
    Vector2 pos;

    float maxChange = 0.03f; 
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.K)) return;
        float noise = Mathf.PerlinNoise(Time.time * 1f, 0); 
        dir += (noise - 0.5f) * 1.0f; 
        POINT p;
        if (GetCursorPos(out p))
        {
            pos = new Vector2(
                (p.X+pos.x+Mathf.Sin(dir)*speed+Screen.currentResolution.width)%(float)Screen.currentResolution.width
                ,(p.Y+pos.y+Mathf.Cos(dir)*speed+Screen.currentResolution.height)%(float)Screen.currentResolution.height);
            
            
            
            SetCursorPos((int)pos.x, (int)pos.y);
                
            pos = new Vector2(pos.x % 1f, pos.y % 1f);
        }

    }
}
