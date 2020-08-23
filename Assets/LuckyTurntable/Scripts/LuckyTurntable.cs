using UnityEngine;
using UnityEngine.UI;

public class LuckyTurntable : MonoBehaviour
{
    public enum State
    {
        None,       //待机状态
        Start,      //加速阶段
        Prepared,   //等待数据阶段
        End,        //减速阶段
    }

    public delegate void OnFinishCallback();
    private event OnFinishCallback OnFinish;

    /// <summary>
    /// 设置完成时的回调
    /// </summary>
    /// <param name="onFinish"></param>
    public void SetOnFinishCallback(OnFinishCallback onFinish)
    {
        OnFinish += onFinish;
    }

    /// <summary>
    /// 最大速度
    /// </summary>
    public int velocity = 3000;

    public Transform node;
    public Button btnStart;
    public Button btnStop;
    public Button btnRandom;
    public InputField input;

    private State _state;
    /// <summary>
    /// 转盘的状态
    /// </summary>
    public  State CurState
    {
        get
        {
            return _state;
        }
        private set
        {
            _state = value;
            switch (value)
            {
                //不同阶段限制各按钮的点击状态
                case State.None:
                    btnStart.enabled = true;
                    btnStop.enabled = false;
                    btnRandom.enabled = false;
                    break;
                case State.Start:
                    btnStart.enabled = false;
                    btnStop.enabled = true;
                    btnRandom.enabled = true;
                    break;
                case State.Prepared:
                case State.End:
                    btnStart.enabled = false;
                    btnStop.enabled = false;
                    btnRandom.enabled = false;
                    break;
            }
        }
    }

    private float _endAngle = 0f;
    /// <summary>
    /// 最终停止的角度[0, 360]
    /// </summary>
    public float EndAngle
    {
        get
        {
            return _endAngle;
        }
        set
        {
            _endAngle = Mathf.Abs(value);
            print("End Angle: " + value);
            _endAngle = _endAngle % 360;    //将角度限定在[0, 360]这个区间
            _endAngle = -_endAngle - 360 * 2;    //多N圈并取反，圈数能使减速阶段变得更长，显示更自然，逼真
        }
    }

    /// <summary>
    /// 加速持续时间
    /// </summary>
    private readonly float AcceleateTime = 1f;

    /// <summary>
    /// 减速前的最短持续时间
    /// </summary>
    private float _minTime = 3.0f;
    /// <summary>
    /// 角度缓存
    /// </summary>
    private float _tmpAngle = 0f;
    /// <summary>
    /// 时间统计
    /// </summary>
    private float _time;
    /// <summary>
    /// 速度变化因子
    /// </summary>
    private float _factor;

    private void Start()
    {
        CurState = State.None;
        btnStart.onClick.AddListener(OnStartClick);
        btnStop.onClick.AddListener(OnStopClick);
        btnRandom.onClick.AddListener(OnRandomClick);
    }

    private void Update()
    {
        if (CurState == State.None)
            return;

        _time += Time.deltaTime;
        if (CurState == State.End)
        {
            //通过差值运算实现精准地旋转到指定角度（球型插值无法实现大于360°的计算）
            float k = 2f;  //如果嫌减速太慢，可以加个系数修正一下
            _tmpAngle = Mathf.Lerp(_tmpAngle, EndAngle, Time.deltaTime * k);

            //这里只存在一个方向的旋转，所以不存在欧拉角万向节的问题，所以使用欧拉角和四元数直接赋值都是可以的
            node.rotation = Quaternion.Euler(0, 0, _tmpAngle);
            //node.eulerAngles = new Vector3(0, 0, _tmpAngle);

            if (1 >= Mathf.Abs(_tmpAngle - EndAngle))
            {
                CurState = State.None;
                if (null != OnFinish)
                {
                    OnFinish();
                    OnFinish = null;
                }
            }
        }
        else
        {
            //利用一个速度因子实现变加速的过程
            _factor = _time / AcceleateTime;
            _factor = _factor > 1 ? 1 : _factor;
            node.Rotate(Vector3.back, _factor * velocity * Time.deltaTime, Space.Self);
        }

        //当收到数据之后并且旋转了一定时间后开始减速
        if (CurState == State.Prepared && _time > _minTime)
        {
            CurState = State.End;
            _tmpAngle = GetCurClockwiseAngle();
        }
    }

    /// <summary>
    /// 将当前指针的欧拉角转换成顺时针统计角度
    /// </summary>
    /// <returns></returns>
    private float GetCurClockwiseAngle()
    {
        //由于读取到的值是[0, 180] U [-180, 0]，左边由0至180递增，右边由180转变成-180，然后递增至0，所以需要转相应的转换
        return (-1) * (360 - node.eulerAngles.z) % 360;
    }

    private void OnStartClick()
    {
        CurState = State.Start;
        _time = 0;
    }

    /// <summary>
    /// 读取输入框中的角度并停止
    /// </summary>
    private void OnStopClick()
    {
        try
        {
            EndAngle = float.Parse(input.text);
        }
        catch
        {
            EndAngle = 0f;
        }
        CurState = State.Prepared;

    }

    /// <summary>
    /// 随机一个角度并停止
    /// </summary>
    private void OnRandomClick()
    {
        EndAngle = UnityEngine.Random.Range(0f, 360f);
        CurState = State.Prepared;
    }
}
