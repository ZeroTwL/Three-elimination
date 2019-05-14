using Assets.Scripts.Enum;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 游戏管理类
/// </summary>
public class GameManager : SerializedMonoBehaviour
{
    /// <summary>
    /// 单例模式
    /// </summary>
    public static GameManager Instance { get; set; }
    /// <summary>
    /// 列
    /// </summary>
    public int xCol;
    /// <summary>
    /// 行
    /// </summary>
    public int yRow;
    /// <summary>
    /// 格子预制体
    /// </summary>
    public GameObject gridPrefab;
    /// <summary>
    /// 预制体字典
    /// </summary>
    public Dictionary<SweetsType, GameObject> prefabDic = new Dictionary<SweetsType, GameObject>();
    /// <summary>
    /// 填充时间
    /// </summary>
    public float fillTime;

    /// <summary>
    /// 已实例化的甜品二维数组
    /// </summary>
    [HideInInspector]
    public GameSweet[,] sweetArr;
    /// <summary>
    /// 鼠标点击的甜品
    /// </summary>
    [HideInInspector]
    public GameSweet clickSweet;
    /// <summary>
    /// 鼠标进入的甜品
    /// </summary>
    [HideInInspector]
    public GameSweet enterSweet;
    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Init();
    }
    void Update()
    {

    }
    /// <summary>
    /// 初始化
    /// </summary>
    void Init()
    {
        //初始化小网格
        for (int x = 0; x < xCol; x++)
        {
            for (int y = 0; y < yRow; y++)
            {
                //在当前x、y位置实例化小格子
                GameObject grid = Instantiate(gridPrefab, CorrectPos(x, y), Quaternion.identity);
                //设置父对象
                grid.transform.SetParent(transform);
            }
        }
        //根据行列实例化二维数组
        sweetArr = new GameSweet[xCol, yRow];
        //初始化甜品
        for (int x = 0; x < xCol; x++)
        {
            for (int y = 0; y < yRow; y++)
            {
                CreateNewSweet(x, y, SweetsType.Empty);
            }
        }
        //实例化障碍甜品
        Destroy(sweetArr[2, 2].gameObject);
        CreateNewSweet(2, 2, SweetsType.Barrier);
        Destroy(sweetArr[4, 4].gameObject);
        CreateNewSweet(4, 4, SweetsType.Barrier);
        Destroy(sweetArr[7, 7].gameObject);
        CreateNewSweet(7, 7, SweetsType.Barrier);
        StartCoroutine(AllFill());
    }
    /// <summary>
    /// 创建新甜品
    /// </summary>
    /// <param name="x">x坐标</param>
    /// <param name="y">y坐标</param>
    /// <param name="type">类型</param>
    /// <returns></returns>
    public GameSweet CreateNewSweet(int x, int y, SweetsType type)
    {
        //在当前x、y位置实例化甜品
        GameObject newSweet = Instantiate(prefabDic[type], CorrectPos(x, y), Quaternion.identity);
        //设置父对象
        newSweet.transform.SetParent(transform);
        //获取脚本并初始化属性
        sweetArr[x, y] = newSweet.GetComponent<GameSweet>(); ;
        sweetArr[x, y].Init(x, y, this, type);

        return sweetArr[x, y];
    }
    /// <summary>
    /// 全部填充
    /// </summary>
    public IEnumerator AllFill()
    {
        bool needRefill = true;

        while (needRefill)
        {
            yield return new WaitForSeconds(fillTime);
            while (Fill())
            {
                yield return new WaitForSeconds(fillTime);
            }

            //清除所有我们已经匹配好的甜品
            needRefill = ClearAllMatchedSweet();
        }
    }
    /// <summary>
    /// 分步填充
    /// </summary>
    public bool Fill()
    {
        //判断本次填充是否完成
        bool filledNotFinished = false;
        for (int y = yRow - 2; y >= 0; y--)
        {
            for (int x = 0; x < xCol; x++)
            {
                GameSweet sweet = sweetArr[x, y];
                //判断是否可以移动，如果不能移动则无法向下填充
                if (sweet.CanMove() == true)
                {
                    //获取当前甜品下面的甜品
                    GameSweet sweetBelow = sweetArr[x, y + 1];
                    //垂直填充
                    if (sweetBelow.Type == SweetsType.Empty)
                    {
                        Destroy(sweetBelow.gameObject);
                        //移动当前甜品
                        sweet.moveSweet.Move(x, y + 1, fillTime);
                        sweetArr[x, y + 1] = sweet;
                        //在当前位置创建甜品
                        CreateNewSweet(x, y, SweetsType.Empty);
                        filledNotFinished = true;
                    }
                    //斜向填充
                    else
                    {
                        //-1 向左 1向右 0垂直
                        for (int down = -1; down <= 1; down++)
                        {
                            if (down != 0)
                            {
                                //新的x坐标
                                int downX = x + down;
                                if (downX >= 0 && downX < xCol)
                                {
                                    //获取游戏对象
                                    GameSweet downSweet = sweetArr[downX, y + 1];
                                    if (downSweet.Type == SweetsType.Empty)
                                    {
                                        //是否可以移动标识
                                        bool canFill = true;
                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            //获取游戏对象
                                            GameSweet aboveSweet = sweetArr[downX, aboveY];
                                            if (aboveSweet.CanMove() == true)
                                            {
                                                break;
                                            }
                                            else if (aboveSweet.CanMove() == false && aboveSweet.Type != SweetsType.Empty)
                                            {
                                                canFill = false;
                                                break;
                                            }
                                        }
                                        if (!canFill)
                                        {
                                            Destroy(downSweet.gameObject);
                                            sweet.moveSweet.Move(downX, y + 1, fillTime);
                                            sweetArr[downX, y + 1] = sweet;
                                            CreateNewSweet(x, y, SweetsType.Empty);
                                            filledNotFinished = true;
                                            break;
                                        }
                                    }

                                }
                            }
                        }
                    }
                }
            }
        }
        //最上排的特殊情况处理
        for (int x = 0; x < xCol; x++)
        {
            GameSweet sweet = sweetArr[x, 0];
            if (sweet.Type == SweetsType.Empty)
            {
                GameObject newSweet = Instantiate(prefabDic[SweetsType.Normal], CorrectPos(x, -1), Quaternion.identity);
                newSweet.transform.parent = transform;
                //获取基础脚本并初始化
                sweetArr[x, 0] = newSweet.GetComponent<GameSweet>();
                sweetArr[x, 0].Init(x, -1, this, SweetsType.Normal);
                if (sweetArr[x, 0].CanMove())
                {
                    sweetArr[x, 0].moveSweet.Move(x, 0, fillTime);

                }
                if (sweetArr[x, 0].CanColor())
                {
                    sweetArr[x, 0].colorSweet.SetSprite((ColorType)Random.Range(0, sweetArr[x, 0].colorSweet.spriteDic.Count));
                }
                filledNotFinished = true;
            }
        }
        return filledNotFinished;
    }
    /// <summary>
    /// 纠正行列位置
    /// </summary>
    /// <param name="x">x坐标</param>
    /// <param name="y">y坐标</param>
    /// <returns></returns>
    public Vector3 CorrectPos(int x, int y)
    {
        //实际需要实例化网格的x位置=GameManager的x坐标-大网格长度的一半+行列对应的x坐标
        //实际需要实例化网格的y位置=GameManager的y坐标+大网格高度的一半-行列对应的y坐标
        return new Vector3(transform.position.x - xCol / 2f + x, transform.position.y + yRow / 2 - y);
    }
    /// <summary>
    /// 玩家对我们甜品操作进行拖拽处理的方法
    /// </summary>
    public void PressSweet(GameSweet sweet)
    {
        if (GameUI.Instance.gameover)
        {
            return;
        }
        clickSweet = sweet;
    }
    /// <summary>
    /// 鼠标进入的甜品
    /// </summary>
    /// <param name="sweet"></param>
    public void EnterSweet(GameSweet sweet)
    {
        if (GameUI.Instance.gameover)
        {
            return;
        }
        enterSweet = sweet;
    }
    /// <summary>
    /// 鼠标抬起
    /// </summary>
    public void ReleaseSweet()
    {
        if (GameUI.Instance.gameover)
        {
            return;
        }
        if (IsNear(clickSweet, enterSweet))
        {
            ExChangeSweet(clickSweet, enterSweet);
        }

    }
    /// <summary>
    /// 判断甜品是否相邻
    /// </summary>
    /// <param name="sweet1"></param>
    /// <param name="sweet2"></param>
    /// <returns></returns>
    private bool IsNear(GameSweet sweet1, GameSweet sweet2)
    {
        return (sweet1.X == sweet2.X && Mathf.Abs(sweet1.Y - sweet2.Y) == 1) || (sweet1.Y == sweet2.Y && Mathf.Abs(sweet1.X - sweet2.X) == 1);
    }
    /// <summary>
    /// 两个甜品位置交换
    /// </summary>
    public void ExChangeSweet(GameSweet sweet1, GameSweet sweet2)
    {
        if (sweet1.CanMove() && sweet2.CanMove())
        {
            sweetArr[sweet1.X, sweet1.Y] = sweet2;
            sweetArr[sweet2.X, sweet2.Y] = sweet1;
            if (MatchSweets(sweet1, sweet2.X, sweet2.Y) != null || MatchSweets(sweet2, sweet1.X, sweet1.Y) != null||sweet1.Type==SweetsType.RainbowCandy||sweet2.Type==SweetsType.RainbowCandy)
            {
                //记录x、y的值 防止sweet1已移动完成后位置跟sweet2重叠
                var x = sweet1.X;
                var y = sweet1.Y;
                sweet1.moveSweet.Move(sweet2.X, sweet2.Y, fillTime);
                sweet2.moveSweet.Move(x, y, fillTime);
                //清除指定颜色所有甜品
                if (sweet1.Type==SweetsType.RainbowCandy&&sweet1.CanClear()&&sweet2.CanClear())
                {
                    ClearColorSweet clearColor = sweet1.GetComponent<ClearColorSweet>();
                    if (clearColor!=null)
                    {
                        clearColor.ClearColor = sweet2.colorSweet.colorType;
                    }
                    ClearSweet(sweet1.X,sweet1.Y);
                }
                if (sweet2.Type == SweetsType.RainbowCandy && sweet1.CanClear() && sweet2.CanClear())
                {
                    ClearColorSweet clearColor = sweet2.GetComponent<ClearColorSweet>();
                    if (clearColor != null)
                    {
                        clearColor.ClearColor = sweet1.colorSweet.colorType;
                    }
                    ClearSweet(sweet2.X, sweet2.Y);
                }
                ClearAllMatchedSweet();
                StartCoroutine(AllFill());
                clickSweet = null;
                enterSweet = null;
            }
            else
            {
                sweetArr[sweet1.X, sweet1.Y] = sweet1;
                sweetArr[sweet2.X, sweet2.Y] = sweet2;
            }
        }
    }
    /// <summary>
    /// 匹配方法
    /// </summary>
    /// <param name="sweet"></param>
    /// <param name="newX"></param>
    /// <param name="newY"></param>
    /// <returns></returns>
    public List<GameSweet> MatchSweets(GameSweet sweet, int newX, int newY)
    {
        if (sweet.CanColor())
        {
            ColorType color = sweet.colorSweet.colorType;
            List<GameSweet> matchRowSweets = new List<GameSweet>();
            List<GameSweet> matchLineSweets = new List<GameSweet>();
            List<GameSweet> finishedMatchingSweets = new List<GameSweet>();

            //行匹配
            matchRowSweets.Add(sweet);

            //i=0代表往左，i=1代表往右
            for (int i = 0; i <= 1; i++)
            {
                for (int xDistance = 1; xDistance < xCol; xDistance++)
                {
                    int x;
                    if (i == 0)
                    {
                        x = newX - xDistance;
                    }
                    else
                    {
                        x = newX + xDistance;
                    }
                    if (x < 0 || x >= xCol)
                    {
                        break;
                    }

                    if (sweetArr[x, newY].CanColor() && sweetArr[x, newY].colorSweet.colorType == color)
                    {
                        matchRowSweets.Add(sweetArr[x, newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (matchRowSweets.Count >= 3)
            {
                for (int i = 0; i < matchRowSweets.Count; i++)
                {
                    finishedMatchingSweets.Add(matchRowSweets[i]);
                }
            }

            //L T型匹配
            //检查一下当前行遍历列表中的元素数量是否大于3
            if (matchRowSweets.Count >= 3)
            {
                for (int i = 0; i < matchRowSweets.Count; i++)
                {
                    //行匹配列表中满足匹配条件的每个元素上下依次进行列遍历
                    // 0代表上方 1代表下方
                    for (int j = 0; j <= 1; j++)
                    {
                        for (int yDistance = 1; yDistance < yRow; yDistance++)
                        {
                            int y;
                            if (j == 0)
                            {
                                y = newY - yDistance;
                            }
                            else
                            {
                                y = newY + yDistance;
                            }
                            if (y < 0 || y >= yRow)
                            {
                                break;
                            }

                            if (sweetArr[matchRowSweets[i].X, y].CanColor() && sweetArr[matchRowSweets[i].X, y].colorSweet.colorType == color)
                            {
                                matchLineSweets.Add(sweetArr[matchRowSweets[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (matchLineSweets.Count < 2)
                    {
                        matchLineSweets.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < matchLineSweets.Count; j++)
                        {
                            finishedMatchingSweets.Add(matchLineSweets[j]);
                        }
                        break;
                    }
                }
            }

            if (finishedMatchingSweets.Count >= 3)
            {
                return finishedMatchingSweets;
            }

            matchRowSweets.Clear();
            matchLineSweets.Clear();

            matchLineSweets.Add(sweet);

            //列匹配

            //i=0代表往左，i=1代表往右
            for (int i = 0; i <= 1; i++)
            {
                for (int yDistance = 1; yDistance < yRow; yDistance++)
                {
                    int y;
                    if (i == 0)
                    {
                        y = newY - yDistance;
                    }
                    else
                    {
                        y = newY + yDistance;
                    }
                    if (y < 0 || y >= yRow)
                    {
                        break;
                    }

                    if (sweetArr[newX, y].CanColor() && sweetArr[newX, y].colorSweet.colorType == color)
                    {
                        matchLineSweets.Add(sweetArr[newX, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (matchLineSweets.Count >= 3)
            {
                for (int i = 0; i < matchLineSweets.Count; i++)
                {
                    finishedMatchingSweets.Add(matchLineSweets[i]);
                }
            }

            //L T型匹配
            //检查一下当前行遍历列表中的元素数量是否大于3
            if (matchLineSweets.Count >= 3)
            {
                for (int i = 0; i < matchLineSweets.Count; i++)
                {
                    //行匹配列表中满足匹配条件的每个元素上下依次进行列遍历
                    // 0代表上方 1代表下方
                    for (int j = 0; j <= 1; j++)
                    {
                        for (int xDistance = 1; xDistance < xCol; xDistance++)
                        {
                            int x;
                            if (j == 0)
                            {
                                x = newY - xDistance;
                            }
                            else
                            {
                                x = newY + xDistance;
                            }
                            if (x < 0 || x >= xCol)
                            {
                                break;
                            }

                            if (sweetArr[x, matchLineSweets[i].Y].CanColor() && sweetArr[x, matchLineSweets[i].Y].colorSweet.colorType == color)
                            {
                                matchRowSweets.Add(sweetArr[x, matchLineSweets[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (matchRowSweets.Count < 2)
                    {
                        matchRowSweets.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < matchRowSweets.Count; j++)
                        {
                            finishedMatchingSweets.Add(matchRowSweets[j]);
                        }
                        break;
                    }
                }
            }

            if (finishedMatchingSweets.Count >= 3)
            {
                return finishedMatchingSweets;
            }
        }

        return null;
    }

    /// <summary>
    /// 清除方法甜品
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool ClearSweet(int x, int y)
    {

        if (sweetArr[x, y].CanClear() && !sweetArr[x, y].clearSweet.IsClearing)
        {
            sweetArr[x, y].clearSweet.Clear();
            CreateNewSweet(x, y, SweetsType.Empty);
            ClearBarrier(x, y);
            return true;
        }

        return false;
    }
    /// <summary>
    /// 清除障碍物体
    /// </summary>
    /// <param name="x">被消除甜品的x坐标</param>
    /// <param name="y">被消除甜品的y坐标</param>
    private void ClearBarrier(int x, int y)
    {
        //左右遍历（x-1 向左查找）（x+1 向右查找）
        for (int friendX = x - 1; friendX <= x + 1; friendX++)
        {
            if (friendX != x && friendX > 0 && friendX < xCol)
            {
                if (sweetArr[friendX, y].Type == SweetsType.Barrier && sweetArr[friendX, y].CanClear())
                {
                    sweetArr[friendX, y].clearSweet.Clear();
                    CreateNewSweet(friendX, y, SweetsType.Empty);
                }
            }
        }
        //上下遍历（y-1 向上查找）（y+1 向下查找）
        for (int friendY = y - 1; friendY <= y + 1; friendY++)
        {
            if (friendY != y && friendY >= 0 && friendY < yRow)
            {
                if (sweetArr[x, friendY].Type == SweetsType.Barrier && sweetArr[x, friendY].CanClear())
                {
                    sweetArr[x, friendY].clearSweet.Clear();
                    CreateNewSweet(x, friendY, SweetsType.Empty);
                }
            }
        }
    }
    /// <summary>
    /// 清除全部完成匹配的甜品
    /// </summary>
    /// <returns></returns>
    private bool ClearAllMatchedSweet()
    {
        bool needRefill = false;

        for (int y = 0; y < yRow; y++)
        {
            for (int x = 0; x < xCol; x++)
            {
                if (sweetArr[x, y].CanClear())
                {
                    List<GameSweet> matchList = MatchSweets(sweetArr[x, y], x, y);

                    if (matchList != null)
                    {
                        //确认是否产生特殊甜品
                        SweetsType sweetsType = SweetsType.Count;
                        //随机获取消除列表中的坐标
                        GameSweet randomSweet = matchList[Random.Range(0, matchList.Count)];
                        //消除4个生成行或列消除甜品
                        if (matchList.Count == 4)
                        {
                            sweetsType = (SweetsType)Random.Range((int)SweetsType.Row_Clear, (int)SweetsType.Column_Clear);
                        }
                        //消除5个生成彩虹糖
                        else if (matchList.Count >= 5)
                        {
                            sweetsType = SweetsType.RainbowCandy;
                        }

                        for (int i = 0; i < matchList.Count; i++)
                        {
                            if (ClearSweet(matchList[i].X, matchList[i].Y))
                            {
                                needRefill = true;
                            }
                        }
                        //创建行列消除特殊甜品
                        if (sweetsType != SweetsType.Count)
                        {
                            Destroy(sweetArr[randomSweet.X, randomSweet.Y]);
                            GameSweet newSweet = CreateNewSweet(randomSweet.X, randomSweet.Y, sweetsType);
                            if (sweetsType == SweetsType.Row_Clear || sweetsType == SweetsType.Column_Clear && newSweet.CanColor() && matchList[0].CanColor())
                            {
                                newSweet.colorSweet.SetSprite(matchList[0].colorSweet.colorType);
                            }
                            else if (sweetsType == SweetsType.RainbowCandy & newSweet.CanColor())
                            {
                                newSweet.colorSweet.SetSprite(ColorType.Any);
                            }
                        }
                    }
                }
            }
        }
        return needRefill;
    }
    /// <summary>
    /// 清除行的方法
    /// </summary>
    /// <param name="row"></param>
    public void ClearRow(int row)
    {
        for (int x = 0; x < xCol; x++)
        {
            ClearSweet(x, row);
        }
    }
    /// <summary>
    /// 清除列的方法
    /// </summary>
    /// <param name="column"></param>
    public void ClearColumn(int column)
    {
        for (int y = 0; y < yRow; y++)
        {
            ClearSweet(column, y);
        }
    }
    /// <summary>
    /// 清除颜色
    /// </summary>
    /// <param name="color"></param>
    public void ClearColor(ColorType color)
    {
        for (int x = 0; x < xCol; x++)
        {
            for (int y = 0; y < yRow; y++)
            {
                if (sweetArr[x,y].CanColor()&&(sweetArr[x,y].colorSweet.colorType==color||color==ColorType.Any))
                {
                    ClearSweet(x,y);
                }
            }
        }
    }
}
