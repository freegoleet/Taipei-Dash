
namespace Traffic
{
    public struct Connections {
        // 0 = Up, 1 = Down, 2 = Left, 3 = Right
        public bool[] InConnections { get; }
        public bool[] OutConnections { get; }

        public Connections(int size) {
            InConnections = new bool[size];
            OutConnections = new bool[size];
        }

        public void AddInConnection(Direction connection) {
            InConnections[(int)connection] = true;
        }

        public void AddOutConnection(Direction connection) {
            OutConnections[(int)connection] = true;
        }

        public void RemoveInConnection(Direction connection) {
            InConnections[(int)connection] = false;
        }

        public void RemoveOutConnection(Direction connection) {
            OutConnections[(int)connection] = false;
        }
    }
}
