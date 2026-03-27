extends Node2D

func _ready() -> void:
	var spine = $SpineSprite
	# 播放空闲动画（循环）
	spine.animation_state.set_animation(0, "idle", true)
