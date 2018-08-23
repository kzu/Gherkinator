namespace Gherkinator.Tests
{
    // NOTE: experiment on what codegen might look like from the .feature files.
    public static partial class Features
    {
        public static class RunnerFeature
        {
            public static class Background
            {
                public static class Given
                {
                    public const string GivenBackground = nameof(GivenBackground);
                }
            }
            public static class Scenarios
            {
                public static class RunnerScenario
                {
                    public static class Given
                    {
                        public const string AGiven = nameof(Given);
                        public const string AndGiven = nameof(AndGiven);
                    }
                    public static class When
                    {
                        public const string AWhen = nameof(When);
                        public const string AndWhen = nameof(AndWhen);
                    }
                    public static class Then
                    {
                        public const string AThen = nameof(Then);
                        public const string AndThen = nameof(AndThen);
                    }
                }
            }
        }
    }
}
