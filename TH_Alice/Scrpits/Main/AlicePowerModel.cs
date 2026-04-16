using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TH_Alice.Scrpits.Cards;
using TH_Alice.Scrpits.Powers;
using TH_Alice.TH_Alice.Scrpits.Main;

namespace TH_Alice.Scrpits.Main
{
    public abstract class AlicePowerModel: CustomPowerModel
    {
        public virtual bool IsDollPower=>false;
        public virtual bool IsWaxDoll => IsWax;
        public bool IsWax = false;
        public int WaxCount = 3;
        protected override IEnumerable<DynamicVar> CanonicalVars
        {
            get
            {
                return [new DamageVar(6m, ValueProp.Unpowered)];
            }
        }
        public override async Task BeforeTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side != base.Owner.Side)
            {
                return;
            }
            if(IsWaxDoll)
                {
                    WaxCount--;
                    if (WaxCount <= 0)
                    {
                      await APM_Remove(false,this);
                    }
            }

        }
        public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            if (player != base.Owner.Player)
            {
                return;
            }
            if(player.Creature.HasPower<AutoPower>())
                for(int i=0;i<player.Creature.GetPower<AutoPower>().Amount;i++)
                {
                    DollAction(choiceContext);
                }
          
            //执行一些buff的判断
        }
        public virtual async Task DollAction(PlayerChoiceContext choiceContext,bool Repeatable=true) { }
        public async Task APM_Remove(bool IsRecyle, AlicePowerModel power) 
        {
            //调用这个函数来移除这个Power，触发移除时的相关效果
            if (Owner.HasPower<DollJudgmentPower>())
            {
                await PowerCmd.Apply<StrengthPower>(Owner, Owner.GetPowerAmount<DollJudgmentPower>(), null, null);
            }

            bool shouldSpawnXiZang = Owner.HasPower<XiZangPower>() || power is XiZangPower;

            if (IsRecyle)
            {
                //触发回收的逻辑
		        SfxCmd.Play(AliceModInit.ToModSfxPath("ArtWorks/SFX/recycle.wav"));
                int amt = ToolBox.GetRecycleNum(Owner, power);
                List<DollPart> list = new List<DollPart>();
                for (int i = 0; i < amt; i++)
                {
                    list.Add(Owner.CombatState.CreateCard<DollPart>(Owner.Player));
                }
                CardCmd.PreviewCardPileAdd(await CardPileCmd.AddGeneratedCardsToCombat(list, PileType.Draw, addedByPlayer: true, CardPilePosition.Random));
                if (shouldSpawnXiZang)
                {
                    if (Owner.Player?.Character is AliceCharacter character)
                    {
                        await CreatureCmd.TriggerAnim(base.Owner, "Summon", character.CastAnimDelay);
                    }
                    await ToolBox.MakeDoll<XiZangPower>(Owner);
                }
                if (power is HinaPower)
                {
                    List<PowerModel> to_remove=new List<PowerModel>();
                    foreach (PowerModel debuff in Owner.Powers) 
                    {
                        if (debuff.Type == PowerType.Debuff) 
                        {
                            to_remove.Add(debuff);
                        }
                    }
                    for (int i = to_remove.Count - 1; i >= 0; i--)
                    {
                        await PowerCmd.Remove(to_remove[i]);
                        to_remove.RemoveAt(i);
                    }
                   
                }
                if (Owner.HasPower<GirlDollPower>()) 
                {
                    foreach (PowerModel pm in Owner.Powers)
                    {
                        if (pm is AlicePowerModel apm&&apm.IsDollPower)
                        {
                            await PowerCmd.ModifyAmount(apm,Owner.GetPowerAmount<GirlDollPower>(),null,null);
                        }
                    }
                }
            }
            else 
            {
		        SfxCmd.Play(AliceModInit.ToModSfxPath("ArtWorks/SFX/dolldie.wav"));
                //触发死亡时的逻辑
                if (Owner.Player?.GetRelic<BottlePowder>() != null)
                {
                    (await PowerCmd.Apply<TheBombPower>(Owner, 1, null, null)).SetDamage(power.Amount);
                }
                if (shouldSpawnXiZang)
                {
                    if (Owner.Player?.Character is AliceCharacter character)
                    {
                        await CreatureCmd.TriggerAnim(base.Owner, "Summon", character.CastAnimDelay);
                    }
                    await ToolBox.MakeDoll<XiZangPower>(Owner);
                }
                if (power is HinaPower)
                {
                    List<PowerModel> to_remove = new List<PowerModel>();
                    foreach (PowerModel debuff in Owner.Powers)
                    {
                        if (debuff.Type == PowerType.Debuff)
                        {
                            to_remove.Add(debuff);
                        }
                    }
                    for (int i = to_remove.Count - 1; i >= 0; i--)
                    {
                        await PowerCmd.Remove(to_remove[i]);
                        to_remove.RemoveAt(i);
                    }

                }
            }
            await PowerCmd.Remove(this);
        }
        public async Task APM_Decline(decimal amt)
        {
            //调用这个函数来减少这个Power层数，触发相关效果
            if (Owner.HasPower<DollRevengePower>()) 
            {
              this.AddDamage(Owner.GetPower<DollRevengePower>().Amount);
            }

                await PowerCmd.ModifyAmount(this,-amt,null,null);
        }
        public void SetDamageAndWax(decimal damage,bool IsWax=false)
        {
            AssertMutable();
            this.IsWax = IsWax;
            base.DynamicVars.Damage.BaseValue = damage;
        }
        public void AddDamage(decimal damage)
        {
            AssertMutable();
            base.DynamicVars.Damage.BaseValue += damage;
        }

        
    }
    
}
