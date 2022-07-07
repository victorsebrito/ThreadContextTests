using FluentAssertions;
using FluentAssertions.Execution;

namespace ThreadContextTests
{
    public class ContextTests
    {
        private static void SetData(object data) => Context.SetData(data);
        private static void GetData(ref object data) => data = Context.GetData();

        [Theory]
        [ClassData(typeof(Runners))]
        public void ShouldNotReplaceParentThreadValue(Func<Action, object> runner)
        {
            object parent1Object1 = null;
            object parent1Object2 = null;
            object child2Object1 = null;
            object child1Object1 = null;

            var parent1 = runner(() =>
            {
                SetData(new object());
                GetData(ref parent1Object1);

                var child1 = runner(() =>
                {
                    var child2 = runner(() =>
                    {
                        SetData(new object());
                        GetData(ref child2Object1);
                    });

                    GetData(ref child1Object1);
                });

                GetData(ref parent1Object2);
            });

            using (new AssertionScope())
            {
                parent1Object1.Should().NotBeNull();
                child2Object1.Should().NotBeSameAs(parent1Object1);
                child1Object1.Should().BeNull();
                parent1Object2.Should().BeSameAs(parent1Object1);
            }
        }

        [Theory]
        [ClassData(typeof(Runners))]
        public void ShouldNotShareValueBetweenSiblingThreads(Func<Action, object> runner)
        {
            object parent1Object1 = null;
            object child1Object1 = null;
            object child2Object1 = null;
            object parent2Object1 = null;

            var parent1 = runner(() =>
            {
                SetData(new object());
                GetData(ref parent1Object1);

                var child1 = runner(() =>
                {
                    SetData(new object());
                    GetData(ref child1Object1);
                });

                var child2 = runner(() =>
                {
                    GetData(ref child2Object1);
                });
            });

            var parent2 = runner(() =>
            {
                GetData(ref parent2Object1);
            });

            using (new AssertionScope())
            {
                parent1Object1.Should().NotBeNull();
                child1Object1.Should().NotBeSameAs(parent1Object1);
                child2Object1.Should().BeNull();
                parent2Object1.Should().BeNull();
            }
        }

        [Theory]
        [ClassData(typeof(Runners))]
        public void ShouldNotShareValueBetweenChildThreads(Func<Action, object> runner)
        {
            object parent1Object1 = null;
            object child1Object1 = null;
            object child2Object1 = null;

            var parent1 = runner(() =>
            {
                SetData(new object());
                GetData(ref parent1Object1);

                var child1 = runner(() =>
                {
                    GetData(ref child1Object1);
                    SetData(new object());

                    var child2 = runner(() =>
                    {
                        GetData(ref child2Object1);
                    });
                });
            });

            using (new AssertionScope())
            {
                parent1Object1.Should().NotBeNull();
                child1Object1.Should().BeNull();
                child2Object1.Should().BeNull();
            }
        }

        [Fact]
#if NET
        public void ShouldShareValueBetweenTasksOfSameThread()
#else
        public void ShouldNotShareValueBetweenTasksOfSameThread()
#endif
        {
            object parent1Object1 = null;
            object child1Object1 = null;
            object child2Object1 = null;

            Runners.ThreadRunner(() =>
            {
                SetData(new object());
                GetData(ref parent1Object1);

                Runners.TaskRunnerSameThread(() => GetData(ref child1Object1));
                Runners.TaskRunnerSameThread(() => GetData(ref child2Object1));
            });

            using (new AssertionScope())
            {
                parent1Object1.Should().NotBeNull();
#if NET
                child1Object1.Should().BeSameAs(parent1Object1);
                child2Object1.Should().BeSameAs(parent1Object1);
#else
                child1Object1.Should().BeNull();
                child2Object1.Should().BeNull();
#endif
            }
        }
    }
}