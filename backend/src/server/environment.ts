import * as dotenv from "dotenv";
dotenv.config();

export const {
	DEV = false,
	PORT = "3000",

	URL = "http://127.0.0.1:" + PORT,

	JWT_SECRET = "changeMe!",

	STEAM_APP_ID = "2161040",
	// http://steamcommunity.com/dev/apikey
	STEAM_DEV_API_KEY = "",
	// https://partner.steamgames.com/pub/group
	// select everyone group and generate web api key
	STEAM_WEB_API_KEY = "",

	DB_URI = "mongodb://localhost:27017/tivolispace",
	// DB_URI = "mongodb://tivoli:changeme@localhost:27017/tivolispace",
} = process.env;
