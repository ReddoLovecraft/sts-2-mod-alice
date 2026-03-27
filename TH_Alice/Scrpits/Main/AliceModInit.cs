using Godot;
using Godot.Bridge;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System.Diagnostics;
using TH_Alice.Scrpits.Powers;

using MegaCrit.Sts2.Core.Models.Events;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Ancients;

using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Combat;
using TH_Alice.Scrpits.Cards;
using MegaCrit.Sts2.Core.Helpers;

using MegaCrit.Sts2.Core.Nodes.Audio;
using Godot;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Saves;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.Extensions;
using System.Reflection.Emit;
using System.Reflection;
using System.Runtime.CompilerServices;
using EmitLabel = System.Reflection.Emit.Label;

namespace TH_Alice.Scrpits.Main
{
    [ModInitializer("Init")]
    public class AliceModInit
    {
        private const string ModSfxPrefix = "mod_sfx://";

        public static string ToModSfxPath(string localPath)
        {
            return ModSfxPrefix + localPath;
        }

        public static void Init()
        {
           
            var harmony = new Harmony("TH_Alice");
            harmony.PatchAll();
            ScriptManagerBridge.LookupScriptsInAssembly(typeof(AliceModInit).Assembly);
            Log.Debug("Alice Mod initialized!");
            //GD.Print("[TH_Alice] Alice mod initialized");
        }
    }

    [HarmonyPatch(typeof(NAudioManager), "PlayOneShot", [typeof(string), typeof(float)])]
    public static class ModSfxPatch
    {
        static bool Prefix(string path, float volume)
        {
            if (path.StartsWith("mod_sfx://"))
            {
                try 
                {
                    string resPath = "res://" + path.Substring(10); // 10 is "mod_sfx://".Length
                    var stream = ResourceLoader.Load<AudioStream>(resPath);
                    if (stream != null)
                    {
                        var player = new AudioStreamPlayer();
                        player.Stream = stream;
                        player.VolumeDb = Mathf.LinearToDb(volume);
                        NGame.Instance.AddChild(player);
                        player.Play();
                        player.Connect("finished", Callable.From(player.QueueFree));
                    }
                }
                catch (System.Exception e)
                {
                    Log.Error($"Failed to play mod sfx: {path}. Error: {e.Message}");
                }
                return false; // 拦截原本的 FMOD 播放
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(TheArchitect), "DefineDialogues")]
    public static class ArchitectDialoguePatch
    {
        static void Postfix(ref AncientDialogueSet __result)
        {
            var aliceKey = ModelDb.Character<AliceCharacter>().Id.Entry;
            var aliceDialogues = new List<AncientDialogue>();
            for (int i = 0; i <= 20; i++)
            {
                aliceDialogues.Add(new AncientDialogue("", "", "", "") { VisitIndex = i, EndAttackers = ArchitectAttackers.Both });
            }
            if (__result.CharacterDialogues is Dictionary<string, IReadOnlyList<AncientDialogue>> dict)
            {
                dict[aliceKey] = aliceDialogues;
            }
        }
    }

    [HarmonyPatch(typeof(TheArchitect), "LoadDialogue")]
    public static class ArchitectLoadDialoguePatch
    {
        // 当建筑师加载对话后，强行覆盖其文本内容
        static void Postfix(TheArchitect __instance)
        {
            if (__instance.Owner.Character is not AliceCharacter) return;
            var diag = Traverse.Create(__instance).Property("Dialogue").GetValue<AncientDialogue>();
            if (diag == null || diag.Lines.Count < 4) return;
            diag.Lines[0].LineText = new LocString("ancients", "TH_Alice_Dialogue_1");
            diag.Lines[0].NextButtonText = new LocString("ancients", "TH_Alice_Next_1");
            diag.Lines[0].Speaker = AncientDialogueSpeaker.Character;

            diag.Lines[1].LineText = new LocString("ancients", "TH_Alice_Dialogue_2");
            diag.Lines[1].NextButtonText = new LocString("ancients", "TH_Alice_Next_2");
            diag.Lines[1].Speaker = AncientDialogueSpeaker.Ancient;

            diag.Lines[2].LineText = new LocString("ancients", "TH_Alice_Dialogue_3");
            diag.Lines[2].NextButtonText = new LocString("ancients", "TH_Alice_Next_3");
            diag.Lines[2].Speaker = AncientDialogueSpeaker.Character;

            diag.Lines[3].LineText = new LocString("ancients", "TH_Alice_Dialogue_4");
            diag.Lines[3].NextButtonText = new LocString("ancients", "TH_Alice_Next_4");
            diag.Lines[3].Speaker = AncientDialogueSpeaker.Ancient;
        }
    }

    [HarmonyPatch(typeof(CardPileCmd), "Add", [typeof(IEnumerable<CardModel>), typeof(CardPile), typeof(CardPilePosition), typeof(AbstractModel), typeof(bool)])]
    public static class CardPileAddPatch
    {
        static void Postfix(IEnumerable<CardModel> cards, CardPile newPile)
        {
            if (newPile.Type != PileType.Deck) return;

            foreach (var card in cards)
            {
                if (card is Collector)
                {
                
                    TaskHelper.RunSafely(CardPileCmd.AddCurseToDeck<Collectomania>(card.Owner));
                }
            }
        }
    }

    // [HarmonyPatch]
    // public static class ChooseACardScreenNoLimitPatch
    // {
    //     static MethodBase TargetMethod()
    //     {
    //         var asyncMethod = typeof(CardSelectCmd).GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
    //             .First(m => m.Name == "FromChooseACardScreen" && m.GetParameters().Length == 4);

    //         var stateMachine = asyncMethod.GetCustomAttribute<AsyncStateMachineAttribute>()?.StateMachineType;
    //         if (stateMachine == null)
    //         {
    //             GD.PrintErr("[TH_Alice] Failed to locate async state machine for CardSelectCmd.FromChooseACardScreen");
    //             return asyncMethod;
    //         }

    //         var moveNext = AccessTools.Method(stateMachine, "MoveNext");
    //         if (moveNext == null)
    //         {
    //             GD.PrintErr("[TH_Alice] Failed to locate MoveNext for CardSelectCmd.FromChooseACardScreen state machine");
    //             return asyncMethod;
    //         }

    //         GD.Print($"[TH_Alice] Patching {stateMachine.FullName}.MoveNext for CardSelectCmd.FromChooseACardScreen");
    //         return moveNext;
    //     }

    //     static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    //     {
    //         var list = new List<CodeInstruction>(instructions);
    //         bool patched = false;

    //         int msgIndex = -1;
    //         for (int i = 0; i < list.Count; i++)
    //         {
    //             if (list[i].opcode == OpCodes.Ldstr && list[i].operand is string s && s.Contains("Only works with less than 3 cards"))
    //             {
    //                 msgIndex = i;
    //                 break;
    //             }
    //         }

    //         int searchStart = (msgIndex >= 0) ? Math.Max(0, msgIndex - 200) : 0;
    //         int searchEnd = (msgIndex >= 0) ? Math.Min(list.Count - 3, msgIndex + 40) : (list.Count - 3);

    //         for (int i = searchStart; i <= searchEnd; i++)
    //         {
    //              if (list[i].operand is not System.Reflection.MethodInfo mi)
    //             {
    //                 continue;
    //             }

    //             if (mi.Name != "get_Count" || mi.ReturnType != typeof(int))
    //             {
    //                 continue;
    //             }

    //             if (list[i + 1].opcode != OpCodes.Ldc_I4_3)
    //             {
    //                 continue;
    //             }

    //             var op = list[i + 2].opcode;
    //             if (op.FlowControl != FlowControl.Cond_Branch)
    //             {
    //                 continue;
    //             }

    //             list[i + 1].opcode = OpCodes.Ldc_I4;
    //             list[i + 1].operand = int.MaxValue;
    //             patched = true;
    //             break;
    //         }

    //         if (!patched)
    //         {
    //             GD.PrintErr("[TH_Alice] FromChooseACardScreen transpiler: failed to patch guard compare");
    //         }

    //         if (patched)
    //         {
    //             Log.Debug("Patched CardSelectCmd.FromChooseACardScreen card count limit");
    //             GD.Print("[TH_Alice] Patched CardSelectCmd.FromChooseACardScreen card count limit");
    //         }
    //         return list;
    //     }
    // }

    [HarmonyPatch(typeof(ArchaicTooth),"GetTranscendenceStarterCard",[typeof(Player)])]
    public static class GetStarterCardTransformPatch
    {
        static void Postfix(ref CardModel __result,Player player)
        {
            if(player.Character is AliceCharacter)
            {
                CardModel cm=player.Deck.Cards.FirstOrDefault((CardModel c) =>c is DollCreate);
                if(cm!=null)
                __result=cm;
            }
           
        }
    }
    [HarmonyPatch(typeof(ArchaicTooth),"GetTranscendenceTransformedCard",[typeof(CardModel)])]
    public static class GetStarterCardTransformedPatch
    {
        static void Postfix(ref CardModel __result,CardModel starterCard)
        {
            if(starterCard is DollCreate)
            {
                CardModel cardModel=ModelDb.Card<DollMake>();
                if (starterCard.IsUpgraded)
			{
				CardCmd.Upgrade(cardModel);
			}
			if (starterCard.Enchantment != null)
			{
				EnchantmentModel enchantmentModel = (EnchantmentModel)starterCard.Enchantment.MutableClone();
				CardCmd.Enchant(enchantmentModel, cardModel, enchantmentModel.Amount);
			}
            __result=cardModel;
            }
        }
    }


    [HarmonyPatch(typeof(TouchOfOrobas),"GetUpgradedStarterRelic",[typeof(RelicModel)])]
     public static class RelicUpgradePatch
     {
        static void Postfix(ref RelicModel __result,RelicModel starterRelic)
        {
            if(starterRelic is Thread)
            __result=ModelDb.Relic<BloodThread>();
        }
     } 

    [HarmonyPatch(typeof(PowerCmd), "ModifyAmount",[typeof(PowerModel),typeof(decimal),typeof(Creature),typeof(CardModel),typeof(bool)])]
    public static class ModifyAmountPatch 
    {
        static async void Prefix(PowerModel power, decimal offset, Creature? applier, CardModel? cardSource, bool silent = false) 
        {
            if(power is PengLaiPower && offset < 0) 
            {
                PengLaiPower pl=power as PengLaiPower;
                await CreatureCmd.Damage(pl.ChoiceContextEndTurn, power.Owner.CombatState.HittableEnemies, new DamageVar(power.Amount+offset, ValueProp.Unpowered), power.Owner);
            }
        }
    }
}
