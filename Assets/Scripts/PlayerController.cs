using UnityEngine;
using System.Collections;
//声明一个委托
public delegate void UpdateDataDelegate(int score, int gem);

/// <summary>
/// 玩家控制
/// </summary>
public class PlayerController : MonoBehaviour {
    //声明想要获得mapList数组中哪一个物体的位置？角标从0开始
    private int z=3;
    private int x=2;

    //s声明主角位置的变量
    private Transform m_Transform;
    //两种蜗牛痕迹的颜色
    private Color color_One = new Color(122/255f,85/255f,179/255f);
    private Color color_Two = new Color(126 / 255f, 93 / 255f, 183 / 255f);
    //想要控制MapManager的信息
    private MapManager m_MapManager;
    //声明摄像机的引用
    private CameraFollow m_CameraFollow;
    //声明UI界面的引用
    private UIManager m_UIManager;

    //角色是否还生存
    private bool lift = true;
    //获得的宝石数量
    private int gemCount = 0;
    //获得分数
    private int score = 0;
    //宝石增加方法
    private void AddGemCount() {
        gemCount++;
        Debug.Log("宝石数"+gemCount);
        updateDataDelegate(score,gemCount);
    }
    //单例设计模式
    public static PlayerController Instance;
    //事件
    public event UpdateDataDelegate updateDataDelegate;

    private void AddScore() {
        score++;
        Debug.Log("当前分数" + score);
        updateDataDelegate(score, gemCount);
    }
    //封装当前角色的Z位置
    public int Z {
        get { return z; }
        set { z = value; }
    }
    //存储资料
    private void SaveData() {
        PlayerPrefs.SetInt("gem",gemCount);
        if (score>PlayerPrefs.GetInt("score", 0))
        {
            PlayerPrefs.SetInt("score", score);
        }
           
    }
    private void Awake()
    {//单例设计模式
        Instance = this;
    }

    void Start () {
        //调用以前的资料
        gemCount= PlayerPrefs.GetInt("gem",0);
        //获得引用
        m_Transform = gameObject.GetComponent<Transform>();
        m_MapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        m_CameraFollow = GameObject.Find("Main Camera").GetComponent<CameraFollow>();
        m_UIManager = GameObject.Find("UI Root").GetComponent<UIManager>();
    }

    public void StartGame() {
        //设置主角位置
        SetPlayerPos();
        //设置摄像机跟随
        m_CameraFollow.StartFollow = true;
        //开启MapManager类的协程方法
        m_MapManager.StartTileDown();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.M))
        {
            StartGame();
        }
        //如果角色还生存才可以控制
        if (lift)
        {
            //调用控制方法
            PlayerControl();
        }       
    }
    /// <summary>
    /// 按下左键之后执行的代码
    /// </summary>
    public void Left() {
        if (lift)
        {
            //控制左边边界  如果左边编辑的x的角标为0 就不让前进了
            if (x != 0)
            {
                //先往前移动一个索引值
                z++;
                //分数增加
                AddScore();
            }

            if (z % 2 == 1 && x != 0)//如果是奇数行（且没有越界）我们就让X的索引值减一
            {
                //再向左移动
                x--;
            }

            Debug.Log("left" + "z:" + z + " x:" + x);
            //设置主角位置和蜗牛痕迹
            SetPlayerPos();
            //检查是否快到达边界
            CalcPosition();
        }
       
    }

    /// <summary>
    /// 按下右键之后执行的代码
    /// </summary>
    public void Right() {
        if (lift)
        {
            //如果x等于4和z不是偶数 就不让前进（反过来写就是x不是4的时候，z是偶数的两种情况就让他前进）
            if (x != 4 || z % 2 != 1)
            {
                //先往前移动一个索引值
                z++;
                //分数增加
                AddScore();
            }


            if (z % 2 == 0 && x != 4)//如果是偶数行（且没有越界）我们就让X的索引值加一 
            {
                //再向右移动
                x++;
            }
            Debug.Log("Right" + "z:" + z + " x:" + x);
            //设置主角位置和蜗牛痕迹
            SetPlayerPos();
            //检查是否快到达边界
            CalcPosition();
        }

    }


    /// <summary>
    /// 角色控制
    /// </summary>
    private void PlayerControl() {
        //左移动
        if (Input.GetKeyDown(KeyCode.A))
        {
            Left();
        }
        //右移动
        if (Input.GetKeyDown(KeyCode.D))
        {
            Right();
        }
    }

    /// <summary>
    /// 设置角色的位置和蜗牛痕迹
    /// </summary>
    private void SetPlayerPos() {
        //获得z和x的位置
        Transform playPos = m_MapManager.mapList[z][x].GetComponent<Transform>();
        //获得子物体的MeshRenderer组件以形成蜗牛痕迹
        MeshRenderer normal_a2 = null;
        //主角的位置在mapList集合中的第三个索引的第二个索引的位置 ，然后加上一个偏移量
        m_Transform.position = playPos.position + new Vector3(0, 0.254f / 2, 0);
        //旋转角度一致
        m_Transform.rotation = playPos.rotation;
        //如果标签是Tile地板
        if (playPos.tag=="Tile")
        { //如果标签是Tile地板获得子物体的渲染组件
            normal_a2 = playPos.FindChild("normal_a2").GetComponent<MeshRenderer>();
        } //如果标签是Spikes地面陷阱
        else if (playPos.tag == "Spikes")
        {//如果标签是Spikes地面陷阱获得子物体的渲染组件
            normal_a2 = playPos.FindChild("moving_spikes_a2").GetComponent<MeshRenderer>();
        } //如果标签是Sky_Spikes天空陷阱
        else if (playPos.tag == "Sky_Spikes")
        {//如果标签是Sky_Spikes天空陷阱获得子物体的渲染组件
            normal_a2 = playPos.FindChild("smashing_spikes_a2").GetComponent<MeshRenderer>();
        }//如果标签还是找不到就证明是空洞 让主角掉落
        if (normal_a2!=null)
        {
            //偶数的蜗牛痕迹深色一点
            if (z % 2 == 0)
            {
                normal_a2.material.color = color_One;
            }
            //奇数的蜗牛痕迹浅色一点
            else
            {
                normal_a2.material.color = color_Two;
            }
        }
        else
        {
            gameObject.AddComponent<Rigidbody>();
            StartCoroutine("GameOver",true);
        }
 
    }

    private void CalcPosition() {
        //地图还剩12排可以走的时候
        if (m_MapManager.mapList.Count-z<=12) {
            //在地图生成之前增加陷阱概率
            m_MapManager.AddPR();
            Debug.Log("动态生成地图");
            //计算要偏移多少的Z值 获得之前地图的最后一排的第一个的Z值再加上底边的2分之一 就是新地图生成位置的起点
            float offSetZ = m_MapManager.mapList[m_MapManager.mapList.Count - 1][0].GetComponent<Transform>().position.z+m_MapManager.bottonLength/2;
            //调用动态生成地图的方法
            m_MapManager.CreateMapItem(offSetZ);
        }
    }
    /// <summary>
    /// 和天空地面陷阱进行触发 
    /// </summary>
    /// <param name="other">与主角进行碰撞的物体</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Spikes_Attack")
        {
            StartCoroutine("GameOver",false);
        }
        if (other.tag == "Gem")
        {
            //增加宝石数
            AddGemCount();
            //删除宝石物体
            Destroy(other.gameObject.GetComponent<Transform>().parent.gameObject); 
        }
    }

    /// <summary>
    /// 游戏结束协程
    /// </summary>
    /// <param name="b">是否延时0.5秒？</param>
    /// <returns></returns>
    public IEnumerator GameOver(bool b) {
        if (b)
        { //如果B是ture等待0.5秒
            yield return new WaitForSeconds(0.5f);
        }
        if (lift)
        {
            Debug.Log("游戏结束");
            m_CameraFollow.StartFollow = false;
            //角色死亡
            lift = false;
            //存储游戏数据
            SaveData();
            //时间暂停
            // Time.timeScale = 0;  
            StartCoroutine("ReseatGame");
        }

      
    }
    /// <summary>
    /// 重置玩家状态
    /// </summary>
    private void ResetPlayer()
    {
        GameObject.Destroy(gameObject.GetComponent<Rigidbody>());

        z = 3;
        x = 2;

        lift = true;

        score = 0;

    }

    /// <summary>
    /// 重置游戏
    /// </summary>
    /// <returns>等待2秒</returns>
    private IEnumerator ReseatGame() {
        yield return new WaitForSeconds(2);
        //角色重置
        ResetPlayer();
        //地图重置
        m_MapManager.ResetGameMap();
        //重置摄像机
        m_CameraFollow.ResetCamera();
        //重置UI
        m_UIManager.ResetUI();
    }

}
