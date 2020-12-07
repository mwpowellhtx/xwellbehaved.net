using System;
using System.Collections.Generic;
using System.Linq;

namespace Xwellbehaved
{
    using Infrastructure;
    using Xunit;

    public abstract class EndToEndAnnotationIntegrationFeatureOne : Feature
    {
        /// <summary>
        /// <c>1</c>
        /// </summary>
        private int Level { get; } = 1;

        /// <summary>
        /// Gets the ExpectedCount, <see cref="Level"/> minus 1.
        /// </summary>
        /// <remarks>Yes, from a class design perspective, these are all private within
        /// each class Level because the Expected must be correct according to that Level
        /// Background, Scenario, and corresponding Tear Down.</remarks>
        private int ExpectedCount => this.Level - 1;

        /// <summary>
        /// gets the ExpectedIndex, similar to <see cref="ExpectedCount"/>, <see cref="Level"/>
        /// minus 1.
        /// </summary>
        private int ExpectedIndex => this.Level - 1;

        protected static Guid BaseOneId { get; } = Guid.NewGuid();

        protected IList<Guid> Visited { get; } = new List<Guid>();

        /// <summary>
        /// Verifies that <see cref="Visited"/> <paramref name="actual"/> Exists.
        /// </summary>
        /// <param name="actual"></param>
        /// <returns>The <see cref="Visited"/> instance following Verification.</returns>
        protected IList<Guid> VerifyVisitedExists(Guid actual) =>
            this.Visited.AssertNotNull().AssertCollectionNotEmpty().AssertEqual(actual, x => x.Last());

        /// <summary>
        /// Verifies that <see cref="Visited"/> <paramref name="actual"/> Does Not Exist.
        /// </summary>
        /// <param name="actual"></param>
        /// <returns>The <see cref="Visited"/> instance following Verification.</returns>
        protected IList<Guid> VerifyVisitedDoesNotExist(Guid actual) =>
            this.Visited.AssertNotNull().AssertFalse(x => x.Contains(actual));

        [Background]
        public void BackgroundOne()
        {
            void OnBackgroundOne() =>
                this.VerifyVisitedDoesNotExist(BaseOneId)
                    .AssertEqual(this.ExpectedCount, x => x.Count).Add(BaseOneId);

#pragma warning disable IDE0022 // Use expression body for methods
            $"[{this.Level}] Background visited".x(OnBackgroundOne);
        }

        [Scenario]
        public void ScenarioOne()
        {
            void OnScenarioOne()
            {
                this.Visited.AssertEqual(3, x => x.Count);
                this.Visited.AssertTrue(x => x.Contains(BaseOneId));
                this.Visited.AssertEqual(this.ExpectedIndex, x => x.IndexOf(BaseOneId));
            }

            $"[{this.Level}] Scenario visited".x(OnScenarioOne);
        }

        [TearDown]
        public void TearDownOne()
        {
            void OnTearDownOne() =>
                this.VerifyVisitedExists(BaseOneId)
                    .AssertTrue(x => x.Remove(BaseOneId))
                    .AssertEqual(this.ExpectedCount, x => x.Count);

            $"[{this.Level}] Tear down visited".x(OnTearDownOne);
        }
    }

    public abstract class EndToEndAnnotationIntegrationFeatureTwo : EndToEndAnnotationIntegrationFeatureOne
    {
        /// <summary>
        /// <c>2</c>
        /// </summary>
        private int Level { get; } = 2;

        /// <summary>
        /// Gets the ExpectedCount, <see cref="Level"/> minus 1.
        /// </summary>
        private int ExpectedCount => this.Level - 1;

        /// <summary>
        /// gets the ExpectedIndex, similar to <see cref="ExpectedCount"/>, <see cref="Level"/>
        /// minus 1.
        /// </summary>
        private int ExpectedIndex => this.Level - 1;

        protected static Guid BaseTwoId { get; } = Guid.NewGuid();

        [Background]
        public void BackgroundTwo()
        {
            void OnBackgroundTwo() =>
                this.VerifyVisitedDoesNotExist(BaseTwoId)
                    .AssertEqual(this.ExpectedCount, x => x.Count).Add(BaseTwoId);

            $"[{this.Level}] Background visited".x(OnBackgroundTwo);
        }

        [Scenario]
        public void ScenarioTwo()
        {
            void OnScenarioTwo()
            {
                this.Visited.AssertEqual(3, x => x.Count);
                this.Visited.AssertTrue(x => x.Contains(BaseTwoId));
                this.Visited.AssertEqual(this.ExpectedIndex, x => x.IndexOf(BaseTwoId));
            }

            $"[{this.Level}] Scenario visited".x(OnScenarioTwo);
        }

        [TearDown]
        public void TearDownTwo()
        {
            void OnTearDownTwo() =>
                this.VerifyVisitedExists(BaseTwoId)
                    .AssertTrue(x => x.Remove(BaseTwoId))
                    .AssertEqual(this.ExpectedCount, x => x.Count);

            $"[{this.Level}] Tear down visited".x(OnTearDownTwo);
        }
    }

    public abstract class EndToEndAnnotationIntegrationFeatureThree : EndToEndAnnotationIntegrationFeatureTwo
    {
        /// <summary>
        /// <c>3</c>
        /// </summary>
        private int Level { get; } = 3;

        /// <summary>
        /// Gets the ExpectedCount, <see cref="Level"/> minus 1.
        /// </summary>
        private int ExpectedCount => this.Level - 1;

        /// <summary>
        /// gets the ExpectedIndex, similar to <see cref="ExpectedCount"/>, <see cref="Level"/>
        /// minus 1.
        /// </summary>
        private int ExpectedIndex => this.Level - 1;

        protected static Guid BaseThreeId { get; } = Guid.NewGuid();

        [Background]
        public void BackgroundThree()
        {
            void OnBackgroundThree() =>
                this.VerifyVisitedDoesNotExist(BaseThreeId)
                    .AssertEqual(this.ExpectedCount, x => x.Count).Add(BaseThreeId);

            $"[{this.Level}] Background visited".x(OnBackgroundThree);
        }

        [Scenario]
        public void ScenarioThree()
        {
            void OnScenarioThree()
            {
                this.Visited.AssertEqual(3, x => x.Count);
                this.Visited.AssertTrue(x => x.Contains(BaseThreeId));
                this.Visited.AssertEqual(this.ExpectedIndex, x => x.IndexOf(BaseThreeId));
            }

            $"[{this.Level}] Scenario visited".x(OnScenarioThree);
        }

        [TearDown]
        public void TearDownThree()
        {
            void OnTearDownThree() =>
                this.VerifyVisitedExists(BaseThreeId)
                    .AssertTrue(x => x.Remove(BaseThreeId))
                    .AssertEqual(this.ExpectedCount, x => x.Count);

            $"[{this.Level}] Tear down visited".x(OnTearDownThree);
        }
    }

    /// <summary>
    /// Ensures that we have correct end to end integration between Background, Scenario,
    /// and TearDown invocations.
    /// </summary>
    public class EndToEndAnnotationIntegrationFeature : EndToEndAnnotationIntegrationFeatureThree
    {
        /// <summary>
        /// We verify Background method visitation this way by building up the Feature hierarchy
        /// itself, rather than focusing on private subject classes. We need to do it this way in
        /// order to maintain the Visited collection properly throughout the test resolution life
        /// cycle.
        /// </summary>
        [Scenario]
        public void IntegrationScenario()
        {
            // And we have early detection in the sense of backgrounds doing a little preliminary verification.
            "Finally, Visited should appear in the expected order".x(
                () => this.Visited.AssertEqual(new[] { BaseOneId, BaseTwoId, BaseThreeId })
            );
        }
    }
}
