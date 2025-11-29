using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastPhysic : MonoBehaviour
{
    [SerializeField] private float _lookDistance = 3f;
    [SerializeField] private LayerMask layerMask;
    public GameObject player;
    Rigidbody2D rb;
    [SerializeField] int _forceJump;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
     
    }
    void Update()
    {
        Vector2 direction = Vector2.right * transform.localScale.x;
        
        Debug.DrawRay(transform.position, direction * _lookDistance, Color.red);
    

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, _lookDistance, layerMask);
        if (hit.collider != null )
        {
            Debug.Log("Ray hit: " + hit.collider.name + " Shape count: " + hit.collider.shapeCount);
        }
       
        if (hit.collider != null && hit.collider.shapeCount == 0)
        {
                rb.AddForce(hit.transform.right * _forceJump);
                // (1, 0) * 
        }
    }
}
