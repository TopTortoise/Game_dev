using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
public class ghost : MonoBehaviour, IKillable
{

    public InputAction MoveAction;
    public InputAction Ret;
    public Stick stick;
    private Health hp;
    public float speed = 10.0f;
    public float health = 5f;
    public float max_health = 5f;
    Rigidbody2D rigidbody2d;
    Vector2 move;
    float dir = 1;


    // Start is called before the first frame update
    void Awake()
    {
        MoveAction.Enable();
        Ret.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
        hp = gameObject.GetComponentInChildren<Health>();
        stick = gameObject.GetComponentInChildren<Stick>();
        hp.set_max_hp(max_health);
        hp.set_hp(health);

    }

    // Update is called once per fram    
    private bool facingRight = true;
    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(
    new Vector3(mouseScreenPos.x, mouseScreenPos.y, Camera.main.nearClipPlane)
    );



        //facing right
        if (mouseWorldPos.x > rigidbody2d.transform.position.x && !facingRight)
        {
            Flip();
            stick.sign = -1;
        }
        //facing left
        else if (mouseWorldPos.x < rigidbody2d.transform.position.x && facingRight)
        {
            Flip();
            stick.sign = 1;
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
    void FixedUpdate()
    {

        Vector2 position = (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;

        rigidbody2d.MovePosition(position);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hit " + collision.gameObject.name);
        if (collision.gameObject.layer == 7)
        {

            hp.change_health(1);
        }
    }
    public void hit(float damage){
      hp.change_health(1);
    }
    /* void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered " + other.gameObject.name);
    } */

    public void OnDeath()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
