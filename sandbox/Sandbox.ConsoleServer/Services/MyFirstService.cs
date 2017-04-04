﻿#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

using MagicOnion;
using MagicOnion.Server;
using System.Threading.Tasks;
using System;

namespace Sandbox.ConsoleServer.Services
{
    public class MyFirstService : ServiceBase<IMyFirstService>, IMyFirstService
    {
        public async Task<UnaryResult<string>> SumAsync(int x, int y)
        {
            return UnaryResult((x + y).ToString());
        }

        public UnaryResult<string> SumAsync2(int x, int y)
        {
            return UnaryResult((x + y).ToString());
        }

        public async Task<ClientStreamingResult<int, string>> StreamingOne()
        {
            var stream = GetClientStreamingContext<int, string>();

            await stream.ForEachAsync(x =>
            {

            });

            return stream.Result("finished");
        }

        public async Task<ServerStreamingResult<string>> StreamingTwo(int x, int y, int z)
        {
            var stream = GetServerStreamingContext<string>();

            var acc = 0;
            for (int i = 0; i < z; i++)
            {
                acc = acc + x + y;
                await stream.WriteAsync(acc.ToString());
            }

            return stream.Result();
        }

        public ServerStreamingResult<string> StreamingTwo2(int x, int y, int z)
        {
            var stream = GetServerStreamingContext<string>();
            return stream.Result();
        }

        public async Task<DuplexStreamingResult<int, string>> StreamingThree()
        {
            var stream = GetDuplexStreamingContext<int, string>();

            var waitTask = Task.Run(async () =>
            {
                await stream.ForEachAsync(x =>
                {
                });
            });

            await stream.WriteAsync("test1");
            await stream.WriteAsync("test2");
            await stream.WriteAsync("finish");

            await waitTask;

            return stream.Result();
        }
    }
}

#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously