const fs = require("fs");
const path = require("path");

if (process.argv.length <= 2) {
	console.error("Usage: node set-version.js <version>");
	process.exit(1);
}

let version = process.argv[2];
if (version.startsWith("v")) {
	version = version.slice(1);
}

const packageJsonPath = path.resolve(__dirname, "package.json");
const packageJson = JSON.parse(fs.readFileSync(packageJsonPath));
packageJson.version = version;
fs.writeFileSync(packageJsonPath, JSON.stringify(packageJson, null, 4));

console.log("Updated package.json to version " + version);
