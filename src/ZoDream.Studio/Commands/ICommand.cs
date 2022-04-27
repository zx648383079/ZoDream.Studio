﻿namespace ZoDream.Studio.Commands
{
    public interface ICommand
    {
        /// <summary>
        /// 执行
        /// </summary>
        /// <returns>执行释放成功</returns>
        public bool Execute();
    }
}
