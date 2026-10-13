using FluentAssertions;
using hsbc_test_20260930.q3;

namespace TestProject;

public class ThrottlerSlidingWindowTests
{
    [Fact]
    public void Constructor_WithStartTrue_ShouldStartImmediately()
    {
        var throttler = new ThrottlerSlidingWindow(10, TimeSpan.FromMilliseconds(10), start: true);

        throttler.TokenCount.Should().BeGreaterThanOrEqualTo(0);

        throttler.Stop();
    }

    [Fact]
    public async Task Start_ShouldBeginRefillingTokens()
    {
        var throttler = new ThrottlerSlidingWindow(10, TimeSpan.FromMilliseconds(50), start: false);

        throttler.Start();

        await Task.Delay(60);

        throttler.TokenCount.Should().BeGreaterThan(0);

        throttler.Stop();
    }

    [Fact]
    public async Task Stop_ShouldFreezeTokenRefill()
    {
        var throttler = new ThrottlerSlidingWindow(10, TimeSpan.FromMilliseconds(50), start: true);

        throttler.Stop();

        int tokensAtStop = throttler.TokenCount;

        await Task.Delay(30);

        throttler.TokenCount.Should().Be(tokensAtStop, "check if Stop() working");
    }

    [Fact]
    public async Task TokenCount_ShouldNotExceedMaxTokens()
    {
        var throttler = new ThrottlerSlidingWindow(10, TimeSpan.FromMilliseconds(10), start: true);

        await Task.Delay(30);

        throttler.TokenCount.Should().BeLessThanOrEqualTo(10, "check maxTokens");

        throttler.Stop();
    }

    [Fact]
    public async Task Restarting_Throttler_ShouldResetTimeAnchorAndContinueRefilling()
    {
        var throttler = new ThrottlerSlidingWindow(100, TimeSpan.FromSeconds(1), start: true);

        await Task.Delay(40);
        throttler.Stop();
        int tokensAfterFirstRun = throttler.TokenCount;

        tokensAfterFirstRun.Should().BeLessThan(100);

        await Task.Delay(50);

        throttler.Start();
        await Task.Delay(40);
        int tokensAfterSecondRun = throttler.TokenCount;

        tokensAfterSecondRun.Should().BeGreaterThan(tokensAfterFirstRun);

        throttler.Stop();
    }
}
