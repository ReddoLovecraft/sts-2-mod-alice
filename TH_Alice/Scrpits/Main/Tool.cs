using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Random;
using System.Collections.Generic;
using System.Linq;
using TH_Alice.Scrpits.Cards;
using TH_Alice.Scrpits.Dolls;
using TH_Alice.Scrpits.Main;
using TH_Alice.Scrpits.Powers;

namespace TH_Alice.TH_Alice.Scrpits.Main
{
    public class ToolBox
    {
        public static LocString L10NStatic(string entry)
        {
            return new LocString("static_hover_tips", entry);

        }
        public static int GetDollCount(Creature player)
        {
            int result = 0;
            foreach (Creature pet in player.Pets)
            {
                if (pet.IsAlive && pet.Monster is AliceDollMonsterModel)
                {
                    result++;
                }
            }
            return result;
        }
        public static int GetDollKind(Creature player)
        {
            HashSet<Type> kinds = new HashSet<Type>();
            foreach (Creature pet in player.Pets)
            {
                if (pet.IsAlive && pet.Monster is AliceDollMonsterModel)
                {
                    kinds.Add(pet.Monster.GetType());
                }
            }
            return kinds.Count;
        }
        public static async Task RecycleDolls(Creature player,bool PengLaiFirst=false)
        {
            if (player == null)
                return;
            await RecycleDolls(player,1,PengLaiFirst);
        }
        public static async Task RecycleDolls(Creature player, int amount, bool PengLaiFirst = false)
        {
            if (player == null)
                return;
            Player? owner = player.Player;
            CombatState? combat = player.CombatState;
            if (owner == null || combat == null)
            {
                return;
            }
            Player ownerPlayer = owner;
            CombatState combatState = combat;
            int cnt = 0;
            List<Creature> dolls = new List<Creature>();
            IEnumerable<Creature> pets = player.Pets.Where(p => p.IsAlive && p.Monster is AliceDollMonsterModel).ToList();
            if (PengLaiFirst)
            {
                foreach (Creature pet in pets)
                {
                    if (pet.Monster is PENGLAI)
                    {
                        dolls.Add(pet);
                        cnt++;
                        if (cnt >= amount)
                        {
                            break;
                        }
                    }
                }
            }
            if (cnt < amount)
            {
                foreach (Creature pet in pets)
                {
                    if (dolls.Contains(pet))
                    {
                        continue;
                    }
                    dolls.Add(pet);
                    cnt++;
                    if (cnt >= amount)
                    {
                        break;
                    }
                }
            }

            PlayerChoiceContext ctx = new ThrowingPlayerChoiceContext();
            for (int i = dolls.Count - 1; i >= 0; i--)
            {
                Creature dollCreature = dolls[i];
                if (dollCreature.Monster is AliceDollMonsterModel dollModel)
                {
                    await dollModel.Recycle(ctx);
                }
            }
        }

        public static int GetRecycleNum(Creature dollCreature, AliceDollMonsterModel dollModel)
        {
            int baseHp = dollModel.BaseHp;
            if (dollCreature.CurrentHp > baseHp)
            {
                return 3;
            }
            if (dollCreature.CurrentHp == baseHp)
            {
                return 2;
            }
            return 1;
        }
        public  static async Task MakeDoll<T>(Creature player, bool isWax = false) where T : AlicePowerModel
        {
            if (player == null)
                return;
		    SfxCmd.Play(AliceModInit.ToModSfxPath("ArtWorks/SFX/summon.wav"));

            if (player.HasPower<DollJudgmentPower>()) 
            {
                await PowerCmd.Apply<SpeedPotionPower>(player,player.GetPowerAmount<DollJudgmentPower>(),null,null);
            }
            if (player.HasPower<MasterPower>()) 
            {
                if (player.Player != null)
                {
                    await PlayerCmd.GainEnergy(player.GetPowerAmount<MasterPower>(), player.Player);
                }
            }
            if (player.HasPower<MakeSequencePower>()) 
            {
                await CreatureCmd.GainBlock(player,new MegaCrit.Sts2.Core.Localization.DynamicVars.BlockVar(player.GetPowerAmount<MakeSequencePower>(), MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered) , null);
            }

            Type dollType = typeof(T);
            if (dollType == typeof(ShangHaiPower))
            {
                await MakeDollMonster<SHANGHAI>(player, isWax);
                return;
            }
            if (dollType == typeof(XiZangPower))
            {
                await MakeDollMonster<XIZANG>(player, isWax);
                return;
            }
            if (dollType == typeof(PengLaiPower))
            {
                await MakeDollMonster<PENGLAI>(player, isWax);
                return;
            }
            if (dollType == typeof(HinaPower))
            {
                await MakeDollMonster<HINA>(player, isWax);
                return;
            }
            if (dollType == typeof(OrlPower))
            {
                await MakeDollMonster<ORL>(player, isWax);
                return;
            }
            if (dollType == typeof(FrancePower))
            {
                await MakeDollMonster<FRANCE>(player, isWax);
                return;
            }
            if (dollType == typeof(GoliathPower))
            {
                await MakeDollMonster<GOLIATH>(player, isWax);
                return;
            }
            if (dollType == typeof(RoundTablePower))
            {
                await MakeDollMonster<ROUNDTABLE>(player, isWax);
                return;
            }
            if (dollType == typeof(RussiaPower))
            {
                await MakeDollMonster<RUSSIA>(player, isWax);
                return;
            }
            if (dollType == typeof(LondonPower))
            {
                await MakeDollMonster<LONDON>(player, isWax);
                return;
            }
            if (dollType == typeof(BombPower))
            {
                await MakeDollMonster<BOMB>(player, isWax);
                return;
            }
            if (dollType == typeof(NetherlandPower))
            {
                await MakeDollMonster<NETHERLAND>(player, isWax);
                return;
            }
            if (dollType == typeof(CursePower))
            {
                await MakeDollMonster<CURSE>(player, isWax);
                return;
            }
        }

        private static async Task MakeDollMonster<TMonster>(Creature ownerCreature, bool isWax) where TMonster : MonsterModel
        {
            Player? owner = ownerCreature.Player;
            CombatState? combat = ownerCreature.CombatState;
            if (owner == null || combat == null || owner.PlayerCombatState == null)
            {
                return;
            }

            int maxCount = int.MaxValue;
            if (ModelDb.Monster<TMonster>() is AliceDollMonsterModel canonicalDoll)
            {
                maxCount = canonicalDoll.MaxCount;
            }

            List<Creature> existing = ownerCreature.Pets.Where(p => p.IsAlive && p.Monster is TMonster).ToList();
            if (existing.Count >= maxCount)
            {
                PlayerChoiceContext ctx = new ThrowingPlayerChoiceContext();
                foreach (Creature dollCreature in existing)
                {
                    if (dollCreature.Monster is AliceDollMonsterModel doll)
                    {
                        await doll.PerformMaxIntent(ctx);
                    }
                }
                return;
            }

            MonsterModel monster = ModelDb.Monster<TMonster>().ToMutable();
            if (monster is AliceDollMonsterModel dollModel)
            {
                dollModel.ConfigureAsWax(isWax);
            }

            Creature pet = combat.CreateCreature(monster, ownerCreature.Side, null);
            await PlayerCmd.AddPet(pet, owner);
            if (pet.Monster is AliceDollMonsterModel dollMonster)
            {
                dollMonster.CacheOwner(owner);
            }
            pet.PrepareForNextTurn(combat.HittableEnemies, rollNewMove: true);
            DollPlacement.Arrange(owner);
        }
        public static int GetRecycleNum(Creature player,AlicePowerModel apm) 
        {
            int res = 2;
            if(apm is ShangHaiPower) 
            {
               return apm.Amount > 10 ? ++res : apm.Amount<5? --res : res;
            }
            else if (apm is XiZangPower)
            {
                return apm.Amount > 6 ? ++res : apm.Amount < 3 ? --res : res;
            }
            else if (apm is PengLaiPower)
            {
                return apm.Amount > 4 ? ++res : apm.Amount < 2 ? --res : res;
            }
            else if (apm is HinaPower)
            {
                return apm.Amount > 5 ? ++res : apm.Amount < 2.5 ? --res : res;
            }
            else if (apm is OrlPower)
            {
                return apm.Amount > 10 ? ++res : apm.Amount < 5 ? --res : res;
            }
            else if (apm is FrancePower)
            {
                return apm.Amount > 10 ? ++res : apm.Amount < 5 ? --res : res;
            }
            else if (apm is GoliathPower)
            {
                return apm.Amount > 20 ? ++res : apm.Amount < 10 ? --res : res;
            }
            else if (apm is RoundTablePower)
            {
                return apm.Amount > 12 ? ++res : apm.Amount < 6 ? --res : res;
            }
            else if (apm is RussiaPower)
            {
                return apm.Amount > 8 ? ++res : apm.Amount < 4 ? --res : res;
            }
            else if (apm is LondonPower)
            {
                return apm.Amount > 12 ? ++res : apm.Amount < 6 ? --res : res;
            }
            else if (apm is BombPower)
            {
                return apm.Amount > 2 ? ++res : apm.Amount < 1 ? --res : res;
            }
            else if (apm is NetherlandPower)
            {
                return apm.Amount > 30 ? ++res : apm.Amount < 15 ? --res : res;
            }
            else if (apm is CursePower)
             {
                return apm.Amount > 6 ? ++res : apm.Amount < 3 ? --res : res;
            }
            return 2;

        }
        public static int GetDebuffTotalCount(Creature target) 
        {
            int result = 0;
            foreach(PowerModel debuff in target.Powers) 
            {
                if(debuff.Type==PowerType.Debuff) 
                {
                    if (debuff.Amount > 0)
                        result += debuff.Amount;
                    else
                        result++;
                }
            }
            return result;

        }
        public static int GetDebuffKind(Creature target)
        {
            int result = 0;
            foreach (PowerModel debuff in target.Powers)
            {
                if (debuff.Type == PowerType.Debuff)
                {
                        result++;
                }
            }
            return result;
        }
        public  static async Task MakeRandomDoll(Creature player,bool isWax=false)
        {
            if (player == null)
                return;
            Rng rng = player.Player.RunState.Rng.CombatCardGeneration;
            int randomNumber = rng.NextInt(1, 14);
            switch(randomNumber)
            {
                case 1:
                    await MakeDoll<ShangHaiPower>(player,isWax);
                    break;
                case 2:
                    await MakeDoll<PengLaiPower>(player, isWax);
                    break;
                case 3:
                    await MakeDoll<XiZangPower>(player, isWax);
                    break;
                case 4:
                    await MakeDoll<HinaPower>(player, isWax);
                    break;
                case 5:
                    await MakeDoll<GoliathPower>(player, isWax);
                    break;
                case 6:
                    await MakeDoll<RoundTablePower>(player, isWax);
                    break;
                case 7:
                    await MakeDoll<OrlPower>(player, isWax);
                    break;
                case 8:
                    await MakeDoll<FrancePower>(player, isWax);
                    break;
                case 9:
                    await MakeDoll<RussiaPower>(player, isWax);
                    break;
                case 10:
                    await MakeDoll<NetherlandPower>(player, isWax);
                    break;
                case 11:
                    await MakeDoll<LondonPower>(player, isWax);
                    break;
                case 12:
                    await MakeDoll<CursePower>(player, isWax);
                    break;
                case 13:
                    await MakeDoll<BombPower>(player, isWax);
                    break;
                default: await MakeDoll<ShangHaiPower>(player, isWax); break;
            }
        }

    }
    
}
