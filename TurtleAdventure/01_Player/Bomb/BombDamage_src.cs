using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDamage_src : MonoBehaviour
{
/*------------- 概要 -------------------
    プレイヤーの爆弾設置アクション時に設置する爆弾の当たり判定用スクリプトである。
    爆弾オブジェクトの子オブジェクトである当たり判定用コライダーにアタッチする。
    他コライダーに接触した際、"Bomb_src"スクリプトの"BombDamage"関数を呼び出す。
*/

/*------------- インスペクター設定用変数 --------------*/
// 無し

/*--------------- 定数 ----------------*/
// 無し

/*------------- 代入用変数----------------*/
    private Bomb_src sc_g_script;    // "Bomb_src"スクリプト取得用


    /// <summary>
    /// スクリプトのインスタンスロード時に呼び出される
    /// </summary>
    private void Awake()
    {
        // 親オブジェクト（爆弾オブジェクト）から"Bomb_src"スクリプトを取得
        sc_g_script = this.transform.parent.gameObject.GetComponent<Bomb_src>();
    }



    /// <summary>
    /// 他コライダーに接触した際に呼び出される処理。
    /// </summary>
    /// <detail>
    /// 接触したコライダーが敵オブジェクトの場合、"Bomb_src"スクリプトの"BombDamage"関数を呼び出す。
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

        // 引数として、接触したコライダー"Hit"を設定して"BombDamage"関数を呼び出す
        sc_g_script.BombDamage(cl_l_hitCol.gameObject);
    }
}
