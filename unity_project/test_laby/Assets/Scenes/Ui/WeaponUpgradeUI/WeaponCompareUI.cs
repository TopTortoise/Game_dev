using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;

public class WeaponCompareUI : MonoBehaviour
{
  public static WeaponCompareUI Instance;

  [Header("Panels")]
  public GameObject panel;

  [Header("Current Weapon")]
  public TextMeshProUGUI currentName;
  public TextMeshProUGUI currentDmg;
  public TextMeshProUGUI currentSpeed;
  public TextMeshProUGUI currentEffects;
  public Image currentIcon;

  [Header("New Weapon")]
  public TextMeshProUGUI newName;
  public TextMeshProUGUI newDmg;
  public TextMeshProUGUI newSpeed;
  public TextMeshProUGUI newEffects;
  public Image newIcon;

  private IWeapon pendingNewWeapon;
  private ghost playerRef;

  void Awake()
  {
    if (Instance != null)
    {
      Destroy(gameObject);
      return;
    }
    Instance = this;
    panel.SetActive(false);
    DontDestroyOnLoad(this.gameObject);
  }

  public void ShowComparison(IWeapon newWeapon, ghost player)
  {
    pendingNewWeapon = newWeapon;
    playerRef = player;
    


    float oldCooldown = GetTotalSpeed(player.weapon);
    float newCooldown = GetTotalSpeed(newWeapon);


    float oldAPS = (oldCooldown > 0.001f) ? 1f / oldCooldown : 0f;
    float newAPS = (newCooldown > 0.001f) ? 1f / newCooldown : 0f;


    UpdateWeaponUI(player.weapon, currentName, currentDmg, currentSpeed, currentEffects, currentIcon);
    UpdateWeaponUI(newWeapon, newName, newDmg, newSpeed, newEffects, newIcon);

    if (player.weapon != null)
    {

      float oldDmgVal = GetTotalDamage(player.weapon);
      float newDmgVal = GetTotalDamage(newWeapon);
      CompareStat(oldDmgVal, newDmgVal, newDmg);


      CompareAPS(oldAPS, newAPS, newSpeed);
    }
    else
    {

      newDmg.color = Color.green;
      newSpeed.text = "New!";
      newSpeed.color = Color.green;
    }
    GameState.Instance.RequestPause(true); 
    panel.SetActive(true);
  }

  void UpdateWeaponUI(IWeapon weapon, TMP_Text tName, TMP_Text tDmg, TMP_Text tSpeed, TMP_Text tEffects, Image uiIcon)
  {
    if (weapon != null)
    {
      string cleanName = weapon.gameObject.name.Replace("(Clone)", "").Replace("_", " ").Trim();
      tName.text = cleanName;

      float totalDmg = GetTotalDamage(weapon);
      float totalCooldown = GetTotalSpeed(weapon);


      float aps = (totalCooldown > 0) ? 1f / totalCooldown : 0f;


      tDmg.text = $"{totalDmg:F2} Damage";
      tSpeed.text = $"{aps:F2} Attacks/s";

      StringBuilder sb = new StringBuilder();
     

      if (weapon.effects != null && weapon.effects.Count > 0)
        foreach (var effect in weapon.effects) sb.AppendLine("• " + effect.GetDescription());

      if (sb.Length == 0) sb.Append("No Mods");
      tEffects.text = sb.ToString();

      SpriteRenderer sr = weapon.gameObject.GetComponent<SpriteRenderer>();
      if (sr == null)
      {
        sr = weapon.gameObject.GetComponentInChildren<SpriteRenderer>();
      }
      // Debug.Log("weapon is " + weapon + " sprite is " + sr);
      if (sr != null && sr.sprite != null)
      {
        uiIcon.sprite = sr.sprite;
        uiIcon.enabled = true;
        uiIcon.color = Color.white;
        uiIcon.preserveAspect = true;
      }
      else uiIcon.enabled = false;
    }
    else
    {
      tName.text = "Empty";
      tDmg.text = "-";
      tSpeed.text = "-";
      tEffects.text = "";
      uiIcon.enabled = false;
    }
  }

  float GetTotalDamage(IWeapon weapon)
  {
    if (weapon == null) return 0;
    float dmg = weapon.stats.damage;

    if (weapon.upgrades != null)
    {
      foreach (var up in weapon.upgrades)
      {
        if (up is Statupgrade statUp) dmg += statUp.damageBonus;
      }
    }
    return dmg;
  }


  float GetTotalSpeed(IWeapon weapon)
  {
    if (weapon == null) return 1f;
    float spd = weapon.stats.attackspeed;

    if (weapon.upgrades != null)
    {
      foreach (var up in weapon.upgrades)
      {
        if (up is Statupgrade statUp) spd += statUp.fireRateBonus;
      }
    }
    return spd;
  }


  void CompareAPS(float oldAPS, float newAPS, TMP_Text textUI)
  {

    if (oldAPS <= 0.001f) oldAPS = 1f;

    float multiplier = newAPS / oldAPS;

    if (Mathf.Abs(multiplier - 1f) < 0.01f)
    {
      textUI.text = "1.0x Spd";
      textUI.color = Color.white;
    }
    else if (multiplier > 1f)
    {
      textUI.text = $"{multiplier:F2}x Spd";
      textUI.color = Color.green;
    }
    else
    {
      textUI.text = $"{multiplier:F2}x Spd";
    }
  }

  void CompareStat(float oldVal, float newVal, TMP_Text text)
  {
    Debug.Log(oldVal + " " + newVal);
    if (Mathf.Abs(newVal - oldVal) < 0.01f)
    {
      text.color = Color.white;

      return;
    }

    if (newVal > oldVal)
    {
      text.color = Color.green;
    }
    else
    {
      text.color = Color.red;
    }
  }

  public void OnClick_Equip() { playerRef.ConfirmSwapWeapon(pendingNewWeapon); CloseMenu(); }
  public void OnClick_Discard() { CloseMenu(); }
  void CloseMenu() 
{ 
    GameState.Instance.RequestPause(false); 
    panel.SetActive(false); 
    pendingNewWeapon = null; 
}
}
