using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialScreen : MonoBehaviour
{
    [SerializeField] private Animator anim;

    private float timer = 0;

    void Update()
    {
        if (timer >= 5f)
            if (Input.anyKeyDown)
                StartCoroutine(LoadNextScene());

        timer += Time.unscaledDeltaTime;
    }

    private IEnumerator LoadNextScene()
    {
        anim.SetTrigger("Exit");

        float counter = 0;
        float waitTime = anim.GetCurrentAnimatorStateInfo(0).length;

        while (counter < waitTime)
        {
            counter += Time.deltaTime;
            yield return null;
        }

        SceneManager.LoadScene(1);
    }
}
