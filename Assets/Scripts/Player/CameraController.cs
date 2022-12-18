using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private PlayerController player;

    private float prevYPos = 0;
    private void Update()
    {
        prevYPos = this.transform.position.y;
        if (player == null) return;

        if (player.transform.position.y > this.transform.position.y)
            this.transform.position = new Vector3(0, player.transform.position.y, -10);

        if (this.transform.position.y > prevYPos)
            GameManager.Instance.IncreaseScore((int)(this.transform.position.y * 1.5f));

    }
    public static IEnumerator Shake(float _duration, float _magnitude)
    {
        float elapsed = 0f;

        while (elapsed < _duration)
        {
            //float x = Random.Range(-1f, 1f) * _magnitude;
            float y = Random.Range(-1f, 1f) * _magnitude;

            Camera.main.transform.position += new Vector3(0, y);
            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }
    }
}
