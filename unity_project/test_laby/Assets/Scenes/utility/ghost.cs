using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ghost : MonoBehaviour, IKillable
{

  bool isDead = false;

  public InputAction MoveAction;
  List<Weaponupgrade> weapon_upgrades;
  public InputAction Ret;
  public InputAction EquipAction;
  public IWeapon weapon;
  public Vector3 spawn_pos;
  private Health hp;

  public float speed = 10.0f;
  public float equip_radius = 10.0f;
  public float health = 5f;
  public float max_health = 5f;
  Rigidbody2D rigidbody2d;
  Vector2 move;
  public Light2D spotlight;
  SpriteRenderer Sr;
  private Animator anim;
  [SerializeField] private ParticleSystem FootstepDust;

  // Dash
  public InputAction DashAction;
  public float dashMultiplier = 2.5f;
  public float dashDuration = 0.15f;
  public float dashCooldown = 0.1f;
  private bool dashOnCooldown = false;
  private bool isDashing = false;
  private Vector2 dashDirection;


  public Image hpImage;


  // torches
  public InputAction PlaceTorchAction;
  public GameObject torchPrefab;
  public int torches = 3;
  Vector3 previousTorchPos = Vector3.zero;
  public LayerMask item_layer;

  // Start is called before the first frame update
  public void Awake()
  {
    isDead = false;
    weapon_upgrades = new();
    spawn_pos = transform.position;
    MoveAction.Enable();
    EquipAction.Enable();
    PlaceTorchAction.Enable();
    Ret.Enable();
    rigidbody2d = GetComponent<Rigidbody2D>();
    hp = gameObject.GetComponentInChildren<Health>();
    weapon = gameObject.GetComponentsInChildren<IWeapon>()[0];

    Debug.Log("weapon as = " + weapon.stats.attackspeed);
    weapon.equip(weapon_upgrades);
    Debug.Log("weapon as = " + weapon.stats.attackspeed);
    //weapon_img.texture = weapon.GetComponent<SpriteRenderer>().sprite.texture; //does not work on scene transition


    hp.set_max_hp(max_health);
    hp.set_hp(health);
    if (spotlight == null)
    {
      spotlight = GetComponentInChildren<Light2D>();
    }
    anim = GetComponent<Animator>();
    DashAction.Enable();

  }

  IWeapon unequip()
  {
      IWeapon to_ret = weapon;

      if (weapon != null)
      {
          Debug.Log("Weapon is unequipped");

          weapon.transform.SetParent(null);
          weapon.unequip();
          weapon = null; // only null briefly during swap
      }

      return to_ret;
  }

  void equip()
  {
      Debug.Log("pressed equip");

      Collider2D[] colliders =
          Physics2D.OverlapCircleAll(transform.position, equip_radius, item_layer);

      Debug.Log("items found " + colliders.Length);

      IWeapon new_weapon = null;
      Vector3 pickup_position = Vector3.zero;

      // ---- First: find a weapon (do NOT unequip yet) ----
      foreach (Collider2D item in colliders)
      {
          Item coin = item.GetComponent<Item>();
          if (coin != null)
          {
              coin.pickup();
              continue;
          }

          IWeapon candidate =
              item.GetComponent<IWeapon>() ??
              item.GetComponentInParent<IWeapon>();

          if (candidate != null && candidate != weapon)
          {
              new_weapon = candidate;
              pickup_position = candidate.transform.position;
              break;
          }
      }

      // ---- HARD GUARANTEE: if no new weapon, do nothing ----
      if (new_weapon == null)
          return;

      // ---- Swap ----
      IWeapon old_weapon = unequip();

      // Old weapon is dropped exactly where new one was
      if (old_weapon != null)
      {
          old_weapon.transform.position = pickup_position;
      }

      // Immediately equip replacement
      weapon = new_weapon;
      weapon.transform.SetParent(transform);
      weapon.transform.localPosition = Vector3.zero;
      weapon.transform.localRotation = Quaternion.identity;
      weapon.equip(weapon_upgrades);
  }


  /*
  IWeapon unequip()
  {
    IWeapon to_ret = weapon;
    if (weapon != null)
    {

      Debug.Log("Weapon is unequipped");

      weapon.transform.SetParent(null);
      weapon.unequip();
      weapon = null;

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
        coin.pickup();//this is not coin pickuo but item pickup
        continue;//NOTE: this might not be smart in future
      }

      IWeapon new_weapon = item.GetComponent<IWeapon>() == null ? item.GetComponentInParent<IWeapon>() : item.GetComponent<IWeapon>();
      if (new_weapon != null && !found_new_weapon && new_weapon != old_weapon)
      {
        weapon = new_weapon;
        weapon.transform.SetParent(transform);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;

        weapon.equip(weapon_upgrades);


      }

    }

  }*/

  private void OnDrawGizmos()
  {
    Gizmos.DrawWireSphere(transform.position, equip_radius);
  }



  // Update is called once per frame    

  private bool facingRight = true;
  void Update()
  {
        
    UpdateUI();
    move = MoveAction.ReadValue<Vector2>();
    if (move != Vector2.zero)
    {
      if (!FootstepDust.isPlaying)
        FootstepDust.Play();
      anim.SetBool("isWalking", true);
      anim.SetFloat("Xinput", move.x);
      anim.SetFloat("Yinput", move.y);
    }
    else
    {
      anim.SetBool("isWalking", false);
      if (FootstepDust.isPlaying)
        FootstepDust.Stop();
    }

    if (DashAction.WasPressedThisFrame()
    && !isDashing
    && !dashOnCooldown
    && move != Vector2.zero)
    {
      anim.SetBool("isWalking", false);
      if (!FootstepDust.isPlaying) FootstepDust.Play();
      StartCoroutine(Dash());
    }


    Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
    Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(
new Vector3(mouseScreenPos.x, mouseScreenPos.y, Camera.main.nearClipPlane)
);
    if (PlaceTorchAction.WasPressedThisFrame())
    {
      TryPlaceTorch();
    }

    if (weapon != null && weapon.AttackAction.IsPressed())
    {
      weapon.Attack();
    }
    if (EquipAction.WasPressedThisFrame())
    {
      equip();
    }

    if (Ret.WasPressedThisFrame())
    {
      OnRet(); //Perhaps make second death function without stat reset here KISS
    }
  }

  void Flip()
  {
    facingRight = !facingRight;
    Vector3 scale = transform.localScale;
    scale.x *= -1;
    transform.localScale = scale;
  }

  private IEnumerator Dash()
  {
    isDashing = true;
    dashOnCooldown = true;
    anim.SetBool("isDashing", true);
    dashDirection = move.normalized;

    float timer = 0f;

    while (timer < dashDuration)
    {
      Vector2 dashPos =
          rigidbody2d.position +
          dashDirection * speed * dashMultiplier * Time.deltaTime;

      rigidbody2d.MovePosition(dashPos);

      timer += Time.deltaTime;
      yield return null;
    }

    isDashing = false;
    anim.SetBool("isDashing", false);

    // cooldown
    yield return new WaitForSeconds(dashCooldown);
    dashOnCooldown = false;
  }


  void FixedUpdate()
  {
    if (isDashing) return;

    Vector2 position = (Vector2)rigidbody2d.position + move * speed * Time.deltaTime;

    rigidbody2d.MovePosition(position);
  }

  public void UpdateUI()
  {

    if (hpImage == null || hp == null) return;


    float hpPercent = hp.health / hp.max_health;

    float targetX = 700f * (1f - hpPercent);

    hpImage.rectTransform.anchoredPosition = new Vector2(-targetX, hpImage.rectTransform.anchoredPosition.y);
  }

  ////////////////////////////////////////////////////////////////////////////////////////////
  //VERY SIMPLE LOOT ROOM LOGIC -> ONLY FOR TESTING AND NEEDS TO BE HANDLED MUCH MORE SECURELY
  //TODO
  ///
  /// ////////////////////////////////////////////////////////////////////////////////////////

  void OnCollisionEnter2D(Collision2D collision)
  {
    Debug.Log("Hit " + collision.gameObject.name);
    if (collision.gameObject.layer == 7)
    {

      hp.change_health(collision.gameObject.GetComponent<IEnemy>().collision_damage);
    }

    else if (collision.gameObject.CompareTag("Enter Loot Room Portal"))
    {

      CollideWithEnterPortal(collision); //TODO
    }

    else if (collision.gameObject.CompareTag("Enter Large Loot Room Portal"))
    {

      CollideWithEnterLargePortal(collision); //TODO
    }

    else if (collision.gameObject.CompareTag("Exit Loot Room Portal"))
    {

      CollideWithExitPortal(collision); //TODO
    }
  }

  public void CollideWithEnterLargePortal(Collision2D collision)
  {
    PlayerPersistence.Instance.SaveReturnPosition(collision);
    GameState.Instance.PauseClock();
    SceneManager.LoadScene("LargeLootRoom");


  }

  public void CollideWithEnterPortal(Collision2D collision)
  {
    PlayerPersistence.Instance.SaveReturnPosition(collision);
    GameState.Instance.PauseClock();
    SceneManager.LoadScene("SmallLootRoom");


  }

  public void CollideWithExitPortal(Collision2D collision)
  {
    SceneManager.LoadScene(GameManager.MainSceneName);

    //////////////////////////////////
    PlayerPersistence.Instance.RestoreReturnPosition();
    GameState.Instance.ResumeClock();
  }

  /// ////////////////////////////////////////////////////////////////////////////////////////
  ///
  public void hit(float damage)
  {
    if (!isDead)
    {
      hp.change_health(damage);
      UpdateUI();
    }
    
  }
  /* void OnTriggerEnter2D(Collider2D other)
  {
      Debug.Log("Triggered " + other.gameObject.name);
  } */

  public void OnRet()
  {
    StartCoroutine(AnimateDeathSpotlight());
    StartCoroutine(RetSequence());

  }


  public void OnDeath()
  {

    if (isDead) return;
    isDead = true;

    StartCoroutine(AnimateDeathSpotlight());
    StartCoroutine(DeathSequence());

    Animator anim = GetComponent<Animator>();
    if (anim != null)
    {
      anim.SetTrigger("die");
    }
  }

  IEnumerator DeathSequence()
  {
      
      yield return new WaitForSeconds(1.2f);
    
      DeathRespawn();
  }

  IEnumerator RetSequence()
  {
      
      yield return new WaitForSeconds(1.2f);
    
      RetRespawn();
  }


  void RetRespawn()
  {
    if (PlayerPersistence.Instance.HasReturnPosition())
    {
      return; //No further checks necessary
    } else
    {
      GameObject spawn = GameObject.Find("PlayerSpawn");

      if (spawn != null)
      {
          transform.position = spawn.transform.position;
          StartCoroutine(AnimateReviveSpotlight());
      }
    }
  
    isDead = false;
    hp.restore_hp();
    
      
  }

  void DeathRespawn()
  {
    if (PlayerPersistence.Instance.HasReturnPosition())
    {
      PlayerPersistence.Instance.ResetReturnPosition();
      SceneManager.LoadScene(GameManager.MainSceneName);
    } else
    {
      GameObject spawn = GameObject.Find("PlayerSpawn");

      if (spawn != null)
      {
          transform.position = spawn.transform.position;
          StartCoroutine(AnimateReviveSpotlight());
      }
    }
    //Todo: Reset Player Stats for Death Here
    isDead = false;
    hp.restore_hp();
    
      
  }

  IEnumerator AnimateDeathSpotlight()
  {
      float duration = 1.2f;   // fast, punchy
      float time = 0f;

      float startT = 0.9f;
      float endT = 0.01f;

      while (time < duration)
      {
          time += Time.deltaTime;
          float t = Mathf.Lerp(startT, endT, time / duration);
          ChangeSpotlight(t);
          yield return null;
      }

      // Clamp final value
      ChangeSpotlight(endT);
  }

  void Start()
{

    GameState.Instance.OnCycleEnded += OnDeath;
    Debug.Log("Player subscribed to OnCycleEnded");
    StartCoroutine(AnimateReviveSpotlight());
}


  IEnumerator AnimateReviveSpotlight()
  {
      float duration = 1.2f;   // fast, punchy
      float time = 0f;

      float startT = 0.01f;
      float endT = 1f;

      while (time < duration)
      {
          time += Time.deltaTime;
          float t = Mathf.Lerp(startT, endT, time / duration);
          ChangeSpotlight(t);
          yield return null;
      }

      // Clamp final value
      ChangeSpotlight(endT);
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
