namespace Enigma.IO
{
    public class WriteReservation
    {
        private readonly long _position;

        public WriteReservation(long position)
        {
            _position = position;
        }

        public long Position { get { return _position; } }
    }
}