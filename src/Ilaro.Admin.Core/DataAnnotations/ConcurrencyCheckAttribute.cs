﻿using System;

namespace Ilaro.Admin.Core.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ConcurrencyCheckAttribute : Attribute
    {
    }
}