using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DungeonKeyManager_src : MonoBehaviour
{
/*------------- 概要 -------------------
下記処理を行う。
・鍵の所持数をテキストへ反映処理
・鍵の使用/取得による鍵所持数の増減処理
・使用する通常鍵/ボス鍵の判定処理
*/

/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] uint u1_g_normalDoor_key;
    [SerializeField] uint u1_g_bossDoor_key;
    [SerializeField] TextMeshProUGUI tx_g_keyCount;
    [SerializeField] Image im_g_bossKey_img;

/*--------------- 定数 ----------------*/
// 無し

/*------------- 代入用変数----------------*/
// 無し



    /// <summary>
    /// スクリプトのインスタンスロード時に呼び出される
    /// </summary>
    private void Awake()
    {   
        KeyCountTextUpdate();
    }



    /// <summary>
    /// 所持している鍵の数をテキストに反映する処理
    /// </summary>
    private void KeyCountTextUpdate()
    {
        tx_g_keyCount.text = "×" + u1_g_normalDoor_key;
        
        if(u1_g_bossDoor_key > 0)
        {
            im_g_bossKey_img.enabled = true;
        }
        else
        {
            im_g_bossKey_img.enabled = false;
        }
    }



    /// <summary>
    /// 通常鍵使用関数
    /// </summary>
    /// <detail>
    /// 鍵を1つ以上所持しているかを判定する。
    /// True：鍵の所持数から1引いて"true"を返す。
    /// False："false"を返す
    /// </detail>
    private bool UsedNormalDoorKey()
    {
        // IF：鍵を1つ以上所持しているか
        if(u1_g_normalDoor_key > 0)
        {
            u1_g_normalDoor_key -=1;
            KeyCountTextUpdate();   // 鍵の所持数テキスト更新
            return true;
        }
        else
        {
            return false;
        }
    }



    /// <summary>
    /// ボス鍵使用関数
    /// </summary>
    /// <detail>
    /// 鍵を1つ以上所持しているかを判定する。
    /// True：鍵の所持数から1引いて"true"を返す。
    /// False："false"を返す
    /// </detail>
    private bool UsedBossDoorKey()
    {
        if(u1_g_bossDoor_key > 0)
        {
            u1_g_bossDoor_key -= 1;
            KeyCountTextUpdate();   // 鍵の所持数テキスト更新
            return true;
        }
        else
        {
            return false;
        }
    }



    /// <summary>
    /// 使用する鍵が通常/ボスかを判定し、対応する鍵の使用関数を呼び出す。
    /// </summary>
    /// <detail>
    /// DungenKeyDoor_srcスクリプト内で呼び出す関数。
    /// 呼び出し時に通常鍵付き扉/ボス鍵付き扉かを指定する。
    /// </detail>
    /// <param name="isBossKey">鍵付き扉の種類</param>
    /// <returns></returns>
    public bool UseDoorKey(bool fg_l_isBossKeyDoor)
    {
        // IF：ボス扉か
        if(fg_l_isBossKeyDoor)
        {
            // ボス鍵の使用関数を呼び出し。
            return UsedBossDoorKey();
        }
        else
        {
            // 通常鍵の使用関数を呼び出し。
            return UsedNormalDoorKey();
        }
    }


    /// <summary>
    /// 鍵を取得した際の所持数加算処理を行う
    /// </summary>
    /// <param name="fg_l_isBossKey">取得した鍵はボス鍵か</param>
    public void GetDoorKey(bool fg_l_isBossKey)
    {
        // IF：ボス鍵か
        if(fg_l_isBossKey)
        {
            // ボス鍵の所持数を1加算する
            u1_g_bossDoor_key += 1;
        }
        else
        {
            // 通常鍵の所持数を1加算する
            u1_g_normalDoor_key +=1;
        }
        // 鍵の所持数テキスト更新
        KeyCountTextUpdate();
    }
}
