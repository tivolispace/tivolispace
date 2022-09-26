import { Box, Center, Heading, Image } from "@chakra-ui/react";
import { NextPage } from "next";
import logoLightNoPink from "../assets/logo-light-no-pink.png";
import { chakra } from "@chakra-ui/react";
import { useEffect, useRef } from "react";

const vertShaderSrc = `#version 300 es
precision mediump float;

in vec2 position;

out vec2 coord;

void main() {
	coord = position;
	gl_Position = vec4(position, 0.0, 1.0);
}
`;

const fragShaderSrc = `#version 300 es
precision highp float;

in vec2 coord;

out vec4 outColor;

uniform float time;
uniform vec2 resolution;

vec3 HSVtoRGB(vec3 hsv) {
	// thx shaderforge <3~
	return mix(vec3(1,1,1),clamp(3.0*abs(1.0-2.0*fract(hsv.r+vec3(0.0,-1.0/3.0,1.0/3.0)))-1.0,0.0,1.0),hsv.g)*hsv.b;
}

#define TIVOLI_PINK_H 340.0 / 360.0
#define TIVOLI_PINK_S 0.87
#define TIVOLI_PINK_V 0.91

vec3 hash33(vec3 p) { 
    float n = sin(dot(p, vec3(7, 157, 113)));    
    return fract(vec3(2097152, 262144, 32768)*n)*2. - 1.;
}

float tetraNoise(in vec3 p)
{
    vec3 i = floor(p + dot(p, vec3(0.333333)) );  p -= i - dot(i, vec3(0.166666)) ;
    vec3 i1 = step(p.yzx, p), i2 = max(i1, 1.0-i1.zxy); i1 = min(i1, 1.0-i1.zxy);    
    vec3 p1 = p - i1 + 0.166666, p2 = p - i2 + 0.333333, p3 = p - 0.5;
    vec4 v = max(0.5 - vec4(dot(p,p), dot(p1,p1), dot(p2,p2), dot(p3,p3)), 0.0);
    vec4 d = vec4(dot(p, hash33(i)), dot(p1, hash33(i + i1)), dot(p2, hash33(i + i2)), dot(p3, hash33(i + 1.)));
    return clamp(dot(d, v*v*v*8.)*1.732 + .5, 0., 1.); // Not sure if clamping is necessary. Might be overkill.
}

#define PI 3.14159265359

vec2 smoothRepeatStart(float x, float size) {
  return vec2(
      mod(x - size / 2., size),
      mod(x, size)
  );
}

float smoothRepeatEnd(float a, float b, float x, float size) {
  return mix(a, b,
      smoothstep(
          0., 1.,
          sin((x / size) * PI * 2. - PI * .5) * .5 + .5
      )
  );
}

void main() {
	vec2 uv = coord.xy;
	uv.x *= resolution.x / resolution.y;
	uv.x -= 0.5;
	uv *= 2.0;

	float repeatSize = 4.;
	float speed = 0.03;
    float x = uv.x - mod(time * speed, repeatSize / 2.);
    float y = uv.y;

	vec2 ab;

    float noise;
    float noiseA, noiseB;

	ab = smoothRepeatStart(x, repeatSize);
    noiseA = tetraNoise(16.+vec3(vec2(ab.x, uv.y) * 1.2, 0)) * .5;
    noiseB = tetraNoise(16.+vec3(vec2(ab.y, uv.y) * 1.2, 0)) * .5;
    noise = smoothRepeatEnd(noiseA, noiseB, x, repeatSize);

    ab = smoothRepeatStart(y, repeatSize / 2.);
    noiseA = tetraNoise(vec3(vec2(uv.x, ab.x) * .5, 0)) * 2.;
    noiseB = tetraNoise(vec3(vec2(uv.x, ab.y) * .5, 0)) * 2.;
    noise *= smoothRepeatEnd(noiseA, noiseB, y, repeatSize / 2.);

    ab = smoothRepeatStart(x, repeatSize);
    noiseA = tetraNoise(9.+vec3(vec2(ab.x, uv.y) * .05, 0)) * 5.;
    noiseB = tetraNoise(9.+vec3(vec2(ab.y, uv.y) * .05, 0)) * 5.;
    noise *= smoothRepeatEnd(noiseA, noiseB, x, repeatSize);

    noise *= .666;
    
	noise = mix(noise, dot(uv, vec2(-.66,1.)*.4), .6);
    
    float spacing = 1./50.;
    float lines = mod(noise, spacing) / spacing;

	lines = min(lines * 2., 1.) - max(lines * 2. - 1., 0.);
    
	lines /= fwidth(noise / spacing);
	
    lines /= 2.;

	lines -= 0.1;

	vec3 color = mix(
		HSVtoRGB(vec3(TIVOLI_PINK_H, TIVOLI_PINK_S - 0.4, TIVOLI_PINK_V)),
		// vec3(1,1,1),
		HSVtoRGB(vec3(TIVOLI_PINK_H, TIVOLI_PINK_S, TIVOLI_PINK_V)),
		clamp(lines, 0.0, 1.0)
	);

	outColor = vec4(color, 1.0);
}
`;

const Home: NextPage = (props: { name: string }) => {
	const canvasRef = useRef();

	useEffect(() => {
		const canvas: HTMLCanvasElement = canvasRef.current;
		canvas.width = window.innerWidth;
		canvas.height = window.innerHeight;

		window.addEventListener("resize", () => {
			canvas.width = window.innerWidth;
			canvas.height = window.innerHeight;
		});
		// canvas.style.width = "100vw";
		// canvas.style.height = "100vh";

		const gl = canvas.getContext("webgl2");

		if (!gl) {
			console.error("WebGL is not supported");
			return;
		}

		gl.clearColor(0, 0, 0, 1.0);
		gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);

		const vertShader = gl.createShader(gl.VERTEX_SHADER);
		const fragShader = gl.createShader(gl.FRAGMENT_SHADER);

		gl.shaderSource(vertShader, vertShaderSrc);
		gl.shaderSource(fragShader, fragShaderSrc);

		gl.compileShader(vertShader);
		if (!gl.getShaderParameter(vertShader, gl.COMPILE_STATUS)) {
			console.error(
				"Failed to compile vertex shader",
				gl.getShaderInfoLog(vertShader),
			);
			return;
		}

		gl.compileShader(fragShader);
		if (!gl.getShaderParameter(fragShader, gl.COMPILE_STATUS)) {
			console.error(
				"Failed to compile fragment shader",
				gl.getShaderInfoLog(fragShader),
			);
			return;
		}

		const program = gl.createProgram();
		gl.attachShader(program, vertShader);
		gl.attachShader(program, fragShader);
		gl.linkProgram(program);
		if (!gl.getProgramParameter(program, gl.LINK_STATUS)) {
			console.error(
				"Failed to link program",
				gl.getProgramInfoLog(program),
			);
			return;
		}

		gl.validateProgram(program);
		if (!gl.getProgramParameter(program, gl.VALIDATE_STATUS)) {
			console.error(
				"Failed to validate program",
				gl.getProgramInfoLog(program),
			);
			return;
		}

		const vertices = [-1, 1, 1, -1, -1, -1, -1, 1, 1, -1, 1, 1];

		const vbo = gl.createBuffer();
		gl.bindBuffer(gl.ARRAY_BUFFER, vbo);
		gl.bufferData(
			gl.ARRAY_BUFFER,
			new Float32Array(vertices),
			gl.STATIC_DRAW,
		);

		const positionAttrib = gl.getAttribLocation(program, "position");
		gl.vertexAttribPointer(
			positionAttrib,
			2, // length
			gl.FLOAT,
			false,
			2 * Float32Array.BYTES_PER_ELEMENT, // stride
			0, // offset
		);
		gl.enableVertexAttribArray(positionAttrib);

		const timeUniform = gl.getUniformLocation(program, "time");
		const resolutionUniform = gl.getUniformLocation(program, "resolution");

		gl.useProgram(program);

		function draw(ms) {
			gl.viewport(0, 0, gl.canvas.width, gl.canvas.height);
			gl.uniform1f(timeUniform, ms / 1000);
			gl.uniform2f(
				resolutionUniform,
				window.innerWidth,
				window.innerHeight,
			);
			gl.drawArrays(gl.TRIANGLES, 0, 6);
			requestAnimationFrame(draw);
		}

		requestAnimationFrame(draw);

		return () => {};
	}, [canvasRef]);

	return (
		<>
			<Box position={"absolute"} top={0} right={0} bottom={0} left={0}>
				<chakra.canvas
					ref={canvasRef}
					width={"100vw"}
					height={"100vh"}
					background={"brand.500"}
				></chakra.canvas>
			</Box>
			<Box position={"absolute"} top={0} right={0} bottom={0} left={0}>
				<Center height={"100vh"} flexDir={"column"}>
					<Box
						backgroundImage={logoLightNoPink.src}
						backgroundSize={"contain"}
						backgroundPosition={"center"}
						backgroundRepeat={"no-repeat"}
						height={"96px"}
						width={"512px"}
					></Box>
					<Heading color={"white"} size={"3xl"} mt={8}></Heading>
				</Center>
			</Box>
		</>
	);
};

export default Home;
