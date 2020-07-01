using System;
using System.Collections.Generic;

namespace Xwellbehaved
{
    using FluentAssertions;
    using Infrastructure;
    using Xunit.Abstractions;

    public abstract class BackgroundEvaluationOrderFeatureOne : Feature
    {
        protected Guid BaseOneId { get; } = Guid.NewGuid();

        protected IList<Guid> Visited { get; } = new List<Guid>();

        [Background]
        public void BackgroundOne()
        {
            "Visited should not be null".x(() => this.Visited.Should().NotBeNull());

            "And visited should not be empty".x(() => this.Visited.Should().BeEmpty());

            "Then record background one visitation".x(() => this.Visited.Add(this.BaseOneId));
        }
    }

    public abstract class BackgroundEvaluationOrderFeatureTwo : BackgroundEvaluationOrderFeatureOne
    {
        protected Guid BaseTwoId { get; } = Guid.NewGuid();

        [Background]
        public void BackgroundTwo()
        {
            "Visited should have 1 item".x(() => this.Visited.Should().HaveCount(1));

            "Then record background two visitation".x(() => this.Visited.Add(this.BaseTwoId));
        }
    }

    public abstract class BackgroundEvaluationOrderFeatureThree : BackgroundEvaluationOrderFeatureTwo
    {
        protected Guid BaseThreeId { get; } = Guid.NewGuid();

        [Background]
        public void BackgroundThree()
        {
            "Visited should have 2 items".x(() => this.Visited.Should().HaveCount(2));

            "Record background three visitation".x(() => this.Visited.Add(this.BaseThreeId));
        }
    }

    public abstract class BackgroundEvaluationOrderFeatureFour : BackgroundEvaluationOrderFeatureThree
    {
        protected Guid BaseFourId { get; } = Guid.NewGuid();

        // TODO: TBD: introduce some backgrounds that yield specific results but only when in the proper order...
        [Background]
        public void BackgroundFour()
        {
            "Visited should have 3 items".x(() => this.Visited.Should().HaveCount(3));

            "Record background four visitation".x(() => this.Visited.Add(this.BaseFourId));
        }
    }

    // #2 MWP 2020-07-01 01:08:55 PM / added a series of Background Evaluation Order features which verify correct evaluation order.
    public class BackgroundEvaluationOrderFeature : BackgroundEvaluationOrderFeatureFour
    {
        /// <summary>
        /// We verify Background method visitation this way by building up the Feature hierarchy
        /// itself, rather than focusing on private subject classes. We need to do it this way in
        /// order to maintain the Visited collection properly throughout the test resolution life
        /// cycle.
        /// </summary>
        [Scenario]
        public void Backgrounds_should_be_visited_in_the_correct_order()
        {
            // And we have early detection in the sense of backgrounds doing a little preliminary verification.
            "Finally, Visited should appear in the expected order".x(() =>
                this.Visited.Should().Equal(
                    this.BaseOneId, this.BaseTwoId, this.BaseThreeId, this.BaseFourId));
        }
    }
}
