using UnityEngine;
using UnityEngine.UI;

public class PlayerHeartManager_src : MonoBehaviour
{
/*------------- 概要 -------------------
プレイヤーのHPを表すハートアイコンを制御するスクリプトである。
"Player_ctrl_src"より、当スクリプトの関数を呼び出して使用する。
下記処理を行う。
・ハートアイコンの生成処理
・被ダメージ/回復時にハートアイコンの増減処理
・ハートアイコン増減時のアニメーション呼び出し
*/


/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] int s4_g_gealthDiv = 2;                // ハートアイコンの分割数(ハートアイコン1つで表すHP数)
    [SerializeField] float fl_g_heartSpace = 100f;          // ハートアイコンを設置する際の間隔
    [SerializeField] GameObject go_g_heartPrefab;           // ハートアイコンのプレハブ
    [SerializeField] Transform tf_g_heartsContainer;        // ハートアイコンを配置するコンテナ(設置位置ピボット役)
    [SerializeField] Player_ctrl_src sc_g_Player_Ctrl_src;  // "sc_g_Player_Ctrl_src"スクリプト(プレイヤー操作全般スクリプト)

/*--------------- 定数 ----------------*/
// 無し

/*------------- 代入用変数----------------*/
    private int s4_g_maxHealth;           // 最大HP
    private int s4_g_currentHealth;       // 現在HP
    private PlayerHeartsAnim_src[] asc_g_PlayerHeartsAnim_src;     // ハートアイコンのUIイメージ配列
    

    /// <summary>
    /// ハートマネージャーの初期化
    /// </summary>
    /// <param name="s4_l_maxHealth">プレイヤーの最大HP</param>
    /// <param name="s4_l_currentHealth">プレイヤーの現在HP</param>
    public void InitializeHeartManager(int s4_l_maxHealth, int s4_l_currentHealth)
    {
        s4_g_maxHealth = s4_l_maxHealth;                // 最大HPを取得
        s4_g_currentHealth = s4_l_currentHealth;        // 現在HPを取得

        GenerateHearts();   // 最大HPに応じた数のハートアイコンを生成
        UpdateHearts();     // ハートアイコンを現在HPに更新
    }



    /// <summary>
    /// 被ダメージ処理
    /// </summary>
    /// <param name="damage"></param>
    public void GetDamage(int s4_l_currentHealth)
    {
        // 現在HPを更新
        s4_g_currentHealth = s4_l_currentHealth;
        // ハートアイコンを更新
        UpdateHearts();
        // アニメーションを適用
        AnimateHearts();
    }



    /// <summary>
    /// 回復処理
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(int s4_l_currentHealth)
    {
        // 現在HPを更新
        s4_g_currentHealth = s4_l_currentHealth;
        // 現在のHPを0から最大HPの範囲に収める
        s4_g_currentHealth = Mathf.Clamp(s4_g_currentHealth, 0, s4_g_maxHealth);
        // ハートアイコンを更新
        UpdateHearts();
        // アニメーションを適用
        AnimateHearts();
    }



    /// <summary>
    /// ハートアイコンの生成処理
    /// </summary>
    private void GenerateHearts()
    {
        // ハートの数を計算
        int numberOfHearts = Mathf.CeilToInt((float)s4_g_maxHealth / s4_g_gealthDiv);
        asc_g_PlayerHeartsAnim_src = new PlayerHeartsAnim_src[numberOfHearts];

        // ハートアイコンを生成し、配列に格納
        for (int i = 0; i < numberOfHearts; i++)
        {
            // ハートアイコンの設置
            GameObject heart = Instantiate(go_g_heartPrefab, tf_g_heartsContainer);
            asc_g_PlayerHeartsAnim_src[i] = heart.GetComponent<PlayerHeartsAnim_src>();

            // ハートアイコンの位置を設定
            RectTransform heartRect = heart.GetComponent<RectTransform>();
            heartRect.anchoredPosition = new Vector2(i * fl_g_heartSpace, 0);
        }
    }



    /// <summary>
    /// ハートアイコンを更新する処理
    /// </summary>
    private void UpdateHearts()
    {
        // ハート1つにつき2HP
        for (int i = 0; i < asc_g_PlayerHeartsAnim_src.Length; i++)
        {
            // ハートのインデックスから対応するHPの範囲を計算
            int heartHP = i * s4_g_gealthDiv;

            if (s4_g_currentHealth >= heartHP + s4_g_gealthDiv)
            {
                // フルのハート
                asc_g_PlayerHeartsAnim_src[i].GetComponent<Image>().fillAmount = 1;
            }
            else if (s4_g_currentHealth > heartHP)
            {
                // 部分的に満たされたハート
                asc_g_PlayerHeartsAnim_src[i].GetComponent<Image>().fillAmount = (float)(s4_g_currentHealth - heartHP) / s4_g_gealthDiv;
            }
            else
            {
                // 空のハート
                asc_g_PlayerHeartsAnim_src[i].GetComponent<Image>().fillAmount = 0;
            }
        }
    }



    /// <summary>
    /// ハートアイコンのアニメーションを適用する処理
    /// </summary>
    private void AnimateHearts()
    {
        for (int i = 0; i < asc_g_PlayerHeartsAnim_src.Length; i++)
        {
            int heartHP = i * s4_g_gealthDiv;

            if (s4_g_currentHealth >= heartHP + s4_g_gealthDiv)
            {
                asc_g_PlayerHeartsAnim_src[i].AnimateHeal();
            }
            else if (s4_g_currentHealth > heartHP)
            {
                asc_g_PlayerHeartsAnim_src[i].AnimateHeal();
            }
            else
            {
                asc_g_PlayerHeartsAnim_src[i].AnimateDamage();
            }
        }
    }
}
