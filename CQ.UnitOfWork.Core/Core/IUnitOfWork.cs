﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Core
{
    public interface IUnitOfWork
    {
        IRepository<T> GetRepository<T>() where T : class;
    }
}
