shader_type canvas_item;

uniform vec3 tint = vec3(1, 0, 0.5);

void fragment() {
	// Place fragment code here.
	vec4 texture_col = texture(TEXTURE, UV);
	//texture_col.rgb = MODULATE.rgb;//vec3(1.0, 0.0, 1.0);
	
	COLOR.a = texture_col.a;
	
	if (any(greaterThan(texture_col.rgb, vec3(0.9f))))
		COLOR.rgb = tint;
	else
		COLOR.rgb = vec3(1, 1, 1);
}
