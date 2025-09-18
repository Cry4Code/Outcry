using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxColliderCheckHelper : MonoBehaviour
{
    private BoxCollider2D collider2D;
    public Color color = Color.red;

    private void Start()
    {
        collider2D = GetComponent<BoxCollider2D>();
        if (collider2D == null)
        {
            Debug.LogError("ColliderCheckHelper: Collider2D component not found!");
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (collider2D != null && collider2D.enabled)
        {
            Gizmos.DrawWireCube(transform.position + (Vector3)collider2D.offset, collider2D.size);
        }
    }
#endif
}
