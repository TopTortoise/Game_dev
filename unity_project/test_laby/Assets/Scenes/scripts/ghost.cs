using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

public class ghost : MonoBehaviour, IKillable
{

    public InputAction MoveAction;
    public InputAction Ret;
    public IWeapon weapon;
    private Health hp;
    public float speed = 10.0f;
    public float health = 5f;
    public float max_health = 5f;
    Rigidbody2D rigidbody2d;
    Vector2 move;
    float dir = 1;
    public Light2D spotlight;




    // Start is called before the first frame update
    void Awake()
    {
        MoveAction.Enable();
        Ret.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
        hp = gameObject.GetComponentInChildren<Health>();
        weapon = gameObject.GetComponentInChildren<IWeapon>();
        weapon.AttackAction.Enable();
        hp.set_max_hp(max_health);
        hp.set_hp(health);
        if (spotlight == null)
    {
        spotlight = GetComponentInChildren<Light2D>();
    }


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
            
            weapon.sign = -1;
        }
        //facing left
        else if (mouseWorldPos.x < rigidbody2d.transform.position.x && facingRight)
        {
            Flip();
            weapon.sign = 1;
        }
        if(weapon.AttackAction.IsPressed()){
          weapon.Attack();
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
    public void ChangeSpotlight(float t)
    {
         if (spotlight != null)
        {
            // t = 1.0 (60 Sek übrig): Normal hell
            // t = 0.0 (0 Sek übrig): Sehr dunkel, kaum sichtbar
            
            // Intensity: Von sehr dunkel zu normal
            spotlight.intensity = Mathf.Lerp(0.2f, 2.5f, t);
            
            // Radius: Licht wird kleiner wenn Zeit abläuft
            spotlight.falloffIntensity = Mathf.Lerp(4f, 12f, t);
            
            // Optional: Farbwechsel - von gelblich (Tag) zu bläulich (Nacht)
            spotlight.color = Color.Lerp(new Color(0.3f, 0.3f, 0.6f), Color.white, t);
            
            Debug.Log($"Licht wird dunkler - Noch {(int)(t * 60)} Sekunden! Intensity: {spotlight.intensity:F2}");
        }
    }

}
