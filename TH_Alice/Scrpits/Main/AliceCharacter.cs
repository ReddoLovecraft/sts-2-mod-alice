using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using TH_Alice.Scrpits.Cards;
using TH_Alice.Scrpits.Character;
namespace TH_Alice.Scrpits.Main
{
    public class AliceCharacter : PlaceholderCharacterModel
    {
        
        public override Color NameColor => new (1, 0.84313726f, 0, 1);
        public override Color EnergyLabelOutlineColor => new(1, 0.84313726f, 0, 1);
        public override Color DialogueColor => new Color("eabf30ff");
        public override Color MapDrawingColor => new Color("ffdb7cff");
        public override Color RemoteTargetingLineColor => new Color("ffdc4aff");
        public override Color RemoteTargetingLineOutline => new Color("786d2cff");
        public override CharacterGender Gender => CharacterGender.Feminine;
        public override int StartingHp => 72;
        public override string CustomVisualPath => "res://ArtWorks/Character/alice.tscn";
        public override string CustomTrailPath => "res://ArtWorks/VFX/AliceCardTrail.tscn";
        public override string CustomIconTexturePath => "res://ArtWorks/Character/alice_icon.png";
        public override string CustomIconPath => "res://ArtWorks/Character/alice_icon.tscn";
       public override string CustomEnergyCounterPath => "res://ArtWorks/VFX/alice_energy_counter.tscn";
        // 篝火休息动画。
         public override string CustomRestSiteAnimPath => "res://ArtWorks/Character/alicerest.tscn";
        // 商店人物动画。
        public override string CustomMerchantAnimPath => "res://ArtWorks/Character/alice_shop.tscn";
        public override string CustomArmPointingTexturePath => "res://ArtWorks/Character/multiplayer_hand_alice_point.png";
        public override string CustomArmRockTexturePath => "res://ArtWorks/Character/multiplayer_hand_alice_rock.png";
        public override string CustomArmPaperTexturePath => "res://ArtWorks/Character/multiplayer_hand_alice_paper.png";
        public override string CustomArmScissorsTexturePath => "res://ArtWorks/Character/multiplayer_hand_alice_scissors.png";
        public override string CustomCharacterSelectBg => "res://ArtWorks/Scenes/Alice_bg.tscn";
        public override string CustomCharacterSelectIconPath => "res://ArtWorks/Character/char_select_alice.png";
        public override string CustomCharacterSelectLockedIconPath => "res://ArtWorks/Character/char_select_alice_locked.png";
         public override string CustomCharacterSelectTransitionPath => "res://materials/transitions/silent_transition_mat.tres";
        public override string CustomMapMarkerPath => "res://ArtWorks/Character/map_marker_alice.png";
        // 攻击音效
         public override string CustomAttackSfx => AliceModInit.ToModSfxPath("ArtWorks/SFX/attack.wav");
        // 施法音效
         public override string CustomCastSfx => AliceModInit.ToModSfxPath("ArtWorks/SFX/cast.wav");
        // 死亡音效
        public override string CustomDeathSfx => AliceModInit.ToModSfxPath("ArtWorks/SFX/die.wav");
        public override string CharacterSelectSfx  => AliceModInit.ToModSfxPath("ArtWorks/SFX/silkshot.mp3");
        public override string CharacterTransitionSfx => "event:/sfx/ui/wipe_ironclad";
        public override CardPoolModel CardPool => ModelDb.CardPool<AliceCardPool>();
        public override RelicPoolModel RelicPool => ModelDb.RelicPool<AliceRelicPool>();
        public override PotionPoolModel PotionPool => ModelDb.PotionPool<AlicePotionPool>();

        // 初始卡组
        public override IEnumerable<CardModel> StartingDeck => [
            ModelDb.Card<Strike>(),
            ModelDb.Card<Strike>(),
            ModelDb.Card<Strike>(),
            ModelDb.Card<Strike>(),
            ModelDb.Card<Strike>(),
            ModelDb.Card<Defend>(),
            ModelDb.Card<Defend>(),
            ModelDb.Card<Defend>(),
            ModelDb.Card<Defend>(),
            ModelDb.Card<DollCreate>()
    ];
         //做完事件去做咲夜的遗物，等那边做完了写完那边的事件可以来这边想一想先古之民神绮，以及若干新先古遗物
        //加入攻击死亡施法音效，还有人偶攻击和爆炸，死亡的音效
        /*
        事件：
神秘金发小女孩(至少有一张稀有卡牌才能出现在事件池)
随机偷你一张稀有度最高的卡牌。
还我！拿回卡牌。获得50(25)金。
再赛几本书。自选删3(2)。 
不管。 无影响
过去、未来？
根据抉择获得不同的效果。
过去：获得爱丽丝的魔法书，选择任意张旧作强力？卡牌加入手中。
未来：移除所有打防。
偶遇咲夜
问你人偶是啥
一般定义：随机敲4(3)
杀人玩偶也是人偶！将一张杀人玩偶加入卡组。
人偶剧表演
你看到了有些人在剧场表演。
上去试试：获得200(150)金。
在下面观看：自选敲3(2)
梦子
神绮大人特别担心所以派梦子来找了。
拒绝。-》比过一场。失去9(12)生命，150(100)g。 言语劝说-》获得苦恼+获得3瓶随机药。
回去。结束这局游戏（真的要选择吗）。
神秘魔界行商路易兹(持有金200+才能出现在事件池)
你遇到了一个奇怪商人，一番推托无果之下你只能选一个东西来买了。
买点喝的。随机稀有药水*3。(180->210g)
买点吃的。回复全部生命。(100->150g)
买点用的。获得稀有遗物*3。(300->400g)

        */
        // 初始遗物
        public override IReadOnlyList<RelicModel> StartingRelics => [
            ModelDb.Relic<Thread>(),
    ];

        // 攻击建筑师的攻击特效列表
        public override List<string> GetArchitectAttackVfx() => [
            "vfx/vfx_attack_blunt",
        "vfx/vfx_heavy_blunt",
        "vfx/vfx_attack_slash",
        "vfx/vfx_bloody_impact",
        "vfx/vfx_rock_shatter"
        ];
        public override CreatureAnimator GenerateAnimator(MegaSprite controller)
        {
            AnimState animState = new AnimState("Idle", isLooping: true);
            AnimState animState2 = new AnimState("Cast");
            AnimState animState3 = new AnimState("Attack");
            AnimState animState4 = new AnimState("Hit");
            AnimState state = new AnimState("die");
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
            creatureAnimator.AddAnyState("Summon",animState6);
            return creatureAnimator;
        }
    }
}
