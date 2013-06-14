using Bteam.NHibernate;
using NUnit.Framework;
using RationalEvs.Sql.Test.DomainFake;
using RationalEvs.Sql.Test.DomainFake.Events;

namespace RationalEvs.Sql.Test
{
    [TestFixture]
    public class DadoUnEventoNuevo
    {
        private INHibernateHelper _nHibernateHelper;
        private EventsRepositorySql<Ship> _eventsRepositorySql;

        [SetUp]
        public void Setup()
        {
            NHibernateHelper.Initialize();
            _nHibernateHelper = new NHibernateHelper("EventSourcing");
            _eventsRepositorySql = new EventsRepositorySql<Ship>(_nHibernateHelper);
        }

        [Test]
        public void CuandoSeAñadeSeGuardaCorrectamente()
        {
            var arrivalEvent = new ArrivalEvent {Port = new Port {Name = "Barcelona"}};
            _eventsRepositorySql.AddEvent(1, arrivalEvent);

        }
         
    }
}