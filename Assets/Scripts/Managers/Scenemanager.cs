using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenemanager : MonoBehaviour
{

    private static Scenemanager instance;
    public static Scenemanager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Scenemanager>();
                if (instance == null)
                {
                    Debug.Log("No Scenemanager");
                }
            }
            return instance;
        }
    }

    public enum Scenes
    {
        Start = 0,
        Game=1
    }

    [Header("当前场景")]
    public Scenes nowscene;
    [Header("切换场景过渡遮罩")]
    public GameObject sceneshader;
    [Header("淡入时间")]
    public float fadeintime;
    [Header("中间态过渡时间")]
    public float fadetime;
    [Header("淡出时间")]
    public float fadeouttime;

    private void Awake()
    {

    }

    void Start()
    {
        sceneshader.SetActive(false);
        sceneshader.GetComponent<CanvasGroup>().alpha = 0;
        nowscene=(Scenes)SceneManager.GetActiveScene().buildIndex;
    }

    void Update()
    {
        
    }

    public void LoadScene(Scenes scene)
    {
        sceneshader.SetActive(true);
        Time.timeScale = 1;
        sceneshader.GetComponent<CanvasGroup>().alpha = 0;
        //PlayerSet.Instance.setbtn = null;
        StartCoroutine(SceneShaderFade(scene));
    }

    IEnumerator SceneShaderFade(Scenes scene)
    {
        CanvasGroup canvasGroup = sceneshader.GetComponent<CanvasGroup>();
        float time = 0;
        while (canvasGroup.alpha < 1)
        {
            time+= Time.deltaTime;
            canvasGroup.alpha=Mathf.Lerp(0,1, time / fadeintime);
            yield return null;
        }
        canvasGroup.alpha = 1;


        SceneManager.LoadScene((int)scene);
        nowscene = scene;
        //PlayerSet.Instance.ifbackhome.SetActive(false);
        //PlayerSet.Instance.sets.SetActive(false);
        yield return new WaitForSeconds(fadetime);

        time = 0;
        while (canvasGroup.alpha >0)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1,0, time / fadeouttime);
            yield return null;
        }
        canvasGroup.alpha = 0;
        sceneshader.SetActive(false);

        yield break;
    }
}
