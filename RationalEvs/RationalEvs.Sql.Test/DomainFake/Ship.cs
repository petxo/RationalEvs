namespace RationalEvs.Sql.Test.DomainFake
{

    public class Ship : IVersionableEntity<long>
    {
        public Ship()
        {
            Port = Port.AtSea;
        }
        public Port Port { get; set; }

        public long Id { get; set; }

        public long Version { get; set; }
    }

    public class Port
    {
        public static Port AtSea { get; private set; }

        static Port()
        {
            AtSea = new Port { Name = "At sea" };
        }

        public string Name { get; set; }

        public bool Equals(Port other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.Name, Name);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Port)) return false;
            return Equals((Port) obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }

        public static bool operator ==(Port left, Port right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Port left, Port right)
        {
            return !Equals(left, right);
        }
    }
}