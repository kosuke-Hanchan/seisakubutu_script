using UnityEngine;
using ES3Types;

public class SaveManager_src : MonoBehaviour
{
    public string saveFile = "SaveFile.es3";
    public Transform player;

    void Start()
    {
        // プレイヤーの位置を読み込み
        LoadPlayerPosition();
    }

    void OnApplicationQuit()
    {
        // プレイヤーの位置を保存
        // SavePlayerPosition();
    }

    public void SavePlayerPosition()
    {
        Vector3 position = player.position;
        ES3.Save("playerPosition", position, saveFile);
        Debug.Log("Player position saved: " + position);
    }

    public void LoadPlayerPosition()
    {
        if (ES3.KeyExists("playerPosition", saveFile))
        {
            Vector3 position = ES3.Load<Vector3>("playerPosition", saveFile);
            player.position = position;
            Debug.Log("Player position loaded: " + position);
        }
    }
}
