using BteamMongoDB.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
using RationalEvs.Entities;
using RationalEvs.Factories;
using RationalEvs.Fsm.Configuration;
using RationalEvs.Repositories;
using RationalEvs.Test.DomainFake;
using NUnit.Framework;
using RationalEvs.Test.DomainFake.Events;
using log4net;

namespace RationalEvs.Test
{
    [TestFixture]
    public class AggregateRootFromFileTest
    {
        private IRepository<EntityEventSource<Ship, ObjectId>, ObjectId> _repository;
        private EventsRepository<Ship, ObjectId> _eventsRepository;
        private IAggregateFactory<Ship, ObjectId> _aggregateFactory;
        private Ship _ship;
        private FsmConfigurator _fsmConfigurator;
        private IQuerySnapshotBuilder<IMongoQuery, Ship> _queryBuilder;


        [SetUp]
        public void Setup()
        {
            _repository = new Repository<EntityEventSource<Ship, ObjectId>, ObjectId>("RationalEvs", "EventSourcing");
            _queryBuilder = new MongoQuerySnapshotBuilder<Ship, ObjectId>();
            _eventsRepository = new EventsRepository<Ship, ObjectId>(_repository, _queryBuilder, 0);

            _fsmConfigurator = FsmConfiguratorFactory.WithXgml("shipStateMachine.xgml").SetInitialState("In Navigation").WithDomainAssembly
    <DepartureEvent>("RationalEvs.Test.DomainFake.Events").
    Create();

            _aggregateFactory = new AggregateRootFactory<Ship, ObjectId>(_eventsRepository, _fsmConfigurator)
                                    {
                                        Logger = LogManager.GetLogger(GetType())
                                    };

            _ship = new Ship { Id = ObjectId.GenerateNewId() };
        }

        [Test]
        public void CreateNewShip()
        {
            
            var arrivalEvent = new ArrivalEvent { Port = new Port { Name = "Barcelona" }, Version = 1 };

            var aggreate = _aggregateFactory.GetAndApplyEvent(_ship.Id, arrivalEvent);

            Assert.AreEqual("Barcelona", aggreate.Root.Port.Name);
        }

        [Test]
        public void AddOrderedEventsToShip()
        {
            var arrivalEvent = new ArrivalEvent { Port = new Port { Name = "Barcelona" }, Version = 1 };
            _aggregateFactory.GetAndApplyEvent(_ship.Id, arrivalEvent);

            var arrivalEvent1 = new ArrivalEvent { Port = new Port { Name = "Roma" }, Version = 1 };
            var aggreate = _aggregateFactory.GetAndApplyEvent(_ship.Id, arrivalEvent1);

            Assert.AreEqual("Barcelona", aggreate.Root.Port.Name);
        }

        [Test]
        public void AddUnorderedEventsToShip()
        {
            var arrivalEvent = new ArrivalEvent { Port = new Port { Name = "Barcelona" }, Version = 2 };
            _aggregateFactory.GetAndApplyEvent(_ship.Id, arrivalEvent);

            var arrivalEvent1 = new ArrivalEvent { Port = new Port { Name = "Roma" }, Version = 1 };
            var aggreate = _aggregateFactory.GetAndApplyEvent(_ship.Id, arrivalEvent1);

            Assert.AreEqual("Roma", aggreate.Root.Port.Name);
        }

        [Test]
        public void AddCollectionEventsToShip()
        {
            var arrivalEvent = new ArrivalEvent { Port = new Port { Name = "Barcelona" }, Version = 3 };
            _aggregateFactory.GetAndApplyEvent(_ship.Id, arrivalEvent);

            var arrivalEvent1 = new ArrivalEvent { Port = new Port { Name = "Roma" }, Version = 1 };
            _aggregateFactory.GetAndApplyEvent(_ship.Id, arrivalEvent1);

            var departureEvent = new DepartureEvent { Port = new Port { Name = "Roma" }, Version = 2 };
            var aggreate = _aggregateFactory.GetAndApplyEvent(_ship.Id, departureEvent);

            Assert.AreEqual("Barcelona", aggreate.Root.Port.Name);
        }

        [Test]
        public void AddALotOfEventsToShip()
        {
            var arrivalEvent = new ArrivalEvent { Port = new Port { Name = "Barcelona" }, Version = 3 };
            _aggregateFactory.GetAndApplyEvent(_ship.Id, arrivalEvent);

            var arrivalEvent1 = new ArrivalEvent { Port = new Port { Name = "Roma" }, Version = 1 };
            _aggregateFactory.GetAndApplyEvent(_ship.Id, arrivalEvent1);

            var departureEvent = new DepartureEvent { Port = new Port { Name = "Roma" }, Version = 2 };
            _aggregateFactory.GetAndApplyEvent(_ship.Id, departureEvent);

            var departureEvent1 = new DepartureEvent { Port = new Port { Name = "Barcelona" }, Version = 4 };
            _aggregateFactory.GetAndApplyEvent(_ship.Id, departureEvent1);

            var arrivalEvent2 = new ArrivalEvent { Port = new Port { Name = "Mallorca" }, Version = 5 };
            _aggregateFactory.GetAndApplyEvent(_ship.Id, arrivalEvent2);

            var departureEvent2 = new DepartureEvent { Port = new Port { Name = "Mallorca" }, Version = 6 };
            var aggreate = _aggregateFactory.GetAndApplyEvent(_ship.Id, departureEvent2);

            Assert.AreEqual(Port.AtSea, aggreate.Root.Port);
        }

        [Test]
        public void ApplyEventToAggreagateRoot()
        {
            var arrivalEvent = new ArrivalEvent { Port = new Port { Name = "Barcelona" }, Version = 3 };
            _aggregateFactory.GetAndApplyEvent(_ship.Id, arrivalEvent);

            var arrivalEvent1 = new ArrivalEvent { Port = new Port { Name = "Roma" }, Version = 1 };
            _aggregateFactory.GetAndApplyEvent(_ship.Id, arrivalEvent1);

            var departureEvent = new DepartureEvent { Port = new Port { Name = "Roma" }, Version = 2 };
            _aggregateFactory.GetAndApplyEvent(_ship.Id, departureEvent);

            var departureEvent1 = new DepartureEvent { Port = new Port { Name = "Barcelona" }, Version = 4 };
            _aggregateFactory.GetAndApplyEvent(_ship.Id, departureEvent1);

            var arrivalEvent2 = new ArrivalEvent { Port = new Port { Name = "Mallorca" }, Version = 5 };
            var aggreate = _aggregateFactory.GetAndApplyEvent(_ship.Id, arrivalEvent2);

            var departureEvent2 = new DepartureEvent { Port = new Port { Name = "Mallorca" }, Version = 6 };
            aggreate.ApplyEvent(departureEvent2);

            Assert.AreEqual("In Navigation", aggreate.State);
        }

        [Test]
        public void AddEventsToSnapShotShip()
        {
            var arrivalEvent = new ArrivalEvent { Port = new Port { Name = "Barcelona" }, Version = 3 };
            _aggregateFactory.GetAndApplyEvent(_ship.Id, arrivalEvent);

            var arrivalEvent1 = new ArrivalEvent { Port = new Port { Name = "Roma" }, Version = 1 };
            _aggregateFactory.GetAndApplyEvent(_ship.Id, arrivalEvent1);

            var departureEvent = new DepartureEvent { Port = new Port { Name = "Roma" }, Version = 2 };
            _aggregateFactory.GetAndApplyEvent(_ship.Id, departureEvent);

            var departureEvent1 = new DepartureEvent { Port = new Port { Name = "Barcelona" }, Version = 4 };
            _aggregateFactory.GetAndApplyEvent(_ship.Id, departureEvent1);

            var arrivalEvent2 = new ArrivalEvent { Port = new Port { Name = "Mallorca" }, Version = 5 };
            _aggregateFactory.GetAndApplyEvent(_ship.Id, arrivalEvent2).CreateSnapShot();

            var departureEvent2 = new DepartureEvent { Port = new Port { Name = "Mallorca" }, Version = 6 };
            var aggreate = _aggregateFactory.GetAndApplyEvent(_ship.Id, departureEvent2);

            Assert.AreEqual(Port.AtSea, aggreate.Root.Port);
        }

        [Test]
        public void CreateSnapShotAfterApplyEventsShip()
        {
            var arrivalEvent = new ArrivalEvent { Port = new Port { Name = "Barcelona" }, Version = 3 };
            _aggregateFactory.GetAndApplyEvent(_ship.Id, arrivalEvent);

            var arrivalEvent1 = new ArrivalEvent { Port = new Port { Name = "Roma" }, Version = 1 };
            _aggregateFactory.GetAndApplyEvent(_ship.Id, arrivalEvent1);

            var departureEvent = new DepartureEvent { Port = new Port { Name = "Roma" }, Version = 2 };
            _aggregateFactory.GetAndApplyEvent(_ship.Id, departureEvent);

            var departureEvent1 = new DepartureEvent { Port = new Port { Name = "Barcelona" }, Version = 4 };
            var aggregateRoot = _aggregateFactory.GetAndApplyEvent(_ship.Id, departureEvent1);

            var arrivalEvent2 = new ArrivalEvent { Port = new Port { Name = "Mallorca" }, Version = 5 };
            _aggregateFactory.GetAndApplyEvent(_ship.Id, arrivalEvent2).CreateSnapShot();

            aggregateRoot.CreateSnapShot(); //Este SnapShot no se deberia guardar

            var departureEvent2 = new DepartureEvent { Port = new Port { Name = "Mallorca" }, Version = 6 };
            var aggreate = _aggregateFactory.GetAndApplyEvent(_ship.Id, departureEvent2);

            Assert.AreEqual(Port.AtSea, aggreate.Root.Port);
        }

        [TearDown]
        public void TearDown()
        {
            
        }
    }
}