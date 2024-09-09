using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallAbyss_src : MonoBehaviour
{
/*------------- 概要 -------------------
奈落として扱うコライダーオブジェクトにアタッチする。
プレイヤーが奈落に落下したかを判定し、結果に伴う処理を行う。
*/

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] GimmickStatus_data dt_g_gimmickStatus_data;  // Enemyステータス管理用スクリプタブルオブジェクト

/*--------------- 定数 ----------------*/
// 無し

/*------------- 代入用変数----------------*/
// 無し


    /// <summary>
    /// 他コライダーに接触した際に呼び出される
    /// プレイヤーが奈落に落下したかを判定し、下記処理を行う。
    /// ・最終安全地帯への復帰処理コルーチン開始
    /// ・プレイヤーへの与ダメージ処理
    /// </summary>
    /// <param name="cl_l_hitCol">接触したコライダー</param>
    private void OnTriggerEnter(Collider cl_l_hitCol)
    {
        GameObject go_l_hitObj = cl_l_hitCol.gameObject;

        // IF：接触したコライダーのGameObjectがプレイヤーかどうかを調べる（毎回GetComponentするのは動作が重いため、違う場合は早期リターン）
        if (!go_l_hitObj.CompareTag("PlayerHitCollider"))
        {
            return;
        }
        else
        {
            // NOP
        }

        // ヒットしたオブジェクトのIDamageableを取得する
        IDamageable damageHit = go_l_hitObj.transform.parent.GetComponent<IDamageable>(); 

        // ダメージ判定が実装されていなければ、ダメージ判例を行わない(早期リターン)
        if (damageHit == null)
        { 
            return;
        }
        else
        {
            // NOP
        }
        // プレイヤー操作スクリプト(Player_ctrl_src)内のSetLastSafePositionコルーチン（プレイヤーの位置の最終安全地帯への復帰処理）を開始
        StartCoroutine(go_l_hitObj.transform.parent.GetComponent<Player_ctrl_src>().SetLastSafePosition());
        // 与ダメージ
        damageHit.Damage(dt_g_gimmickStatus_data.DAMAGE_VALUE);
    }
}