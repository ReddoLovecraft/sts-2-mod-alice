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
using TH_Alice.Scrpits.Dolls;

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
            DollIntentLocalization.Install();
            Log.Debug("Alice Mod initialized!");
            //GD.Print("[TH_Alice] Alice mod initialized");
        }
    }
    /*
 

    */
      [HarmonyPatch(typeof(NAudioManager), "PlayOneShot", [typeof(string), typeof(System.Collections.Generic.Dictionary<string, float>), typeof(float)])]
    public static class ModSfxWithParamsPatch
    {
        static bool Prefix(string path, System.Collections.Generic.Dictionary<string, float> parameters, float volume)
        {
            if (path.StartsWith("mod_sfx://"))
            {
                try
                {
                    string resPath = "res://" + path.Substring(10);
                    var stream = ResourceLoader.Load<AudioStream>(resPath);
                    if (stream != null)
                    {
                        var player = new AudioStreamPlayer();
                        player.Stream = stream;
                        player.Bus = "SFX";
                        float master = SaveManager.Instance.SettingsSave.VolumeMaster;
                        float sfx = SaveManager.Instance.SettingsSave.VolumeSfx;
                        float finalVol = volume * Mathf.Pow(master, 2f) * Mathf.Pow(sfx, 2f);
                        player.VolumeDb = Mathf.LinearToDb(Mathf.Max(0.0001f, finalVol));
                        NGame.Instance.AddChild(player);
                        player.Play();
                        player.Connect("finished", Callable.From(player.QueueFree));
                    }
                }
                catch (System.Exception e)
                {
                    Log.Error($"Failed to play mod sfx: {path}. Error: {e.Message}");
                }
                return false;
            }
            return true;
        }
    }
    //baselib新版本已有接口，弃用patch
    // [HarmonyPatch(typeof(TheArchitect), "DefineDialogues")]
    // public static class ArchitectDialoguePatch
    // {
    //     static void Postfix(ref AncientDialogueSet __result)
    //     {
    //         var aliceKey = ModelDb.Character<AliceCharacter>().Id.Entry;
    //         var aliceDialogues = new List<AncientDialogue>();
    //         for (int i = 0; i <= 20; i++)
    //         {
    //             aliceDialogues.Add(new AncientDialogue("", "", "", "") { VisitIndex = i, EndAttackers = ArchitectAttackers.Both });
    //         }
    //         if (__result.CharacterDialogues is Dictionary<string, IReadOnlyList<AncientDialogue>> dict)
    //         {
    //             dict[aliceKey] = aliceDialogues;
    //         }
    //     }
    // }

    // [HarmonyPatch(typeof(TheArchitect), "LoadDialogue")]
    // public static class ArchitectLoadDialoguePatch
    // {
    //     // 当建筑师加载对话后，强行覆盖其文本内容
    //     static void Postfix(TheArchitect __instance)
    //     {
    //         if (__instance.Owner.Character is not AliceCharacter) return;
    //         var diag = Traverse.Create(__instance).Property("Dialogue").GetValue<AncientDialogue>();
    //         if (diag == null || diag.Lines.Count < 4) return;
    //         diag.Lines[0].LineText = new LocString("ancients", "TH_Alice_Dialogue_1");
    //         diag.Lines[0].NextButtonText = new LocString("ancients", "TH_Alice_Next_1");
    //         diag.Lines[0].Speaker = AncientDialogueSpeaker.Character;

    //         diag.Lines[1].LineText = new LocString("ancients", "TH_Alice_Dialogue_2");
    //         diag.Lines[1].NextButtonText = new LocString("ancients", "TH_Alice_Next_2");
    //         diag.Lines[1].Speaker = AncientDialogueSpeaker.Ancient;

    //         diag.Lines[2].LineText = new LocString("ancients", "TH_Alice_Dialogue_3");
    //         diag.Lines[2].NextButtonText = new LocString("ancients", "TH_Alice_Next_3");
    //         diag.Lines[2].Speaker = AncientDialogueSpeaker.Character;

    //         diag.Lines[3].LineText = new LocString("ancients", "TH_Alice_Dialogue_4");
    //         diag.Lines[3].NextButtonText = new LocString("ancients", "TH_Alice_Next_4");
    //         diag.Lines[3].Speaker = AncientDialogueSpeaker.Ancient;
    //     }
    // }

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
            if (player == null)
            {
                return;
            }

            if(player.Character is AliceCharacter)
            {
                CardModel? cm = player.Deck?.Cards?.FirstOrDefault((CardModel c) => c is DollCreate);
                if(cm!=null)
                __result=cm;
                else
                 Log.Debug("DollCreate not found in deck");
            }
           
        }
    }
    [HarmonyPatch(typeof(ArchaicTooth),"GetTranscendenceTransformedCard",[typeof(CardModel)])]
    [HarmonyPriority(Priority.First)]
    public static class GetStarterCardTransformedPatch
    {
        static bool _logged;

        static bool Prefix(ArchaicTooth __instance, ref CardModel __result, CardModel starterCard, ref CardModel? __state)
        {
            try
            {
                if (starterCard == null)
                {
                    return true;
                }

                if (starterCard is not DollCreate)
                {
                    return true;
                }

                if (!_logged)
                {
                    _logged = true;
                    Log.Debug("GetTranscendenceTransformedCard Prefix: handling DollCreate");
                }

                var template = ModelDb.Card<DollMake>();
                if (template == null)
                {
                    Log.Error("DollMake card template not found in ModelDb");
                    return true;
                }

                CardModel? cardModel = template.MutableClone() as CardModel;
                if (cardModel == null)
                {
                    Log.Error("Failed to clone DollMake card template");
                    return true;
                }

                if (starterCard.IsUpgraded)
                {
                    CardCmd.Upgrade(cardModel);
                }

                if (starterCard.Enchantment is EnchantmentModel enchantment)
                {
                    if (enchantment.MutableClone() is EnchantmentModel enchantmentModel)
                    {
                        CardCmd.Enchant(enchantmentModel, cardModel, enchantmentModel.Amount);
                    }
                }

                __state = cardModel;
                __result = cardModel;
                __result.Owner = starterCard.Owner;
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"GetTranscendenceTransformedCard patch failed: {e}");
                return true;
            }
        }

        [HarmonyPriority(Priority.Last)]
        static void Postfix(ref CardModel __result, CardModel starterCard, CardModel? __state)
        {
            if (starterCard is DollCreate && __state != null)
            {
                __result = __state;
            }
        }
    }


    [HarmonyPatch(typeof(TouchOfOrobas),"GetUpgradedStarterRelic",[typeof(RelicModel)])]
     public static class RelicUpgradePatch
     {
        static void Postfix(ref RelicModel __result,RelicModel starterRelic)
        {
            if(starterRelic ==null)
            return;
            if(starterRelic is Thread)
            __result=ModelDb.Relic<BloodThread>();
        }
     } 

    [HarmonyPatch(typeof(DustyTome), "SetupForPlayer", [typeof(Player)])]
    public static class DustyTomeSetupForPlayerPatch
    {
        static bool Prefix(DustyTome __instance, Player player)
        {
            if (player?.Character is not AliceCharacter)
            {
                return true;
            }

            try
            {
                __instance.AncientCard = ModelDb.Card<WitchForm>().Id;
                return false;
            }
            catch (System.Exception e)
            {
                Log.Error($"Failed to set DustyTome.AncientCard to WitchForm: {e}");
                return true;
            }
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
