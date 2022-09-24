import torus32Info from "./torus-bold-32.json";
import torus32Atlas from "./torus-bold-32.png";
import torus64Info from "./torus-bold-64.json";
import torus64Atlas from "./torus-bold-64.png";
import torus128Info from "./torus-bold-128.json";
import torus128Atlas from "./torus-bold-128.png";

export default function Torus(props: {
	size?: 16 | 32 | 64;
	white?: boolean;
	center?: boolean;
	children: any;
}) {
	const text =
		typeof props.children == "string"
			? props.children
			: typeof props.children == "object"
			? props.children.join("")
			: "";

	let fontSize = props.size ?? 16;
	const white = props.white ?? false;
	const center = props.center ?? false;
	let letterSpacing = -0.02;

	// https://evanw.github.io/font-texture-generator/
	// use double the size for macos retina to look good
	let atlas: any = null;
	let info: any = null;
	switch (fontSize) {
		case 16:
			atlas = torus32Atlas;
			info = torus32Info;
			break;
		case 32:
			atlas = torus64Atlas;
			info = torus64Info;
			break;
		case 64:
			atlas = torus128Atlas;
			info = torus128Info;
			break;
	}

	const atlasWidth = info.width;
	const atlasHeight = info.height;
	let atlasFontSize = info.size;

	const size = fontSize / atlasFontSize;
	atlasFontSize *= size;
	letterSpacing *= atlasFontSize;

	return (
		<div
			style={{
				display: "flex",
				flexDirection: "row",
				filter: white ? "invert(1)" : null,
				justifyContent: center ? "center" : "flex-start",
			}}
		>
			{text.split("").map((char, i) => {
				let { x, y, width, height, originX, originY, advance } =
					info.characters[char];

				const scaledWidth = width * size;
				const scaledHeight = height * size;
				const scaledOriginX = originX * size;
				const scaledOriginY = originY * size;
				const scaledAdvance = advance * size;

				return (
					<div
						key={i}
						style={{
							width: scaledWidth + "px",
							height: scaledHeight + "px",
							display: "inline-block",
							backgroundImage: `url(${atlas.src})`,
							marginLeft:
								-scaledOriginX +
								(scaledAdvance - scaledWidth) +
								letterSpacing +
								"px",
							marginTop: atlasFontSize - scaledOriginY + "px",
							backgroundSize: `${(100 * atlasWidth) / width}% ${
								(100 * atlasHeight) / height
							}%`,
							backgroundPosition: `${
								(100 * x) / (-width + atlasWidth)
							}% ${(100 * y) / (-height + atlasHeight)}%`,
						}}
					></div>
				);
			})}
		</div>
	);
}
