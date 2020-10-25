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
    private static GameManager instance;


    public static string EnemyTag => instance.enemyTag;
    public static string PlayerTag => instance.playerTag;
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
