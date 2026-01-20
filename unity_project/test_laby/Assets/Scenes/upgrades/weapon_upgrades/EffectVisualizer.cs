using UnityEngine;
using System.Collections;

public class ExplosionVisualizer : MonoBehaviour
{
  public static ExplosionVisualizer Instance;

  private void Awake()
  {
    if (Instance != null)
      Destroy(gameObject);
    else
      Instance = this;
  }

  public void ShowCircle(Vector3 position, float radius, float duration)
  {
    StartCoroutine(DrawCircleRoutine(position, radius, duration));
  }

  private IEnumerator DrawCircleRoutine(Vector3 position, float radius, float duration)
  {
    // Create GameObject with LineRenderer
    GameObject go = new GameObject("ExplosionCircle");
    go.transform.position = position;
    LineRenderer lr = go.AddComponent<LineRenderer>();
    lr.positionCount = 50;
    lr.loop = true;
    lr.widthMultiplier = 0.05f;
    lr.material = new Material(Shader.Find("Sprites/Default"));
    lr.startColor = lr.endColor = Color.red;

    // Calculate circle points
    for (int i = 0; i < lr.positionCount; i++)
    {
      float angle = i * Mathf.PI * 2f / lr.positionCount;
      Vector3 pos = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
      lr.SetPosition(i, pos);
    }

    // Wait duration
    yield return new WaitForSeconds(duration);

    GameObject.Destroy(go);
  }
}
