using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StackExchange.Redis;

namespace Microsoft.AspNetCore.SignalR.Redis
{
    internal class RedisSubscriptionManager
    {
        private readonly ISubscriber _redisSubscriber;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim();
        private readonly Dictionary<string, int> _refCounts = new Dictionary<string, int>();

        public RedisSubscriptionManager(ISubscriber redisSubscriber)
        {
            _redisSubscriber = redisSubscriber;
        }

        /// <summary>
        /// Subscribes to the channel. This will only actually add a subscription if there isn't one already.
        /// If there is an existing subscription, <paramref name="action"/> is COMPLETELY IGNORED.
        /// </summary>
        public async Task SubscribeAsync(string channel, Action<RedisChannel, RedisValue> action)
        {
            await _lock.WaitAsync();
            try
            {
                // Check if there's already a ref-count value for this
                if (_refCounts.TryGetValue(channel, out var currentCount))
                {
                    // There is, just update it and return
                    _refCounts[channel] = currentCount;
                }
                else
                {
                    // We need to subscribe, do so and store a count
                    _refCounts[channel] = 1;
                    await _redisSubscriber.SubscribeAsync(channel, action);
                }
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}
