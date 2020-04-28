﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Services.API.Communication.Models.Abstractions
{
    interface IResponse : IBase
    {
        bool Success { get; }
        string Message { get; }
    }
}
