using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Pool;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField]
    private Character player;
    [SerializeField]
    private int maxEnemyCount = 10;
    
    [SerializeField]
    private float dropRate = 50f;

    [SerializeField]
    private GameObject itemPrefab = null;
    [SerializeField]
    private GameObject enemyPrefab = null;
    [SerializeField]
    private GameObject damageTextPrefab = null;

    [SerializeField]
    private float minimumX = -9.4f;
    [SerializeField]
    private float maximumX = 11.2f;

    private Pool<ItemController> itemPool;
    private Pool<EnemyController> enemyPool;
    private Pool<DamageText> damageTextPool;
    
    private int enemyCount = 0;
    private bool isDead = false;
    private bool initialized = false;

    private PlayerMove playerMove = null;
    private DataManager dataManager = null;

    public Action Dead;
    public PlayerMove PlayerMove
    {
        get
        {
            if(playerMove == null)
            {
                playerMove = FindObjectOfType<PlayerMove>();
            }
            return playerMove;
        }
    }

    public Character PlayerInfo {
        get { return player; }
    }

    public DataManager GetDataManager
    {
        get { return dataManager; }
    }

    public void ChangeHealthValue(int atk)
    {
        player.health += atk;        
    }

    public float GetHealthRate()
    {
        return (float)player.health / player.maxHealth;
    }

    private void DoDead()
    {
        player.health = 0;
        EventManager.TriggerEvent(EventManager.EventName.PLAYER_DAMAGED);
        isDead = true;
        StartCoroutine(DoGameOver());
    }

    private IEnumerator DoGameOver()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("GameOver");
    }

    private void Start()
    {
        if (!initialized) {
            Init();
        }
        
    }

    private void Init()
    {
        itemPool = new Pool<ItemController>(new PrefabFactory<ItemController>(itemPrefab));
        enemyPool = new Pool<EnemyController>(new PrefabFactory<EnemyController>(enemyPrefab));
        damageTextPool = new Pool<DamageText>(new PrefabFactory<DamageText>(damageTextPrefab));
        dataManager = GetComponent<DataManager>();

        SceneManager.LoadScene("UI", LoadSceneMode.Additive);
        ResetPreloadEnemies();
        StartCoroutine(RandomSpawnEnemies());
        Dead = new Action(DoDead);
        EventManager.StartListening(EventManager.EventName.PLAYER_DEAD, Dead);
        initialized = true;
    }

    private void ResetPreloadEnemies()
    {
        EnemyController[] preloadEnemies = FindObjectsOfType<EnemyController>();
        GameObject tempEnemy = null;
        foreach (EnemyController enemy in preloadEnemies)
        {
            enemy.Reset();
            tempEnemy = SpawnEnemy();
            tempEnemy.transform.position = enemy.transform.position;
        }
    }

    private IEnumerator RandomSpawnEnemies()
    {
        GameObject randomEnemy = null;        
        while (!isDead)
        {            
            randomEnemy = SpawnEnemy();
            randomEnemy.transform.position = new Vector2(RandomEnemyPositionX(), 5f);
            yield return new WaitUntil(()=> enemyCount < maxEnemyCount);
            yield return new WaitForSeconds(5f);
        }
    }

    private float RandomEnemyPositionX()
    {
        float result = UnityEngine.Random.Range(0f, 1f) > 0.5f ? 
            PlayerMove.transform.position.x - 2.1f : PlayerMove.transform.position.x + 2.1f;
        if(result < minimumX)
        {
            result = PlayerMove.transform.position.x + 2f;
        }
        if(result > maximumX)
        {
            result = PlayerMove.transform.position.x - 2f;
        }
        
        return result;
    }

    private void GetDogTag()
    {        
        player.dogtag++;
        EventManager.TriggerEvent(EventManager.EventName.GET_DOGTAG);
    }

    public void ShowDamageText(Transform target, int value)
    {
        DamageText damageText = damageTextPool.Allocate();

        EventHandler hide = null;
        hide = (sender, e) =>
        {
            damageText.gameObject.SetActive(false);
            damageTextPool.Release(damageText);
            damageText.Hide -= hide;
        };
        damageText.Hide += hide;
        damageText.gameObject.SetActive(true);
        damageText.transform.position = target.position;
        damageText.Show(value);
    }

    public GameObject SpawnDogtag()
    {
        ItemController newDogtag = itemPool.Allocate();

        EventHandler eventHandler = null;
        eventHandler = (sender, e) => {
            GetDogTag();
            itemPool.Release(newDogtag);
            newDogtag.Collect -= eventHandler;
        };

        newDogtag.Collect += eventHandler;
        newDogtag.gameObject.SetActive(true);

        return newDogtag.gameObject;
    }

    public GameObject SpawnEnemy()
    {
        EnemyController newEnemy = enemyPool.Allocate();
        enemyCount++;

        EventHandler eventHandler = null;
        eventHandler = (sender, e) => {
            enemyCount--;
            GameObject newDogtag = OnDropDogtag();
            if (newDogtag != null)
            {
                newDogtag.transform.position = newEnemy.transform.position;
            }            
            enemyPool.Release(newEnemy);
            newEnemy.Dead -= eventHandler;
        };

        newEnemy.Dead += eventHandler;
        newEnemy.gameObject.SetActive(true);

        return newEnemy.gameObject;
    }
    
    private GameObject OnDropDogtag()
    {
        UnityEngine.Random.InitState(DateTime.UtcNow.Second);
        float randomRate = UnityEngine.Random.Range(0f, 100f);
        if(randomRate <= dropRate)
        {
            return SpawnDogtag();
        }
        else
        {
            return null;
        }
    }

    private void OnDestroy()
    {
        EventManager.StopListening(EventManager.EventName.PLAYER_DEAD, Dead);
    }
}
