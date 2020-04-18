﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Services.Shared.Models
{
    public class RequestBase<T> : IBase<T>
        where T : class
    {
        public RequestBase(T data) : this(Guid.NewGuid(), DateTime.Now, data)
        {
            Data = data;
        }

        public RequestBase(DateTime date, T data) : this(Guid.NewGuid(),date, data)
        {
            ID = Guid.NewGuid();
            CreationDate = date;
        }

        [JsonConstructor]
        public RequestBase(Guid iD, DateTime creationDate, T data)
        {
            ID = iD;
            CreationDate = creationDate;
            Data = data;
        }

        public Guid ID { get; private set; }

        public DateTime CreationDate { get; set; }

        public T Data { get; private set; }
    }
}
