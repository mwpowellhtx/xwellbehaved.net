using System;
using System.Globalization;
using System.Threading;

namespace Xwellbehaved
{
    using Xunit;
    using Xwellbehaved.Infrastructure;

    public class TestClassFeature : Feature
    {
        private class InstanceScenarioWithThreeStepsInADisposableType : IDisposable
        {
            private static int _instanceCount;
            private readonly int _instanceNumber;
            private int _disposalCount;

            public InstanceScenarioWithThreeStepsInADisposableType()
            {
                this._instanceNumber = Interlocked.Increment(ref _instanceCount);
                typeof(TestClassFeature).SaveTestEvent(
                    string.Concat("created", this._instanceNumber.ToString(CultureInfo.InvariantCulture)));
            }

            [Scenario]
            public void Scenario()
            {
                "Given".x(() => typeof(TestClassFeature).SaveTestEvent("step1"));

                "When".x(() => typeof(TestClassFeature).SaveTestEvent("step2"));

                "Then".x(() => typeof(TestClassFeature).SaveTestEvent("step3"));
            }

            public void Dispose()
            {
                var @event = string.Concat("disposed"
                    , this._instanceNumber.ToString(CultureInfo.InvariantCulture)
                    , "."
                    , (++this._disposalCount).ToString(CultureInfo.InvariantCulture));

                typeof(TestClassFeature).SaveTestEvent(@event);
            }
        }

        [Background]
        public void Background() => "Given no events have occurred".x(
            () => typeof(TestClassFeature).EnqueueFeatureForDisposal(this));

        [Scenario]
        public void SingleScenario(Type feature)
        {
            "Given an instance scenario with three steps in a disposable type".x(
                () => feature = typeof(InstanceScenarioWithThreeStepsInADisposableType));

            "When I run the scenario".x(() => this.Run(feature));

            "Then an instance of the type is created and disposed once either side of the step execution".x(() =>
                typeof(TestClassFeature).GetTestEvents().AssertEqual(
                    new[] { "created1", "step1", "step2", "step3", "disposed1.1" }));
        }
    }
}
