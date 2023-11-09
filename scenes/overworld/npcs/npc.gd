extends Node

@export var player: CharacterBody2D
@export var json_file_path:String
var player_in_range: bool
var json_as_dict
var index: int = 0
var npc_name: String 

func _ready():
	var json_as_text = FileAccess.get_file_as_string(json_file_path)
	json_as_dict = JSON.parse_string(json_as_text)
	npc_name = get_node(".").name
	$Dialogue.visible = false
	$Dialogue/Name.text = npc_name

func _process(_delta):
	if player_in_range:
		if Input.is_action_just_pressed("ui_interact"):
			$Dialogue.visible = true
			if !(index == len(json_as_dict[npc_name][index]) - 1) :	
				$Dialogue/Text.text = json_as_dict[npc_name][index]["text"]
				index += 1
			else:
				$Dialogue/Text.text = json_as_dict[npc_name][index]["text"]
				index = 0

func _on_area_2d_body_entered(body):
	if body == player:
		player_in_range = true

func _on_area_2d_body_exited(body):
	if body == player:
		player_in_range = false
