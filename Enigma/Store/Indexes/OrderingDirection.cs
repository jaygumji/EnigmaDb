namespace Enigma.Store.Indexes
{
    public enum OrderingDirection
    {
        Ascending,
        Descending
    }

    public static class OrderingDirections
    {
        public static OrderingDirection Get(Remotion.Linq.Clauses.OrderingDirection direction)
        {
            return direction == Remotion.Linq.Clauses.OrderingDirection.Asc
                ? OrderingDirection.Ascending
                : OrderingDirection.Descending;
        }
    }
}