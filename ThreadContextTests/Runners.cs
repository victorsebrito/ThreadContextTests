using System.Collections;

namespace ThreadContextTests
{
    internal class Runners : IEnumerable<object[]>
    {
        private static Thread ThreadRunner(Action action)
        {
            var thread = new Thread(() => action());
            thread.Start();
            thread.Join();

            return thread;
        }

        private static Task TaskRunner(Action action)
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
            yield return new object[] { TaskRunner };
        }
#pragma warning restore CS8974 // Converting method group to non-delegate type

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
