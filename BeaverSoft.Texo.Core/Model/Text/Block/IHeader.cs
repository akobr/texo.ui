﻿namespace BeaverSoft.Texo.Core.Model.Text
{
    public interface IHeader : IBlock
    {
        ushort? Level { get; }
    }
}