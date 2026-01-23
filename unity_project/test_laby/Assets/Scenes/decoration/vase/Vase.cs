using UnityEngine;
using UnityEditor;
using System.IO;
public class Vase : MonoBehaviour, IKillable
{
  public SpriteRenderer sr;
  public Sprite destroyed_vase;
  public float rarity;
  public float distance_to_start;
  public GameObject item;
  public Statupgrade weapon_upgrade;
  //if we dont want the to be one shot
  public float health;
  public float max_health;
  // Start is called once before the first execution of Update after the MonoBehaviour is created
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

  }

  void spawn_item()
  {
    if (item != null)
    {
      GameObject dropped_item = Instantiate(item, transform.position, Quaternion.identity);   // spawn into the scene
      // dropped_item.GetComponent<IWeapon>().stats = new();
      // IWeapon weapon = dropped_item.GetComponent<IWeapon>();
      // Debug.Log("weaon is " + weapon.stats.attackspeed);
      // weapon_upgrade.Apply(weapon);
      // weapon.applied_upgrades.Add(weapon_upgrade.upgradeID);
      // Debug.Log("weaon is " + weapon.stats.attackspeed);
    }
  }
}
