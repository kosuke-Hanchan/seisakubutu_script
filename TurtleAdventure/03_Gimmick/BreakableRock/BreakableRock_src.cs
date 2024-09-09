using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableRock_src : MonoBehaviour
{
/*------------- 概要 -------------------
爆弾によって破壊される岩の処理を行うスクリプトである。
爆弾の当たり判定用コライダーに触れた際、岩破壊エフェクトを生成して当オブジェクトを削除する
*/

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] GameObject go_g_break_effect;      // 岩破壊エフェクトオブジェクト

/*--------------- 定数 ----------------*/
// 無し

/*------------- 代入用変数----------------*/
// 無し


    /// <summary>
    /// 他コライダーに接触した際に呼び出される処理。
    /// </summary>
    /// <detail>
    /// 接触したコライダーが"BombCollider"(爆弾の当たり判定用コライダー)であった場合、
    /// 岩破壊エフェクトを生成して岩オブジェクトを削除する。
    /// </detail>
    /// <param name="cl_l_hitCol">
    /// 接触したコライダー
    /// </param>
    private void OnTriggerEnter(Collider cl_l_hitCol)
    {
       // IF:接触したコライダーが"BombCollider"(爆弾の当たり判定用コライダー)でない場合、早期リターン
        if (!(cl_l_hitCol.gameObject.name == "BombCollider"))
        {
            return;
        }
        else
        {
            // NOP
        }

        // 岩が破壊された際のエフェクトを生成
        Instantiate(go_g_break_effect, this.transform.position, Quaternion.identity);

        // 岩オブジェクト(当オブジェクト)を削除
        Destroy(this.gameObject);
    }
}
