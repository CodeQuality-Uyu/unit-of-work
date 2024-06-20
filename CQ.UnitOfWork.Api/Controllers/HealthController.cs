using CQ.UnitOfWork.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace CQ.UnitOfWork.Api.Controllers
{
    [ApiController]
    [Route("/", Name = "Ping")]
    [Route("health", Name = "HealthCheck")]
    public class HealthController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEnumerable<IDatabaseContext>? _dataBaseContexts;

        public HealthController(IUnitOfWork unitOfWork, IEnumerable<IDatabaseContext> databaseContexts)
        {
            this._unitOfWork = unitOfWork;
            this._dataBaseContexts = databaseContexts;
        }

        [HttpGet]
        public object Get()
        {

            return new
            {
                Alive = true,
                Databases = this._dataBaseContexts.Select(d =>
                {
                    return new
                    {
                        Alive = d.Ping()
                    };
                })
            };
        }
    }
}
