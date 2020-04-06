﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MarketingService.API.ViewModels.NewsletterPublish.Abstractions
{
    public interface IAtSpecifiedTime
    {
        [Required]
        DateTime Time { get; set; }
    }
}
