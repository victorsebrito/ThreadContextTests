using FluentAssertions;
using FluentAssertions.Execution;

namespace ThreadContextTests
{
    public class LogicalContextTests
    {
        private static void SetData(object data) => Context.LogicalSetData(data);
        private static void GetData(ref object data) => data = Context.LogicalGetData();

        [Theory]
        [ClassData(typeof(Runners))]
        public void ShouldNotReplaceParentThreadValue(Func<Action, object> runner)
        {
            object parent1Object1 = null;
            object parent1Object2 = null;
            object child2Object1 = null;
            object child1Object1 = null;
            object child1Object2 = null;

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

                    SetData(new object());
                    GetData(ref child1Object2);
                });

                GetData(ref parent1Object2);
            });

            using (new AssertionScope())
            {
                parent1Object1.Should().NotBeNull();
                child2Object1.Should().NotBeSameAs(parent1Object1);
                child1Object1.Should().BeSameAs(parent1Object1);
                child1Object2.Should().NotBeSameAs(child1Object1);
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
                child2Object1.Should().BeSameAs(parent1Object1);
                parent2Object1.Should().BeNull();
            }
        }

        [Theory]
        [ClassData(typeof(Runners))]
        public void ShouldShareValueBetweenChildThreads(Func<Action, object> runner)
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

                    var child2 = runner(() =>
                    {
                        GetData(ref child2Object1);
                    });
                });
            });

            using (new AssertionScope())
            {
                parent1Object1.Should().NotBeNull();
                child1Object1.Should().BeSameAs(parent1Object1);
                child2Object1.Should().BeSameAs(parent1Object1);
            }
        }
    }
}
