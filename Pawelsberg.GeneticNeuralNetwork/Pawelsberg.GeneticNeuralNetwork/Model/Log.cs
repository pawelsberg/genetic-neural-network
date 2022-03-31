namespace Pawelsberg.GeneticNeuralNetwork.Model
{
    public class Log
    {
        private readonly Queue<string> _queue = new Queue<string>();
        private readonly object _lock;
        private int _maxLength;

        public IEnumerable<string> LogItems
        {
            get
            {
                lock (_lock) return new List<string>(_queue);
            }
        }
        public int MaxLength
        {
            get { return _maxLength; }
            set
            {
                lock (_lock)
                {
                    _maxLength = value;
                    Dequeue();
                }
            }
        }

        public Log(int maxLength)
        {
            _lock = new object();
            MaxLength = maxLength;
        }

        private void Dequeue()
        {
            lock (_lock)
            {
                while (_queue.Count > MaxLength)
                    _queue.Dequeue();
            }
        }
        public void Enqueue(string item)
        {
            lock (_lock)
            {
                _queue.Enqueue(item);
                Dequeue();
            }
        }
    }
}