using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Encounters;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using TH_Alice.Scrpits.Dolls;

namespace TH_Alice.Scrpits.Main
{
    public static class DollCmd
    {
       
        public static async Task Summon(PlayerChoiceContext choiceContext, Player summoner,MonsterModel dollToSummon, AbstractModel? source)
        {
            if (dollToSummon == null) return;

            CombatState combatState = summoner.Creature.CombatState;
            if (CombatManager.Instance.IsInProgress)
            {
                SfxCmd.Play("event:/sfx/characters/necrobinder/necrobinder_summon");
            }

            if (dollToSummon is ShangHai)
            {
                Creature doll = await PlayerCmd.AddPet<ShangHai>(summoner);
                NCreature dollNode = NCombatRoom.Instance?.GetCreatureNode(doll);
               
                if (dollNode != null)
                {
                    dollNode.Modulate = Colors.Transparent;
                    Tween tween = dollNode.CreateTween();
                    tween.TweenProperty(dollNode, "modulate:a", 1, 0.3499999940395355).From(0);
                    dollNode.StartReviveAnim();
                }
                dollNode?.TrackBlockStatus(summoner.Creature);
                // await CreatureCmd.SetMaxHp(doll, 12);
                //await CreatureCmd.Heal(doll,12);
                
                //NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(doll);
                //nCreature.OstyScaleToSize(doll.MaxHp, 0.75f);
            }
            else 
            {
                Creature doll = await PlayerCmd.AddPet<Osty>(summoner);
                NCreature ostyNode = NCombatRoom.Instance?.GetCreatureNode(doll);
                if (ostyNode != null)
                {
                    ostyNode.Modulate = Colors.Transparent;
                    Tween tween = ostyNode.CreateTween();
                    tween.TweenProperty(ostyNode, "modulate:a", 1, 0.3499999940395355).From(0);
                    ostyNode.StartReviveAnim();
                }
                await PowerCmd.Apply<DieForYouPower>(doll, 1m, null, null);
                ostyNode?.TrackBlockStatus(summoner.Creature);
                NCreature nCreature = NCombatRoom.Instance?.GetCreatureNode(doll);
                nCreature.OstyScaleToSize(doll.MaxHp, 0.75f);
            }
            return ;
        }
    }

}
