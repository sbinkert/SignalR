// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace SignalRSamples.Hubs
{
    public class Streaming : Hub
    {
        public IObservable<int> ObservableCounter(int count, int delay)
        {
            return Observable.Create(
                async (IObserver<int> observer) =>
                {
                    for (int i = 0; i < 100; i++)
                    {
                        if (i == 5)
                        {
                            observer.OnError(new Exception("Test."));
                        }

                        observer.OnNext(i);
                        await Task.Delay(1000);
                    }
                });

            //Subject<int> rateChanged = new Subject<int>();
            //rateChanged.OnError(new Exception("Test."));
            //return rateChanged.AsObservable();
        }

        public ChannelReader<int> ChannelCounter(int count, int delay)
        {
            var channel = Channel.CreateUnbounded<int>();

            Task.Run(async () =>
            {
                for (var i = 0; i < count; i++)
                {
                    await channel.Writer.WriteAsync(i);
                    await Task.Delay(delay);
                }

                channel.Writer.TryComplete();
            });

            return channel.Reader;
        }
    }
}
