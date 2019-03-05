﻿using System;
using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Game;
using Sakuno.ING.Game.Json;
using Sakuno.ING.Game.Logger;
using Sakuno.ING.Game.Logger.Entities.Combat;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.Combat;
using Sakuno.ING.Game.Models.MasterData;
using Sakuno.ING.Localization;
using Sakuno.ING.Shell;
using Sakuno.ING.ViewModels.Logging.Combat;

namespace Sakuno.ING.ViewModels.Logging
{
    public class BattleVM : ITimedEntity
    {
        private readonly BattleLogsVM owner;
        private readonly BattleEntity entity;
        internal BattleVM(BattleLogsVM owner, BattleEntity entity)
        {
            this.owner = owner;
            this.entity = entity;
            Map = owner.masterData.MapInfos[entity.MapId];
            Drop = owner.masterData.ShipInfos[entity.ShipDropped];
            UseItemDrop = owner.masterData.UseItems[entity.UseItemAcquired];
        }

        public DateTimeOffset TimeStamp => entity.TimeStamp;
        public MapInfo Map { get; }
        public string MapName => entity.MapName;
        public int RouteId => entity.RouteId;
        public string WinRank => entity.Rank switch
        {
            BattleRank.Perfect => owner.rankPerfect,
            BattleRank.S => owner.rankS,
            BattleRank.A => owner.rankA,
            BattleRank.B => owner.rankB,
            BattleRank.C => owner.rankC,
            BattleRank.D => owner.rankD,
            BattleRank.E => owner.rankE,
            _ => null
        };
        public string EnemyFleetName => entity.EnemyFleetName;
        public ShipInfo Drop { get; }
        public UseItemInfo UseItemDrop { get; }

        public void LoadDetail()
        {
            if (entity.Details is null) return;
            var battle = new Battle
            (
                entity.Details.SortieFleetState?.Select(x => new LoggedShip(owner.masterData, x)).ToArray(),
                entity.Details.SortieFleet2State?.Select(x => new LoggedShip(owner.masterData, x)).ToArray(),
                entity.CombinedFleetType,
                entity.BattleKind
            );
            TryAppend(battle, entity.Details.FirstBattleDetail);
            TryAppend(battle, entity.Details.SecondBattleDetail);
            owner.shell.ShowViewWithParameter("BattleLogDetail", battle);
        }

        private void TryAppend(Battle battle, string json)
        {
            if (json is null) return;
            var api = owner.provider.Deserialize<BattleDetailJson>(json);
            var raw = new RawBattle(api, TimeStamp < RawBattle.EnemyIdChangeTime);
            battle.Append(owner.masterData, raw);
        }
    }

    [Export(typeof(BattleLogsVM))]
    public class BattleLogsVM : LogsVM<BattleVM>, IDisposable
    {
        private readonly Logger logger;
        internal readonly GameProvider provider;
        private readonly ILocalizationService localization;
        internal readonly IShell shell;
        internal readonly MasterDataRoot masterData;
        internal readonly string rankPerfect, rankS, rankA, rankB, rankC, rankD, rankE;
        private LoggerContext context;

        public BattleLogsVM(Logger logger, NavalBase navalBase, GameProvider provider, ILocalizationService localization, IShell shell)
        {
            this.logger = logger;
            this.provider = provider;
            masterData = navalBase.MasterData;
            this.localization = localization;
            this.shell = shell;
            rankPerfect = "SS";
            rankS = "S";
            rankA = "A";
            rankB = "B";
            rankC = "C";
            rankD = "D";
            rankE = "E";
        }

        private protected override FilterVM<BattleVM>[] CreateFilters()
            => new[]
            {
                new FilterVM<BattleVM>("Map",
                    x => x.Map.Id,
                    x => x.Map.Id.ToString()),
                new FilterVM<BattleVM>("WinRank",
                    x => x.WinRank.GetHashCode(),
                    x => x.WinRank),
            };

        private protected override IReadOnlyCollection<BattleVM> GetEntities()
        {
            if (!logger.PlayerLoaded) return Array.Empty<BattleVM>();
            if (context is null)
                context = logger.CreateContext();
            return context.BattleTable.AsEnumerable()
                .Select(e => new BattleVM(this, e)).ToList();
        }

        public void Dispose()
        {
            if (context != null)
            {
                context.Dispose();
                context = null;
            }
        }
    }
}