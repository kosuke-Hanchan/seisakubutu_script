using UnityEngine;
using UnityEngine.UI;

public class RadialMenu_src : MonoBehaviour
{
/*------------- インスペクター設定用変数 --------------*/
    [SerializeField] GameObject go_g_buttonPrefab;      // ボタンのプレハブ
    [SerializeField] Button bt_g_toggleButton;          // メニュー開閉用のボタン
    [SerializeField] float fl_l_buttonRadius = 100f;    // ボタン生成位置の半径
    [SerializeField] Sprite[] buttonImages;             // 各ボタンに設定する画像
    [SerializeField] Vector2 buttonSize = new Vector2(150f, 150f); // 生成するボタンのサイズ

/*--------------- 定数 ----------------*/
    private const uint u1_g_numberOfButtons = 5;   // ボタンの数


/*------------- 代入用変数----------------*/
    private Player_ctrl_src sc_g_Player_Ctrl_src;       // sc_g_Player_Ctrl_srcスクリプト格納用
    private bool isMenuOpen = false;                    // メニューの開閉状態を保持



    void Awake()
    {
        sc_g_Player_Ctrl_src = GameObject.FindGameObjectWithTag("Player").GetComponent<Player_ctrl_src>();
        // トグルボタンのクリックイベントを設定
        bt_g_toggleButton.onClick.AddListener(ToggleMenu);
        
        // 円形メニューを作成
        CreateRadialMenu();
        
        // 初期状態ではメニューを非表示にする
        SetMenuActive(false);
    }


    /// <summary>
    /// 円形メニューの生成処理
    /// </summary>
    void CreateRadialMenu()
    {
        // 各ボタンの配置角度を計算
        float angleStep = 360f / u1_g_numberOfButtons;

        for (int i = 0; i < u1_g_numberOfButtons; i++)
        {
            // 配置角度を計算
            float angle = i * angleStep;
            
            // 円形の位置を計算
            float xPos = Mathf.Cos(angle * Mathf.Deg2Rad) * fl_l_buttonRadius;
            float yPos = Mathf.Sin(angle * Mathf.Deg2Rad) * fl_l_buttonRadius;

            // ボタンを生成して配置
            GameObject button = Instantiate(go_g_buttonPrefab, transform);
            button.transform.localPosition = new Vector3(xPos, yPos, 0);

            // ボタンのサイズを設定
            RectTransform rectTransform = button.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.sizeDelta = buttonSize;
            }
            
            // ボタンに名前を付ける
            button.name = "Button " + (i + 1);
            
            // ボタンに画像を設定
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImages.Length > i)
            {
                buttonImage.sprite = buttonImages[i];
            }
            
            // ボタンにクリックイベントを追加
            int index = i; // クロージャ問題を回避するためにローカル変数を使用
            button.GetComponent<Button>().onClick.AddListener(() => OnButtonClick(index));
        }
    }


    /// <summary>
    /// 
    /// </summary>
    void ToggleMenu()
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
    /// 
    /// </summary>
    /// <param name="isActive"></param>
    void SetMenuActive(bool isActive)
    {
        // 子要素（ボタン）の表示/非表示を設定
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(isActive);
        }
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="index"></param>
    void OnButtonClick(int index)
    {
        // ボタンがクリックされたときの処理を設定

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
