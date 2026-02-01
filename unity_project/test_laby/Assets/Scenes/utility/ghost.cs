using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
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
  public bool isDashing = false;
  private Vector2 dashDirection;


  public Image hpImage;


  // torches
  public InputAction PlaceTorchAction;
  public GameObject torchPrefab;
  public int torches = 3;
  public int maxTorches = 5;
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

      weapon.unequip();
      weapon.transform.SetParent(null);
      weapon = null; // only null briefly during swap
    }

    return to_ret;
  }

  void equip()
  {
    Debug.Log("pressed equip");
    LayerMask layer = LayerMask.GetMask("Weapon", "Player");
    Collider2D[] colliders =
        Physics2D.OverlapCircleAll(transform.position, equip_radius, layer);

    Debug.Log("items found " + colliders.Length);

    IWeapon new_weapon = null;
    Vector3 pickup_position = Vector3.zero;


    foreach (Collider2D item in colliders)
    {
      Item coin = item.GetComponent<Item>();
      Debug.Log("items found " + coin);
      if (coin != null)
      {
        coin.pickup();
        continue;
      }

      TorchTurret placedTorch = item.gameObject.GetComponent<TorchTurret>();
      Debug.Log("placedTorch  is " + placedTorch);
      if (placedTorch != null)
      {
        if (torches < maxTorches)
        {
          GameManager.Instance.Torchpoint.Remove(item.gameObject.GetEntityId());
          torches++;
          HUDManager.Instance.UpdateTorchUI(torches, maxTorches); // UI Update
          Destroy(item.gameObject); // Fackel aus der Welt entfernen
          continue;
        }
        else
        {
          Debug.Log("Fackeltasche voll!");
          continue;
        }
      }

      IWeapon candidate =
          item.GetComponent<IWeapon>() ??
          item.GetComponentInParent<IWeapon>();

      if (candidate != null && candidate != weapon)
      {
        if (WeaponCompareUI.Instance != null)
        {
          WeaponCompareUI.Instance.ShowComparison(candidate, this);
        }
        else
        {

          ConfirmSwapWeapon(candidate);
        }
        return;
      }
    }


    if (new_weapon == null)
      return;


  }
  public void ConfirmSwapWeapon(IWeapon new_weapon)
  {
    Vector3 pickup_position = new_weapon.transform.position;


    IWeapon old_weapon = unequip();


    if (old_weapon != null)
    {
      old_weapon.transform.position = pickup_position;

      old_weapon.gameObject.SetActive(true);
    }


    weapon = new_weapon;
    weapon.transform.SetParent(transform);
    weapon.transform.localPosition = Vector3.zero;
    weapon.transform.localRotation = Quaternion.identity;


    weapon.equip(weapon_upgrades);


    Debug.Log("Swapped to: " + weapon.gameObject.name);
  }


  private void OnDrawGizmos()
  {
    Gizmos.DrawWireSphere(transform.position, equip_radius);
  }





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
    if (DashAction.WasPressedThisFrame()) Debug.Log("Dash pressed");
    if (DashAction.WasPressedThisFrame()
    && !isDashing
    && !dashOnCooldown
    && move != Vector2.zero)
    {
      AudioManager.Instance.Play(AudioManager.SoundType.Groan);
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
      AudioManager.Instance.Play(AudioManager.SoundType.Torch);
      TryPlaceTorch();
    }

    if (weapon != null && weapon.AttackAction.IsPressed())
    {

     if (weapon != null && weapon.AttackAction.IsPressed())
{
    
    if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
     {}
    else
    {
        weapon.Attack();
    }
}
    }
    if (EquipAction.WasPressedThisFrame())
    {
      equip();
    }

    if (Ret.WasPressedThisFrame())
    {
      AudioManager.Instance.Play(AudioManager.SoundType.Teleport);
      OnRet();
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

    anim.SetBool("isDashing", true);

    dashDirection = move.normalized;


    rigidbody2d.linearVelocity = dashDirection * speed * dashMultiplier;


    yield return new WaitForSeconds(dashDuration);

    rigidbody2d.linearVelocity = Vector2.zero;
    isDashing = false;
    anim.SetBool("isDashing", false);

    dashOnCooldown = true;
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

  void OnCollisionEnter2D(Collision2D collision)
  {
    Debug.Log("Hit " + collision.gameObject.name);
    if (collision.gameObject.layer == 7)
    {
      var obj = collision.gameObject.GetComponent<IEnemy>();
      if (obj != null)
      {

        hp.change_health(obj.collision_damage);

      }
    }

    else if (collision.gameObject.CompareTag("Enter Loot Room Portal"))
    {

      CollideWithEnterPortal(collision);
    }

    else if (collision.gameObject.CompareTag("Enter Large Loot Room Portal"))
    {

      CollideWithEnterLargePortal(collision);
    }

    else if (collision.gameObject.CompareTag("Exit Loot Room Portal"))
    {

      CollideWithExitPortal(collision);
    }
  }

  public void CollideWithEnterLargePortal(Collision2D collision)
  {
    if (!GameState.Instance.enemyWaveActive)
    {
      PlayerPersistence.Instance.SaveReturnPosition(collision);
      GameState.Instance.PauseClock();

      AudioManager.Instance.Play(AudioManager.SoundType.Loot_Room);
      SceneManager.LoadScene("LargeLootRoom");
    }

    GameManager.Instance.lootrooms.Add(collision.gameObject.transform.position);

  }

  public void CollideWithEnterPortal(Collision2D collision)
  {
    if (!GameState.Instance.enemyWaveActive)
    {
      PlayerPersistence.Instance.SaveReturnPosition(collision);
      GameState.Instance.PauseClock();

      AudioManager.Instance.Play(AudioManager.SoundType.Loot_Room);
      SceneManager.LoadScene("SmallLootRoom");
    }

    GameManager.Instance.lootrooms.Add(collision.gameObject.transform.position);
  }

  public void CollideWithExitPortal(Collision2D collision)
  {
    SceneManager.LoadScene(GameManager.MainSceneName);


    PlayerPersistence.Instance.RestoreReturnPosition();
    GameState.Instance.ResumeClock();

  }


  public void hit(float damage)
  {

    if (!isDead)
    {
      hp.change_health(damage);
      UpdateUI();
      if (damage > 0)
      {
        if (CameraShake.Instance != null) CameraShake.Instance.Shake(0.1f, 0.1f);
      }

    }


  }


  public void OnRet()
  {
    StartCoroutine(AnimateDeathSpotlight());
    StartCoroutine(RetSequence());

  }


  public void OnDeath()
  {

    if (isDead) return;
    isDead = true;

    AudioManager.Instance.Play(AudioManager.SoundType.Teleport);
    StartCoroutine(AnimateDeathSpotlight());
    StartCoroutine(DeathSequence());
    CurrencyManager.Instance.ResetCoins();

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
      return;
    }
    else
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
    }
    else
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

  IEnumerator AnimateDeathSpotlight()
  {
    float duration = 1.2f;
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


    ChangeSpotlight(endT);
  }

  void Start()
  {
    isDashing = false;
    dashOnCooldown = false;
    GameState.Instance.OnCycleEnded += OnDeath;
    Debug.Log("Player subscribed to OnCycleEnded");
    StartCoroutine(AnimateReviveSpotlight());

    if (HUDManager.Instance != null)
    {
      HUDManager.Instance.UpdateTorchUI(torches, maxTorches);
    }
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

      t -= 0.1f;

      spotlight.falloffIntensity = Mathf.Lerp(1f, 0f, t);



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
    if (Vector3.Distance(placePos, previousTorchPos) < 0.5f) { return; }

    previousTorchPos = placePos;
    torches--;

    if (HUDManager.Instance != null)
      HUDManager.Instance.UpdateTorchUI(torches, maxTorches);


    GameObject torch = Instantiate(torchPrefab, placePos, Quaternion.identity);
    GameManager.Instance.Torchpoint.Add(torch.GetEntityId(), (placePos, torch.GetComponent<TorchTurret>().hp.health));
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
