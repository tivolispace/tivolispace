import { Injectable } from "@nestjs/common";
import { PassportStrategy } from "@nestjs/passport";
import { Strategy } from "passport-steam";
import { STEAM_DEV_API_KEY, URL } from "../environment";
import { UserService } from "../user/user.service";

export interface SteamProfile {
	id: string;
	displayName: string;
	photos: {
		value: string;
	}[];
}

@Injectable()
export class SteamStrategy extends PassportStrategy(Strategy, "steam") {
	constructor(private readonly userService: UserService) {
		super({
			returnURL: URL + "/api/auth/steam/callback",
			realm: URL,
			apiKey: STEAM_DEV_API_KEY,
		});
	}

	async validate(
		identifier: string,
		profile: SteamProfile,
		// done: (error: any, user: any) => void,
	) {
		return profile;
	}
}
