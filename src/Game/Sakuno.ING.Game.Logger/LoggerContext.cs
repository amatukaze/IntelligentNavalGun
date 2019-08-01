﻿using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sakuno.ING.Game.Logger.Binary;
using Sakuno.ING.Game.Logger.Entities;
using Sakuno.ING.Game.Logger.Entities.Combat;
using Sakuno.ING.Game.Logger.Entities.Homeport;
using Sakuno.ING.Game.Models.MasterData;

[assembly: InternalsVisibleTo("Sakuno.ING.Game.Logger.Design")]
[assembly: InternalsVisibleTo("Sakuno.ING.Game.Logger.Tests")]
namespace Sakuno.ING.Game.Logger
{
    public class LoggerContext : DbContext
    {
        internal LoggerContext(DbContextOptions<LoggerContext> options) : base(options)
        {
            ChangeTracker.AutoDetectChangesEnabled = false;
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<ShipCreationEntity> ShipCreationTable { get; protected set; }
        public DbSet<EquipmentCreationEntity> EquipmentCreationTable { get; protected set; }
        public DbSet<ExpeditionCompletionEntity> ExpeditionCompletionTable { get; protected set; }
        public DbSet<BattleEntity> BattleTable { get; protected set; }
        public DbSet<ExerciseEntity> ExerciseTable { get; protected set; }

        public DbSet<BattleConsumptionEntity> BattleConsumptionTable { get; protected set; }
        public DbSet<MaterialsChangeEntity> MaterialsChangeTable { get; protected set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<ShipCreationEntity>(e =>
                {
                    e.Property(x => x.TimeStamp).HasConversion(new DateTimeOffsetToBinaryConverter());
                    e.Property(x => x.ShipBuilt).HasConversion<int>(v => v, v => (ShipInfoId)v);
                    e.Property(x => x.Secretary).HasConversion<int>(v => v, v => (ShipInfoId)v);
                });
            modelBuilder
                .Entity<EquipmentCreationEntity>(e =>
                {
                    e.Property(x => x.TimeStamp).HasConversion(new DateTimeOffsetToBinaryConverter());
                    e.Property(x => x.EquipmentCreated).HasConversion<int>(v => v, v => (EquipmentInfoId)v);
                    e.Property(x => x.Secretary).HasConversion<int>(v => v, v => (ShipInfoId)v);
                });
            modelBuilder
                .Entity<ExpeditionCompletionEntity>(e =>
                {
                    e.Property(x => x.TimeStamp).HasConversion(new DateTimeOffsetToBinaryConverter());
                    e.Property(x => x.ExpeditionId).HasConversion<int>(v => v, v => (ExpeditionId)v);
                });
            modelBuilder
                .Entity<BattleEntity>(e =>
                {
                    e.Property(x => x.TimeStamp).HasConversion(new DateTimeOffsetToBinaryConverter());
                    e.OwnsOne(x => x.Details, d =>
                    {
                        d.Property(x => x.SortieFleetState).HasConversion(x => x.Store(), x => BinaryObjectExtensions.ParseFleet(x));
                        d.Property(x => x.SortieFleet2State).HasConversion(x => x.Store(), x => BinaryObjectExtensions.ParseFleet(x));
                        d.Property(x => x.SupportFleetState).HasConversion(x => x.Store(), x => BinaryObjectExtensions.ParseFleet(x));
                        d.Property(x => x.LbasState).HasConversion(x => x.Store(), x => BinaryObjectExtensions.ParseAirForce(x));
                        d.ToTable("BattleDetails");
                    });
                    e.Property(x => x.CompletionTime).HasConversion(new DateTimeOffsetToBinaryConverter());
                    e.Property(x => x.MapId).HasConversion<int>(v => v, v => (MapId)v);
                    e.HasIndex(x => x.MapId);
                    e.Property(x => x.ShipDropped).HasConversion<int?>(v => v, v => (ShipInfoId?)v);
                    e.Property(x => x.UseItemAcquired).HasConversion<int?>(v => v, v => (UseItemId?)v);
                });
            modelBuilder
                .Entity<ExerciseEntity>(e =>
                {
                    e.Property(x => x.TimeStamp).HasConversion(new DateTimeOffsetToBinaryConverter());
                    e.OwnsOne(x => x.Details, d =>
                    {
                        d.Property(x => x.SortieFleetState).HasConversion(x => x.Store(), x => BinaryObjectExtensions.ParseFleet(x));
                        d.Ignore(x => x.SortieFleet2State);
                        d.Ignore(x => x.SupportFleetState);
                        d.Ignore(x => x.LbasState);
                        d.Ignore(x => x.LandBaseDefence);
                        d.ToTable("ExerciseDetails");
                    });
                });

            modelBuilder
                .Entity<MaterialsChangeEntity>(e =>
                {
                    e.Property(x => x.TimeStamp).HasConversion(new DateTimeOffsetToBinaryConverter());
                });
            modelBuilder
                .Entity<BattleConsumptionEntity>(e =>
                {
                    e.Property(x => x.TimeStamp).HasConversion(new DateTimeOffsetToBinaryConverter());
                    e.Property(x => x.MapId).HasConversion<int>(v => v, v => (MapId)v);
                });
        }
    }
}
