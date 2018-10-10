﻿using System;
using System.Collections.Generic;
using System.Text;

namespace BeaverSoft.Texo.Core.Model.Configuration
{
    public partial class Parameter
    {
        public static Parameter Empty { get; } = new Parameter();

        public static Builder CreateBuilder()
        {
            return Empty.ToBuilder();
        }
    }
}
