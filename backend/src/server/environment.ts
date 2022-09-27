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

	DB_HOST = "127.0.0.1",
	DB_PORT = "5432",
	DB_USERNAME = "postgres",
	DB_PASSWORD = "postgres",
	DB_NAME = "tivolispace",
} = process.env;
