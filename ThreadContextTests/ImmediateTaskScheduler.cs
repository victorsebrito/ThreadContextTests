namespace ThreadContextTests
{
    /// <summary>
    /// Task scheduler that executes the task immeditely
    /// in the same thread (doesn't schedule)
    /// </summary>
    internal class ImmediateTaskScheduler : TaskScheduler
    {
        protected override IEnumerable<Task> GetScheduledTasks()
        {
            return Enumerable.Empty<Task>();
        }

        protected override void QueueTask(Task task)
        {
            base.TryExecuteTask(task);
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            base.TryExecuteTask(task);
            return true;
        }
    }
}
