using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Xwellbehaved
{
    using Xunit;
    using Xunit.Abstractions;
    using Xwellbehaved.Infrastructure;
    using Xwellbehaved.Sdk;

    /// <summary>
    /// In order to prevent bugs due to incorrect code as a developer I want to run automated
    /// acceptance tests describing each feature of my product using scenarios.
    /// </summary>
    public class ScenarioFeature : Feature
    {
        private class FeatureWithAScenarioWithThreeSteps
        {
            [Scenario]
            public void Scenario()
            {
                "Step 1".x(() => { });

                "Step 2".x(() => { });

                "Step 3".x(() => { });
            }
        }

        private class TenStepsNamedAlphabeticallyBackwardsStartingWithZ
        {
            [Scenario]
            public static void Scenario()
            {
                "z".x(() => { });

                "y".x(() => { });

                "x".x(() => { });

                "w".x(() => { });

                "v".x(() => { });

                "u".x(() => { });

                "t".x(() => { });

                "s".x(() => { });

                "r".x(() => { });

                "q".x(() => { });
            }
        }

        private class FeatureWithAScenarioWithTwoPassingStepsAndOneFailingStep
        {
            [Scenario]
            public static void Scenario()
            {
                var i = 0;

                "Given 1".x(() => i = 1);

                "When I add 1".x(() => i += 1);

                "Then I have 3".x(() => i.AssertEqual(3));
            }
        }

        private class FeatureWithAScenarioBodyWhichThrowsAnException
        {
            [Scenario]
            public static void Scenario() => throw new InvalidOperationException();
        }

        private class AFailingStepAndTwoPassingStepsNamedAlphabeticallyBackwards
        {
            [Scenario]
            public static void Scenario()
            {
                "Step z".x(() => throw new NotImplementedException());

                "Step y".x(() => { });

                "Step x".x(() => { });
            }
        }

        private class FeatureWithANonStaticScenarioButNoDefaultConstructor
        {

#pragma warning disable IDE0060 // Remove unused parameter
            public FeatureWithANonStaticScenarioButNoDefaultConstructor(int ignored)
#pragma warning restore IDE0060 // Remove unused parameter
            {
            }

            [Scenario
                , SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for testing.")]
            public void Scenario() => "Given something".x(() => { });
        }

        private class FeatureWithAFailingConstructor
        {
            public FeatureWithAFailingConstructor() => throw new InvalidOperationException();

            [Scenario
                , SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for testing.")]
            public void Scenario() => "Given something".x(() => { });
        }

        private class FeatureWithAScenarioWithNoSteps
        {
            [Scenario]
            public void Scenario()
            {
            }
        }

        private class ScenarioWithANestedStep
        {
            [Scenario]
            public void Scenario()
            {

#pragma warning disable IDE0022 // Use expression body for methods
                "Given something".x(() => "With something nested".x(() => { }));
#pragma warning restore IDE0022 // Use expression body for methods

            }
        }

        // NOTE (adamralph): a plain xunit fact to prove that plain scenarios work in 2.x
        [Fact]
        public void ScenarioWithTwoPassingStepsAndOneFailingStepYieldsTwoPassesAndOneFail()
        {
            // arrange
            var feature = typeof(FeatureWithAScenarioWithTwoPassingStepsAndOneFailingStep);

            // act
            var results = this.Run<ITestResultMessage>(feature);

            // assert
            results.Length.AssertEqual(3);
            results.Take(2).All(result => result is ITestPassed).AssertTrue();
            results.Skip(2).All(result => result is ITestFailed).AssertTrue();
        }

        [Scenario]
        public void ScenarioWithThreeSteps(Type feature, IMessageSinkMessage[] messages, ITestResultMessage[] results)
        {
            "Given a feature with a scenario with three steps".x(
                () => feature = typeof(FeatureWithAScenarioWithThreeSteps));

            "When I run the scenarios".x(
                () => results = (messages = this.Run<IMessageSinkMessage>(feature))
                    .OfType<ITestResultMessage>().ToArray());

            "Then there should be three results".x(() => results.Length.AssertEqual(3));

            "And the first result should have a display name ending with 'Step 1'".x(
                () => results[0].Test.DisplayName.AssertEndsWith("Step 1"));

            "And the second result should have a display name ending with 'Step 2'".x(
                () => results[1].Test.DisplayName.AssertEndsWith("Step 2"));

            "And the third result should have a display name ending with 'Step 3'".x(
                () => results[2].Test.DisplayName.AssertEndsWith("Step 3"));

            "And the messages should satisfy the xunit message contract".x(
                () => messages.Select(message => message.GetType().Name).AssertContainsSequence(
                    "TestCollectionStarting"
                    , "TestClassStarting"
                    , "TestMethodStarting"
                    , "TestCaseStarting"
                    , "TestStarting"
                    , "TestPassed"
                    , "TestFinished"
                    , "TestStarting"
                    , "TestPassed"
                    , "TestFinished"
                    , "TestStarting"
                    , "TestPassed"
                    , "TestFinished"
                    , "TestCaseFinished"
                    , "TestMethodFinished"
                    , "TestClassFinished"
                    , "TestCollectionFinished")
            );
        }

        [Scenario]
        public void OrderingStepsByDisplayName(Type feature, ITestResultMessage[] results)
        {
            "Given ten steps named alphabetically backwards starting with 'z'".x(
                () => feature = typeof(TenStepsNamedAlphabeticallyBackwardsStartingWithZ));

            "When I run the scenarios".x(() => results = this.Run<ITestResultMessage>(feature));

            "And I sort the results by their display name".x(
                () => results = results.OrderBy(result => result.Test.DisplayName).ToArray());

            "Then a concatenation of the last character of each result display names should be 'zyxwvutsrq'".x(
                () => new string(results.Select(result => result.Test.DisplayName.Last()).ToArray()).AssertEqual("zyxwvutsrq"));
        }

        [Scenario]
        public void ScenarioWithTwoPassingStepsAndOneFailingStep(Type feature, ITestResultMessage[] results)
        {
            "Given a feature with a scenario with two passing steps and one failing step".x(
                () => feature = typeof(FeatureWithAScenarioWithTwoPassingStepsAndOneFailingStep));

            "When I run the scenarios".x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be three results".x(() => results.Length.AssertEqual(3));

            "And the first two results should be passes".x(() => results.Take(2).All(result => result is ITestPassed).AssertTrue());

            "And the third result should be a fail".x(() => results.Skip(2).All(result => result is ITestFailed).AssertTrue());
        }

        [Scenario]
        public void ScenarioBodyThrowsAnException(Type feature, Exception exception, ITestResultMessage[] results)
        {
            "Given a feature with a scenario body which throws an exception".x(
                () => feature = typeof(FeatureWithAScenarioBodyWhichThrowsAnException));

            "When I run the scenarios".x(() => exception = Record.Exception(
                () => results = this.Run<ITestResultMessage>(feature)));

            "Then no exception should be thrown".x(() => exception.AssertNull());

            "And the results should not be empty".x(() => results.AssertNotEmpty());

            "And each result should be a failure".x(() => results.All(result => result is ITestFailed).AssertTrue());
        }

        [Scenario]
        public void FeatureCannotBeConstructed(Type feature, Exception exception, ITestResultMessage[] results)
        {
            "Given a feature with a non-static scenario but no default constructor".x(
                () => feature = typeof(FeatureWithANonStaticScenarioButNoDefaultConstructor));

            "When I run the scenarios".x(() => exception = Record.Exception(
                () => results = this.Run<ITestResultMessage>(feature)));

            "Then no exception should be thrown".x(() => exception.AssertNull());

            "And the results should not be empty".x(() => results.AssertNotEmpty());

            "And each result should be a failure".x(() => results.All(result => result is ITestFailed).AssertTrue());
        }

        [Scenario]
        public void FeatureConstructionFails(Type feature, ITestFailed[] failures)
        {
            "Given a feature with a failing constructor".x(
                () => feature = typeof(FeatureWithAFailingConstructor));

            "When I run the scenarios".x(() => failures = this.Run<ITestFailed>(feature));

            "Then there should be one test failure".x(() => failures.Length.AssertEqual(1));
        }

        [Scenario]
        public void FailingStepThenPassingSteps(Type feature, ITestResultMessage[] results)
        {
            const StringComparison equivalentOf = StringComparison.InvariantCultureIgnoreCase;

            "Given a failing step and two passing steps named alphabetically backwards".x(
                () => feature = typeof(AFailingStepAndTwoPassingStepsNamedAlphabeticallyBackwards));

            "When I run the scenario".x(() => results = this.Run<ITestResultMessage>(feature));

            "And I sort the results by their display name".x(
                () => results = results.OrderBy(result => result.Test.DisplayName).ToArray());

            "Then the there should be three results".x(() => results.Length.AssertEqual(3));

            "Then the first result should be a failure".x(() => results[0].AssertIsAssignableFrom<ITestFailed>());

            "And the second and third results should be skips".x(() => results.Skip(1).All(result => result is ITestSkipped).AssertTrue());

            "And the second result should refer to the second step".x(() =>
                results[1].Test.DisplayName.AssertContains("Step y", equivalentOf));

            "And the third result should refer to the third step".x(() =>
                results[2].Test.DisplayName.AssertContains("Step x", equivalentOf));

            "And the second and third result messages should indicate that the first step failed".x(() =>
            {
                foreach (var result in results.Skip(1).Cast<ITestSkipped>())
                {
                    result.Reason.AssertContains("Failed to execute preceding step", equivalentOf);
                    result.Reason.AssertContains("Step z", equivalentOf);
                }
            });
        }

        [Scenario]
        public void ScenarioWithNoSteps(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario with no steps".x(
                () => feature = typeof(FeatureWithAScenarioWithNoSteps));

            "When I run the scenario".x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be one result".x(() => results.Length.AssertEqual(1));

            "And the result should be a pass".x(() => results.Single().AssertIsAssignableTo<ITestPassed>());
        }

        [Scenario]
        public void NullStepText() => ((string)null).x(() => { });

        [Scenario]
        public void NullStepBody() => "Given a null body".x((Action)null);

        [Scenario]
        public void NullContextualStepBody() => "Given a null body".x((Action<IStepContext>)null);

        [Scenario]
        public void NestedStep(Type feature, ITestResultMessage[] results)
        {
            "Given a scenario with a nested step".x(
                () => feature = typeof(ScenarioWithANestedStep));

            "When I run the scenario".x(() => results = this.Run<ITestResultMessage>(feature));

            "Then there should be one result".x(() => results.Length.AssertEqual(1));

            "And the result should be a fail".x(() => results.Single().AssertIsAssignableTo<ITestFailed>());
        }
    }
}
