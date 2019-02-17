﻿using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Logger.Entities.Combat
{
    public struct AirForceInBattle
    {
        public AirForceGroupId Id { get; set; }
        public IReadOnlyList<SlotInBattleEntity> Squadrons { get; set; }

        public AirForceInBattle(AirForceGroup group)
        {
            Id = group.GroupId;
            Squadrons = group.Squadrons
                .Select(x => new SlotInBattleEntity
                {
                    Id = x.Equipment?.Info.Id ?? default,
                    Count = x.AircraftCount.Current,
                    MaxCount = x.AircraftCount.Max
                }).ToArray();
        }
    }
}
