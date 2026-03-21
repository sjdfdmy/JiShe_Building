using UnityEngine;
using UnityEngine.UI;

public class PlayerSet : MonoBehaviour
{
    [Header("设置界面")]
    public GameObject sets;
    [Header("是否返回主页面子物体")]
    public GameObject ifbackhome;
    [Header("打开音量图标")]
    public Sprite openvoice;
    [Header("静音图标")]
    public Sprite closevoice;

    [Header("总音量")]
    [Range(0, 100)]
    public float totalvolumn;
    public bool istotalvolumn;
    [Header("音量")]
    [Range(0, 100)]
    public float volumn;
    public bool isvolumn;
    [Header("音效")]
    [Range(0, 100)]
    public float voiceeffect;
    public bool isvoiceeffect;

    public Button yesbackhome;//确认返回主页面按钮
    public Button setbtn;//打开设置按钮
    private Slider totalvolumnslider;//总音量滑动条
    private Slider volumnslider;//音量滑动条
    private Slider voiceeffectslider;//音效滑动条
    private Button istotalvolumnbutton;//是否关闭总音量按钮
    private Button isvolumnbutton;//是否关闭音量按钮
    private Button isvoiceeffectbutton;//是否关闭音效按钮

    private static PlayerSet instance;
    public static PlayerSet Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PlayerSet>();
                if (instance == null)
                {
                    Debug.Log("No PlayerSet");
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        yesbackhome.onClick.AddListener(() => {
            Scenemanager.Instance.LoadScene(Scenemanager.Scenes.Start);
            ifbackhome.SetActive(false);
            sets.SetActive(false);
        });
    }

    void Start()
    {
        totalvolumnslider = sets.transform.GetChild(2).GetChild(0).GetComponent<Slider>();
        volumnslider = sets.transform.GetChild(2).GetChild(1).GetComponent<Slider>();
        voiceeffectslider = sets.transform.GetChild(2).GetChild(2).GetComponent<Slider>();
        istotalvolumnbutton = sets.transform.GetChild(2).GetChild(3).GetComponent<Button>();
        isvolumnbutton = sets.transform.GetChild(2).GetChild(4).GetComponent<Button>();
        isvoiceeffectbutton = sets.transform.GetChild(2).GetChild(5).GetComponent<Button>();

        sets.SetActive(false);
        ifbackhome.SetActive(false);

        totalvolumn = PlayerPrefs.GetFloat("TotalVolumn", 100f);
        volumn = PlayerPrefs.GetFloat("Volumn", 100f);
        voiceeffect = PlayerPrefs.GetFloat("VoiceEffect", 100f);
        istotalvolumn = PlayerPrefs.GetInt("IsTotalVolumn", 1) == 1;
        isvolumn = PlayerPrefs.GetInt("IsVolumn", 1) == 1;
        isvoiceeffect = PlayerPrefs.GetInt("IsVoiceEffect", 1) == 1;

        totalvolumnslider.value = istotalvolumn ? totalvolumn : 0;
        volumnslider.value = isvolumn ? volumn : 0;
        voiceeffectslider.value = isvoiceeffect ? voiceeffect : 0;

        setbtn.onClick.RemoveAllListeners();
        setbtn.onClick.AddListener(() =>
        {
            sets.SetActive(true);
            Time.timeScale = 0;
        });
    }


    void Update()
    {
        if (setbtn == null)
        {
            if (GameObject.Find("SetBtn") != null)
            {
                setbtn = GameObject.Find("SetBtn").GetComponent<Button>();
                setbtn.onClick.RemoveAllListeners();
                setbtn.onClick.AddListener(() =>
                {
                    sets.SetActive(true);
                    Time.timeScale = 0;
                });
            }
            else
            {
                Debug.Log("No setbtn");
            }
        }

        if (totalvolumnslider.value != totalvolumn)
        {
            totalvolumn = totalvolumnslider.value;
            istotalvolumn = totalvolumn > 0;
            PlayerPrefs.DeleteKey("IsTotalVolumn");
            PlayerPrefs.SetInt("IsTotalVolumn", istotalvolumn?1:0);
            PlayerPrefs.Save();

            if (istotalvolumn)
            {
                PlayerPrefs.DeleteKey("TotalVolumn");
                PlayerPrefs.SetFloat("TotalVolumn", totalvolumn);
                PlayerPrefs.Save();
            }
        }

        if(volumnslider.value != volumn)
        {
            volumn = volumnslider.value;
            isvolumn = volumn > 0;
            PlayerPrefs.DeleteKey("IsVolumn");
            PlayerPrefs.SetInt("IsVolumn", isvolumn?1:0);
            PlayerPrefs.Save();

            if (isvolumn)
            {
                PlayerPrefs.DeleteKey("Volumn");
                PlayerPrefs.SetFloat("Volumn", volumn);
                PlayerPrefs.Save();
            }
        }

        if(voiceeffectslider.value != voiceeffect)
        {
            voiceeffect = voiceeffectslider.value;
            isvoiceeffect = voiceeffect > 0;
            PlayerPrefs.DeleteKey("IsVoiceEffect");
            PlayerPrefs.SetInt("IsVoiceEffect", isvoiceeffect?1:0);
            PlayerPrefs.Save();

            if (isvoiceeffect)
            {
                PlayerPrefs.DeleteKey("VoiceEffect");
                PlayerPrefs.SetFloat("VoiceEffect", voiceeffect);
                PlayerPrefs.Save();
            }
        }

        istotalvolumnbutton.image.sprite = istotalvolumn?openvoice:closevoice;
        isvolumnbutton.image.sprite = isvolumn ? openvoice : closevoice;
        isvoiceeffectbutton.image.sprite = isvoiceeffect ? openvoice : closevoice;
    }

    public void ClickTotal()
    {
        istotalvolumn=!istotalvolumn;
        totalvolumnslider.value = istotalvolumn ? PlayerPrefs.GetFloat("TotalVolumn",100) : 0;
        if (totalvolumnslider.value <1&&istotalvolumn)
        {
            totalvolumnslider.value = 1;
        }
    }

    public void ClickVolumn()
    {
        isvolumn = !isvolumn;
        volumnslider.value = isvolumn ? PlayerPrefs.GetFloat("Volumn", 100) : 0;
        if (volumnslider.value <1 && isvolumn)
        {
            volumnslider.value = 1;
        }
    }

    public void ClickVoiceEffect()
    {
        isvoiceeffect = !isvoiceeffect;
        voiceeffectslider.value = isvoiceeffect ? PlayerPrefs.GetFloat("VoiceEffect", 100) : 0;
        if (voiceeffectslider.value <1 && isvoiceeffect)
        {
            voiceeffectslider.value = 1;
        }
    }

    public void CloseSet()
    {
        sets.SetActive(false);
        Time.timeScale = 1;
    }
}
