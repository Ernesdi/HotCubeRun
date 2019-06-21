using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 地图管理器
/// </summary>
public class MapManager : MonoBehaviour { 
    //声明地图的地板预制体
    private GameObject m_prefabs_tile;
    //声明地图的墙壁预制体
    private GameObject m_prefabs_wall;
    //声明地图的地面陷阱预制体
    private GameObject m_prefabs_spikes;
    //声明地图的天空陷阱预制体
    private GameObject m_prefabs_sky_spikes;
    //声明地图的奖励物体预制体
    private GameObject m_prefabs_gem;
    //声明一个可以存储地图信息的List泛型集合 集合规定只能存储游戏物体数组；
    public List<GameObject[]> mapList = new List<GameObject[]>();

    //声明此脚本依附的物体的Transfrom组件
    private Transform m_Transform;
    //Mathf.Sqrt函数求三角形的腰长 
    public float bottonLength = Mathf.Sqrt(2) * 0.254f;
    //单排的地板颜色
    private Color colorOne = new Color(124/255f, 155 / 255f, 230 / 255f);
    //双排的地板颜色
    private Color colorTwo = new Color(125 / 255f, 169 / 225f, 233 / 255f);
    //墙壁的颜色
    private Color colorWall = new Color(87 / 255f, 93 / 255f, 190 / 255f);
    //当前需要塌陷的地板
    private int index = 0;

    //声明PlayerController的引用
    private PlayerController m_PlayerController = new PlayerController();

    //空洞，地面陷阱，天空陷阱 的概率
    private int pr_hole = 0;
    private int pr_spikes = 0;
    private int pr_sky_spikes = 0;
    private int pr_gem = 2;

    void Start () {
        //读取地板资源文件并转化为GameObject类型
        m_prefabs_tile = Resources.Load("tile_white") as GameObject;
        //读取墙壁资源文件并转化为GameObject类型
        m_prefabs_wall = Resources.Load("wall2") as GameObject;
        //读取地面陷阱资源文件并转化为GameObject类型
        m_prefabs_spikes = Resources.Load("moving_spikes") as GameObject;
        //读取天空陷阱资源文件并转化为GameObject类型
        m_prefabs_sky_spikes = Resources.Load("smashing_spikes") as GameObject;
        //读取奖励物体资源文件并转化为GameObject类型
        m_prefabs_gem = Resources.Load("gem 2") as GameObject;

        //获得此脚本依附的物体的Transfrom组件的引用
        m_Transform = gameObject.GetComponent<Transform>();
        //获得PlayerController的引用
        m_PlayerController = GameObject.Find("cube_books").GetComponent<PlayerController>();
        //调用生成菱形地板的方法
        CreateMapItem(0);
    }

/// <summary>
/// 动态生成地图
/// </summary>
/// <param name="offSetZ">偏移量 生成地图然后拼接在一起</param>
    public void CreateMapItem(float offSetZ) {
        for (int i = 0; i < 10; i++)
        {
            GameObject[] item = new GameObject[6];
            for (int j = 0; j < 6; j++)
            {
                //菱形地板的生成位置 Mathf.Sqrt函数求三角形的腰长       offSetZ新地图的偏移量
                Vector3 pos = new Vector3(j* bottonLength, 0,i * bottonLength+ offSetZ);
                //菱形地板的生成角度  
                Vector3 rot = new Vector3(-90, 45, 0);
                GameObject tile = null;
                if (j==0||j==5)
                {
                    //Euler欧拉角转换成四元数生成墙壁
                    tile = GameObject.Instantiate(m_prefabs_wall, pos, Quaternion.Euler(rot)) as GameObject;
                    //墙壁的颜色使用
                    tile.GetComponent<MeshRenderer>().material.color = colorWall;
                }
                else
                {
                    //调用方法存储返回值看到底生成什么物体
                   int pr=  CalcPR();
                    if (pr == 0)//生成地板
                    {  //Euler欧拉角转换成四元数生成地板
                        tile = GameObject.Instantiate(m_prefabs_tile, pos, Quaternion.Euler(rot)) as GameObject;
                        //更改地板的表面颜色
                        tile.GetComponent<Transform>().FindChild("normal_a2").GetComponent<MeshRenderer>().material.color = colorOne;
                        //更改地板的侧边颜色
                        tile.GetComponent<MeshRenderer>().material.color = colorOne;
                        //生成宝石 CalcGemPR（）返回一个值
                        int gem_pr = CalcGemPR();
                        if (gem_pr == 1)
                        {//生成宝石在地板上方 然后设置宝石的父物体跟着一起掉落
                            GameObject gem = GameObject.Instantiate(m_prefabs_gem, tile.GetComponent<Transform>().position + new Vector3(0, 0.06f, 0), Quaternion.identity) as GameObject;
                            gem.GetComponent<Transform>().SetParent(tile.GetComponent<Transform>());
                        }
                    }
                    else if (pr == 1)//生成空洞，生成空物体 具有Transform组件
                    {
                        tile = new GameObject();//生成空物体
                        tile.GetComponent<Transform>().position = pos;//空物体的位置
                        tile.GetComponent<Transform>().rotation = Quaternion.Euler(rot);//空物体的旋转情况
                    }
                    else if (pr == 2)//生成地面陷阱
                    {
                        tile = GameObject.Instantiate(m_prefabs_spikes, pos, Quaternion.Euler(rot)) as GameObject;
                    }
                    else if (pr == 3)//生成天空陷阱
                    {
                        tile = GameObject.Instantiate(m_prefabs_sky_spikes, pos, Quaternion.Euler(rot)) as GameObject;
                    }


                }
              

                //设置实例化出来的物体的父物体
                tile.GetComponent<Transform>().SetParent(m_Transform);
                item[j]=tile;
            }
            mapList.Add(item);
            GameObject[] item2 = new GameObject[5];
            for (int j = 0; j < 5; j++)
            {                 //    第二排的位置 往前走一个等腰三角形的腰长再加上半个腰长    往右也是一样        offSetZ新地图的偏移量    
                    Vector3 pos = new Vector3(j * bottonLength + bottonLength / 2, 0, i * bottonLength + bottonLength / 2+ offSetZ);
                    //需要旋转一下角度
                    Vector3 rot = new Vector3(-90, 45, 0);
                    GameObject tile = null;
                //调用方法存储返回值看到底生成什么物体
                int pr = CalcPR();
                if (pr == 0)//生成地板
                {  //Euler欧拉角转换成四元数生成地板
                    tile = GameObject.Instantiate(m_prefabs_tile, pos, Quaternion.Euler(rot)) as GameObject;
                    //更改地板的表面颜色
                    tile.GetComponent<Transform>().FindChild("normal_a2").GetComponent<MeshRenderer>().material.color = colorTwo;
                    //更改地板的侧边颜色
                    tile.GetComponent<MeshRenderer>().material.color = colorTwo;
                }
                else if (pr == 1)//生成空洞，生成空物体 具有Transform组件
                {
                    tile = new GameObject();//生成空物体
                    tile.GetComponent<Transform>().position = pos;//空物体的位置
                    tile.GetComponent<Transform>().rotation = Quaternion.Euler(rot);//空物体的旋转情况
                }
                else if (pr == 2)//生成地面陷阱
                {
                    tile = GameObject.Instantiate(m_prefabs_spikes, pos, Quaternion.Euler(rot)) as GameObject;
                }
                else if (pr == 3)//生成天空陷阱
                {
                    tile = GameObject.Instantiate(m_prefabs_sky_spikes, pos, Quaternion.Euler(rot)) as GameObject;
                }

                tile.GetComponent<Transform>().SetParent(m_Transform);
                item2[j] = tile;
                }
            mapList.Add(item2);
        }
    }
	
	//检查mapList
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            string temporary = "";
            //双层for循环遍历数组，外层循环List
            for (int i = 0; i < mapList.Count; i++)
            {
                //内层循环数组
                for (int j = 0; j < mapList[i].Length; j++)
                {
                    //取出元素名并添加到temporary
                    temporary += mapList[i][j].name;
                    mapList[i][j].name=i+"--"+j;
                }
                //每循环完一次就换行
                temporary += "\n";
            }
            Debug.Log(temporary);
        }
	}
    /// <summary>
    /// 开启协程的公开方法
    /// </summary>
    public void StartTileDown() {
        StartCoroutine("TileDown");
    }
    /// <summary>
    /// 关闭协程的公开方法
    /// </summary>
    public void StopTileDown()
    {
        StopCoroutine("TileDown");
    }


    /// <summary>
    /// 地面塌陷协程
    /// </summary>
    /// <returns></returns>
    private IEnumerator TileDown() {
        while (true) {
            //等待0.5秒执行
            yield return new WaitForSeconds(0.2f);
            //给List指定索引的数组的各个物体添加刚体组件模仿掉落
            for (int i = 0; i < mapList[index].Length; i++)
            {
                //添加刚体组件并获得引用
                Rigidbody m_Rigidbody= mapList[index][i].AddComponent<Rigidbody>();
                //随机的一个角度
                m_Rigidbody.angularVelocity =new Vector3(Random.Range(0.0f,1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f)) *Random.Range(1,10);
                //添加了刚体组件之后1.5秒后销毁物体
                Destroy(mapList[index][i].gameObject,1.5f);
            }
            //如果塌陷到达了角色的位置
            if (m_PlayerController.Z==index)
            {
                //地面停止塌陷
                StopTileDown();
                //让物体掉落
                m_PlayerController.gameObject.AddComponent<Rigidbody>();
                //调用游戏结束协程  
                m_PlayerController.StartCoroutine("GameOver", true);
            }

            //第一行塌陷完毕到第二行
            index++;
        }
    }

    /// <summary>
    /// 计算概率
    /// 0:普通地板
    /// 1:坑洞
    /// 2:地面陷阱
    /// 3:天空陷阱
    /// </summary>
    private int CalcPR() {
        //随机生成1-100的数 百分比：
        int pr=Random.Range(1, 100);
        if (pr<=pr_hole)
        {          
            return 1;
        }
        else if (pr>31&&pr<pr_spikes+30)
        {
            return 2;
        }
        else if (pr > 61 && pr < pr_sky_spikes + 60)
        {
            return 3;
        }

        return 0;
    }

    /// <summary>
    /// 计算宝石生成概率
    /// </summary>
    /// <returns>0不生成 1生成</returns>
    private int CalcGemPR() {
        int pr = Random.Range(1,100);
        if (pr<= pr_gem)
        {
            return 1;
        }
        return 0;
    }
    /// <summary>
    /// 增加概率
    /// </summary>
    public void AddPR() {
        //每段地图空洞概率+2
        pr_hole += 2;
        //每段地图地面陷阱概率+1
        pr_spikes += 1;
        //每段地图天空陷阱概率+1
        pr_sky_spikes += 1;
    }

    /// <summary>
    /// 游戏结束重置地图
    /// </summary>
    public void ResetGameMap()
    {
        //地图删除子物体
        Transform[] sonTransform = m_Transform.GetComponentsInChildren<Transform>();
        //I的初始值不能为0  为0会把父物体也删除掉
        for (int i = 1; i < sonTransform.Length; i++)
        {//移除所以的子物体
            GameObject.Destroy(sonTransform[i].gameObject);
        }

        //空洞，地面陷阱，天空陷阱 的概率
        pr_hole = 0;
        pr_spikes = 0;
        pr_sky_spikes = 0;
        pr_gem = 2;

        //重置塌陷角标
        index = 0;

        //地图清空
        mapList.Clear();

        //重新生成地图
        CreateMapItem(0);

    }
}
