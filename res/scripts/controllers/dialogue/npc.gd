extends Node

@export var player: CharacterBody2D
@export var shop: Node2D
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
	$Dialogue/Options.visible = false
	$Dialogue/Name.text = npc_name

func _process(_delta):
	if player_in_range:
		if Input.is_action_just_pressed("ui_interact"):
			_get_text()

func _on_area_2d_body_entered(body):
	if body == player:
		player_in_range = true

func _on_area_2d_body_exited(body):
	if body == player:
		player_in_range = false

func _get_text():
	$Dialogue/Text.text = json_as_dict[npc_name][index]["text"]
	$Dialogue.visible = true
	if !(index == len(json_as_dict[npc_name]) - 1) :	
		if !(len(json_as_dict[npc_name][index]["options"]) == 0):
			$Dialogue/Options/Button_1.text = json_as_dict[npc_name][index]["options"][0]
			$Dialogue/Options/Button_2.text = json_as_dict[npc_name][index]["options"][1]
			$Dialogue/Options.visible = true
		else:
			$Dialogue/Options.visible = false
		index += 1
	else:
		if !(len(json_as_dict[npc_name][index]["options"]) == 0):
			$Dialogue/Options/Button_1.text = json_as_dict[npc_name][index]["options"][0]
			$Dialogue/Options/Button_2.text = json_as_dict[npc_name][index]["options"][1]
			$Dialogue/Options.visible = true
		else:
			$Dialogue/Options.visible = false
		index = 0
	
func _on_button_1_pressed():
	if len(json_as_dict[npc_name][index - 1]["options"]) == 3:
		if json_as_dict[npc_name][index - 1]["options"][2] == "shop":
			shop.visible = true
		if json_as_dict[npc_name][index - 1]["options"][2] == "card":
			print("Give Player Card: " + json_as_dict[npc_name][index - 1]["card"])
	_get_text()
			
func _on_button_2_pressed():
	_get_text()
