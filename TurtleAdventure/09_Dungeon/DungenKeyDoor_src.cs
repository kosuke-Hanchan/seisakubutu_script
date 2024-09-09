using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungenKeyDoor_src : MonoBehaviour
{
/*------------- 概要 -------------------
施錠された扉にアタッチする。
鍵を使用して施錠された扉の開錠を行う。
*/

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] DungenKeyManager_src sc_g_keyManager;
    [SerializeField] bool fg_g_bossDoor;
    
/*--------------- 定数 ----------------*/
// 無し

/*------------- 代入用変数----------------*/
// 無し



    /// <summary>
    /// 他コライダーに接触した際に呼び出される処理。
    /// 扉の鍵を開錠する。
    /// </summary>
    /// <detail>
    /// 接触したコライダーがプレイヤーの場合、対象の鍵所持数を使用して当オブジェクトを削除(開錠)する。
    /// </detail>
    /// <param name="cl_l_hitCol">
    /// 接触したコライダー
    /// </param>
    private void OnTriggerEnter(Collider cl_l_hitCol)
    {
        // IF：接触したコライダーのGameObjectがプレイヤーか(早期リターン)
        if(!cl_l_hitCol.gameObject.CompareTag("PlayerHitCollider"))
        {
            return;   
        }
        else
        {
            // NOP
        }

        // IF:対象のカギを1つ以上所持しているか
        if(sc_g_keyManager.UseDoorKey(fg_g_bossDoor))
        {
            Destroy(this.gameObject);
        }
        else
        {
            // NOP
        }
    }
}
