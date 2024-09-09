using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableObject_src : MonoBehaviour
{
/*------------- 概要 -------------------
破壊可能オブジェクトとするオブジェクト（樽や壺など）にアタッチする。
プレイヤーの攻撃によるオブジェクト破壊処理を行うスクリプトである。
プレイヤーの攻撃判定用コライダーに触れた際、破壊エフェクトを生成して当オブジェクトを削除する。
ドロップアイテムを設定することによってアイテムドロップを行う。
アイテムドロップのランダム設定が可能
*/

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] GameObject go_g_break_effect;      // 破壊エフェクトオブジェクト
    [SerializeField] GameObject go_g_dropItem;          // 破壊時のドロップアイテム

    [SerializeField] bool fg_g_randomDrop_flg;          // アイテムドロップをランダム化するか
    [SerializeField] uint u1_g_dropProb;                // ランダムドロップ設定時の確率

/*--------------- 定数 ----------------*/
// 無し

/*------------- 代入用変数----------------*/
// 無し



    /// <summary>
    /// コライダーに接触した際に呼び出される処理。
    /// オブジェクトの破壊処理を行う。
    /// </summary>
    /// <detail>
    /// 接触したコライダーが"AttackCollider"(プレイヤーの攻撃判定用コライダー)であった場合、
    /// 破壊エフェクトを生成して破壊対象オブジェクト(当オブジェクト)を削除する。
    /// </detail>
    /// <param name="cl_l_hitCol">
    /// 接触したコライダー
    /// </param>
    private void OnTriggerEnter(Collider cl_l_hitCol)
    {
       // IF:接触したコライダーが"BombCollider"(爆弾の当たり判定用コライダー)でない場合、早期リターン
        if (!cl_l_hitCol.gameObject.CompareTag("PlayerAttackCollider"))
        {
            return;
        }
        else
        {
            // NOP
        }

        // 破壊エフェクトを生成
        Instantiate(go_g_break_effect, this.transform.position, Quaternion.identity);

        // 破壊オブジェクト(当オブジェクト)を削除
        Destroy(this.gameObject);
        
        // ドロップアイテムが設定されていない場合は早期リターン(エラー回避)
        if(go_g_dropItem == null)
        {
            return;
        }
        else
        {
            // NOP
        }

        // ドロップアイテムを生成
        DropItem();
    }



    /// <summary>
    /// アイテムのドロップ処理
    /// ランダムドロップ化設定時は1/u1_g_dropProbの確率でアイテム生成(アイテムドロップ)を行う
    /// </summary>
    private void DropItem()
    {
        if(fg_g_randomDrop_flg)
        {
            int s1_l_rand = Random.Range(0,(int)u1_g_dropProb);
            if(s1_l_rand == 0)
            {
                // 破壊時ドロップアイテムを生成
                Instantiate(go_g_dropItem, this.transform.position + new Vector3(0,3,0), Quaternion.identity);
            }
            else
            {
                // NOP
            }
        }
        else
        {
            // 破壊時ドロップアイテムを生成
            Instantiate(go_g_dropItem, this.transform.position + new Vector3(0,3,0), Quaternion.identity);
        }
    }
}
