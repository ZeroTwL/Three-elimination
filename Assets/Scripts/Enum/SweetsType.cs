using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Enum
{
    /// <summary>
    /// 甜品的种类
    /// </summary>
    public enum SweetsType
    {
        /// <summary>
        /// 空的
        /// </summary>
        Empty,
        /// <summary>
        /// 普通
        /// </summary>
        Normal,
        /// <summary>
        /// 障碍
        /// </summary>
        Barrier,
        /// <summary>
        /// 列消除
        /// </summary>
        Column_Clear,
        /// <summary>
        /// 行消除
        /// </summary>
        Row_Clear,
        /// <summary>
        /// 彩虹糖果
        /// </summary>
        RainbowCandy,
        /// <summary>
        /// 标记类型
        /// </summary>
        Count
    }
}
