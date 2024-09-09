using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneFader_src : MonoBehaviour
{
    [SerializeField] Material fadeMaterial;
    [SerializeField] float fadeDuration = 2f;

    private void Start()
    {
        // 初期フェードイン
        StartCoroutine(FadeIn());
    }

    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    private IEnumerator FadeIn()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeMaterial.SetFloat("_Fade", t);
            yield return null;
        }
    }

    private IEnumerator FadeOut(string sceneName)
    {
        float elapsedTime = 1f;

        while (elapsedTime > 0)
        {
            elapsedTime -= Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            fadeMaterial.SetFloat("_Fade", t);
            yield return null;
        }

        // シーンをロード
        SceneManager.LoadScene(sceneName);
    }
}
