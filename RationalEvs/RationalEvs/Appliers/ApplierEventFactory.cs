namespace RationalEvs.Appliers
{
    public static class ApplierEventFactory
    {
        /// <summary>
        /// Defaults the applier.
        /// </summary>
        /// <returns></returns>
        public static IApplierEvents DefaultApplier()
        {
            return CreateOrderedVersioningApplierEvents();
        }


        /// <summary>
        /// Creates the ordered versioning applier events.
        /// </summary>
        /// <returns></returns>
        public static IApplierEvents CreateOrderedVersioningApplierEvents()
        {
            return new OrderedVersioningApplierEvents();
        }


        /// <summary>
        /// Creates the ordered versioning applier events.
        /// </summary>
        /// <param name="notApplyEventPreviousRootVersion">if set to <c>true</c> [not apply event previous root version].</param>
        /// <returns></returns>
        public static IApplierEvents CreateOrderedVersioningApplierEvents(bool notApplyEventPreviousRootVersion)
        {
            return new OrderedVersioningApplierEvents(notApplyEventPreviousRootVersion);
        }

        /// <summary>
        /// Creates the ordered versioning applier events.
        /// </summary>
        /// <returns></returns>
        public static IApplierEvents CreateFirtComeApplierEvents()
        {
            return new FirtComeApplierEvents();
        }

        /// <summary>
        /// Creates the ordered versioning applier events.
        /// </summary>
        /// <param name="notApplyEventPreviousRootVersion">if set to <c>true</c> [not apply event previous root version].</param>
        /// <returns></returns>
        public static IApplierEvents CreateFirtComeApplierEvents(bool notApplyEventPreviousRootVersion)
        {
            return new FirtComeApplierEvents(notApplyEventPreviousRootVersion);
        }

    }
}