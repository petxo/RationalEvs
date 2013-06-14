using RationalEvs.Factories;
using RationalEvs.Services;
using RationalEvs.Services.RefuseEngines;
using RationalEvs.Services.RefuseEngines.Factories;

namespace RationalEvs.Sql.Test.DomainFake.Events
{
    public class ShipStateStrategy : StateStrategy<Ship, long>
    {
        public ShipStateStrategy(IAggregateFactory<Ship, long> factory)
            : base(factory, RefuseEngineFactory.DefaultRefuseEngine<Ship, long>())
        {
        }


        public ShipStateStrategy(IAggregateFactory<Ship, long> factory, IRefuseEngine<Ship, long> refuseEngine)
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