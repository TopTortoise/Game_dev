using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
public class ghost : MonoBehaviour, IKillable
{

    public InputAction MoveAction;
    public InputAction Ret;
    public InputAction EquipAction;
    public IWeapon weapon;
    public Vector3 spawn_pos;
    private Health hp;
    public RawImage weapon_img; 
    public float speed = 10.0f;
    public float equip_radius = 10.0f;
    public float health = 5f;
    public float max_health = 5f;
    Rigidbody2D rigidbody2d;
    Vector2 move;
    public Light2D spotlight;
    SpriteRenderer Sr;
    private Animator anim;

    // torches
    public InputAction PlaceTorchAction;
    public GameObject torchPrefab;
    public int torches = 3;
    Vector3 previousTorchPos = Vector3.zero;
    public LayerMask item_layer;

    // Start is called before the first frame update
    void Awake()
    {
        spawn_pos = transform.position;
        MoveAction.Enable();
        EquipAction.Enable();
        PlaceTorchAction.Enable();
        Ret.Enable();
        rigidbody2d = GetComponent<Rigidbody2D>();
        hp = gameObject.GetComponentInChildren<Health>();
        weapon = gameObject.GetComponentsInChildren<IWeapon>()[0];
        weapon.equip();
        weapon_img.texture = weapon.GetComponent<SpriteRenderer>().sprite.texture;
        hp.set_max_hp(max_health);
        hp.set_hp(health);
        if (spotlight == null)
        {
            spotlight = GetComponentInChildren<Light2D>();
        }
        anim = GetComponent<Animator>();

    }

    IWeapon unequip()
    {
      IWeapon to_ret = weapon;
        if (weapon != null)
        {

            Debug.Log("Weapon is unequipped");

            weapon.transform.SetParent(null);
            weapon.unequip();
            weapon = null;
            weapon_img.texture = null;
        }
        return to_ret;
    }

    void equip()
    {
        Debug.Log("pressed equip");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, equip_radius, item_layer);

        Debug.Log("items found " + colliders.Length);
        bool found_new_weapon = false;
        IWeapon old_weapon = unequip();
        foreach (Collider2D item in colliders)
        {
            Item coin = item.GetComponent<Item>();
            if (coin != null)
            {
                coin.pickup();
                continue;//NOTE: this might not be smart in future
            }

            IWeapon new_weapon = item.GetComponent<IWeapon>() == null? item.GetComponentInParent<IWeapon>(): item.GetComponent<IWeapon>();
            if (new_weapon != null && !found_new_weapon && new_weapon != old_weapon)
            {
                weapon = new_weapon;
                weapon.transform.SetParent(transform);
                weapon.transform.localPosition = Vector3.zero;
                weapon.transform.localRotation = Quaternion.identity;
                weapon.equip();
                weapon_img.texture = weapon.GetComponent<SpriteRenderer>().sprite.texture;
            }

        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, equip_radius);
    }



    // Update is called once per frame    

    private bool facingRight = true;
    void Update()
    {
        move = MoveAction.ReadValue<Vector2>();
        if (move != Vector2.zero) {
            anim.SetBool("isWalking", true);
            anim.SetFloat("Xinput", move.x);
            anim.SetFloat("Yinput", move.y);
        } else {
            anim.SetBool("isWalking", false);
        }
        
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(
    new Vector3(mouseScreenPos.x, mouseScreenPos.y, Camera.main.nearClipPlane)
    );
        if (PlaceTorchAction.WasPressedThisFrame())
        {
            TryPlaceTorch();
        }

        if (weapon !=null && weapon.AttackAction.IsPressed())
        {
            weapon.Attack();
        }
        if (EquipAction.WasPressedThisFrame())
        {
            equip();
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
            
            hp.change_health(collision.gameObject.GetComponent<IEnemy>().collision_damage);
        }
    }
    public void hit(float damage)
    {
        hp.change_health(1);
    }
    /* void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Triggered " + other.gameObject.name);
    } */

    public void OnDeath()
    {
        this.enabled =false; 

        if (weapon != null)
    {
        IWeapon droppedWeapon = unequip(); 
        
        // Optional: Den Speer ein Stück wegwerfen oder leicht rotieren, damit es besser aussieht
        Rigidbody2D weaponRb = droppedWeapon.GetComponent<Rigidbody2D>();
        if (weaponRb != null)
        {
            weaponRb.bodyType = RigidbodyType2D.Dynamic; // Physik wieder aktivieren
            weaponRb.AddForce(Random.insideUnitCircle * 2f, ForceMode2D.Impulse);
        }
    }

        if (rigidbody2d != null)
        {
            rigidbody2d.linearVelocity = Vector2.zero;
            rigidbody2d.bodyType = RigidbodyType2D.Kinematic; // Optional: Verhindert, dass Gegner dich schieben
        }
        MoveAction.Disable();
        EquipAction.Disable();
        PlaceTorchAction.Disable();
        Ret.Disable();


        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("die");
        }
        GameOverManager goManager = FindObjectOfType<GameOverManager>();
        if (goManager != null)
        {
            goManager.StartGameOver();
        }
        else
        {
            Debug.LogWarning("GameOverManager nicht in der Szene gefunden!");
        }
    }
    public void ChangeSpotlight(float t)
    {
        if (spotlight != null)
        {



            spotlight.falloffIntensity = Mathf.Lerp(1f, 0f, t);


            // Debug.Log($"Licht wird dunkler - Noch {(int)(t * 60)} Sekunden! Intensity: {spotlight.intensity:F2}");
        }
    }
    void TryPlaceTorch()
    {
        if (torches <= 0)
        {
            Debug.Log("No torches left!");
            return;
        }





        Vector3 placePos = transform.position;
        if (placePos == previousTorchPos) { return; }
        previousTorchPos = placePos;
        torches--;
        // spawn at player pos.
        GameObject torch = Instantiate(torchPrefab, placePos, Quaternion.identity);

        // copy light spotlight
        Light2D torchLight = torch.GetComponentInChildren<Light2D>();
        if (torchLight != null && spotlight != null)
        {
            torchLight.intensity = spotlight.intensity;
            torchLight.pointLightOuterRadius = spotlight.pointLightOuterRadius;
            torchLight.pointLightInnerRadius = spotlight.pointLightInnerRadius;
            torchLight.falloffIntensity = spotlight.falloffIntensity;
        }

        Debug.Log("Torch placed. Remaining: " + torches);
    }

}
