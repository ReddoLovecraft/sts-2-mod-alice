using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace TH_Alice.Scrpits.Dolls;

[HarmonyPatch]
public static class DollBlockForPetsPatch
{
	private static MethodBase TargetMethod()
	{
		return AccessTools.Method(
			typeof(CreatureCmd),
			nameof(CreatureCmd.Damage),
			[
				typeof(PlayerChoiceContext),
				typeof(IEnumerable<Creature>),
				typeof(decimal),
				typeof(ValueProp),
				typeof(Creature),
				typeof(CardModel)
			]
		);
	}

	private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
	{
		List<CodeInstruction> codes = instructions.ToList();
		MethodInfo getPetOwner = AccessTools.PropertyGetter(typeof(Creature), nameof(Creature.PetOwner));
		MethodInfo getBlockCreature = AccessTools.Method(typeof(DollDamageHelpers), nameof(DollDamageHelpers.GetBlockCreature));

		for (int i = 0; i < codes.Count; i++)
		{
			if (!codes[i].Calls(getPetOwner))
			{
				continue;
			}

			int startPos = i - 1;
			if (startPos < 0)
			{
				continue;
			}

			int endPos = -1;
			OpCode stlocOp = default;
			object? stlocOperand = null;
			for (int j = i; j < Math.Min(codes.Count, i + 30); j++)
			{
				OpCode op = codes[j].opcode;
				if (op == OpCodes.Stloc || op == OpCodes.Stloc_S || op == OpCodes.Stloc_0 || op == OpCodes.Stloc_1 || op == OpCodes.Stloc_2 || op == OpCodes.Stloc_3)
				{
					endPos = j;
					stlocOp = op;
					stlocOperand = codes[j].operand;
					break;
				}
			}

			if (endPos < 0)
			{
				continue;
			}

			CodeInstruction ldlocOriginalTarget = new CodeInstruction(codes[startPos].opcode, codes[startPos].operand);
			codes.RemoveRange(startPos, endPos - startPos + 1);
			codes.InsertRange(startPos, new[]
			{
				ldlocOriginalTarget,
				new CodeInstruction(OpCodes.Call, getBlockCreature),
				new CodeInstruction(stlocOp, stlocOperand)
			});
			break;
		}

		return codes;
	}
}
