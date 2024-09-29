using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SelectTypeMenu_src : MonoBehaviour
{
/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] Button bt_g_toggleButton;          // メニュー開閉用のボタン
    [SerializeField] GameObject go_g_radialMenu;          // メニュー開閉用のボタン
    [SerializeField] Button[] radialButtons;            // プレハブで配置したボタンの配列

/*--------------- 定数 ----------------*/
// 無し

/*------------- 代入用変数----------------*/
    private Player_ctrl_src sc_g_Player_Ctrl_src;       // sc_g_Player_Ctrl_srcスクリプト格納用
    private bool isMenuOpen = false;                    // メニューの開閉状態を保持



    /// <summary>
    /// スクリプトのインスタンスロード時に呼び出される
    /// </summary>
    private void Awake()
    {
        // Player_ctrl_srcスクリプト取得
        sc_g_Player_Ctrl_src = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ctrl_src>();

        // トグルボタンのクリックイベントを設定
        bt_g_toggleButton.onClick.AddListener(ToggleMenu);

        // 初期状態ではメニューを非表示にする
        SetMenuActive(false);
    }



    /// <summary>
    /// ボタンセットアップ処理
    /// </summary>
    public void SetupRadialButtons(Dictionary<int, bool> ActionTypeStatus)
    {
        // ボタンの数と辞書のエントリ数が同じだと仮定
        for (int i = 0; i < radialButtons.Length; i++)
        {
            // ActionTypeStatusに基づいてボタンを表示・非表示
            if (ActionTypeStatus.ContainsKey(i))
            {
                bool isUnlocked = ActionTypeStatus[i];
                radialButtons[i].gameObject.SetActive(isUnlocked);

                // ボタンが表示されている場合、背景色などの設定を行う
                if (isUnlocked)
                {
                    // ボタンにクリックイベントを追加
                    int index = i;  // クロージャ問題を回避するためにローカル変数を使用
                    radialButtons[i].onClick.AddListener(() => OnButtonClick(index));
                }
            }
        }
    }



    /// <summary>
    /// ボタンの表示/非表示処理
    /// </summary>
    public void ToggleMenu()
    {
        // IF:プレイヤー状態が"待機状態"か
        if(sc_g_Player_Ctrl_src.player_state == 0)
        {
            // メニューの開閉状態を切り替える
            isMenuOpen = !isMenuOpen;
        }
        else
        {
            // NOP
        }
        
        // メニューの表示/非表示を設定
        SetMenuActive(isMenuOpen);

        // 時間の停止/再開を設定
        Time.timeScale = isMenuOpen ? 0 : 1;
    }


    /// <summary>
    /// 配置したボタンの表示/非表示を切り替える
    /// </summary>
    /// <param name="isActive"></param>
    private void SetMenuActive(bool isActive)
    {
        // 配置したボタンの表示/非表示を切り替える
        go_g_radialMenu.SetActive(isActive);
    }


    /// <summary>
    /// 各ボタン処理
    /// </summary>
    /// <param name="index"></param>
    public void OnButtonClick(int index)
    {
        // ボタンがクリックされたときの処理を設定
        Debug.Log("Button " + (index + 1) + " clicked");

        // ボタンを押した際にメニューを閉じる
        SetMenuActive(false);
        isMenuOpen = false;
        Time.timeScale = 1;
        
        // ボタンごとの処理を追加
        switch (index)
        {
            // ボタン1の処理
            case 0:
                // プレイヤーのアクションタイプを"ショット攻撃"へ切り替える
                sc_g_Player_Ctrl_src.Button_SetPlayerMode((uint)Player_ctrl_src.ACTION_TYPE.SHOT);
                break;

            // ボタン2の処理
            case 1:
                // プレイヤーのアクションタイプを"飛び道具攻撃"へ切り替える
                sc_g_Player_Ctrl_src.Button_SetPlayerMode((uint)Player_ctrl_src.ACTION_TYPE.GUN);
                break;

            // ボタン3の処理
            case 2:
                // プレイヤーのアクションタイプを"防御"へ切り替える
                sc_g_Player_Ctrl_src.Button_SetPlayerMode((uint)Player_ctrl_src.ACTION_TYPE.GUARD);
                break;

            // ボタン4の処理 
            case 3:
                // プレイヤーのアクションタイプを"爆弾"へ切り替える
                sc_g_Player_Ctrl_src.Button_SetPlayerMode((uint)Player_ctrl_src.ACTION_TYPE.BOMB);
                break;

            // ボタン5の処理   
            case 4:
                // プレイヤーのアクションタイプを"ダッシュ"へ切り替える
                sc_g_Player_Ctrl_src.Button_SetPlayerMode((uint)Player_ctrl_src.ACTION_TYPE.DASH);
                break;

            // それ以外
            default:
                // NOP
                break;
        }
    }
}
