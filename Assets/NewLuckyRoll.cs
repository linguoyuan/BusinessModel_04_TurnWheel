using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewLuckyRoll : MonoBehaviour
{
    //此案例是指针不动，转盘动  --转盘的旋转过程是先加速然后加速时间到了后减速直到停止
    //注意：Unity里面角度是逆时针计算的，需要顺时针旋转就添加负号
    public enum RollState
    {
        None,
        SpeedUp,
        SpeedDown,
        End
    }
    public GameObject ResultUI;
    public Text resultTxt;
    public Transform RollPanel;//旋转的转盘
    RollState curState; //当前转盘的状态
    int rollID;//旋转结果ID--此案例的转盘分为6份

    float allTime = 0;//旋转时间  总的时间
    float endAngle;//最终的角度
    //--------------------加速段---------------------------------
    float MaxSpeed = 500;//最大速度
    float factor;//速度因子
    float accelerateTime = 1;//加速到最大速度的时间 ---暂定为1
    float speedUpTime = 3;//加速段的总时间

    //--------------------减速段---------------------------------
    float tempAngle;//开始减速段时转盘此时的旋转角度
    float k = 2f; //减速阶段的速度系数 --减速快慢由此决定 

    void Start()
    {
        rollID = 0;
        ResultUI.SetActive(false);

    }

    void Update()
    {
        if (curState == RollState.None)
        {
            return;
        }
        allTime += Time.deltaTime;
        //先进入加速阶段
        if (curState == RollState.SpeedUp)
        {
            factor = allTime / accelerateTime;
            factor = factor > 1 ? 1 : factor;
            RollPanel.Rotate(new Vector3(0, 0, -1) * factor * MaxSpeed * Time.deltaTime, Space.Self);
        }
        //当旋转时间大于等于了加速段的时间就开始进行减速
        if (allTime >= speedUpTime && curState == RollState.SpeedUp)
        {
            curState = RollState.SpeedDown;
            tempAngle = GetTempAngle();
            Debug.Log("tempAngle:" + tempAngle);
        }
        if (curState == RollState.SpeedDown)
        {
            //通过差值运算实现旋转到指定角度（球型插值无法实现大于360°的计算）
            tempAngle = Mathf.Lerp(tempAngle, endAngle, Time.deltaTime * k);
            RollPanel.rotation = Quaternion.Euler(0, 0, tempAngle);
            //RollPanel.eulerAngles = new Vector3(0, 0, tempAngle);
            //旋转结束
            if (Mathf.Abs(tempAngle - endAngle) <= 1)
            {
                curState = RollState.None;
                ResultUI.SetActive(true);
            }
        }
    }
    /// <summary>
    /// 开始旋转转盘
    /// </summary>
    public void StartTurnWheel()
    {
        if (curState != RollState.None)
        {
            return;
        }
        allTime = 0;
        tempAngle = 0;
        rollID = GetRandomID();
        Debug.Log("rollID: " + rollID);
        endAngle = (-1) * rollID * 60;
        curState = RollState.SpeedUp;
    }
    /// <summary>
    /// 得到当前转盘的旋转角度
    /// </summary>
    /// <returns></returns>
    private float GetTempAngle()
    {
        Debug.Log("RollPanel.eulerAngles.z: " + RollPanel.eulerAngles.z);
        return (360 - RollPanel.eulerAngles.z) % 360;
    }
    //获取旋转的结果(一般从后端获取) 这里就通过设定的概率然后随机得到结果
    private int GetRandomID()
    {
        //转盘倒着数，因为顺时针旋转，旋转角度为负数
        int id = 0;
        int a = Random.Range(0, 100);
        if (a <= 1)//%1
        {
            id = 6;
            resultTxt.text = "恭喜你获得10000金币";
        }
        else if (1 < a && a <= 3)  //%2
        {
            id = 3;
            resultTxt.text = "恭喜你获得7000金币";
        }
        else if (3 < a && a <= 10)  //%7
        {
            id = 2;
            resultTxt.text = "恭喜你获得3800金币";
        }
        else if (10 < a && a <= 30) // %20
        {
            id = 4;
            resultTxt.text = "恭喜你获得2600金币";
        }
        else if (30 < a && a <= 60) //%30
        {
            id = 5;
            resultTxt.text = "恭喜你获得1200金币";
        }
        else   //%40
        {
            id = 1;
            resultTxt.text = "恭喜你获得500金币";
        }
        return id;
    }

    public void CloseResultUI()
    {
        RestRollPanel();
        ResultUI.SetActive(false);

    }
    void RestRollPanel()
    {
        RollPanel.rotation = new Quaternion(0, 0, 0, 0);
        curState = RollState.None;
    }
}

