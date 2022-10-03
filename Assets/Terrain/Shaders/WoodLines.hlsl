void LinesNodeMethod_float(float A, float B, out float Out)
{
	float scale = 10.0;
	A *= scale;
	Out = smoothstep(0.0, 0.5 + (B * 0.5), abs((sin(A.x * 3.1415) + B * 2.0)) * 0.5);
}