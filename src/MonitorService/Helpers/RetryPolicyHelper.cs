using Polly;
using System;

namespace MonitorService.Helpers
{
    public static class RetryPolicyHelper
    {
        public static void RetryAction(int retryCount, int secondsToWait, Action method)
        {
            var genericPolicy = Policy.Handle<Exception>();
            Policy retryPolicy;

            if (retryCount == 0)
                retryPolicy = genericPolicy.WaitAndRetryForever(_ => TimeSpan.FromSeconds(secondsToWait));
            else
                retryPolicy = genericPolicy.WaitAndRetry(retryCount, _ => TimeSpan.FromSeconds(secondsToWait));

            retryPolicy.Execute(() => method());
        }

        public static T RetryFunc<T>(int retryCount, int secondsToWait, Func<T> method)
        {
            var genericPolicy = Policy.Handle<Exception>();
            Policy retryPolicy;

            if (retryCount == 0)
                retryPolicy = genericPolicy.WaitAndRetryForever(_ => TimeSpan.FromSeconds(secondsToWait));
            else
                retryPolicy = genericPolicy.WaitAndRetry(retryCount, _ => TimeSpan.FromSeconds(secondsToWait));

            return retryPolicy.Execute(() => method());
        }
    }
}
