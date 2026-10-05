namespace hsbc_test_20260930.interfaces;

public interface Throttler
{
    // check if we can proceed (poll)
    ThrottleResult shouldProceed();
    // subscribe to be told when we can proceed (Push)
    void notifyWhenCanProceed();

    enum ThrottleResult
    {
        PROCEED, // publish, aggregate etc
        DO_NOT_PROCEED //
    }
}