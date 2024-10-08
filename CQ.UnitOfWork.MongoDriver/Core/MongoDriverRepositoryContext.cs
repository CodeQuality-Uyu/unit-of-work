﻿namespace CQ.UnitOfWork.MongoDriver.Core;
public class MongoDriverRepositoryContext<TEntity, TContext>(TContext context)
    : MongoDriverRepository<TEntity>(context)
    where TEntity : class
    where TContext : MongoContext
{
}
