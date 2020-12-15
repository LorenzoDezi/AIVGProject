using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private string enemyTag = "Enemy";
    [SerializeField]
    private string playerTag = "Player";
    [SerializeField]
    private BulletSpawner bulletSpawner;
    [SerializeField]
    private GameObject player;
    private static GameManager instance;


    public static string EnemyTag => instance.enemyTag;
    public static string PlayerTag => instance.playerTag;
    public static GameObject Player => instance.player;
    public static BulletSpawner BulletSpawner => instance.bulletSpawner;

    private void Awake() {
        if(instance != null) {
            Destroy(instance);
        }
        instance = this;
    }

    void Start()
    {
        Cursor.visible = false;      
    }
}
