using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class dropAndMoveItems : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public static GameObject itemBeingDragged;
    Vector3 startPosition;
 
    public float speed;
    public bool goingBack = false;
    public bool goingToNewPosition = false;
    public bool goingToNewtoNewPosition = false;
    Transform startParent;
    static GameObject NewSlot;
    Vector3 newPosition = NewSlot.transform.position;


    void Start()
    {

        goingBack = false;
        goingToNewPosition = false;

    }
    private void OnMouseEnter()
    {
        //gameObject oldItem
        if (gameObject.TryGetComponent<Item>(out Item item) != false)
        {
            NewSlot = gameObject;
            
        }
    }
    void Update()
    {
        
        PositionChanging();

    }

    void PositionChanging()
    {

        if (goingBack == true)
        {
            transform.position = Vector3.Lerp(transform.position, startPosition, Time.deltaTime * speed);
        }
        if (goingToNewPosition == true)
        {
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * speed);
            
        }
        if (goingToNewPosition == true)
        {
           
        }

    }




    #region IBeginDragHandler implementation

    public void OnBeginDrag(PointerEventData eventData)
    {

        itemBeingDragged = gameObject;
        startPosition = transform.position;
        startParent = transform.parent;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
    }

    #endregion

    #region IDragHandler implementation

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    #endregion

    #region IEndDragHandler implementation

    public void OnEndDrag(PointerEventData eventData)
    {

        itemBeingDragged = null;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        if (transform.parent == startParent)
        {
            goingBack = true;
        }

        #endregion



    }
}