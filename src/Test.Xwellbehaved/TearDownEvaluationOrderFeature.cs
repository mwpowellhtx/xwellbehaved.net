using System;
using System.Collections.Generic;

namespace Xwellbehaved
{
    using Infrastructure;
    using Xunit;

    public abstract class TearDownEvaluationOrderFeatureOne : Feature
    {
        protected Guid BaseOneId { get; } = Guid.NewGuid();

        protected IList<Guid> Visited { get; } = new List<Guid>();

        [Scenario]
        public void Scenario()
        {

#pragma warning disable IDE0022 // Use expression body for methods
            "At least one Scenario is required in order to discover Support methods".x(() => true.AssertTrue());

        }

        [TearDown]
        public void TearDownOne()
        {
            "Visited should have 3 item".x(() => this.Visited.Count.AssertEqual(3));

            "Then record teardown one visitation".x(() => this.Visited.Add(this.BaseOneId));
        }
    }

    public abstract class TearDownEvaluationOrderFeatureTwo : TearDownEvaluationOrderFeatureOne
    {
        protected Guid BaseTwoId { get; } = Guid.NewGuid();

        [TearDown]
        public void TearDownTwo()
        {
            "Visited should have 2 item".x(() => this.Visited.Count.AssertEqual(2));

            "Then record teardown two visitation".x(() => this.Visited.Add(this.BaseTwoId));
        }
    }

    public abstract class TearDownEvaluationOrderFeatureThree : TearDownEvaluationOrderFeatureTwo
    {
        protected Guid BaseThreeId { get; } = Guid.NewGuid();

        [TearDown]
        public void TearDownThree()
        {
            "Visited should have 1 items".x(() => this.Visited.Count.AssertEqual(1));

            "Record teardown three visitation".x(() => this.Visited.Add(this.BaseThreeId));
        }
    }

    public abstract class TearDownEvaluationOrderFeatureFour : TearDownEvaluationOrderFeatureThree
    {
        protected Guid BaseFourId { get; } = Guid.NewGuid();

        /// <summary>
        /// Some of which should admitted be done during background. Also remembering that
        /// the Scenario Background is like a stack in term of TearDown, the stack unwinds,
        /// so we expect Visitation to occur in reverse order.
        /// </summary>
        [TearDown]
        public void TearDownFour()
        {
            "Visited should not be null".x(() => this.Visited.AssertNotNull());

            "And visited should not empty".x(() => this.Visited.AssertCollectionEmpty());

            "Record teardown four visitation".x(() => this.Visited.Add(this.BaseFourId));
        }
    }

    // #2 MWP 2020-07-01 01:08:55 PM / added a series of TearDown Evaluation Order features which verify correct evaluation order.
    public class TearDownEvaluationOrderFeature : TearDownEvaluationOrderFeatureFour
    {
        /// <summary>
        /// We verify TearDown method visitation this way by building up the Feature hierarchy
        /// itself, rather than focusing on private subject classes. We need to do it this way in
        /// order to maintain the Visited collection properly throughout the test resolution life
        /// cycle.
        /// </summary>
        /// <see cref="IDisposable"/>
        /// <see cref="Dispose(bool)"/>
        [Scenario]
        public void Scenario_occurs_before_TearDown()
        {
            // And we have early detection in the sense of TearDowns doing a little preliminary verification.
            "Remember, Scenario occurs before TearDown, so nothing Visted yet".x(
                () => this.Visited.AssertCollectionEmpty()
            );
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // We should be able to verify the TearDown visitation did occur on disposal, however.
                this.Visited.AssertEqual(new[] { this.BaseFourId, this.BaseThreeId, this.BaseTwoId, this.BaseOneId });
            }

            base.Dispose(disposing);
        }
    }
}
