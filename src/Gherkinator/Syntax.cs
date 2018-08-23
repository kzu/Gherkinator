﻿using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Gherkinator
{
    /// <summary>
    /// Provides the fluent entry point <see cref="Scenario"/>, to be used by 
    /// importing the class statically, using:
    /// <c>
    /// using static Gherkinator.Syntax;
    /// </c>
    /// </summary>
    public static class Syntax
    {
        /// <summary>
        /// Creates the scenario builder for the given feature and scenario.
        /// </summary>
        /// <param name="featureName">Name of the feature, which is used as the file name (with an added `.feature` extension) 
        /// to locate the feature definition. If not provided, <paramref name="testFile"/> will be used to infer it from the caller file name.</param>
        /// <param name="scenarioName">Name of the scenario, which must exist in the feature file. If not provided, 
        /// the <paramref name="testMethod"/> will be used to infer it from the caller method, by replacing its underscores with spaces.</param>
        /// <param name="testFile">Provides the default value for <paramref name="featureName"/>.</param>
        /// <param name="testMethod">Provides the default value for <paramref name="scenarioName"/>.</param>
        /// <returns></returns>
        public static ScenarioBuilder Scenario(string scenarioName = null, string featureName = null, [CallerFilePath] string testFile = null, [CallerMemberName] string testMethod = null)
            => new ScenarioBuilder(
                (featureName ?? Path.GetFileNameWithoutExtension(testFile)) + ".feature", 
                scenarioName ?? testMethod.Replace('_', ' '))
                .BeforeGiven(state => state.Set(nameof(testFile), testFile))
                .BeforeGiven(state => state.Set(nameof(testMethod), testMethod));
    }
}