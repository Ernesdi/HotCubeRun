using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour {
    //获得UI引用
    private GameObject m_StartUI;
    private GameObject m_GameUI;
    //获得start界面标签引用
    private UILabel m_Score_Label;
    private UILabel m_Gem_Label;
    private GameObject m_play_btn;
    //获得game界面标签引用
    private UILabel m_GameGemLabel;
    private UILabel m_GameScoreLabel;

    //为了自动生成角色
    private PlayerController m_PlayerController;

    private GameObject m_Left;
    private GameObject m_Right;

    void Start () {
        //获得UI引用
        m_StartUI = GameObject.Find("Start_UI");
        m_GameUI = GameObject.Find("Game_UI");
        //获得start界面标签组件引用
        m_Score_Label = GameObject.Find("Score_Label").GetComponent<UILabel>();
        m_Gem_Label = GameObject.Find("Gem_Label").GetComponent<UILabel>();
        m_play_btn = GameObject.Find("play_btn");
        //获得game界面标签组件引用    
        m_GameScoreLabel = GameObject.Find("GameScoreLabel").GetComponent<UILabel>();
        m_GameGemLabel = GameObject.Find("GameGemLabel").GetComponent<UILabel>();
        //获得游戏控制脚本引用
        m_PlayerController = GameObject.Find("cube_books").GetComponent<PlayerController>();

        //获取左右侧触摸引用
        m_Left = GameObject.Find("Left");
        m_Right = GameObject.Find("Right");
        //把更新UI的方法添加到多播委托中
        PlayerController.Instance.updateDataDelegate += UpdateData;

        //使用委托绑定点击事件
        UIEventListener.Get(m_play_btn).onClick = PlayButtonClick; 
        UIEventListener.Get(m_Left).onClick = Left;
        UIEventListener.Get(m_Right).onClick = Right;
        //调用方法
        Init();
        //设置游戏界面UI不显示
        m_GameUI.SetActive(false);
    }

    /// <summary>
    /// 初始化开始UI 游戏得分为注册表中最高得分 宝石为注册表中的宝石数
    /// </summary>
    private void Init()
    {
        m_Score_Label.text = PlayerPrefs.GetInt("score", 0)+"";
        m_Gem_Label.text = PlayerPrefs.GetInt("gem", 0) + "/100";
        m_GameScoreLabel.text = "0";
        m_GameGemLabel.text = PlayerPrefs.GetInt("gem", 0) + "/100";
    }

    /// <summary>
    /// 游戏界面分数和宝石更新 开始界面的宝石数
    /// </summary>
    /// <param name="score">分数</param>
    /// <param name="gem">宝石</param>
    private void UpdateData(int score,int gem)
    {
        m_GameScoreLabel.text = score.ToString();
        m_GameGemLabel.text = gem + "/100";
        m_Gem_Label.text = gem + "/100";
    }

    /// <summary>
    /// 开始游戏事件监听
    /// </summary>
    private void PlayButtonClick(GameObject go) {
        Debug.Log("游戏开始");
        m_StartUI.SetActive(false);
        m_GameUI.SetActive(true);
        //生成角色
        m_PlayerController.StartGame();
    }

    //左右键触摸转换
    private void Left(GameObject go) {
        m_PlayerController.Left();
    }
    private void Right(GameObject go)
    {
        m_PlayerController.Right();
    }

    /// <summary>
    /// 游戏结束重置UI
    /// </summary>
    public void ResetUI()
    {
        m_StartUI.SetActive(true);
        m_GameUI.SetActive(false);
        //游戏的分数要清零
        m_GameGemLabel.text = "0";
    }
}
