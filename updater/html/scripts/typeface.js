export class Typeface {
	atlasSrc = "";
	jsonSrc = "";
	info = {};

	// https://evanw.github.io/font-texture-generator/

	constructor(name) {
		this.atlasSrc = name + ".png";
		this.jsonSrc = name + ".json";
	}

	async load() {
		const res = await fetch(this.jsonSrc);
		this.info = await res.json();
	}

	setText(
		el,
		text,
		fontSize = 32,
		letterSpacing = 0,
		white = false,
		center = false,
	) {
		const atlasWidth = this.info.width;
		const atlasHeight = this.info.height;
		let atlasFontSize = this.info.size;

		while (el.firstChild) el.removeChild(el.firstChild);

		el.style.display = "flex";
		el.style.flexDirection = "row";
		el.style.filter = white ? "invert(1)" : "";
		el.style.justifyContent = center ? "center" : "flex-start";

		const size = fontSize / atlasFontSize;
		atlasFontSize *= size;
		letterSpacing *= atlasFontSize;

		for (const char of text.split("")) {
			let { width, height, originX, originY, advance } =
				this.info.characters[char];

			width *= size;
			height *= size;
			originX *= size;
			originY *= size;
			advance *= size;

			const charEl = document.createElement("div");
			charEl.style.width = width + "px";
			charEl.style.height = height + "px";
			charEl.style.display = "inline-block";
			charEl.style.backgroundImage = `url(${this.atlasSrc})`;
			charEl.style.marginLeft =
				-originX + (advance - width) + letterSpacing + "px";
			charEl.style.marginTop = atlasFontSize - originY + "px";

			{
				const { x, y, width, height } = this.info.characters[char];

				charEl.style.backgroundSize = `${(100 * atlasWidth) / width}% ${
					(100 * atlasHeight) / height
				}%`;

				charEl.style.backgroundPosition = `${
					(100 * x) / (-width + atlasWidth)
				}% ${(100 * y) / (-height + atlasHeight)}%`;
			}

			charEl.style.backgroundRepeat = "repeat";
			el.appendChild(charEl);
		}
	}
}
