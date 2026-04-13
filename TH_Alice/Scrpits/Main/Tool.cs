using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Random;
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
            foreach(PowerModel power in player.Powers)
            {
               if(power is AlicePowerModel&&((AlicePowerModel)power).IsDollPower)
                {
                    result++;
                }
            }
            return result;
        }
        public static int GetDollKind(Creature player)
        {
            int result = 0;
            if(player.HasPower<ShangHaiPower>())
                result++;
            if(player.HasPower<PengLaiPower>())
                result++;
            if(player.HasPower<XiZangPower>())
                result++;
            if(player.HasPower<RoundTablePower>())
                result++;
            if(player.HasPower<OrlPower>())
                result++;
            if(player.HasPower<FrancePower>())
                result++;
            if(player.HasPower<NetherlandPower>())
                result++;
            if(player.HasPower<LondonPower>())
                result++;
            if(player.HasPower<CursePower>())
                result++;
            if(player.HasPower<BombPower>())
                    result++;
            if(player.HasPower<RussiaPower>())
                result++;
            if(player.HasPower<HinaPower>())
                result++;
            if(player.HasPower<GoliathPower>())
                result++;
                return result;
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
            int cnt = 0;
            List<AlicePowerModel> apms = new List<AlicePowerModel>();
            if (PengLaiFirst)
            {
                foreach (PowerModel pm in player.Powers)
                {
                    if (pm is PengLaiPower plm)
                    {
                        apms.Add(plm);
                        cnt++;
                        if (cnt >= amount)
                            break;
                    }
                }
            }
            if(cnt<amount)
            foreach (PowerModel pm in player.Powers)
            {
                if (pm is AlicePowerModel apm&&apm.IsDollPower && !apms.Contains(apm))
                {
                    apms.Add(apm);
                    cnt++;
                    if (cnt >= amount)
                            break;
                }
            }
            for (int i = apms.Count - 1; i >= 0; i--)
            {
                await apms[i].APM_Remove(true, apms[i]);
                apms.RemoveAt(i);
            }
        }
        public  static async Task MakeDoll<T>(Creature player, bool isWax = false) where T : AlicePowerModel
        {
		    SfxCmd.Play(AliceModInit.ToModSfxPath("ArtWorks/SFX/summon.wav"));
            if (player == null)
                return;
            T inst= ModelDb.Power<T>();
            if (player.HasPower<DollJudgmentPower>()) 
            {
                await PowerCmd.Apply<DexterityPower>(player,player.GetPowerAmount<DollJudgmentPower>(),null,null);
            }
            if (player.HasPower<MasterPower>()) 
            {
                await PlayerCmd.GainEnergy(player.GetPowerAmount<MasterPower>(), player.Player);
            }
            if (player.HasPower<MakeSequencePower>()) 
            {
                await CreatureCmd.GainBlock(player,new MegaCrit.Sts2.Core.Localization.DynamicVars.BlockVar(player.GetPowerAmount<MakeSequencePower>(), MegaCrit.Sts2.Core.ValueProps.ValueProp.Unpowered) , null);
            }
            if(inst is ShangHaiPower) 
            {
                (await PowerCmd.Apply<ShangHaiPower>(player, 10, player, null)).SetDamageAndWax(6,isWax);
                return;
            }
            if (inst is XiZangPower)
            {
                (await PowerCmd.Apply<XiZangPower>(player, 6, player, null)).SetDamageAndWax(0, isWax);
                return;
            }
            if (inst is PengLaiPower)
            {
                (await PowerCmd.Apply<PengLaiPower>(player, 4, player, null)).SetDamageAndWax(4, isWax);
                return;
            }
            if (inst is HinaPower)
            {
                (await PowerCmd.Apply<HinaPower>(player, 5, player, null)).SetDamageAndWax(1, isWax);
                return;
            }
            if (inst is OrlPower)
            {
                (await PowerCmd.Apply<OrlPower>(player, 10, player, null)).SetDamageAndWax(6, isWax);
                return;
            }
            if (inst is FrancePower)
            {
                (await PowerCmd.Apply<FrancePower>(player, 10, player, null)).SetDamageAndWax(4, isWax);
                return;
            }
            if (inst is GoliathPower)
            {
                (await PowerCmd.Apply<GoliathPower>(player, 20, player, null)).SetDamageAndWax(4, isWax);
                return;
            }
            if (inst is RoundTablePower)
            {
                (await PowerCmd.Apply<RoundTablePower>(player, 12, player, null)).SetDamageAndWax(2, isWax);
                return;
            }
            if (inst is RussiaPower)
            {
                (await PowerCmd.Apply<RussiaPower>(player, 8, player, null)).SetDamageAndWax(2, isWax);
                return;
            }
            if (inst is LondonPower)
            {
                (await PowerCmd.Apply<LondonPower>(player, 12, player, null)).SetDamageAndWax(3, isWax);
                return;
            }
            if (inst is BombPower)
            {
                (await PowerCmd.Apply<BombPower>(player, 2, player, null)).SetDamageAndWax(8, isWax);
                return;
            }
            if (inst is NetherlandPower)
            {
                (await PowerCmd.Apply<NetherlandPower>(player, 30, player, null)).SetDamageAndWax(10, isWax);
                return;
            }
            if (inst is CursePower)
            {
                (await PowerCmd.Apply<CursePower>(player, 6, player, null)).SetDamageAndWax(1, isWax);
                return;
            }
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
