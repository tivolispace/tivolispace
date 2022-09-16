const { ipcRenderer } = require("electron");

import { Typeface } from "./scripts/typeface.js";

const progressText = document.querySelector(".progress-text");
const progressBar = document.querySelector(".progress-bar");
const torus = new Typeface("typeface/torus-32");
await torus.load();

function setText(message) {
	torus.setText(progressText, message, 20, 0, true, true);
}

function setProgress(percentage) {
	if (percentage > 100) percentage = 100;
	if (percentage < 0) percentage = 0;
	progressBar.style.width = percentage + "%";
	setText(`Updating... ${Math.floor(percentage)}% done!`);
}

ipcRenderer.on("set-progress", (event, percentage) => {
	if (typeof percentage != "number") return;
	setProgress(percentage);
});

ipcRenderer.on("set-text", (event, text) => {
	setText(percentage);
});

setText("Updating...");
