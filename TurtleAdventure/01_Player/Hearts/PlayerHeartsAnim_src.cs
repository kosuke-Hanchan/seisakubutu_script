using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHeartsAnim_src : MonoBehaviour
{
/*------------- 概要 -------------------
プレイヤーのHPを表すハートアイコンのアニメーション用スクリプトである。
"PlayerHeartManager_src"より、当スクリプトの関数を呼び出す。
*/

/*------------- インスペクター設定用変数 --------------*/
// 無し

/*--------------- 定数 ----------------*/
// 無し

/*------------- 代入用変数----------------*/
    private Image im_g_heartImage;  // "Image"コンポーネント取得用



    /// <summary>
    /// スクリプトのインスタンスロード時に呼び出される
    /// </summary>
    void Awake()
    {
        im_g_heartImage = GetComponent<Image>();
    }



    /// <summary>
    /// ハートがダメージを受けた際のアニメーション
    /// </summary>
    public void AnimateDamage()
    {
        // サイズを縮小し、色を赤に変化させるアニメーション
        StartCoroutine(AnimateHeart(Vector3.one, Vector3.one * 0.8f, Color.white, Color.red, 0.1f));
    }



    /// <summary>
    /// ハートが回復した際のアニメーション
    /// </summary>
    public void AnimateHeal()
    {
        // サイズを拡大し、色を緑に変化させるアニメーション
        StartCoroutine(AnimateHeart(Vector3.one, Vector3.one * 1.2f, Color.white, Color.green, 0.1f));
    }



    /// <summary>
    /// ハートのサイズと色をアニメーションするコルーチン
    /// </summary>
    /// <param name="startScale">アニメーション開始時のスケール</param>
    /// <param name="endScale">アニメーション終了時のスケール</param>
    /// <param name="startColor">アニメーション開始時の色</param>
    /// <param name="endColor">アニメーション終了時の色</param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private IEnumerator AnimateHeart(Vector3 startScale, Vector3 endScale, Color startColor, Color endColor, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            // 指定された期間でサイズと色を補間
            im_g_heartImage.transform.localScale = Vector3.Lerp(startScale, endScale, elapsed / duration);
            im_g_heartImage.color = Color.Lerp(startColor, endColor, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // アニメーションの終了時に元のサイズと色に戻す
        im_g_heartImage.transform.localScale = startScale;
        im_g_heartImage.color = startColor;
    }
}
