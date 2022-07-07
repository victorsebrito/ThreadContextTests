using System.Collections;

namespace ThreadContextTests
{
    internal class Runners : IEnumerable<object[]>
    {
        public static Thread ThreadRunner(Action action)
        {
            var thread = new Thread(() => action());
            thread.Start();
            thread.Join();

            return thread;
        }

        public static Task LongRunningTaskRunner(Action action)
        {
            // TaskCreationOptions.LongRunning makes sure the task
            // will be executed in a new thread
            var task = Task.Factory.StartNew(action, TaskCreationOptions.LongRunning);
            task.Wait();

            return task;
        }

#pragma warning disable CS8974 // Converting method group to non-delegate type
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { ThreadRunner };
            yield return new object[] { LongRunningTaskRunner };
        }
#pragma warning restore CS8974 // Converting method group to non-delegate type

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
