using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootballGate : MonoBehaviour
{
    public GatePos leftGate;
    public GatePos rightGate;

    public static FootballGate instance;

    private void Awake()
    {
        if (!instance)
            instance = this;
    }


    private void OnDrawGizmos()
    {
        leftGate.DrawGizmo(Color.blue);
        rightGate.DrawGizmo(Color.blue);
    }
}


[System.Serializable]
public struct GatePos
{
    public Vector2 min;
    public Vector2 max;

    public void DrawGizmo(Color gizmoColor)
    {
        Gizmos.color = gizmoColor;

        Gizmos.DrawLine(new Vector3(min.x, min.y, 0), new Vector3(min.x , max.y , 0));
        Gizmos.DrawLine(new Vector3(min.x, min.y, 0), new Vector3(max.x , min.y , 0));
        Gizmos.DrawLine(new Vector3(max.x, max.y, 0), new Vector3(min.x , max.y , 0));
        Gizmos.DrawLine(new Vector3(max.x, max.y, 0), new Vector3(max.x , min.y , 0));
    }
}
