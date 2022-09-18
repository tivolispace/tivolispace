float mod(float x, float y)
{
    return x - y * floor(x / y);
}

float2 mod(float2 x, float2 y)
{
    return x - y * floor(x / y);
}

float3 mod(float3 x, float3 y)
{
    return x - y * floor(x / y);
}

float4 mod(float4 x, float4 y)
{
    return x - y * floor(x / y);
}

float2 smoothRepeatStart(float x, float size)
{
    return float2(mod(x - (size / 2.0f), size), mod(x, size));
}

float3 hash33(float3 p)
{
    float n = sin(dot(p, float3(7.0f, 157.0f, 113.0f)));
    return (frac(float3(2097152.0f, 262144.0f, 32768.0f) * n) * 2.0f) - 1.0f.xxx;
}

float tetraNoise(inout float3 p)
{
    float3 i = floor(p + dot(p, 0.3333329856395721435546875f.xxx).xxx);
    p -= (i - dot(i, 0.1666660010814666748046875f.xxx).xxx);
    float3 i1 = step(p.yzx, p);
    float3 i2 = max(i1, 1.0f.xxx - i1.zxy);
    i1 = min(i1, 1.0f.xxx - i1.zxy);
    float3 p1 = (p - i1) + 0.1666660010814666748046875f.xxx;
    float3 p2 = (p - i2) + 0.3333329856395721435546875f.xxx;
    float3 p3 = p - 0.5f.xxx;
    float4 v = max(0.5f.xxxx - float4(dot(p, p), dot(p1, p1), dot(p2, p2), dot(p3, p3)), 0.0f.xxxx);
    float3 param = i;
    float3 param_1 = i + i1;
    float3 param_2 = i + i2;
    float3 param_3 = i + 1.0f.xxx;
    float4 d = float4(dot(p, hash33(param)), dot(p1, hash33(param_1)), dot(p2, hash33(param_2)), dot(p3, hash33(param_3)));
    return clamp((dot(d, ((v * v) * v) * 8.0f) * 1.73199999332427978515625f) + 0.5f, 0.0f, 1.0f);
}

float smoothRepeatEnd(float a, float b, float x, float size)
{
    return lerp(a, b, smoothstep(0.0f, 1.0f, (sin((((x / size) * 3.1415927410125732421875f) * 2.0f) - 1.57079637050628662109375f) * 0.5f) + 0.5f));
}

void frag_main()
{
    float2 uv = 0.0f.xx;
    float iTime = 0.0f;
    uv /= 1.0f.xx;
    float repeatSize = 4.0f;
    float x = uv.x - mod(iTime * 0.0199999995529651641845703125f, repeatSize / 2.0f);
    float y = uv.y;
    float param = x;
    float param_1 = repeatSize;
    float2 ab = smoothRepeatStart(param, param_1);
    float3 param_2 = 16.0f.xxx + float3(float2(ab.x, uv.y) * 1.2000000476837158203125f, 0.0f);
    float _245 = tetraNoise(param_2);
    float noiseA = _245 * 0.5f;
    float3 param_3 = 16.0f.xxx + float3(float2(ab.y, uv.y) * 1.2000000476837158203125f, 0.0f);
    float _260 = tetraNoise(param_3);
    float noiseB = _260 * 0.5f;
    float param_4 = noiseA;
    float param_5 = noiseB;
    float param_6 = x;
    float param_7 = repeatSize;
    float _noise = smoothRepeatEnd(param_4, param_5, param_6, param_7);
    float param_8 = y;
    float param_9 = repeatSize / 2.0f;
    ab = smoothRepeatStart(param_8, param_9);
    float3 param_10 = float3(float2(uv.x, ab.x) * 0.5f, 0.0f);
    float _288 = tetraNoise(param_10);
    noiseA = _288 * 2.0f;
    float3 param_11 = float3(float2(uv.x, ab.y) * 0.5f, 0.0f);
    float _300 = tetraNoise(param_11);
    noiseB = _300 * 2.0f;
    float param_12 = noiseA;
    float param_13 = noiseB;
    float param_14 = y;
    float param_15 = repeatSize / 2.0f;
    _noise *= smoothRepeatEnd(param_12, param_13, param_14, param_15);
    float param_16 = x;
    float param_17 = repeatSize;
    ab = smoothRepeatStart(param_16, param_17);
    float3 param_18 = 9.0f.xxx + float3(float2(ab.x, uv.y) * 0.0500000007450580596923828125f, 0.0f);
    float _333 = tetraNoise(param_18);
    noiseA = _333 * 5.0f;
    float3 param_19 = 9.0f.xxx + float3(float2(ab.y, uv.y) * 0.0500000007450580596923828125f, 0.0f);
    float _348 = tetraNoise(param_19);
    noiseB = _348 * 5.0f;
    float param_20 = noiseA;
    float param_21 = noiseB;
    float param_22 = x;
    float param_23 = repeatSize;
    _noise *= smoothRepeatEnd(param_20, param_21, param_22, param_23);
    _noise *= 0.66600000858306884765625f;
    _noise = lerp(_noise, dot(uv, float2(-0.263999998569488525390625f, 0.4000000059604644775390625f)), 0.60000002384185791015625f);
    float spacing = 0.0199999995529651641845703125f;
    float lines = mod(_noise, spacing) / spacing;
    lines = min(lines * 2.0f, 1.0f) - max((lines * 2.0f) - 1.0f, 0.0f);
    lines /= fwidth(_noise / spacing);
    lines /= 2.0f;
    float4 fragColor = float4(lines.xxx, 1.0f);
}

void main()
{
    frag_main();
}
