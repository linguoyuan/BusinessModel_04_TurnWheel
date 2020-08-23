using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetIcons : MonoBehaviour
{
    public struct Split
    {
        /// <summary>
        /// 起始角度[0, 360]
        /// </summary>
        public float angle;
        /// <summary>
        /// 区间大小[0, 360]
        /// </summary>
        public float range;
        /// <summary>
        /// 区间划分
        /// </summary>
        /// <param name="_angle">起始角度[0, 360]</param>
        /// <param name="_range">区间大小[0, 360]</param>
        public Split(float _angle, float _range)
        {
            angle = _angle;
            range = _angle;
        }
    }
    /// <summary>
    /// Icon对象
    /// </summary>
    public struct Item
    {
        public Transform node;  //根节点
        public Image img;       //子节点的图片
        public Item(Transform _node, Image _img)
        {
            node = _node;
            img = _img;
        }
    }

    private Split[] splits;
    /// <summary>
    /// 区间划分
    /// </summary>
    public Split[] Splits
    {
        get { return splits; }
        private set { splits = value; }
    }

    private Item[] items;
    /// <summary>
    /// Icon数组
    /// </summary>
    public Item[] Items
    {
        get { return items; }
        private set { items = value; }
    }

    private Transform _item;

    /// <summary>
    /// 区间的数量
    /// </summary>
    private readonly int len = 8;

    private void Awake()
    {
        splits = new Split[len];
        items = new Item[len];
    }

    private void Start()
    {
        _item = transform.Find("item");

        float startAngle = 0, range = 0, midRadian = 0;
        float radius = _item.localPosition.y;
        for (int i = 0; i < len; i++)
        {
            //实际这份数据应该从配置中获取，这里为了方便就本地写死了
            range = 0 == i % 2 ? 30 : 60;
            splits[i] = new Split(startAngle, range);
            midRadian = (startAngle + range * 0.5f) * Mathf.Deg2Rad;
            startAngle += range;

            Transform node = 0 == i ? _item : Instantiate(_item.gameObject, transform, false).transform;
            Image img = node.Find("sp_icon").GetComponent<Image>();

            node.localPosition = new Vector3(radius * Mathf.Sin(midRadian), radius * Mathf.Cos(midRadian), 0);
            img.sprite = Resources.Load<Sprite>(string.Format("icon/shengdanlaoren-2014-{0:D2}", i + 1));
            items[i] = new Item(node, img);
        }
    }

}
