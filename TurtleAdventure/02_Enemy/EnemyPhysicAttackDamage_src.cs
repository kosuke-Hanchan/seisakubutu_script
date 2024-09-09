using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPhysicAttackDamage_src : MonoBehaviour
{
/*------------- 概要 -------------------
    Enemyによる攻撃の当たり判定用スクリプトである。
    Enemyオブジェクトの子オブジェクトである、攻撃当たり判定用コライダーにアタッチする。
    他コライダーに触れた際、"EnemyCtrl_Skeleton_src"スクリプトの"AttackDamage"関数を呼び出す。
*/

/*------------- インスペクター設定用変数 --------------*/
// 無し

/*--------------- 定数 ----------------*/
// 無し

/*------------- 代入用変数----------------*/
    [SerializeField] EnemyStatus_data dt_g_enemyStatus_data;    // Enemyステータス管理用スクリプタブルオブジェクト



    /// <summary>
    /// 他コライダーに接触した際に呼び出される処理
    /// </summary>
    /// <detail>
    /// 接触したコライダーが敵オブジェクトの場合、"EnemyCtrl_Skeleton_src"スクリプトの"AttackDamage"関数を呼び出す。
    /// </detail>
    /// <param name="cl_l_hitCol">
    /// 接触したコライダー
    /// </param>
    private void OnTriggerEnter(Collider cl_l_hitCol)
    {
        // IF：接触したコライダーのGameObjectがプレイヤーかどうかを調べる（毎回GetComponentするのは動作が重いため、違う場合は早期リターン）
        if (!cl_l_hitCol.gameObject.CompareTag("PlayerHitCollider"))
        {
            return;   
        }
        else
        {
            // NOP
        }
        // 接触したGameObjectを引数として"ShotAttackDamage"関数を呼び出す
        AttackDamage(cl_l_hitCol.gameObject);
    }

    
    /// <summary>
    /// 攻撃の当たり判定用コライダーにアタッチしたスクリプト(EnemyAttackDamage_src)で呼び出す関数。
    /// 攻撃による敵への与ダメージ処理を行う
    /// </summary>
    /// <param name="go_l_hitObj">
    /// 当たり判定用コライダーが接触したGameObject
    /// </param>
    public void AttackDamage(GameObject go_l_hitObj)
    {
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

        // 与ダメージ処理(引数：与ダメージ値)
        damageHit.Damage(dt_g_enemyStatus_data.ATTACK_DAMAGE_VALUE);
    }
}
