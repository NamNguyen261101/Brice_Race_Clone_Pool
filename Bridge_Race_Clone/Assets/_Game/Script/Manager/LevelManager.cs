using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LevelManager : Singleton<LevelManager>
{
    readonly List<ColorType> colorTypes = new List<ColorType>() {  ColorType.Black, ColorType.Red, ColorType.Blue, ColorType.Green, ColorType.Yellow, ColorType.Orange, ColorType.Brown, ColorType.Violet };


    public Level[] levelPrefabs;
    public Bot botPrefab;
    public Player player;
    public Vector3 FinishPoint => currentLevel.finishPoint.position;

    public int CharacterAmount => currentLevel.botAmount + 1;

    private List<Bot> bots = new List<Bot>();
    private Level currentLevel;

    private int levelIndex;

    private void Awake()
    {
        levelIndex = PlayerPrefs.GetInt("Level", 0);
    }

    private void Start()
    {
        LoadLevel(levelIndex);
        OnInit();
        UIManager.Instance.OpenUI<MainMenu>();
    }

    public void OnInit()
    {
        //init vi tri bat dau game
        Vector3 index = currentLevel.startPoint.position;
        float space = 2f;
        Vector3 leftPoint = ((CharacterAmount / 2) + (CharacterAmount % 2) * 0.5f - 0.5f) * space * Vector3.left + index;

        List<Vector3> startPoints = new List<Vector3>();

        for (int i = 0; i < CharacterAmount; i++)
        {
            startPoints.Add(leftPoint + space * Vector3.right * i);
        }

        //update navmesh data
        NavMesh.RemoveAllNavMeshData();
        NavMesh.AddNavMeshData(currentLevel.navMeshData);

        //init random mau
        List<ColorType> colorDatas = Utilities.SortOrder(colorTypes, CharacterAmount);
        //

        //set vi tri player
        int rand = Random.Range(0, CharacterAmount);
        player.TF.position = startPoints[rand];
        player.TF.rotation = Quaternion.identity;
        startPoints.RemoveAt(rand);
        //set color player
        player.ChangeColor(colorDatas[rand]);
        colorDatas.RemoveAt(rand);

        player.OnInit();
        for (int i = 0; i < CharacterAmount - 1; i++)
        {
            //Bot bot = SimplePool.Spawn<Bot>(botPrefab, startPoints[i], Quaternion.identity);
            Bot bot = SimplePool.Spawn<Bot>(PoolType.Bot, startPoints[i], Quaternion.identity);
            bot.ChangeColor(colorDatas[i]);
            bot.OnInit();
            bots.Add(bot);
        }
    }

    public void LoadLevel(int level)
    {
        if (currentLevel != null)
        {
            Destroy(currentLevel.gameObject);
        }

        if (level < levelPrefabs.Length)
        {
            currentLevel = Instantiate(levelPrefabs[level]);
            currentLevel.OnInit();
        }
        else
        {
            //TODO: level vuot qua limit
        }
    }

    public void OnStartGame()
    {
        GameManager.Instance.ChangeState(GameState.Gameplay);
        for (int i = 0; i < bots.Count; i++)
        {
            bots[i].ChangeState(new PatrolState());
        }
    }

    public void OnFinishGame()
    {
        for (int i = 0; i < bots.Count; i++)
        {
            bots[i].ChangeState(null);
            bots[i].MoveStop();
        }
    }

    public void OnReset()
    {
        SimplePool.CollectAll();
        bots.Clear();
    }

    internal void OnRetry()
    {
        OnReset();
        LoadLevel(levelIndex);
        OnInit();
        UIManager.Instance.OpenUI<MainMenu>();
    }

    internal void OnNextLevel()
    {
        levelIndex++;
        PlayerPrefs.SetInt("Level", levelIndex);
        OnReset();
        LoadLevel(levelIndex);
        OnInit();
        UIManager.Instance.OpenUI<MainMenu>();
    }
}
