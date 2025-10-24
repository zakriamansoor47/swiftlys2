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

public class TestConfig
{
  public string Name { get; set; }
  public int Age { get; set; }
}


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

  [GameEventHandler(HookMode.Pre)]
  public HookResult OnPlayerSpawn(EventPlayerSpawn @event)
  {
    if (!@event.UserIdPlayer.IsValid)
    {
      return HookResult.Continue;
    }
    var player = @event.UserIdPlayer.RequiredController;
    if (player.InGameMoneyServices?.IsValid == true)
    {
      player.InGameMoneyServices.Account = Core.ConVar.Find<int>("mp_maxmoney")?.Value ?? 16000;
      player.InGameMoneyServices.AccountUpdated();
    }
    return HookResult.Continue;
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


    // var provider = services.BuildServiceProvider();

    // provider.GetRequiredService<TestService>();


    // Host.CreateDefaultBuilder()
    //   .ConfigureLogging((context, logging) => {
    //     logging.AddConsole();
    //   })
    //   .ConfigureAppConfiguration((context, config) => {
    //     config.SetBasePath(Core.Configuration.GetBasePath());
    //     config.AddJsonFile("test.jsonc", optional: false, reloadOnChange: true);
    //   })
    //   .ConfigureServices((context, services) => {
    //     services.AddOptionsWithValidateOnStart<IOptionsMonitor<TestConfig>>()
    //       .Bind(context.Configuration.GetSection("Main"));
    //   })
    //   .Build();

    // This can be used everywhere and the value will be updated when the config is changed
    // Console.WriteLine(config.CurrentValue.Age);


    // var config = new TestConfig();

    // throw new Exception("TestPlugin loaded");

    // Core.

    int i = 0;

    // var token2 = Core.Scheduler.Repeat(10, () => {
    //   Console.WriteLine(Core.Engine.TickCount);
    //   Console.WriteLine("TestPlugin Timer");
    // });
    Core.Logger.LogInformation("TestPlugin loaded");

    using var se = new SoundEvent();

    // var func = Core.Memory.GetUnmanagedFunctionByAddress<Test>(Core.Memory.GetAddressBySignature(Library.Server, "AAAAA")!.Value);

    // func.CallOriginal(1, 2);

    // func.Call(1, 2);

    // func.AddHook((next) => {
    //   return (a, b) => {
    //     Console.WriteLine("TestPlugin Hook " + a + " " + b);
    //     next()(a, b);
    //   };
    // });


    // Entrypoint

    // Core.Event.OnTick += () => {
    //   Console.WriteLine("TestPlugin OnTick ");
    // };

    // Core.Event.OnClientConnected += (@event) => {
    //   Console.WriteLine("TestPlugin OnClientConnected " + @event.PlayerId);
    // };

    // Core.Event.OnClientPutInServer += (@event) => {
    //   Console.WriteLine("TestPlugin OnClientPutInServer " + @event.PlayerId);
    // };

    Core.Event.OnClientDisconnected += (@event) =>
    {
      Console.WriteLine("TestPlugin OnClientDisconnected " + @event.PlayerId);
    };
    Core.Event.OnTick += () =>
    {
      int i = 0;
    };


    // Core.Event.OnClientProcessUsercmds += (@event) => {
    //   foreach(var usercmd in @event.Usercmds) {
    //     usercmd.Base.ButtonsPb.Buttonstate1 &= 1UL << (int)GameButtons.Ctrl;
    //     usercmd.Base.ButtonsPb.Buttonstate2 &= 1UL << (int)GameButtons.Ctrl;
    //     usercmd.Base.ButtonsPb.Buttonstate3 &= 1UL << (int)GameButtons.Ctrl;
    //   }
    // };

    // Core.NetMessage.HookClientMessage<CCLCMsg_Move>((msg, id) => {
    //   Console.WriteLine("TestPlugin OnClientMove ");
    //   Console.WriteLine(BitConverter.ToString(msg.Data));
    //   return HookResult.Continue;
    // });

    // Core.Event.OnEntityTakeDamage += (@event) => {
    //   Console.WriteLine("TestPlugin OnEntityTakeDamage " + @event.Entity.Entity?.DesignerName + " " + @event.Info.HitGroupId);
    // };

    // Core.Event.OnTick += () => {

    //   Console.WriteLine("TestPlugin OnTick");
    // };

    // Core.Event.OnEntityCreated += (ev) => {
    //   var entity = ev.Entity;
    //   entity.Entity.DesignerName = "a";
    //   Console.WriteLine("TestPlugin OnEntityCreated " + ev.Entity.Entity?.DesignerName);
    // };


    using CEntityKeyValues kv = new();

    kv.SetBool("test", true);

    Console.WriteLine(kv.Get<bool>("test2"));


    CUtlStringToken token = new("hello");
    Console.WriteLine($"2");
  }

  CEntityKeyValues kv { get; set; }
  CEntityInstance entity { get; set; }


  [Command("tt")]
  public void TestCommand(ICommandContext context)
  {
    // token2?.Cancel();
    // kv = new();
    // kv.SetString("test", "SAFE");



    // _Core.Logger.LogInformation("!@#");

    // _Core.Logger.LogInformation(_Core.GameData.GetSignature("CEntityInstance::AcceptInput").ToString());

    // entity = _Core.EntitySystem.CreateEntityByDesignerName<CPointWorldText>("point_worldtext");
    // entity.DispatchSpawn(kv);
    // Console.WriteLine("Spawned entity with keyvalues");

    int j = 0;

    var cvar = Core.ConVar.Find<bool>("sv_cheats")!;
    Console.WriteLine(cvar);
    Console.WriteLine(cvar.Value);
    var cvar2 = Core.ConVar.Find<bool>("sv_autobunnyhopping")!;
    Console.WriteLine(cvar2);
    Console.WriteLine(cvar2.Value);

    var cvar3 = Core.ConVar.Create<string>("sw_test_cvar", "Test cvar", "ABCDEFG");
    Console.WriteLine(cvar3);
    Console.WriteLine(cvar3.Value);

    var cvar4 = Core.ConVar.Find<bool>("r_drawworld")!;

    cvar2.ReplicateToClient(0, true);

    cvar4.QueryClient(0, (value) =>
    {
      Console.WriteLine("QueryCallback " + value);
    });
  }

  [Command("w")]
  public void TestCommand1(ICommandContext context)
  {
    var attacker = context.Sender!;
    var weapons = attacker.Pawn!.WeaponServices!.MyWeapons;
    foreach (var weaponHandle in weapons)
    {
      var weapon = weaponHandle.Value?.As<CCSWeaponBase>();
      if (weapon == null)
        return;

      Console.WriteLine($"Weapon: {weapon.DesignerName}");
    }

  }

  [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
  delegate nint DispatchSpawnDelegate(nint pEntity, nint pKV);
  int order = 0;

  IUnmanagedFunction<DispatchSpawnDelegate>? _dispatchspawn;

  [Command("h1")]
  public void TestCommand2(ICommandContext context)
  {
    var token = Core.Scheduler.DelayAndRepeat(500, 1000, () =>
    {

    });

    var addres = Core.GameData.GetSignature("CBaseEntity::DispatchSpawn");
    var func = Core.Memory.GetUnmanagedFunctionByAddress<DispatchSpawnDelegate>(addres);

    var guid = func.AddHook((next) =>
    {
      return (pEntity, pKV) =>
      {
        Console.WriteLine("TestPlugin DispatchSpawn " + order++);
        return next()(pEntity, pKV);
      };
    });

    _dispatchspawn.AddHook((next) =>
    {
      return (pEntity, pKV) =>
      {
        Console.WriteLine("TestPlugin DispatchSpawn2 " + order++);
        return next()(pEntity, pKV);
      };
    });

  }

  [EventListener<EventDelegates.OnEntityCreated>]
  public void OnEntityCreated(IOnEntityCreatedEvent @event)
  {
    // @event.Entity.Entity.DesignerName = "abc";
    Console.WriteLine("TestPlugin OnEntityCreated222 " + @event.Entity.Entity?.DesignerName);
  }

  Guid _hookId = Guid.Empty;

  [Command("bad")]
  public void TestCommandBad(ICommandContext context)
  {
    try
    {
      var isValveDS = Core.EntitySystem.GetGameRules()!.IsValveDS;
    }
    catch (Exception e)
    {
      Core.Logger.LogWarning("{Exception}", e.Message);
    }

    try
    {
      Core.EntitySystem.GetGameRules()!.IsValveDS = true;
    }
    catch (Exception e)
    {
      Core.Logger.LogWarning("{Exception}", e.Message);
    }

    try
    {
      Core.EntitySystem.GetGameRules()!.IsValveDSUpdated();
    }
    catch (Exception e)
    {
      Core.Logger.LogWarning("{Exception}", e.Message);
    }
  }

  [Command("h2")]
  public void TestCommand3(ICommandContext context)
  {
    Core.Command.HookClientCommand((playerId, commandLine) =>
    {
      Console.WriteLine("TestPlugin HookClientCommand " + playerId + " " + commandLine);
      return HookResult.Continue;
    });
    Console.WriteLine(Core.GameData.GetSignature("CBaseEntity::DispatchSpawn"));
    var ent = Core.EntitySystem.CreateEntity<CPointWorldText>();
    ent.DispatchSpawn();
  }

  [Command("tt3")]
  public void TestCommand33(ICommandContext context)
  {
  }

  [Command("tt4")]
  public void TestCommand4(ICommandContext context)
  {
    Console.WriteLine(Core.Permission.PlayerHasPermission(7656, context.Args[0]));
  }

  [Command("tt5")]
  public void TestCommand5(ICommandContext context)
  {
    Console.WriteLine("TestPlugin TestCommand5");
  }

  [Command("tt6", permission: "tt6")]
  public void TestCommand6(ICommandContext context)
  {
    Console.WriteLine("TestPlugin TestCommand6");
  }

  [Command("tt7")]
  public void TestCommand7(ICommandContext context)
  {
    Core.Engine.ExecuteCommandWithBuffer("@ping", (buffer) =>
    {
      Console.WriteLine($"pong: {buffer}");
    });
  }

  [GameEventHandler(HookMode.Pre)]
  public HookResult TestGameEventHandler(EventPlayerJump @e)
  {
    Console.WriteLine(@e.UserIdController.PlayerName);
    return HookResult.Continue;
  }

  [ServerNetMessageHandler]
  public HookResult TestServerNetMessageHandler(CCSUsrMsg_SendPlayerItemDrops msg)
  {

    Console.WriteLine("FIRED");
    // foreach(var item in msg.Accessor) {
    //   Console.WriteLine($"TestPlugin ServerNetMessageHandler: {item.EconItem.Defindex}");
    // }
    return HookResult.Continue;
  }

  [Command("menu")]
  public void MenuCommand(ICommandContext context)
  {
    var player = context.Sender!;
    var menu = Core.Menus.CreateMenu("Test Menu");

    menu.Builder
      .AddButton("Button 1", (ctx) =>
      {
        player.SendMessage(MessageType.Chat, "You clicked Button 1");
      })
      .AddButton("Button 2", (ctx) =>
      {
        player.SendMessage(MessageType.Chat, "You clicked Button 2");
      })
      .AddButton("Button 3", (ctx) =>
      {
        player.SendMessage(MessageType.Chat, "You clicked Button 3");
      })
      .AddButton("Button 4", (ctx) =>
      {
        player.SendMessage(MessageType.Chat, "You clicked Button 4");
      })
      .AddButton("Button 5", (ctx) =>
      {
        player.SendMessage(MessageType.Chat, "You clicked Button 5");
      })
      .AddButton("Button 6", (ctx) =>
      {
        player.SendMessage(MessageType.Chat, "You clicked Button 6");
      })
      .AddButton("Button 7", (ctx) =>
      {
        player.SendMessage(MessageType.Chat, "You clicked Button 7");
      })
      .AddButton("Button 8", (ctx) =>
      {
        player.SendMessage(MessageType.Chat, "You clicked Button 8");
      })
      .AddSeparator()
      .AddText("hello!", size: SwiftlyS2.Shared.Menus.IMenuTextSize.ExtraLarge)
      .SetColor(new(0, 186, 105, 255))
      .AutoClose(15f)
      .HasSound(true)
      .ForceFreeze();

    Core.Menus.OpenMenu(player, menu);
  }

  public override void Unload()
  {
    Console.WriteLine("TestPlugin unloaded");
  }
}