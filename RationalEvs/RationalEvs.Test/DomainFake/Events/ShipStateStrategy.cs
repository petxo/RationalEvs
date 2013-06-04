using MongoDB.Bson;
using RationalEvs.Factories;
using RationalEvs.Services;
using RationalEvs.Services.RefuseEngines;
using RationalEvs.Services.RefuseEngines.Factories;

namespace RationalEvs.Test.DomainFake.Events
{
    public class ShipStateStrategy : StateStrategy<Ship, ObjectId>
    {
        public ShipStateStrategy(IAggregateFactory<Ship, ObjectId> factory)
            : base(factory, RefuseEngineFactory.DefaultRefuseEngine<Ship, ObjectId>())
        {
        }


        public ShipStateStrategy(IAggregateFactory<Ship, ObjectId> factory, IRefuseEngine<Ship, ObjectId> refuseEngine)
            : base(factory, refuseEngine)
        {
        }


        protected override void ConfigureStrategy()
        {
            AddAction("Anchored", root => OnActionStateNotification(root.State, root.Root));
            AddAction("In Navigation", root => OnActionStateNotification(root.State, root.Root));
            AddAction("Sunken", root => OnActionStateNotification(root.State, root.Root));
        }
    }
}