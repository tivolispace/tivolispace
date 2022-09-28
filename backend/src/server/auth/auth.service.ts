import { Injectable } from "@nestjs/common";
import { JwtService } from "@nestjs/jwt";
import axios from "axios";
import { JWT_SECRET, STEAM_APP_ID, STEAM_WEB_API_KEY } from "../environment";
import { User } from "../user/user.schema";
import { UserService } from "../user/user.service";
import { JwtPayload } from "./jwt.strategy";

@Injectable()
export class AuthService {
	constructor(
		private readonly userService: UserService,
		private readonly jwtService: JwtService,
	) {}

	private async generateAccessToken(user: User) {
		const [accessToken] = await Promise.all([
			this.jwtService.signAsync({ id: user.id } as JwtPayload, {
				secret: JWT_SECRET,
				expiresIn: "7d",
			}),
		]);

		return {
			accessToken,
		};
	}

	async login(steamId: string) {
		let user = await this.userService.findBySteamId(steamId);
		if (user == null) {
			user = await this.userService.createUser(steamId);
		}

		return this.generateAccessToken(user);
	}

	async loginSteamTicket(ticket: string) {
		const response = await axios({
			method: "GET",
			url: "https://partner.steam-api.com/ISteamUserAuth/AuthenticateUserTicket/v1/",
			params: {
				key: STEAM_WEB_API_KEY,
				appid: STEAM_APP_ID,
				ticket,
			},
		});

		const data: {
			response: {
				params: {
					result: "OK";
					steamid: string;
					ownersteamid: string;
					vacbanned: boolean;
					publisherbanned: boolean;
				};
			};
		} = response.data;

		return this.login(data.response.params.steamid);
	}
}
