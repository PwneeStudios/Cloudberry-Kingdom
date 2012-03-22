from math import cos, sin

n = 32
for i in range(n):
    angle = i * 2 * 3.141592654 / n
    print "v += abs(l - tex2D(SceneSampler, uv + float2(%g, %g) * d * 1.25).r);" % (cos(angle), sin(angle))

print

n = 32
for i in range(n):
    angle = i * 2 * 3.141592654 / n
    print "v += tex2D(SceneSampler, uv + float2(%g, %g) * d2).r;" % (cos(angle), sin(angle))

raw_input("")


