using UnityEngine;
using UnityEngine.InputSystem;

public class ghost : MonoBehaviour
{

    public InputAction MoveAction;
    public InputAction Ret;
    public float speed = 10.0f;
    Rigidbody2D rigidbody2d;
    Vector2 move;


    // Start is called before the first frame update
    void Start()
    {
        MoveAction.Enable();
        Ret.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();
        if(Ret.IsPressed()){
          Debug.Log("returnign");
          rigidbody2d.position =  new Vector2(15,15);
        }
        // Debug.Log(move);
    }


    void FixedUpdate()
    {

        Vector2 position = (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hit " + collision.gameObject.name);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered " + other.gameObject.name);
    }
}
