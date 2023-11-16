shader_type canvas_item;

const vec4 COL_TOP_G = vec4(0.5, 1, 0.5, 1);
const vec4 COL_BOT_G = vec4(0, 0.4, 0, 1);

const vec4 COL_TOP_R = vec4(1, 0.5, 0.5, 1);
const vec4 COL_BOT_R = vec4(0.4, 0, 0, 1);

const vec4 COL_DARK = vec4(0, 0, 0, 1);


uniform float health_factor;
uniform float draining_factor;

void fragment() {
	// Place fragment code here.
	
	vec4 col_g = mix(COL_TOP_G, COL_BOT_G, UV.y+0.2);
	vec4 col_r = mix(COL_TOP_R, COL_BOT_R, UV.y+0.2);
	vec4 col = mix(col_r, col_g, step(UV.x, health_factor));
	
	col = mix(col, COL_DARK, smoothstep(0.75, 0.95, UV.y)*0.75);
	
	vec4 white = vec4(1, 1, 1, 1);
	float fac = health_factor+draining_factor;
	
	vec4 white_overlay = mix(white, col, step(UV.x, health_factor));
	vec4 final_col = mix(col, white_overlay, step(UV.x, fac));
	
	COLOR = final_col;
}
