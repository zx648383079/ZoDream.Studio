using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public class MusicItem
    {
        #region 属性
        /// <summary>
        /// 曲名
        /// </summary>
        public string Title { get; set; } = string.Empty;
        /// <summary>
        /// 作词
        /// </summary>
        public string Author { get; set; } = string.Empty;
        /// <summary>
        /// 作曲
        /// </summary>
        public string Composer { get; set; } = string.Empty;
        /// <summary>
        /// 演唱者
        /// </summary>
        public string Artist { get; set; } = string.Empty;

        /// <summary>
        /// 每一拍经过的Tick数，范围从48到960
        /// </summary>
        public ushort TicksPerBeat { get; set; } = 960;
        /// <summary>
        /// 文件的格式 0－单轨 1－多规，同步 2－多规，异步
        /// </summary>
        public ushort FormatType { get; set; } = 0;
        /// <summary>
        /// 轨道列表
        /// </summary>
        public List<TrackItem> Items { get; set; } = new();
        #endregion

    }
}
