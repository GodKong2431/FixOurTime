using Unity.VisualScripting;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;



public class BookCase : MonoBehaviour
{
    private IState<BookCase> _bookCaseState;

    [Header("쿨타임")]
    [SerializeField] private float _cooldown;

    [Header("감지 거리")]
    [SerializeField] private float _distance;

    [Header("정지 시간")]
    [SerializeField] private float _stayTime;
    
    [Header("속도")]
    [SerializeField] private float _speed;




    public float StayTime => _stayTime;
    public float Speed => _speed;




    public Rigidbody2D Rb { get; private set; }
    public Collider2D Col {  get; private set; }
    public Vector2 StartPos {  get; private set; }
    public float CheckDir { get ; private set; }
    public float BookCaseBotton { get; private set; }
    public bool IsLeft { get; private set; }


    public Vector2 Dir() =>new Vector2( IsLeft ? Col.bounds.max.x : Col.bounds.min.x, BookCaseBotton);
    public Vector3 MoveDir(bool Re) => (Re ? IsLeft : !IsLeft) ? Vector3.right : Vector3.left;



    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Col = GetComponent<BoxCollider2D>();
        StartPos = transform.position;
        BookCaseBotton = Col.bounds.min.y;
        IsLeft = CheckPos();
        SetState(new BookCaseIdleState());
    }

    private void Start()
    {
    }

    private void Update()
    {
        _bookCaseState?.Execute(this);
    }

    

    public void SetState(IState<BookCase> newState)
    {
        _bookCaseState?.Exit(this);
        _bookCaseState = newState;
        _bookCaseState?.Enter(this);
    }
    bool CheckPos()
    {
        Vector2 origin = new Vector2( transform.position.x, BookCaseBotton);
        RaycastHit2D hit = Physics2D.Raycast(origin,Vector2.down,1f,1 << 31);

        if(!hit)
        {
            Debug.Log("없음");
        }

        Bounds platformBounds = hit.collider.bounds;
        float platformLeft = platformBounds.min.x;
        float platformRight = platformBounds.max.x;

        Vector2 startPos = transform.position;

        // 책장이 어느 쪽 끝에 있는지 판단
        float distToLeft = Mathf.Abs(startPos.x - platformLeft);
        float distToRight = Mathf.Abs(startPos.x - platformRight);

        return distToLeft < distToRight;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(new Vector2(transform.position.x, BookCaseBotton), new Vector2(transform.position.x, BookCaseBotton) + Vector2.down * 1f);
        
    }
}
