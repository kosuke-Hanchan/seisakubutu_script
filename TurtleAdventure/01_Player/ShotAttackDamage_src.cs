using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotAttackDamage_src : MonoBehaviour
{
/*------------- 概要 -------------------
    プレイヤーアクションであるショット攻撃の当たり判定用スクリプトである。
    プレイヤーオブジェクトの子オブジェクトである当たり判定用コライダーにアタッチする。
    他コライダーに触れた際、"Player_ctrl_src"スクリプトの"ShotAttackDamage"関数を呼び出す。
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
        // 親オブジェクト（プレイヤーオブジェクト）から"Player_ctrl_src"スクリプトを取得
        sc_g_script = this.transform.parent.GetComponent<Player_ctrl_src>();
    }



    /// <summary>
    /// 他コライダーに接触した際に呼び出される処理
    /// </summary>
    /// <detail>
    /// 接触したコライダーが敵オブジェクトの場合、"Player_ctrl_src"スクリプトの"ShotAttackDamage"関数を呼び出す。
    /// </detail>
    /// <param name="cl_l_hitCol">
    /// 接触したコライダー
    /// </param>
    private void OnTriggerEnter(Collider cl_l_hitCol)
    {
        // IF：接触したコライダーのGameObjectが敵かどうかを調べる（毎回GetComponentするのは動作が重いため、違う場合は早期リターン）
        if (!cl_l_hitCol.gameObject.CompareTag("Enemy"))
        {
            return;   
        }
        else
        {
            // NOP
        }
   
        // 接触したGameObjectを引数として"ShotAttackDamage"関数を呼び出す
        sc_g_script.ShotAttackDamage(cl_l_hitCol.gameObject);
    }
}
