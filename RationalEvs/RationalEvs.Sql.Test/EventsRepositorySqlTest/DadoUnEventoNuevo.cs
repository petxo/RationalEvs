using Bteam.NHibernate;
using NHibernate.Criterion;
using NUnit.Framework;
using RationalEvs.Repositories;
using RationalEvs.Sql.Test.DomainFake;
using RationalEvs.Sql.Test.DomainFake.Events;

namespace RationalEvs.Sql.Test.EventsRepositorySqlTest
{
    [TestFixture]
    public class DadoUnEventoNuevo
    {
        private INHibernateHelper _nHibernateHelper;
        private EventsRepositorySql<Ship> _eventsRepositorySql;
        private IQuerySnapshotBuilder<ICriterion,Ship> _queryBuilder;

        [SetUp]
        public void Setup()
        {
            BteamMongoDB.Repository.RepositoryClassMapper.RegisterClassMap<ArrivalEvent>();
            BteamMongoDB.Repository.RepositoryClassMapper.RegisterClassMap<DepartureEvent>();
            NHibernateHelper.Initialize();
            _nHibernateHelper = new NHibernateHelper("EventSourcing");
            _queryBuilder = new SqlQuerySnapshotBuilder<Ship, long>();
            _eventsRepositorySql = new EventsRepositorySql<Ship>(_nHibernateHelper, _queryBuilder);
        }

        [Test]
        public void CuandoSeAñadeSeGuardaCorrectamente()
        {
            var arrivalEvent = new ArrivalEvent {Port = new Port {Name = "Barcelona"}};
            _eventsRepositorySql.AddEvent(1, arrivalEvent);
        }

        [Test]
        public void CuandoSeGeneraElSnapshotSeGuardaCorrectamente()
        {
            var ship = new Ship
                {
                    Id = 1,
                    Port = Port.AtSea,
                    Version = 1
                };
            
            var arrivalEvent = new ArrivalEvent {Port = new Port {Name = "Barcelona"}};
            _eventsRepositorySql.AddEvent(1, arrivalEvent);

            _eventsRepositorySql.SaveSnapShot(ship, SnapShotType.ByVersion, new[]{arrivalEvent}, "In Navigation");
        }


        [Test]
        public void CuandoSeAñadeOtroEventoSeGuardaCorrectamente()
        {
            var departureEvent = new DepartureEvent { Port = new Port { Name = "Barcelona" } };
            _eventsRepositorySql.AddEvent(1, departureEvent);
        }
    }
}