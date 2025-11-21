using UnityEngine;
using UnityEngine.EventSystems;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public static GameObject itemBeingDragged;
    private Vector3 startPosition;
    public GameObject a;
    private Transform player;


    public float speed;
    public bool goingBack = false;
    public bool goingToNewPosition = false;
    public bool goingToNewtoNewPosition = false;
    private Transform startParent;
    public GameObject NewSlot;
    private Vector3 newPosition;
    [SerializeField] private float _lookDistance = 3f;
    [SerializeField] private LayerMask layerMask;
    Rigidbody2D rb;
    [SerializeField] int _forceJump;
    private GameObject StartSlot;
    int chet = 1;


    void Start()
    {
        newPosition = NewSlot.transform.position;
        goingBack = false;
        goingToNewPosition = false;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnMouseEnter()
    {
        //gameObject oldItem
        if (gameObject.TryGetComponent<Item>(out Item item) != false)
        {
            if (chet == 1)
            {
                StartSlot = gameObject;
            }
            if (chet == 2)
            {
                NewSlot = gameObject;
            }
        }

        }
    void Update()
    {
        Vector2 direction = Vector2.right * transform.localScale.x;

        Debug.DrawRay(transform.position, direction * _lookDistance, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, _lookDistance, layerMask);
        if (hit.collider != null && hit.collider.tag == "Item")
        {
            hit.collider.transform.position = transform.position = Vector3.Lerp(transform.position, StartSlot.transform.position, Time.deltaTime * speed);

        }

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
        Vector2 direction = Vector2.right * transform.localScale.x;
        Debug.DrawRay(transform.position, direction * _lookDistance, Color.red);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, _lookDistance, layerMask);
        if (hit.collider != null && hit.collider.tag == "Item")
        {
            transform.position = Vector3.Lerp(transform.position, NewSlot.transform.position, Time.deltaTime * speed);
        }


        #endregion



    }

    public void SpawnDroppedItem()
    {
        Vector2 playerPos = new Vector2(player.position.x + 1, player.position.y);
     //   Instantiate(item, playerPos, Quaternion.identity);
        Destroy(gameObject);
    }
}