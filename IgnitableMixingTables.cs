﻿/*
 * Copyright (C) 2024 Game4Freak.io
 * This mod is provided under the Game4Freak EULA.
 * Full legal terms can be found at https://game4freak.io/eula/
 */

using Facepunch;
using System.Collections.Generic;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Ignitable Mixing Tables", "VisEntities", "1.0.0")]
    [Description(" ")]
    public class IgnitableMixingTables : RustPlugin
    {
        #region Fields

        private static IgnitableMixingTables _plugin;

        #endregion Fields

        #region Oxide Hooks

        private void Init()
        {
            _plugin = this;
        }

        private void Unload()
        {
            _plugin = null;
        }

        private void OnInputUpdate(Igniter igniter, int inputAmount, int slot)
        {
            if (igniter == null)
                return;

            if (inputAmount > 0 && igniter.CanIgnite())
                IgniteMixingTablesInRange(igniter);
        }

        #endregion Oxide Hooks

        #region Mixing Table Ignition

        private void IgniteMixingTablesInRange(Igniter igniter)
        {
            List<MixingTable> mixingTablesInRange = Pool.GetList<MixingTable>();
            Vis.Entities(igniter.LineOfSightEyes.position, igniter.IgniteRange, mixingTablesInRange, 1237019409, QueryTriggerInteraction.Collide);

            foreach (MixingTable mixingTable in mixingTablesInRange)
            {
                if (mixingTable.IsVisible(igniter.LineOfSightEyes.position, float.PositiveInfinity) && !mixingTable.HasFlag(BaseEntity.Flags.On))
                {
                    BasePlayer player = FindPlayerById(mixingTable.OwnerID);
                    if (player == null)
                        player = FindPlayerById(igniter.OwnerID);

                    if (player != null)
                        mixingTable.StartMixing(player);
                }
            }

            Pool.FreeList(ref mixingTablesInRange);
        }

        #endregion Mixing Table Ignition

        #region Helper Functions

        public static BasePlayer FindPlayerById(ulong playerId)
        {
            return RelationshipManager.FindByID(playerId);
        }

        #endregion Helper Functions
    }
}