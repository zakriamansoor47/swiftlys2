using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SwiftlyS2.Shared;
using SwiftlyS2.Shared.Commands;
using SwiftlyS2.Shared.GameEventDefinitions;
using SwiftlyS2.Shared.GameEvents;
using SwiftlyS2.Shared.NetMessages;
using SwiftlyS2.Shared.Misc;
using SwiftlyS2.Shared.Natives;
using SwiftlyS2.Shared.Plugins;
using SwiftlyS2.Shared.SchemaDefinitions;
using SwiftlyS2.Shared.ProtobufDefinitions;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using SwiftlyS2.Shared.Events;
using SwiftlyS2.Shared.Memory;
using YamlDotNet.Core.Tokens;
using Dapper;
using SwiftlyS2.Shared.Sounds;
using SwiftlyS2.Shared.EntitySystem;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using SwiftlyS2.Shared.Players;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
using BenchmarkDotNet.Toolchains.InProcess.Emit;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;

namespace TestPlugin;

public class InProcessConfig : ManualConfig
{
    public InProcessConfig()
    {
        AddLogger(ConsoleLogger.Default);
        AddJob(Job.Default
            .WithToolchain(new InProcessNoEmitToolchain(true))
            .WithId("InProcess"));
    }
}


[PluginMetadata(Id = "testplugin", Version = "1.0.0")]
public class TestPlugin : BasePlugin
{

  public TestPlugin(ISwiftlyCore core) : base(core)
  {
    Console.WriteLine("[TestPlugin] TestPlugin constructed successfully!");
  }



  [Command("be")]
  public void Test2Command(ICommandContext context)
  {
    BenchContext.Controller = context.Sender!.RequiredController;
    BenchmarkRunner.Run<PlayerBenchmarks>(new InProcessConfig());
  }

  public override void Load(bool hotReload)
  {
    // Core.Event.OnConsoleOutput += (@event) =>
    // {
    //   Console.WriteLine($"[TestPlugin] ConsoleOutput: {@event.Message}");
    // };

    // Core.Event.OnCommandExecuteHook += (@event) =>
    // {
    //   if (@event.HookMode == HookMode.Pre) return;
    //   Core.Logger.LogInformation("CommandExecute: {name} with {args}", @event.OriginalName, @event.OriginalArgs.Length > 0 ? string.Join(" ", @event.OriginalArgs) : "no args");
    //   // @event.SetCommandName("test");
    // };
    Core.Engine.ExecuteCommandWithBuffer("@ping", (buffer) =>
    {
      Console.WriteLine($"pong: {buffer}");
    });

    Core.GameEvent.HookPre<EventShowSurvivalRespawnStatus>(@event =>
    {
      @event.LocToken = "test";
      return HookResult.Continue;
    });

    Core.Configuration
      .InitializeJsonWithModel<TestConfig>("test.jsonc", "Main")
      .Configure((builder) =>
      {
        builder.AddJsonFile("test.jsonc", optional: false, reloadOnChange: true);
      });

    ServiceCollection services = new();

    services
      .AddSwiftly(Core);

    Core.Event.OnPrecacheResource += (@event) =>
    {
      @event.AddItem("soundevents/mvp_anthem.vsndevts");
    };

    Core.Event.OnConVarValueChanged += (@event) =>
    {
      Console.WriteLine($"ConVar {@event.ConVarName} changed from {@event.OldValue} to {@event.NewValue} by player {@event.PlayerId}");
    };
  }
}