const { app, BrowserWindow, dialog } = require("electron");
const { autoUpdater } = require("electron-updater");
const { spawn } = require("child_process");
const path = require("path");
const fs = require("fs");

const DEV = process.env.NODE_ENV == "development";

const APP_ROOT = path.resolve(__dirname, "./html/index.html");

const appLock = app.requestSingleInstanceLock();
if (!appLock) app.quit();

let win;

app.on("activate", () => {
	if (win) win.show();
});

app.on("window-all-closed", () => {
	app.quit();
});

// restore if opened twice
app.on("second-instance", () => {
	if (win) win.show();
});

if (DEV) {
	// auto reload in dev
	fs.watchFile(APP_ROOT, { interval: 500 }, () => {
		if (win != null) {
			win.loadFile(APP_ROOT);
		}
	});
}

function showErrorSync(title, content) {
	// seems to be blocking *shrug*
	dialog.showErrorBox(title, content);
}

function setProgress(percentage) {
	if (win) {
		win.webContents.send("set-progress", percentage);
	}
}

function setText(text) {
	if (win) {
		win.webContents.send("set-text", text);
	}
}

function createWindow() {
	if (win) return;

	win = new BrowserWindow({
		title: "Tivoli Space Updater",
		width: 300,
		height: 200,
		frame: false,
		resizable: false,
		webPreferences: {
			nodeIntegration: true,
			contextIsolation: false, // require()
			devTools: DEV,
		},
		autoHideMenuBar: true,
	});

	win.menuBarVisible = false;

	win.loadFile(APP_ROOT);
}

function pathToTivoli() {
	switch (process.platform) {
		case "win32":
		case "win64":
			return path.resolve(
				app.getAppPath(),
				"StandaloneWindows64",
				"Tivoli Space.exe",
			);
		case "darwin":
			return path.resolve(
				app.getAppPath(),
				"StandaloneOSX",
				"Tivoli Space.app/Contents/MacOS/Tivoli Space",
			);
		// case "linux":
		// 	return "StandaloneLinux64";
		default:
			return null;
	}
}

function launchTivoli() {
	const tivoliPath = pathToTivoli();
	const child = spawn(tivoliPath, { detached: true });
	child.unref();
}

app.once("ready", async () => {
	autoUpdater.on("error", async error => {
		showErrorSync("Failed to update Tivoli", String(error));
		launchTivoli();
		setTimeout(() => {
			app.exit(1);
		}, 1000);
	});

	// autoUpdater.on("checking-for-update", () => {});

	autoUpdater.on("update-available", info => {
		createWindow();
	});

	autoUpdater.on("update-not-available", async info => {
		launchTivoli();
		setTimeout(() => {
			app.exit(0);
		}, 1000);
	});

	autoUpdater.on("download-progress", info => {
		setProgress(info.percent);
	});

	autoUpdater.on("update-downloaded", event => {
		autoUpdater.quitAndInstall();
	});

	if (DEV) {
		autoUpdater.updateConfigPath = path.join(
			__dirname,
			"dev-app-update.yml",
		);
		Object.defineProperty(app, "isPackaged", {
			get() {
				return true;
			},
		});
	}

	autoUpdater.autoDownload = true;
	autoUpdater.checkForUpdates().catch(() => {});
});
