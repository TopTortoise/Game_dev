using UnityEngine;
public class Vase : MonoBehaviour, IKillable
{
  public SpriteRenderer sr;
  public Sprite destroyed_vase;
  public float rarity;
  public float distance_to_start;
  public GameObject item;
  public GameObject torch;
  public Statupgrade weapon_upgrade;
  public Weaponupgrade weapon_effect;
  
  public float health;
  public float max_health;
  public float torch_drop_rate = 0.1f;
  
  void Start()
  {
    sr = GetComponent<SpriteRenderer>();
    distance_to_start = Vector3.Distance(transform.position, new Vector3(2.9f, -30, 0));

  }

  // Update is called once per frame

  public void hit(float damage)
  {
    OnDeath();
  }

  public void OnDeath()
  {
    AudioManager.Instance.Play(AudioManager.SoundType.Amphora);
    sr.sprite = destroyed_vase;
    BoxCollider2D box = GetComponent<BoxCollider2D>();
    Rigidbody2D rb = GetComponent<Rigidbody2D>();
    Destroy(rb);
    Destroy(box);
    spawn_item();
    GetComponent<LootDropper>().DropLoot();

    TutorialObjective tutorialObj = GetComponent<TutorialObjective>();
    if (tutorialObj != null)
    {

      tutorialObj.CompleteObjective();
    }

  }

  void spawn_item()
  {
    if (item != null)
    {
      item.transform.position = transform.position;  
      weapon_upgrade.Apply(item.GetComponent<IWeapon>());


     
    }
    else
    {
      for (int i = 0; i <= 2; i++)
      {

        if (Random.value < torch_drop_rate)
        {

          Instantiate(torch,
              transform.position +
              new Vector3(Random.value, Random.value, 0), Quaternion.identity);

        }
      }
    }

  }
}
