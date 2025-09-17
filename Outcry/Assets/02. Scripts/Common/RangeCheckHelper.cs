using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Shape
{
    Circle,
    Box,
    Line
}
[Serializable]
public class RangeCheckSetter
{
    public Color color;
    public bool isActive;
    public float range;
    public Shape shape;
}
public class RangeCheckHelper : MonoBehaviour
{
    [Header("Range Check")] 
    public List<RangeCheckSetter> rangeChecks = new List<RangeCheckSetter>();

#if UNITY_EDITOR
    void OnDrawGizmos() //히트박스 색상 변경
    {
        foreach (var rangeCheck in rangeChecks)
        {
            var fixedColor = new Color(rangeCheck.color.r, rangeCheck.color.g, rangeCheck.color.b, 1.0f);
            Gizmos.color = fixedColor;
            
            if (rangeCheck.isActive)
            {
                Debug.Log(rangeCheck.range);
                switch (rangeCheck.shape)
                {
                    case Shape.Circle:
                        Debug.Log("Draw Circle Gizmo");
                        Gizmos.DrawWireSphere(transform.position, rangeCheck.range);
                        break;
                    case Shape.Box:
                        Gizmos.DrawWireCube(transform.position, new Vector3(rangeCheck.range, rangeCheck.range, 0));
                        break;
                    case Shape.Line:
                        Gizmos.DrawLine(transform.position, transform.position + transform.right * rangeCheck.range);
                        break;
                }
            }
        }
    }
#endif
}
