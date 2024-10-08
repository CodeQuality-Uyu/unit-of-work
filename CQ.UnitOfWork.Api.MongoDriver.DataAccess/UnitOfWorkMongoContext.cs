﻿using CQ.UnitOfWork.MongoDriver.Core;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CQ.UnitOfWork.Api.MongoDriver.DataAccess
{
    public sealed class UnitOfWorkMongoContext : MongoContext
    {
        public UnitOfWorkMongoContext(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
            base.AddCollection<UserMongo>("users");
        }
    }
}
