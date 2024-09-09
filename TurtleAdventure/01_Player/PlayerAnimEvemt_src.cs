using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvemt_src : MonoBehaviour
{
/*------------- 概要 -------------------
    当スクリプト内の関数はアニメーションイベントとして設定して使用する。
    アニメーションイベントとして設定したい"Player_Ctrl_src"スクリプト内の関数を、当スクリプトの関数で呼び出す。
    "Animator"コンポーネントを付けたプレイヤーオブジェクトへ当スクリプトを付与する。
    また、プレイヤーオブジェクトと同一のプレハブ内に"Player_Ctrl_src"スクリプトが付与されていることが前提であるする。
*/

/*------------- インスペクター設定用変数 --------------*/
// 無し

/*--------------- 定数 ----------------*/
// 無し

/*------------- 代入用変数----------------*/
    private Player_ctrl_src sc_g_script;     // "Player_ctrl_src"スクリプト取得用



    /// <summary>
    /// スクリプトのインスタンスロード時に呼び出される
    /// </summary>
    private void Awake()
    {
        // 親オブジェクト（プレイヤーオブジェクト）からPlayer_ctrl_srcを取得
        sc_g_script = this.transform.parent.GetComponent<Player_ctrl_src>();
    }


    /// <summary>
    /// アニメーションイベント(待機状態遷移)
    /// </summary>
    public void AnimEventStateSetIdle()
    {
        sc_g_script.AnimEventStateSetIdle();
    }


    /// <summary>
    /// 爆弾設置アニメーションイベント
    /// </summary>
    public void AnimEventSetBomb()
    {
        sc_g_script.AnimEventSetBomb();
    }


/* =================<エフェクト>===========================*/
    /// <summary>
    /// 銃弾発射アニメーションイベント
    /// </summary>
    public void AnimEventShotEffect()
    {
        sc_g_script.AnimEventShotEffect();
    }

        
    /// <summary>
    /// 銃攻撃アニメーションイベント(弾生成、エフェクト)
    /// </summary>
    public void AnimEventGunEffect()
    {
        sc_g_script.AnimEventGunEffect();
    }


/* =================<サウンドエフェクト>===========================*/
    /// <summary>
    /// 足音アニメーションイベント
    /// </summary>
    public void AnimEventStateFootStepSound()
    {
        sc_g_script.AnimEventStateFootStepSound();
    }


    /// <summary>
    /// ショット攻撃アニメーションイベント（サウンドエフェクト）
    /// </summary>
    public void AnimEventShotSE()
    {
        sc_g_script.AnimEventShotSE();
    }


    /// <summary>
    /// 銃弾発射アニメーションイベント（サウンドエフェクト）
    /// </summary>
    public void AnimEventGunSE()
    {
        sc_g_script.AnimEventGunSE();
    }


    /// <summary>
    /// 爆弾設置アニメーションイベント（サウンドエフェクト）
    /// </summary>
    public void AnimEventBombSetSE()
    {
        sc_g_script.AnimEventBombSetSE();
    }
}
