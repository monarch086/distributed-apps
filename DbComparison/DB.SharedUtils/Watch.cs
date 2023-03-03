using System.Diagnostics;

namespace DB.SharedUtils
{
    public class Watch
    {
        private Stopwatch _watch;

        public void Start()
        {
            _watch = Stopwatch.StartNew();
        }

        public void Stop()
        {
            _watch.Stop();
            var elapsedMs = _watch.ElapsedMilliseconds;

            Console.WriteLine($"Elapsed time: {elapsedMs} ms.\n");
        }
    }
}
