using UnityEngine;
using System.Collections;

public class ExplosionVisualizer : MonoBehaviour
{
  public static ExplosionVisualizer Instance;

  public LineRenderer ringRenderer;
  private void Awake()
  {
    if (Instance != null)
      Destroy(gameObject);
    else
      Instance = this;
    if (ringRenderer != null)
    {
      ringRenderer.positionCount = 65;
      ringRenderer.enabled = false;
    }
  }

  public void ShowCircle(Vector3 position, float radius, float duration)
  {
    StopAllCoroutines();
    StartCoroutine(ExpandRing(position, radius, duration));
  }

  IEnumerator ExpandRing(Vector3 position, float radius, float duration)
  {
    ringRenderer.enabled = true;

    float time = 0f;
    float maxRadius = radius;

    while (time < duration)
    {
      // StartCoroutine(ExpandRing(position, radius, duration));
      float t_radius = Mathf.Lerp(0f, maxRadius, time / duration);
      DrawRing(position, t_radius);
      time += Time.deltaTime;
      yield return null;
    }

    ringRenderer.enabled = false;
  }

  void DrawRing(Vector3 position, float radius)
  {
    for (int i = 0; i <= 64; i++)
    {
      float angle = i * Mathf.PI * 2f / 64;
      Vector3 offset = new Vector3(
          Mathf.Cos(angle) * radius,
          Mathf.Sin(angle) * radius,
          0f
      );

      ringRenderer.SetPosition(i, position + offset);

    }
  }

}
