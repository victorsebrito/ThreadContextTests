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

        public static Task TaskRunnerDifferentThread(Action action)
        {
            // TaskCreationOptions.LongRunning makes sure the task
            // will be executed in a new thread
            var task = Task.Factory.StartNew(action, TaskCreationOptions.LongRunning);
            task.Wait();

            return task;
        }

        public static Task TaskRunnerSameThread(Action action)
        {
            var scheduler = new ImmediateTaskScheduler();

            var task = Task.Factory.StartNew(
                    action,
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    scheduler);
                
            task.Wait();

            return task;
        }

#pragma warning disable CS8974 // Converting method group to non-delegate type
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { ThreadRunner };
            yield return new object[] { TaskRunnerDifferentThread };
        }
#pragma warning restore CS8974 // Converting method group to non-delegate type

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
