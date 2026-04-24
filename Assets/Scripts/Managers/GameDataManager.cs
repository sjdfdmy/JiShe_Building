using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDataManager : MonoBehaviour
{
    private static GameDataManager instance;
    public static GameDataManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance=FindObjectOfType<GameDataManager>();
                if (instance == null)
                {
                    Debug.Log("No GameDataManager found!");
                }
            }
            return instance;
        }
    }

    [System.Serializable]
    public class ObjData
    {
        public MaterialData objdata;
        public int num;
    }
    public GameObject cursor;
    public int playermoney;
    public List<MaterialData> materials;
    public List<ObjData> bags;
    [Header("UI引用")]
    public TextMeshProUGUI promptText;      // UI Text组件
    public GameObject promptPanel; // 提示面板
    [Header("���")]
    public Transform player;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        if (cursor != null)
            cursor.SetActive(true);
        for (int i = 0; i < materials.Count; i++)
        {
            var tmp = new ObjData { objdata = materials[i], num = i };//先用i测试
            bags.Add(tmp);
        }
    }

    void Update()
    {

    }
    
    public MaterialData GetMaterialData(int type)
    {
        int allvalue = 0;
        System.Func<MaterialData, int> GetRavity = type switch
        {
            0 => m => m.simvalravity,
            1 => m => m.highvalravity,
            _ => throw new System.ArgumentException("Invalid type")
        };

                foreach(var i in materials)
                {
            allvalue += GetRavity(i);
                }
        int aim = Random.Range(1, allvalue + 1);

                foreach (var i in materials)
                {
                    aim -= GetRavity(i);
                    if (aim <= 0)
                    {
                        return i;
                    }
                }
        Debug.Log("Error!");
        return null;
    }
}
