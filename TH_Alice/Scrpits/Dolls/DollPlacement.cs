using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TH_Alice.Scrpits.Dolls;

public static class DollPlacement
{
	public static float MinGapFromOwner { get; set; } = 60f;
	public static float GapBetweenDolls { get; set; } = 18f;
	public static float RingGap { get; set; } = 26f;
	public static float ArcStartDegrees { get; set; } = 200f;
	public static float ArcEndDegrees { get; set; } = 340f;
	public static float FaceExclusionStartDegrees { get; set; } = 250f;
	public static float FaceExclusionEndDegrees { get; set; } = 290f;
	public static float SingleDollAngleDegrees { get; set; } = 225f;
	public static Vector2 CenterOffset { get; set; } = new Vector2(0, 36);
	public static float OwnerFaceClearance { get; set; } = 24f;

	public static void Arrange(Player owner)
	{
		NCombatRoom room = NCombatRoom.Instance;
		if (room == null)
		{
			return;
		}
		NCreature ownerNode = room.GetCreatureNode(owner?.Creature);
		if (ownerNode == null)
		{
			return;
		}

		List<(Creature creature, NCreature node)> dolls = owner.Creature.Pets
			.Where(p => p.IsAlive && p.Monster is AliceDollMonsterModel)
			.Select(p => (creature: p, node: room.GetCreatureNode(p)))
			.Where(x => x.node != null)
			.Select(x => (x.creature, x.node))
			.ToList();

		if (dolls.Count == 0)
		{
			return;
		}

		foreach ((Creature _, NCreature node) in dolls)
		{
			node.ToggleIsInteractable(true);
		}

		Vector2 center = ownerNode.Position + CenterOffset;
		float ownerRadius = MathF.Max(ownerNode.Hitbox.Size.X, ownerNode.Hitbox.Size.Y) * 0.5f;
		float dollRadius = dolls.Max(d => MathF.Max(d.node.Hitbox.Size.X, d.node.Hitbox.Size.Y) * 0.5f);

		float baseRadius = ownerRadius + dollRadius + MinGapFromOwner + OwnerFaceClearance;
		float minSpacing = dollRadius * 2f + GapBetweenDolls;

		int placed = 0;
		int ringIndex = 0;
		while (placed < dolls.Count)
		{
			float radius = baseRadius + ringIndex * (minSpacing + RingGap);
			float arcSpan = GetSpanRadians(ArcStartDegrees, ArcEndDegrees);
			float arcLength = radius * arcSpan;
			int capacity = Math.Max(2, (int)MathF.Floor(arcLength / minSpacing) + 1);
			int remaining = dolls.Count - placed;
			int countThisRing = Math.Min(capacity, remaining);

			List<float> angles = BuildAngles(countThisRing);

			for (int i = 0; i < countThisRing; i++)
			{
				float ang = Mathf.DegToRad(angles[i]);
				Vector2 offset = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * radius;
				NCreature node = dolls[placed + i].node;
				node.Position = center + offset;
			}

			placed += countThisRing;
			ringIndex++;
		}
	}

	private static List<float> BuildAngles(int count)
	{
		List<float> result = new List<float>(count);
		if (count <= 1)
		{
			result.Add(SingleDollAngleDegrees);
			return result;
		}

		for (int i = 0; i < count; i++)
		{
			float t = (float)i / (count - 1);
			float angle = Mathf.Lerp(ArcStartDegrees, ArcEndDegrees, t);
			if (IsInExclusion(angle))
			{
				float distToStart = Mathf.Abs(Mathf.AngleDifference(angle, FaceExclusionStartDegrees));
				float distToEnd = Mathf.Abs(Mathf.AngleDifference(angle, FaceExclusionEndDegrees));
				angle = distToStart < distToEnd ? FaceExclusionStartDegrees : FaceExclusionEndDegrees;
			}
			result.Add(angle);
		}
		return result;
	}

	private static float GetSpanRadians(float startDeg, float endDeg)
	{
		float span = endDeg - startDeg;
		while (span < 0f)
		{
			span += 360f;
		}
		return Mathf.DegToRad(span);
	}

	private static bool IsInExclusion(float deg)
	{
		float normalized = NormalizeDeg(deg);
		float start = NormalizeDeg(FaceExclusionStartDegrees);
		float end = NormalizeDeg(FaceExclusionEndDegrees);
		if (start <= end)
		{
			return normalized >= start && normalized <= end;
		}
		return normalized >= start || normalized <= end;
	}

	private static float NormalizeDeg(float deg)
	{
		float result = deg % 360f;
		if (result < 0f)
		{
			result += 360f;
		}
		return result;
	}
}
