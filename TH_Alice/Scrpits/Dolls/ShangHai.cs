using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace TH_Alice.Scrpits.Dolls;

public sealed class ShangHai : MonsterModel
{
    protected override string VisualsPath => "res://ArtWorks/Character/doll.tscn";

    public override int MinInitialHp => 12;

    public override int MaxInitialHp => 12;
    
    public override bool IsHealthBarVisible => true;

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        MoveState moveState = new MoveState("NOTHING_MOVE", (IReadOnlyList<Creature> _) => Task.CompletedTask);
        moveState.FollowUpState = moveState;
        List<MonsterState> list = new List<MonsterState>();
        list.Add(moveState);
        return new MonsterMoveStateMachine(list, moveState);
    }
    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        AnimState animState = new AnimState("Idle", isLooping: true);
        AnimState animState2 = new AnimState("Cast");
        AnimState animState3 = new AnimState("Attack");
        AnimState animState4 = new AnimState("Hit");
        AnimState state = new AnimState("Die");
        AnimState animState5 = new AnimState("Relax", isLooping: true);
        AnimState animState6 = new AnimState("Summon");
        animState6.NextState = animState;
        animState2.NextState = animState;
        animState3.NextState = animState;
        animState4.NextState = animState;
        animState5.AddBranch("Idle", animState);
        CreatureAnimator creatureAnimator = new CreatureAnimator(animState, controller);
        creatureAnimator.AddAnyState("Idle", animState);
        creatureAnimator.AddAnyState("Dead", state);
        creatureAnimator.AddAnyState("Hit", animState4);
        creatureAnimator.AddAnyState("Attack", animState3);
        creatureAnimator.AddAnyState("Cast", animState2);
        creatureAnimator.AddAnyState("Relaxed", animState5);
        creatureAnimator.AddAnyState("Summon", animState6);
        return creatureAnimator;
    }
    
}
